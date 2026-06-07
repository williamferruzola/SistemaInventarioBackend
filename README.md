# Sistema Inventario — API Backend

API REST desarrollada en **.NET** para la gestión de inventario, proveedores, catálogo de productos, órdenes de compra y lista de deseos. Expone endpoints consumidos por el panel web de administración (Angular) y la aplicación móvil de compradores (Kotlin/Android).

El proyecto sigue una arquitectura en capas con separación de responsabilidades: dominio, aplicación, infraestructura y capa de presentación (API).

---

## Funcionalidades

- **Autenticación JWT** con roles `Admin` y `Buyer`.
- **Gestión de productos** — CRUD con paginación y búsqueda (rol Admin).
- **Gestión de inventario por proveedor** — Precio, stock y lote por producto (rol Admin).
- **Catálogo para compradores** — Consulta de productos disponibles con ofertas (rol Buyer).
- **Órdenes de compra** — Creación y consulta de pedidos (rol Buyer).
- **Lista de deseos** — Agregar y consultar productos favoritos (rol Buyer).

---

## Stack tecnológico

| Tecnología                              | Versión  | Uso                              |
|-----------------------------------------|----------|----------------------------------|
| .NET                                    | 10.0     | Framework                        |
| ASP.NET Core                            | 10.0.8   | API Web                          |
| Entity Framework Core                   | 10.0.8   | ORM y migraciones                |
| SQL Server                              | 2025     | Base de datos                    |      
| Swashbuckle (Swagger)                   | 7.2.0    | Documentación OpenAPI            |

---

## Requisitos previos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- **SQL Server**
- IDE recomendado: Visual Studio 2022+ o Visual Studio Code

---

## Estructura de la solución

```
SistemaInventario/
├── Domain/                  # Entidades del dominio
│   └── Entities/
├── Application/             # Contratos, DTOs e interfaces de servicios
│   ├── DTOs/
│   └── Interfaces/
├── Infrastructure/          # Implementaciones, EF Core y migraciones
│   ├── Data/
│   ├── Migrations/
│   └── Services/
└── SistemaInventario/       # API (controllers, middleware, configuración)
    ├── Controllers/
    ├── Middlewares/
    ├── Properties/
    ├── appsettings.json
    └── database.sql         # Datos semilla (seed)
```

### Capas

| Capa             | Responsabilidad                                              |
|------------------|--------------------------------------------------------------|
| **Domain**       | Entidades de negocio puras                                   |
| **Application**  | DTOs, interfaces de servicios y reglas de aplicación         |
| **Infrastructure** | Acceso a datos (EF Core), servicios e implementaciones     |
| **SistemaInventario** | Controllers, autenticación, CORS, Swagger y middleware |

---

## Configuración

### Base de datos

Edita la cadena de conexión en `SistemaInventario/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=SistemaInventario;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### JWT

```json
"Jwt": {
  "Key": "tu-clave-secreta-de-al-menos-32-caracteres",
  "Issuer": "SistemaInventario",
  "Audience": "SistemaInventario",
  "ExpirationMinutes": 120
}
```

## Base de datos

### 1. Aplicar migraciones

```bash
dotnet ef database update --project Infrastructure --startup-project SistemaInventario
```

### 2. Cargar datos de prueba (opcional)

Ejecuta el script `SistemaInventario/database.sql` en SQL Server Management Studio (SSMS) o Azure Data Studio contra la base de datos `SistemaInventario`.

### Credenciales de prueba

| Rol           | Email                  | Contraseña     |
|---------------|------------------------|----------------|
| Administrador | `admin@inventario.com` | `P@ssw0rd123!` |
| Comprador     | `buyer@inventario.com` | `P@ssw0rd123!` |

---

## Ejecución

### Restaurar dependencias y compilar

```bash
dotnet restore
dotnet build
```

### Ejecutar la API

```bash
dotnet run --project SistemaInventario
```

## Migraciones

Las migraciones de Entity Framework Core se encuentran en `Infrastructure/Migrations/`.

### Crear una nueva migración

```bash
dotnet ef migrations add NombreMigracion --project Infrastructure --startup-project SistemaInventario
```

### Aplicar migraciones

```bash
dotnet ef database update --project Infrastructure --startup-project SistemaInventario
```

---

## Relación con otros proyectos

| Proyecto                    | Rol                                      |
|-----------------------------|------------------------------------------|
| `SistemaInventario`         | API REST backend (este repositorio)      |
| `SistemaInventarioFrontend` | Panel web de administración (Angular 21) |
| `SistemaCompradores`        | App móvil Android para compradores       |

---

## Recursos

- [Documentación de ASP.NET Core](https://learn.microsoft.com/aspnet/core)
