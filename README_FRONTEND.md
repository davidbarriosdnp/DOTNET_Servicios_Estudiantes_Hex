# Guía para integración frontend — Servicios Estudiantes API

Base URL de ejemplo: `https://{host}/api/v1`. Todas las respuestas JSON del cuerpo principal siguen el envoltorio:

```json
{
  "operacionExitosa": true,
  "mensaje": "texto opcional",
  "errores": [],
  "resultado": {}
}
```

En error: `operacionExitosa` es `false`, `mensaje` describe el fallo y `errores` puede contener detalles (validación).

**401 y 403** también usan este envoltorio (por ejemplo `resultado` suele ser `null`). El cuerpo es siempre `application/json` en camelCase.

## Health (sin `/api`, sin autenticación)

### GET `/health`

JSON con `status` agregado (`Healthy`, `Degraded`, `Unhealthy`), `totalDurationMs` y `checks` (detalle por comprobación).

### GET `/health/live`

Mismo formato de respuesta; solo incluye comprobaciones etiquetadas como liveness.

---

**Cabecera de autorización** (rutas protegidas):

```http
Authorization: Bearer {tokenAcceso}
```

En Swagger UI use el botón **Authorize** y el esquema **Bearer**.

---

## Auth (`/api/v1/Auth`)

### GET `.../Auth/programas` — público

Lista programas de crédito **activos** (`ProgramaCreditoDto[]` en `resultado`), pensada para el formulario de registro en línea sin JWT.

---

### POST `.../Auth/registro` — público

**Request body** (camelCase)

```json
{
  "nombreUsuario": "estudiante1",
  "email": "est@correo.com",
  "password": "Minimo8chars",
  "nombreCompleto": "Nombre Apellido",
  "programaCreditoId": 1
}
```

**Response `resultado`**: igual que login (`TokenParDto`). El JWT incluye el claim `estudiante_id` cuando el registro crea el perfil académico.

---

### POST `.../Auth/login` — público

**Request body**

```json
{
  "nombreUsuario": "admin",
  "password": "Admin123!"
}
```

**Response `resultado`** (`TokenParDto`)

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `tokenAcceso` | string | JWT |
| `tokenRenovacion` | string | Token opaco (guardar de forma segura) |
| `expiraSegundosAcceso` | number | Vida útil del JWT en segundos |
| `tipoToken` | string | `"Bearer"` |

---

### POST `.../Auth/refresh` — público

**Request body**

```json
{
  "tokenRenovacion": "<valor devuelto en login o refresh anterior>"
}
```

**Response**: mismo shape que login (`TokenParDto`).

El refresh anterior deja de ser válido (rotación).

---

### POST `.../Auth/logout` — protegido (JWT)

**Request headers**: `Authorization: Bearer {tokenAcceso}`

**Request body** (opcional)

```json
{
  "tokenRenovacion": "<opcional: revoca también este refresh en base de datos>"
}
```

**Response `resultado`**: `true` (boolean).

Efectos: el `jti` del JWT de acceso se invalida en memoria del servidor hasta su expiración; si envió `tokenRenovacion`, ese hash se marca revocado en SQL.

---

## Usuarios (`/api/v1/Usuarios`) — rol Administrador

Todas las rutas requieren JWT con claim de rol **`Administrador`**.

### GET `.../Usuarios?soloActivos=true|false`

**Response `resultado`**: arreglo de objetos:

| Campo | Tipo |
|-------|------|
| `usuarioId` | number |
| `nombreUsuario` | string |
| `email` | string |
| `rol` | string |
| `fechaRegistro` | string (ISO 8601) |
| `fechaModificacion` | string \| null |
| `estado` | number (0 = inactivo, 1 = activo) |

---

### GET `.../Usuarios/{usuarioId}`

**Response `resultado`**: un objeto con los mismos campos que un elemento de la lista (sin contraseña).

404 si no existe (cuerpo de error según middleware).

---

### POST `.../Usuarios`

**Request body** (alineado con `CrearUsuarioCommand`)

```json
{
  "nombreUsuario": "nuevo",
  "email": "nuevo@correo.com",
  "password": "Minimo8chars",
  "rol": "Estudiante"
}
```

**Response**: `201 Created` con `Location` y `resultado` = id numérico del usuario creado.

---

### PUT `.../Usuarios/{usuarioId}`

**Request body**

```json
{
  "nombreUsuario": "nuevo",
  "email": "nuevo@correo.com",
  "rol": "Estudiante",
  "estado": 1,
  "nuevaPassword": null
}
```

Si `nuevaPassword` es string no vacío, debe cumplir validación (mínimo 8 caracteres).

**Response `resultado`**: `true`.

---

### DELETE `.../Usuarios/{usuarioId}`

Desactiva usuario (`estado = 0`) y revoca todos los refresh tokens de ese usuario.

**Response `resultado`**: `true`.

---

## Dominio académico — autenticado (cualquier JWT válido)

Rutas bajo `/api/v1/Estudiantes`, `/api/v1/ProgramasCredito`, `/api/v1/Profesores`, `/api/v1/Materias` requieren **Authorization Bearer**; no exigen rol de administrador.

Consulte Swagger para parámetros de ruta, query y cuerpos (`Respuesta<T>` con DTOs específicos: estudiantes, inscripciones, catálogos, etc.).

---

## Códigos HTTP frecuentes

| Código | Situación |
|--------|-----------|
| 401 | Sin JWT o JWT inválido/expirado |
| 403 | JWT válido pero sin rol (p. ej. CRUD usuarios sin ser administrador) |
| 400 | Validación o reglas de negocio (`ExcepcionAplicacion`, `ExcepcionValidacion`, dominio) |
| 404 | Recurso no encontrado (`KeyNotFoundException`) |
| 500 | Error no controlado |

---

## Notas

- Propiedades JSON en camelCase (serialización por defecto de ASP.NET Core).
- Guarde `tokenRenovacion` solo en almacenamiento seguro del cliente; nunca en logs públicos.
- Si despliega varias réplicas sin sticky sessions, coordine la lista negra de JWT con Redis u otro almacén compartido (sustituir implementación de `IJwtListaNegra`).
