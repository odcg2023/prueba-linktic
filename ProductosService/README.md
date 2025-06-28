
# 📦 ProductosService

Microservicio desarrollado en **.NET 7**, basado en Clean Architecture adaptada, con:

✅ **JWT Authentication** (AES-256 CBC en claims)  
✅ **UnitOfWork + Repositorio Genérico**  
✅ **JSON:API** como contrato uniforme  
✅ **Logs estructurados con Serilog**  
✅ **HealthChecks**  
✅ **Pruebas unitarias (xUnit + Moq)** con +80% cobertura.

---

## 🗂 Estructura del proyecto

```
/ProductosService.sln
│
├── ProductosService.Application
│   ├── Dtos
│   ├── Interfaces
│   ├── Mapping
│   ├── Services
│   ├── Exceptions
│   └── Helpers
│
├── ProductosService.Domain
│   ├── Entity
│   └── Interfaces
│
├── ProductosService.Infraestructure
│   ├── Context
│   └── Repository
│
├── ProductosService.Common
│
├── ProductosService.Service
│   ├── Controllers
│   ├── HealthChecks
│   ├── Helpers
│   └── Middleware
│
└── ProductosService.UnitTests
```

---

## 🔐 Seguridad

- Autenticación con **JWT Bearer**, configurado en `Program.cs`.
- Validación estricta de `Issuer`, `Audience` y lifetime.
- Los claims dentro del token se encriptan con **AES-256 CBC**, usando la llave `KeyCrypto` del `AppConstants`.

---

## 🩺 Health Checks

- Verifica la conexión a base de datos.
- Expuesto en `/health` devolviendo:

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0016221",
  "entries": {}
}
```

---

## 🖥️ Logs estructurados

- Se generan automáticamente con **Serilog**:
  - Captura `400` (excepciones de negocio) y `500` (errores inesperados).
  - Guarda en `Logs/log-*.json`.

Ejemplo:

```json
{
  "@t": "2025-06-27T03:12:54.001Z",
  "@m": "Error de validación o negocio",
  "@l": "Warning",
  "RequestPath": "/api/Productos/crear-producto",
  "Exception": "System.ApplicationException: Ya existe un producto con el nombre: ...",
  "RequestId": "0H..."
}
```

---

## 📝 Respuestas JSON uniformes

### Éxito (`200 OK`)

```json
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
  }
}
```

### Error de negocio (`400` BadRequest)

```json
{
  "errors": [
    {
      "status": "400",
      "title": "Bad Request",
      "detail": "Ya existe un producto con el nombre: Producto A"
    }
  ],
  "meta": {
    "success": false,
    "message": "Error al procesar la solicitud. Valide los valores enviados."
  }
}
```

### No encontrado (`404` NotFound)

```json
{
  "errors": [
    {
      "status": "404",
      "title": "NotFound",
      "detail": "No existe un producto con ID = 123"
    }
  ],
  "meta": {
    "success": false,
    "message": "Error al obtener producto"
  }
}
```

### Error inesperado (`500` InternalServerError)

```json
{
  "errors": [
    {
      "status": "500",
      "title": "Internal Server Error",
      "detail": "Ha ocurrido un error inesperado."
    }
  ],
  "meta": {
    "success": false,
    "message": "Error al procesar la solicitud."
  }
}
```

---

## ⚙️ Configuración `appsettings.json`

```json
{
  "Authentication": {
    "Issuer": "ProductosAPI",
    "Audience": "PruebaTecnica",
    "JwtExpired": 20
  },
  "_comments": {
    "SecretKey": "Se obtiene desde AppConstants.CryptoKeys.KeyToken",
    "KeyCrypto": "Se usa para encriptar/desencriptar AES-256 con CipherMode.CBC"
  }
}
```

---

## 🧪 Pruebas unitarias

- Se usa **xUnit + Moq** para la capa `Application`.
- Se cubren escenarios como:
  - Creación válida y duplicados.
  - Consultas de producto por ID (existente / no existente).
  - Fallos simulados en repositorio (errores BD).
