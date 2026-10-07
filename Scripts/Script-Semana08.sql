-- Semana 08: procedimientos almacenados para Biblioteca.Web (MVC + Dapper)
-- Reutiliza BibliotecaDB de la semana 07 (no crea base de datos nueva).
-- Ejecutar completo en SSMS conectado a .\SQLEXPRESS. Es re-ejecutable (CREATE OR ALTER).
USE BibliotecaDB;
GO

-- ========== LIBROS ==========

-- Listado de libros activos con el nombre del autor (INNER JOIN con Autores).
CREATE OR ALTER PROCEDURE dbo.usp_Libros_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, a.Nombre AS AutorNombre,
           l.Ejemplares, l.Activo
    FROM Libros l
    INNER JOIN Autores a ON a.AutorId = l.AutorId
    WHERE l.Activo = 1
    ORDER BY l.Titulo;
END
GO

-- Búsqueda por título (coincidencia parcial) sobre libros activos.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_BuscarPorTitulo
    @Titulo NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, a.Nombre AS AutorNombre,
           l.Ejemplares, l.Activo
    FROM Libros l
    INNER JOIN Autores a ON a.AutorId = l.AutorId
    WHERE l.Activo = 1
      AND l.Titulo LIKE N'%' + @Titulo + N'%'
    ORDER BY l.Titulo;
END
GO

CREATE OR ALTER PROCEDURE dbo.usp_Libros_ObtenerPorId
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.LibroId, l.Titulo, l.ISBN, l.AutorId, a.Nombre AS AutorNombre,
           l.Ejemplares, l.Activo
    FROM Libros l
    INNER JOIN Autores a ON a.AutorId = l.AutorId
    WHERE l.LibroId = @LibroId AND l.Activo = 1;
END
GO

-- Devuelve el Id nuevo, o -1 si el ISBN ya existe.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_Insertar
    @Titulo     NVARCHAR(150),
    @ISBN       NVARCHAR(20),
    @AutorId    INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Libros WHERE ISBN = @ISBN)
    BEGIN
        SELECT -1;
        RETURN;
    END
    INSERT INTO Libros (Titulo, ISBN, AutorId, Ejemplares, Activo)
    VALUES (@Titulo, @ISBN, @AutorId, @Ejemplares, 1);
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- Devuelve filas afectadas (0 = no existe), o -1 si el ISBN pertenece a otro libro.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_Actualizar
    @LibroId    INT,
    @Titulo     NVARCHAR(150),
    @ISBN       NVARCHAR(20),
    @AutorId    INT,
    @Ejemplares INT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Libros WHERE ISBN = @ISBN AND LibroId <> @LibroId)
    BEGIN
        SELECT -1;
        RETURN;
    END
    UPDATE Libros
       SET Titulo = @Titulo, ISBN = @ISBN, AutorId = @AutorId, Ejemplares = @Ejemplares
     WHERE LibroId = @LibroId AND Activo = 1;
    SELECT @@ROWCOUNT;
END
GO

-- Eliminación LÓGICA: Activo = 0. Nunca DELETE físico.
CREATE OR ALTER PROCEDURE dbo.usp_Libros_Eliminar
    @LibroId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Libros SET Activo = 0 WHERE LibroId = @LibroId AND Activo = 1;
    SELECT @@ROWCOUNT;
END
GO

-- ========== AUTORES ==========

CREATE OR ALTER PROCEDURE dbo.usp_Autores_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AutorId, Nombre
    FROM Autores
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

-- ========== SOCIOS ==========

CREATE OR ALTER PROCEDURE dbo.usp_Socios_ListarActivos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT SocioId, DNI, Nombre, Email, Activo
    FROM Socios
    WHERE Activo = 1
    ORDER BY Nombre;
END
GO

-- Devuelve el Id nuevo, o -1 si el DNI ya existe.
CREATE OR ALTER PROCEDURE dbo.usp_Socios_Insertar
    @DNI    NVARCHAR(8),
    @Nombre NVARCHAR(100),
    @Email  NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Socios WHERE DNI = @DNI)
    BEGIN
        SELECT -1;
        RETURN;
    END
    INSERT INTO Socios (DNI, Nombre, Email, Activo) VALUES (@DNI, @Nombre, @Email, 1);
    SELECT CAST(SCOPE_IDENTITY() AS INT);
END
GO

-- ========== PRÉSTAMOS ==========

-- Reporte por intervalo de FechaPrestamo (inclusive). Una fila por libro prestado.
CREATE OR ALTER PROCEDURE dbo.usp_Prestamos_Reporte
    @Desde DATE,
    @Hasta DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT p.PrestamoId, s.Nombre AS Socio, s.DNI, l.Titulo AS Libro,
           p.FechaPrestamo, p.FechaLimite, d.FechaDevolucion, p.Estado
    FROM Prestamos p
    INNER JOIN DetallePrestamo d ON d.PrestamoId = p.PrestamoId
    INNER JOIN Libros l          ON l.LibroId    = d.LibroId
    INNER JOIN Socios s          ON s.SocioId    = p.SocioId
    WHERE p.FechaPrestamo BETWEEN @Desde AND @Hasta
    ORDER BY p.FechaPrestamo DESC, p.PrestamoId, l.Titulo;
END
GO
