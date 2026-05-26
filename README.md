# Control de Stock Zamponi

## Descripción
Sistema de gestión de stock para bicicletería.

Permite administrar:
- Productos
- Marcas
- Tipos de productos
- Almacenes

## Tecnologías
- C#
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Swagger

## Funcionalidades actuales
- CRUD de productos
- CRUD de marcas
- CRUD de almacenes
- Relaciones entre tablas
- API REST
- Persistencia con SQL Server

## Funcionalidades futuras
- Frontend interactivo
- Login administrador
- Carga de imágenes
- Control de movimientos de stock
- Dashboard

## Base de datos
Relación:
- Un almacén tiene muchos productos
- Un producto pertenece a una marca
- Un producto pertenece a un tipo

## Cómo ejecutar
1. Clonar repositorio
2. Configurar connection string
3. Ejecutar migraciones

```bash
Add-Migration InitialCreate
Update-Database
