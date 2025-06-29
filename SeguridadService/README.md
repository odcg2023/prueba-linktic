
# 🔐 SeguridadService - Microservicio de Autenticación

Este microservicio forma parte de una arquitectura basada en microservicios, desarrollado en **.NET 7**, aplicando principios de limpieza, pruebas y separación de responsabilidades.

Permite **autenticar usuarios mediante login**, y devuelve un **JWT** con claims cifrados usando AES-256 (CipherMode.CBC), consumido por otros microservicios.

---

## 🏗 Estructura del proyecto

```
SeguridadService.sln
│
├── SeguridadService.Application
│   ├── Dto
│   ├── Interfaces
│   ├── Services
│   ├── Exceptions
│   ├── Helpers
│   └── Mapping
│
├── SeguridadService.Domain
│   ├── Entidades
│   └── Interfaces (repositorios)
│
├── SeguridadService.Common
│   ├── AppConstants.cs
│   └── CryptoHelper.cs
│
├── SeguridadService.Infraestructure
│   ├── Context (DbContext DB First)
│   └── Repository (Unidad de Trabajo y Repositorio Genérico)
│
├── SeguridadService.Service
│   ├── Controllers
│   ├── Middleware
│   ├── Helpers (JwtHelper)
│   ├── HealthChecks
│   └── Program.cs
│
└── SeguridadService.UnitTests
    └── Tests.Application.Services
```

✅ Sigue el principio de **Clean Architecture**, con dependencias dirigidas hacia afuera.

---

## ⚙ Configuración

### 🔑 Conexión cifrada
`appsettings.json` tiene placeholders:
```json
"ConnectionStrings": {
  "Db": "Server=[SERVER];DataBase=[DATABASE];User=[USER];Password=[PASSWORD];..."
},
"Levels": {
  "Level1": "...",
  "Level2": "...",
  "Level3": "...",
  "Level4": "..."
}
```
Desencriptados con `CryptoHelper` usando claves en `AppConstants.CryptoKeys`.

---

### 🔒 JWT
- `Issuer`, `Audience` y expiración (`JwtExpired`) configurados en `appsettings.json`.
- La clave `SecretKey` **no se expone allí**, se toma de:
```csharp
AppConstants.CryptoKeys.KeyToken
```

---

### 🔐 AES-256
Para claims y datos críticos:
- Usamos AES-256 con `CipherMode.CBC`.
- La llave se obtiene de:
```csharp
AppConstants.CryptoKeys.KeyCrypto
```

---

## ❤️ Health Checks
Expuestos en:
```
/health
```
Permiten:
- Validar estado general.
- Probar conexión a SQL Server.
- Registrar en logs si hay fallos.

```bash
curl https://localhost:{PORT}/health
```

---

## 🚀 Cómo ejecutar localmente

```bash
dotnet restore
dotnet run --project SeguridadService.Service
```

---

## 🧪 Pruebas unitarias

```bash
dotnet test SeguridadService.UnitTests
```
Cubre:
- Login correcto
- Usuario no encontrado
- Usuario inactivo
- Password incorrecto
- Request nulo
- Simulación de fallos en la base

---

## 🗂 Swagger

Disponible en:

```
https://localhost:{PORT}/swagger
```

Documenta controllers y DTOs automáticamente.

---

## 📊 JSON:API

### ✅ Ejemplo éxito
```json
{
  "data": {
    "type": "login",
    "attributes": {
      "token": "eyJhbGciOiJIUzI1NiIs..."
    }
  },
  "meta": {
    "success": true,
    "message": "Inicio de sesión exitoso"
  }
}
```

### ❌ Ejemplo error
```json
{
  "errors": [
    {
      "status": "400",
      "title": "BadRequest",
      "detail": "Usuario y/o password inválidos"
    }
  ],
  "meta": {
    "success": false,
    "message": "Error al procesar la solicitud"
  }
}
```

---

## 📋 Logs estructurados globales con Serilog

✅ Serilog gestiona logs estructurados a nivel global:

- Captura **errores no controlados** (ex: fallos infraestructura).
- Captura **errores de negocio** (`ApplicationException`).

Ejemplo:
```json
{
  "@t": "2025-06-27T01:23:45Z",
  "@m": "Error de validación o negocio",
  "@l": "Warning",
  "@x": "System.ApplicationException: Usuario y/o password inválidos"
}
```

---

## 🐳 Docker (guía futura)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish SeguridadService.Service -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SeguridadService.Service.dll"]
```

---

📞 Para soporte, contacta a **tu equipo de arquitectura**.
