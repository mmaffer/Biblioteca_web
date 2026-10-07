# Biblioteca.Web

Aplicación web de gestión de biblioteca desarrollada con **ASP.NET Core MVC (.NET 10)** y **Dapper**, conectada a SQL Server. Permite administrar libros y socios y consultar un reporte de préstamos.

## Funcionalidades

- **Libros:** listado, búsqueda por título, detalle, registro, edición y eliminación lógica.
- **Socios:** listado y registro, con validación de DNI duplicado.
- **Reporte de préstamos:** consulta por rango de fechas (socio, libro, fecha límite y estado).

## Tecnologías

- ASP.NET Core MVC y Razor
- Dapper y Microsoft.Data.SqlClient
- SQL Server (procedimientos almacenados)
- Bootstrap 5

## Estructura

```
Scripts/            Procedimientos almacenados (Script-Semana08.sql)
lab8/
  Controllers/      Libros, Socios, Prestamos
  Models/           Libro, Socio, Autor, PrestamoReporte
  Repositorios/     Acceso a datos con Dapper
  Views/            Vistas Razor
```

## Requisitos

- .NET SDK 10
- SQL Server (por defecto `.\SQLEXPRESS`)
- Base de datos `BibliotecaDB` con sus tablas y datos de prueba

## Puesta en marcha

1. Crear `BibliotecaDB` con sus tablas y datos de prueba.
2. Ejecutar `Scripts/Script-Semana08.sql` para crear los procedimientos almacenados.
3. Revisar la cadena de conexión en `lab8/appsettings.json` (`ConnectionStrings:BibliotecaDB`).
4. Ejecutar:

```bash
cd lab8
dotnet run
```

La aplicación abre en la sección de Libros.
