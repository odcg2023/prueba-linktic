# Prueba Técnica - Microservicios Clean Architecture

## 🏗️ Descripción general

Este proyecto implementa un conjunto de **microservicios desarrollados en .NET 7**, basados en una adaptación de **Clean Architecture**, que incluyen:

✅ **JWT Authentication** (AES-256 CBC en claims)  
✅ **UnitOfWork + Repositorio Genérico**  
✅ **JSON:API** como contrato uniforme  
✅ **Logs estructurados con Serilog**  
✅ **HealthChecks** expuestos en `/health`  
✅ **Pruebas unitarias (xUnit + Moq)** con +80% cobertura

El diseño sigue el principio de **Clean Architecture**, donde las dependencias siempre están dirigidas hacia afuera.

---

## 🔒 JWT Authentication

- Configuración de `Issuer`, `Audience` y expiración (`JwtExpired`) en `appsettings.json`.
- La clave `SecretKey` no está expuesta en el archivo, se obtiene desde:

    AppConstants.CryptoKeys.KeyToken

### Encriptación de claims y datos críticos

- Usamos AES-256 con `CipherMode.CBC`.
- La llave para encriptación proviene de:

    AppConstants.CryptoKeys.KeyCrypto

---

## ⚙️ Configuración de conexión cifrada

El archivo `appsettings.json` contiene placeholders para las cadenas de conexión y niveles de seguridad:

    "ConnectionStrings": {
      "Db": "Server=[SERVER];DataBase=[DATABASE];User=[USER];Password=[PASSWORD];..."
```    },
    "Levels": {
      "Level1": "...",
      "Level2": "...",
      "Level3": "...",
      "Level4": "..."
```    }

Estos valores son desencriptados mediante `CryptoHelper` usando claves definidas en `AppConstants.CryptoKeys`.

## 💾 Health Checks

Expuestos en la ruta:

    /health

Ejemplo:

    https://localhost:{PORT}/health

---

## 📜 Logs estructurados con Serilog

Serilog gestiona logs estructurados a nivel global, capturando:

- Errores no controlados (por ejemplo, fallos de infraestructura).
- Errores de negocio (`ApplicationException`).

Ejemplo de log:

    {
      "@t": "2025-06-27T01:23:45Z",
      "@m": "Error de validación o negocio",
      "@l": "Warning",
      "@x": "System.ApplicationException: Usuario y/o password inválidos"
```    }


## 📦 Respuestas JSON:API estandarizadas

### ✅ Éxito (`200 OK`)

    {
      "data": {
        "type": "producto",
        "attributes": {
          "idProducto": 1,
          "nombreProducto": "Producto A",
          "descripcion": "Prueba",
          "precio": 1000,
          "activo": true
        }
      },
      "meta": {
        "success": true,
        "message": "Petición ejecutada de forma correcta"
```      }
    }
```
---

### ⚠️ Error de negocio (`400 BadRequest`)

    {
      "errors": [
        {
          "status": "400",
          "title": "Bad Request",
          "detail": "Ya existe un producto con el nombre: Producto A"
```        }
      ],
      "meta": {
        "success": false,
        "message": "Error al procesar la solicitud. Valide los valores enviados."
```      }
    }
```
---

### 🔍 No encontrado (`404 NotFound`)

    {
      "errors": [
        {
          "status": "404",
          "title": "NotFound",
          "detail": "No existe un producto con ID = 123"
```        }
      ],
      "meta": {
        "success": false,
        "message": "Error al obtener producto"
```      }
    }
```
---

### 💥 Error inesperado (`500 InternalServerError`)

    {
      "errors": [
        {
          "status": "500",
          "title": "Internal Server Error",
          "detail": "Ha ocurrido un error inesperado."
```        }

## 🧪 Pruebas unitarias

- Se usa **xUnit + Moq** para probar la capa `Application`.
- Se cubren escenarios como:
  - Creación válida y detección de duplicados.
  - Consultas de producto por ID (existente / no existente).
  - Simulación de fallos en repositorio (errores de base de datos).

---

## 🐳 Dockerización futura

**NOTA:**  
Por temas de tiempo no se alcanzó a dockerizar completamente.  
Se definirá un stack que incluya:

- **SQL Server**
- Variables de entorno seguras para claves JWT y conexión cifrada.

---

## 🗄️ Scripts de base de datos y modelos ER

### Microservicio Seguridad (`SeguridadService`)### Microservicio Seguridad (SeguridadService)

SCRIPT SQL:

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Usuarios')
BEGIN
    EXEC('CREATE SCHEMA Usuarios');
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Usuarios' AND TABLE_NAME = 'Usuario'
```)
BEGIN
    CREATE TABLE [Usuarios].[Usuario]
    (
        IdUsuario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Login NVARCHAR(50) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        UsuarioCreacion SMALLINT NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioModificacion SMALLINT NULL,
        FechaModificacion DATETIME NULL
    );
END;
GO

---

MODELO ENTIDAD RELACIÓN SeguridadService:

Entidad: Usuarios.Usuario
---------------------------------
- IdUsuario (PK)
- Nombre
- Login (UNIQUE)
- Password
- Activo
- UsuarioCreacion
- FechaCreacion
- UsuarioModificacion
- FechaModificacion

(No hay claves foráneas en este microservicio)


### Microservicio Compras (ComprasService)

SCRIPT SQL:

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Compras')
BEGIN
    EXEC('CREATE SCHEMA Compras');
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Compras' AND TABLE_NAME = 'Compra'
```)
BEGIN
    CREATE TABLE [Compras].[Compra]
    (
        IdCompra INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCliente INT NOT NULL,
        IdMovimientoInventario INT NOT NULL,
        FechaCompra DATETIME NOT NULL DEFAULT GETDATE(),
        TotalItems INT NOT NULL,
        ValorTotalCompra DECIMAL(18,2) NOT NULL,
        UsuarioCreacion SMALLINT NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioModificacion SMALLINT NULL,
        FechaModificacion DATETIME NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Compras' AND TABLE_NAME = 'CompraDetalle'
```)
BEGIN
    CREATE TABLE [Compras].[CompraDetalle]
    (
        IdCompraDetalle INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdCompra INT NOT NULL,
        IdProducto INT NOT NULL,
        CantidadProducto INT NOT NULL,
        UsuarioCreacion SMALLINT NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioModificacion SMALLINT NULL,
        FechaModificacion DATETIME NULL,
        CONSTRAINT FK_CompraDetalle_Compra FOREIGN KEY (IdCompra)
            REFERENCES [Compras].[Compra](IdCompra)
    );
END;
GO

---

MODELO ENTIDAD RELACIÓN ComprasService:

Entidad: Compras.Compra
---------------------------------
- IdCompra (PK)
- IdCliente
- IdMovimientoInventario
- FechaCompra
- TotalItems
- ValorTotalCompra
- UsuarioCreacion
- FechaCreacion
- UsuarioModificacion
- FechaModificacion

Entidad: Compras.CompraDetalle
---------------------------------
- IdCompraDetalle (PK)
- IdCompra (FK → Compras.Compra.IdCompra)
- IdProducto
- CantidadProducto
- UsuarioCreacion
- FechaCreacion
- UsuarioModificacion
- FechaModificacion

### Microservicio Productos (ProductosService)

SCRIPT SQL:

DROP TABLE IF EXISTS [Productos].[Producto];
GO

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Productos')
BEGIN
    EXEC('CREATE SCHEMA Productos');
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Productos' AND TABLE_NAME = 'Producto'
```)
BEGIN
    CREATE TABLE [Productos].[Producto]
    (
        IdProducto INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        NombreProducto NVARCHAR(100) NOT NULL,
        Descripcion NVARCHAR(255) NULL,
        Precio DECIMAL(18,2) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        UsuarioCreacion SMALLINT NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioModificacion SMALLINT NULL,
        FechaModificacion DATETIME NULL
    );
END;
GO

---

MODELO ENTIDAD RELACIÓN ProductosService:

Entidad: Productos.Producto
---------------------------------
- IdProducto (PK)
- NombreProducto
- Descripcion
- Precio
- Activo
- UsuarioCreacion
- FechaCreacion
- UsuarioModificacion
- FechaModificacion


### Microservicio Inventarios (InventariosService)

SCRIPT SQL:

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'Inventarios')
BEGIN
    EXEC('CREATE SCHEMA Inventarios');
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Inventarios' AND TABLE_NAME = 'Inventario'
```)
BEGIN
    CREATE TABLE [Inventarios].[Inventario]
    (
        IdInventario INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        IdProducto INT NOT NULL UNIQUE,
        ExistenciasActuales INT NOT NULL,
        UsuarioCreacion SMALLINT NOT NULL,
        FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
        UsuarioModificacion SMALLINT,
        FechaModificacion DATETIME NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Inventarios' AND TABLE_NAME = 'InventarioMovimiento'
```)
BEGIN
    CREATE TABLE Inventarios.InventarioMovimiento
    (
      IdMovimiento INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
      TipoMovimiento TINYINT NOT NULL,
      Observaciones VARCHAR(255) NULL,
      UsuarioCreacion SMALLINT NOT NULL,
      FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.TABLES 
    WHERE TABLE_SCHEMA = 'Inventarios' AND TABLE_NAME = 'InventarioMovimientoDetalle'
```)
BEGIN
    CREATE TABLE Inventarios.InventarioMovimientoDetalle
    (
      IdMovimientoDetalle INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
      IdMovimiento INT NOT NULL,
      IdInventario INT NOT NULL,
      CantidadAntes INT NOT NULL,
      CantidadMovimiento INT NOT NULL,
      CantidadDespues INT NOT NULL,
      Observaciones VARCHAR(255) NULL,
      UsuarioCreacion SMALLINT NOT NULL,
      FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
      CONSTRAINT FK_InventarioMovimientoDetalle_InventarioMovimiento 
          FOREIGN KEY (IdMovimiento)
          REFERENCES Inventarios.InventarioMovimiento(IdMovimiento),
      CONSTRAINT FK_InventarioMovimientoDetalle_Inventario 
          FOREIGN KEY (IdInventario)
          REFERENCES Inventarios.Inventario(IdInventario)
    );
END;
GO

---

MODELO ENTIDAD RELACIÓN InventariosService:

Entidad: Inventarios.Inventario
---------------------------------
- IdInventario (PK)
- IdProducto (UNIQUE)
- ExistenciasActuales
- UsuarioCreacion
- FechaCreacion
- UsuarioModificacion
- FechaModificacion

Entidad: Inventarios.InventarioMovimiento
---------------------------------
- IdMovimiento (PK)
- TipoMovimiento
- Observaciones
- UsuarioCreacion
- FechaCreacion

Entidad: Inventarios.InventarioMovimientoDetalle
---------------------------------
- IdMovimientoDetalle (PK)
- IdMovimiento (FK → Inventarios.InventarioMovimiento.IdMovimiento)
- IdInventario (FK → Inventarios.Inventario.IdInventario)
- CantidadAntes
- CantidadMovimiento
- CantidadDespues
- Observaciones
- UsuarioCreacion
- FechaCreacion


## 📂 Estructura del microservicio SeguridadService

```plaintext
SeguridadService.sln
│
├───SeguridadService.Application
│   │   SeguridadService.Application.csproj
│   │
│   ├───Dto
│   │   │   LoggedDto.cs
│   │   │   LoginRequestDto.cs
│   │   │   LoginResponseDto.cs
│   │   │
│   │   └───JsonResponse
│   │           JsonApiData.cs
│   │           JsonApiError.cs
│   │           JsonApiErrorResponse.cs
│   │           JsonApiResponse.cs
│   │           Meta.cs
│   │
│   ├───Exceptions
│   │       ApplicationException.cs
│   │
│   ├───Helpers
│   │       CryptoHelper.cs
│   │
│   ├───Interfaces
│   │       ICryptoHelper.cs
│   │       ILoginService.cs
│   │
│   ├───Mapping
│   │       Mapper.cs
│   │
│   └───Services
│           LoginService.cs
│           ServiceBase.cs
│
├───SeguridadService.Common
│   │   AppConstants.cs
│   │   Crypto.cs
│   │   SeguridadService.Common.csproj
│
├───SeguridadService.Domain
│   │   SeguridadService.Domain.csproj
│   │
│   ├───Entities
│   │       Usuario.cs
│   │
│   └───Interfaces
│       └───Repositorio
│               IGenericRepository.cs
│               IUnitOfWork.cs
│
├───SeguridadService.Infraestructure
│   │   SeguridadService.Infraestructure.csproj
│   │
│   ├───Context
│   │       ContextSeguridad.cs
│   │
│   └───Repository
│           GenericRepository.cs
│           UnitOfWork.cs
│
├───SeguridadService.Service
│   │   appsettings.Development.json
│   │   appsettings.json
│   │   Program.cs
│   │   SeguridadService.Service.csproj
│   │   SeguridadService.Service.csproj.user
│   │
│   ├───Controllers
│   │       LoginController.cs
│   │
│   ├───HealthChecks
│   │       HealthCheckConfig.cs
│   │       LoggingHealthCheckPublisher.cs
│   │
│   ├───Helpers
│   │       ConfigurationExtensions.cs
│   │       JsonApiResponseFactory.cs
│   │       JwtHelper.cs
│   │
│   ├───Middleware
│   │       GlobalExceptionMiddleware.cs
│   │
│   └───Properties
│           launchSettings.json
│
├───SeguridadService.UnitTests
│   │   SeguridadService.UnitTests.csproj
│   │
│   └───SeguridadService.Tests
│       └───Application
│           └───Services
│                   SeguridadServiceTests.cs


## 📂 Estructura del microservicio ProductosService

```plaintext
ProductosService.sln
│
├───ProductosService.Application
│   │   ProductosService.Application.csproj
│   │
│   ├───Dto
│   │   │   ProductoDto.cs
│   │   │   ProductoNuevoDto.cs
│   │   │
│   │   └───JsonResponse
│   │           JsonApiData.cs
│   │           JsonApiError.cs
│   │           JsonApiErrorResponse.cs
│   │           JsonApiResponse.cs
│   │           Meta.cs
│   │
│   ├───Exceptions
│   │       ApplicationException.cs
│   │
│   ├───Helpers
│   │       CryptoHelper.cs
│   │
│   ├───Interfaces
│   │       ICryptoHelper.cs
│   │       ICurrentUserService.cs
│   │       IProductoService.cs
│   │
│   ├───Mapping
│   │       Mapper.cs
│   │
│   └───Services
│           ProductoService.cs
│           ServiceBase.cs
│
├───ProductosService.Common
│   │   AppConstants.cs
│   │   Crypto.cs
│   │   ProductosService.Common.csproj
│
├───ProductosService.Domain
│   │   ProductosService.Domain.csproj
│   │
│   ├───Entities
│   │       Producto.cs
│   │
│   └───Interfaces
│       └───Repositorio
│               IGenericRepository.cs
│               IUnitOfWork.cs
│
├───ProductosService.Infraestructure
│   │   ProductosService.Infraestructure.csproj
│   │
│   ├───Context
│   │       ContextProductos.cs
│   │
│   └───Repository
│           GenericRepository.cs
│           UnitOfWork.cs
│
├───ProductosService.Service
│   │   appsettings.Development.json
│   │   appsettings.json
│   │   ProductosService.Service.csproj
│   │   ProductosService.Service.csproj.user
│   │   Program.cs
│   │
│   ├───Controllers
│   │       ProductosController.cs
│   │
│   ├───HealthChecks
│   │       HealthCheckConfig.cs
│   │       LoggingHealthCheckPublisher.cs
│   │
│   ├───Helpers
│   │       ConfigurationExtensions.cs
│   │       CurrentUserService.cs
│   │       JsonApiResponseFactory.cs
│   │
│   ├───Middleware
│   │       GlobalExceptionMiddleware.cs
│   │
│   └───Properties
│           launchSettings.json
│
└───ProductosService.UnitTests
    │   ProductosService.UnitTests.csproj
    │
    └───ProductosService.Tests
        └───Application
            └───Services
                    ProductoServiceTests.cs



## 📂 Estructura del microservicio InventariosService

```plaintext
InventariosService.sln
│
├───InventariosService.Application
│   │   InventariosService.Application.csproj
│   │
│   ├───Dto
│   │   │   ActualizaInventarioDto.cs
│   │   │   InventarioCompraActualizadoDto.cs
│   │   │   InventarioMovimientoDto.cs
│   │   │   InventarioProductoDto.cs
│   │   │   ProductoInventarioDto.cs
│   │   │   ProductosCompraDto.cs
│   │   │
│   │   ├───Integrations
│   │   │       ProductoDto.cs
│   │   │
│   │   └───JsonResponse
│   │           JsonApiData.cs
│   │           JsonApiError.cs
│   │           JsonApiErrorResponse.cs
│   │           JsonApiResponse.cs
│   │           Meta.cs
│   │
│   ├───Enumerations
│   │       Enumeration.cs
│   │
│   ├───Exceptions
│   │       ApplicationException.cs
│   │
│   ├───Helpers
│   │       CryptoHelper.cs
│   │
│   ├───Interfaces
│   │   │   ICryptoHelper.cs
│   │   │   ICurrentUserService.cs
│   │   │   IInventarioService.cs
│   │   │
│   │   └───Integrations
│   │           IProductoApiService.cs
│   │
│   ├───Mapping
│   │       Mapper.cs
│   │
│   └───Services
│           InventarioService.cs
│           ServiceBase.cs
│
├───InventariosService.Common
│   │   AppConstants.cs
│   │   Crypto.cs
│   │   InventariosService.Common.csproj
│
├───InventariosService.Domain
│   │   InventariosService.Domain.csproj
│   │
│   ├───Entities
│   │       Inventario.cs
│   │       InventarioMovimiento.cs
│   │       InventarioMovimientoDetalle.cs
│   │
│   └───Interfaces
│       └───Repositorio
│               IGenericRepository.cs
│               ITransaction.cs
│               IUnitOfWork.cs
│
├───InventariosService.Infraestructure
│   │   InventariosService.Infraestructure.csproj
│   │
│   ├───Context
│   │       ContextInventarios.cs
│   │
│   └───Repository
│           EfTransaction.cs
│           GenericRepository.cs
│           UnitOfWork.cs
│
├───InventariosService.Infraestructure.Integrations
│   │   InventariosService.Infraestructure.Integrations.csproj
│   │
│   └───Services
│           ProductoApiService.cs
│
├───InventariosService.Service
│   │   appsettings.Development.json
│   │   appsettings.json
│   │   InventariosService.Service.csproj
│   │   InventariosService.Service.csproj.user
│   │   Program.cs
│   │
│   ├───Controllers
│   │       InventariosController.cs
│   │
│   ├───HealthChecks
│   │       HealthCheckConfig.cs
│   │       LoggingHealthCheckPublisher.cs
│   │
│   ├───Helpers
│   │       ConfigurationExtensions.cs
│   │       CurrentUserService.cs
│   │       JsonApiResponseFactory.cs
│   │
│   ├───Logs
│   │       log-20250628.json
│   │
│   ├───Middleware
│   │       GlobalExceptionMiddleware.cs
│   │
│   └───Properties
│           launchSettings.json
│
└───InventariosService.UnitTests
    │   InventariosService.UnitTests.csproj
    │
    ├───InventariosService.Tests
    │   └───Application
    │       └───Services
    │               InventarioServiceTests.cs


## 📂 Estructura del microservicio ComprasService

```plaintext
ComprasService.sln
│
├───ComprasService.Application
│   │   ComprasService.Application.csproj
│   │
│   ├───Dto
│   │   │   CompraDetalleDto.cs
│   │   │   CompraDto.cs
│   │   │   RegistraCompraDto.cs
│   │   │   RegistrarCompraDetalleDto.cs
│   │   │
│   │   ├───Integrations
│   │   │       InventarioCompraActualizadoDto.cs
│   │   │       InventarioProductoDto.cs
│   │   │       ProductoDto.cs
│   │   │       ProductosCompraDto.cs
│   │   │
│   │   └───JsonResponse
│   │           JsonApiData.cs
│   │           JsonApiError.cs
│   │           JsonApiErrorResponse.cs
│   │           JsonApiResponse.cs
│   │           Meta.cs
│   │
│   ├───Exceptions
│   │       ApplicationException.cs
│   │
│   ├───Helpers
│   │       CryptoHelper.cs
│   │
│   ├───Interfaces
│   │   │   ICompraService.cs
│   │   │   ICryptoHelper.cs
│   │   │   ICurrentUserService.cs
│   │   │
│   │   └───Integrations
│   │           IInventarioApiService.cs
│   │           IProductoApiService.cs
│   │
│   ├───Mapping
│   │       Mapper.cs
│   │
│   └───Services
│           CompraService.cs
│           ServiceBase.cs
│
├───ComprasService.Common
│   │   AppConstants.cs
│   │   ComprasService.Common.csproj
│   │   Crypto.cs
│
├───ComprasService.Domain
│   │   ComprasService.Domain.csproj
│   │
│   ├───Entities
│   │       Compra.cs
│   │       CompraDetalle.cs
│   │
│   └───Interfaces
│       └───Repositorio
│               IGenericRepository.cs
│               IUnitOfWork.cs
│
├───ComprasService.Infraestructure
│   │   ComprasService.Infraestructure.csproj
│   │
│   ├───Context
│   │       ContextCompras.cs
│   │
│   └───Repository
│           GenericRepository.cs
│           UnitOfWork.cs
│
├───ComprasService.Infraestructure.Integrations
│   │   ComprasService.Infraestructure.Integrations.csproj
│   │
│   └───Services
│           InventarioApiService.cs
│           ProductoApiService.cs
│
├───ComprasService.Service
│   │   appsettings.Development.json
│   │   appsettings.json
│   │   ComprasService.Service.csproj
│   │   ComprasService.Service.csproj.user
│   │   Program.cs
│   │
│   ├───Controllers
│   │       ComprasController.cs
│   │
│   ├───HealthChecks
│   │       HealthCheckConfig.cs
│   │       LoggingHealthCheckPublisher.cs
│   │
│   ├───Helpers
│   │       ConfigurationExtensions.cs
│   │       CurrentUserService.cs
│   │       JsonApiResponseFactory.cs
│   │
│   ├───Logs
│   │       log-20250628.json
│   │
│   ├───Middleware
│   │       GlobalExceptionMiddleware.cs
│   │
│   └───Properties
│           launchSettings.json
│
└───ComprasService.UnitTests
    │   ComprasService.UnitTests.csproj
    │
    └───ComprasService.Tests
        └───Application
            └───Services
                    ComprasServiceTests.cs

---


## 🚀 Ejemplos de consumo del API ComprasService

### 🔥 POST `/api/compras/registrar-compra`

Registra una nueva compra en el sistema.

#### 📤 Request
```bash
```bash
curl -X POST https://localhost:7150/api/Compras/registrar-compra   -H "Authorization: Bearer TU_TOKEN_AQUI"   -H "Content-Type: application/json"   -d 
'{
  "idCliente": 45,
  "productos": [
    {
      "idProducto": 2,
      "cantidadProducto": 10
    }
  ]
}'
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "compra",
    "attributes": {
      "idCompra": 2,
      "idCliente": 45,
      "fechaCompra": "2025-06-28T21:21:10.193",
      "totalItems": 1,
      "valorTotalCompra": 123000,
      "productos": [
        {
          "idProducto": 2,
          "nombreProducto": "Crema Colgate 50mg",
          "cantidadProducto": 10,
          "precioUnitario": 12300,
          "total": 123000
        }
      ]
    }
  },
  "meta": {
    "success": true,
    "message": "Compra registrada correctamete"
```  }
}
```

---

### 📚 GET `/api/compras/obtener-compras`

Obtiene todas las compras registradas.

#### 📤 Request
```bash
```bash
curl -X GET https://localhost:7150/api/Compras/obtener-compras   -H "Authorization: Bearer TU_TOKEN_AQUI"
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "compra",
    "attributes": [
      {
        "idCompra": 1,
        "idCliente": 123,
        "fechaCompra": "2025-06-28T19:28:54.83",
        "totalItems": 2,
        "valorTotalCompra": 638400,
        "productos": [
          {
            "idProducto": 2,
            "nombreProducto": "Crema Colgate 50mg",
            "cantidadProducto": 8,
            "precioUnitario": 12300,
            "total": 98400
          },
          {
            "idProducto": 4,
            "nombreProducto": "Alcohol Panda 1L",
            "cantidadProducto": 12,
            "precioUnitario": 45000,
            "total": 540000
          }
        ]
      }
    ]
  },
  "meta": {
    "success": true,
    "message": "Compras obtenidas correctamente"
```  }
}
```

---

### 🔍 GET `/api/compras/obtener-compra-por-id/{id}`

Obtiene una compra específica por su ID.

#### 📤 Request
```bash
```bash
curl -X GET https://localhost:7150/api/Compras/obtener-compra-por-id/2   -H "Authorization: Bearer TU_TOKEN_AQUI"
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "compra",
    "attributes": {
      "idCompra": 2,
      "idCliente": 45,
      "fechaCompra": "2025-06-28T21:21:10.193",
      "totalItems": 1,
      "valorTotalCompra": 123000,
      "productos": [
        {
          "idProducto": 2,
          "nombreProducto": "Crema Colgate 50mg",
          "cantidadProducto": 10,
          "precioUnitario": 12300,
          "total": 123000
        }
      ]
    }
  },
  "meta": {
    "success": true,
    "message": "Compra 2 obtenida correctamente"
```  }
}
```

#### ❌ Response JSON:API (`404 NotFound`)
```json
{
  "errors": [
    {
      "status": "404",
      "title": "NotFound",
      "detail": "No existe una compra con Id = 99"
```    }
  ],
  "meta": {
    "success": false,
    "message": "Compra no encontrada"
```  }
}
```

#### ❌ Response JSON:API (`500 Error interno del servidor`)
```json
{
  "errors": [
    {
      "status": "string",
      "title": "string",
      "detail": "string"
```    }
  ],
  "meta": {
    "success": true,
    "message": "string"
```  }
}
```

## 🚀 Ejemplos de consumo del API SeguridadService

### 🔥 POST `/api/Login`

Autentica un usuario con su login y contraseña, devolviendo un token JWT si es exitoso.

#### 📤 Request
```bash
```bash
curl -X POST https://localhost:7149/api/Login 
'{
  "usuarioLogin": "prueba",
  "password": "Fm/l0FLs22vndIL4lWJmQQ=="
```}'
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "login",
    "attributes": {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJFeHByZXNpb24xIjoiblAzZzBZK1BOWmVheGIvM0pXZVFudz09IiwiRXhwcmVzaW9uMiI6IlM4K202WXBtTElkTTVGVVNFbFlNY3c9PSIsIm5iZiI6MTc1MTE3MTI1NiwiZXhwIjoxNzUxMTcyNDU2LCJpc3MiOiJTZWd1cmlkYWRMb2dpbiIsImF1ZCI6IlBydWViYVRlY25pY2EifQ.roi_OTxynwRWAV5PbtJDedQ6GYDGif6FtAUy1mn6maw"
```    }
  },
  "meta": {
    "success": true,
    "message": "Inicio de sesión exitoso"
```  }
}
```


## 🚀 Ejemplos de consumo del API ProductosService

### 🔥 GET `/api/Productos/obtener-producto-por-id`

Obtiene un producto por su identificador único..

#### 📤 Request
```bash
```bash
curl -X GET https://localhost:7148/api/Productos/obtener-producto-por-id/2  -H "Authorization: Bearer TU_TOKEN_AQUI"   -H "Content-Type: application/json"   -d 

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "producto",
    "attributes": {
      "idProducto": 2,
      "nombreProducto": "Crema Colgate 50mg",
      "descripcion": "Higiene Bucal",
      "precio": 12300,
      "activo": true
    }
  },
  "meta": {
    "success": true,
    "message": "Petición ejecutada de forma correcta"
```  }
}
```

---

### 📚 GET `/api/Productos/obtener-productos`

Obtiene la lista de todos los productos registrados.

#### 📤 Request
```bash
```bash
curl -X GET https://localhost:7148/api/Productos/obtener-productos   -H "Authorization: Bearer TU_TOKEN_AQUI"
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "producto",
    "attributes": [
      {
        "idProducto": 1,
        "nombreProducto": "LISTERINE 500MG",
        "descripcion": "Higiene bucal",
        "precio": 23700,
        "activo": true
      },
      {
        "idProducto": 2,
        "nombreProducto": "Crema Colgate 50mg",
        "descripcion": "Higiene Bucal",
        "precio": 12300,
        "activo": true
      },
      {
        "idProducto": 3,
        "nombreProducto": "Shampoo Pantene Pro V",
        "descripcion": "Cuidado Personal",
        "precio": 23000,
        "activo": true
      },
      {
        "idProducto": 4,
        "nombreProducto": "Alcohol Panda 1L",
        "descripcion": "Alcohol",
        "precio": 45000,
        "activo": true
      }
    ]
  },
  "meta": {
    "success": true,
    "message": "Petición ejecutada de forma correcta"
```  }
}
```

---

### 🔍 POST `/api/Productos/crear-producto`

Crea un nuevo producto en el sistema.

#### 📤 Request
```bash
```bash
curl -X POST https://localhost:7148/api/Productos/crear-producto  -H "Authorization: Bearer TU_TOKEN_AQUI"
```'{
  "nombreProducto": "Tio Nacho Manzanilla",
  "descripcion": "Shampoo",
  "precio": 73500
}'
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "producto",
    "attributes": {
      "idProducto": 5,
      "nombreProducto": "Tio Nacho Manzanilla",
      "descripcion": "Shampoo",
      "precio": 73500,
      "activo": true
    }
  },
  "meta": {
    "success": true,
    "message": "Petición ejecutada de forma correcta"
```  }
}
```

#### ❌ Response JSON:API (`404 NotFound`)
```json
{
  "errors": [
    {
      "status": "404",
      "title": "NotFound",
      "detail": "Excepcion"
```    }
  ],
  "meta": {
    "success": false,
    "message": "string"
```  }
}
```

#### ❌ Response JSON:API (`500 Error interno del servidor`)
```json
{
  "errors": [
    {
      "status": "string",
      "title": "string",
      "detail": "string"
```    }
  ],
  "meta": {
    "success": true,
    "message": "string"
```  }
}
```


## 🚀 Ejemplos de consumo del API InventariosService

### 🔥 GET `/api/Inventario/obtener-existencias`

Obtiene las existencias actuales de un producto en el inventario.

#### 📤 Request
```bash
```bash
curl -X GET https://localhost:7159/api/Inventario/obtener-existencias/2  -H "Authorization: Bearer TU_TOKEN_AQUI"   

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "inventario",
    "attributes": {
      "idProducto": 2,
      "existenciasActuales": 67
    }
  },
  "meta": {
    "success": true,
    "message": "Existencias del producto 2 obtenidas correctamente"
```  }
}
```

---

### 🔍 POST `/api/Inventario/actualizar-inventario`

Actualiza las existencias de un producto en el inventario.

#### 📤 Request
```bash
```bash
curl -X POST https://localhost:7159/api/Inventario/actualizar-inventario  -H "Authorization: Bearer TU_TOKEN_AQUI"
```'{
  "producto": {
    "idProducto": 2,
    "cantidad": 50
  },
  "tipoMovimiento": 1
}'
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "inventario",
    "attributes": {
      "idProducto": 2,
      "existenciasActuales": 117
    }
  },
  "meta": {
    "success": true,
    "message": "Existencias actualizadas correctamente"
```  }
}
```

---

### 🔍 POST `/api/Inventario/actualizar-inventario-compras`

Actualiza el inventario de productos al realizar una compra.

#### 📤 Request
```bash
```bash
curl -X POST https://localhost:7159/api/Inventario/actualizar-inventario-compras -H "Authorization: Bearer TU_TOKEN_AQUI"
```'{
  "tipoMovimiento": 2,
  "listaProductos": [
    {
      "idProducto": 3,
      "cantidad": 7
    }
  ],
  "observaciones": "Compra efectuada hoy"
```}'
``````

#### ✅ Response JSON:API (`200 OK`)
```json
```json
{
  "data": {
    "type": "inventario",
    "attributes": {
      "idMovimiento": 11,
      "tipoMovimiento": 2
    }
  },
  "meta": {
    "success": true,
    "message": "Existencias por compras actualizadas correctamente"
```  }
}
```

---

#### ❌ Response JSON:API (`404 NotFound`)
```json
{
  "errors": [
    {
      "status": "404",
      "title": "NotFound",
      "detail": "Excepcion"
```    }
  ],
  "meta": {
    "success": false,
    "message": "string"
```  }
}
```

#### ❌ Response JSON:API (`500 Error interno del servidor`)
```json
{
  "errors": [
    {
      "status": "string",
      "title": "string",
      "detail": "string"
```    }
  ],
  "meta": {
    "success": true,
    "message": "string"
```  }
}
```
