# Servicios Estudiantes (arquitectura hexagonal)

API en **.NET 8** para dominio académico (programas, profesores, materias, estudiantes, inscripciones) y **gestión de usuarios con JWT** (acceso + renovación). La persistencia usa **SQL Server** exclusivamente mediante **procedimientos almacenados**.

## Estructura de la solución

| Proyecto | Rol |
|----------|-----|
| `Servicios_Estudiantes.Api` | Minimal APIs, pipeline, autenticación JWT, Swagger |
| `Servicios_Estudiantes.Aplicacion` | Casos de uso (MediatR), DTOs, puertos, validación |
| `Servicios_Estudiantes.Dominio` | Entidades y excepciones de dominio |
| `Servicios_Estudiantes.Infraestructura` | Adaptadores SQL (`RepositorioAcademicoSql`, `RepositorioUsuariosSql`) |
| `Servicios_Estudiantes.Pruebas` | Pruebas de integración (xUnit + `WebApplicationFactory`) |

## Formato de respuesta unificado

- Los endpoints de negocio devuelven `Respuesta<T>` (camelCase en JSON) vía el middleware de errores o `Results.Ok`.
- **401** (JWT inválido, ausente o expirado): cuerpo JSON con el mismo envoltorio `operacionExitosa`, `mensaje`, `errores`, `resultado` (evento `OnChallenge` de JwtBearer + `JsonRespuestaEscritorio`).
- **403** (identidad válida pero sin rol/política): mismo envoltorio (`IAuthorizationMiddlewareResultHandler` personalizado).
- La **firma HS256** emite y valida con la misma instancia `FirmaSymmetricaJwt` (clave `Jwt:ClaveSecreta`); evita el error *“The signature key was not found”* por desalineación emisor/validador.

## Health checks

- `GET /health` — informe JSON de todas las comprobaciones registradas.
- `GET /health/live` — solo comprobaciones con etiqueta `live` (liveness).
- Anónimos; no requieren JWT.

## Pruebas

```bash
dotnet test Servicios_Estudiantes.Pruebas/Servicios_Estudiantes.Pruebas.csproj
```

## Base de datos

Orden recomendado de scripts en `Database/Scripts/`:

1. `01_Tablas.sql` — esquema (incluye `Usuario` y `RefreshToken`).
2. `02_Semillas.sql` — datos iniciales (incluye usuario **admin**).
3. `03_Procedimientos.sql` — SP del dominio académico.
4. `04_Usuarios_Auth.sql` — SP de usuarios y tokens de renovación.

**Usuario semilla**

- `NombreUsuario`: `admin`
- `Email`: `admin@local.test`
- `Password`: `Admin123!`
- `Rol`: `Administrador`

Tras ejecutar los scripts, configure `ConnectionStrings:Estudiantes` en `appsettings` o variables de entorno.

## Autenticación y autorización

- **JWT Bearer** (HS256): emisor, audiencia y clave en sección `Jwt` de configuración (`JwtOpciones`).
- **Token de acceso**: claims `sub` (id usuario), `jti`, nombre y rol.
- **Token de renovación**: opaco, aleatorio; en base de datos solo se guarda **SHA-256 en hexadecimal**.
- **Cerrar sesión**: el `jti` del acceso actual pasa a una **lista negra en memoria** (`IMemoryCache`) hasta la expiración natural del JWT. El refresh indicado (si se envía) se **revoca** en SQL.
- **CRUD de usuarios** (`/api/v1/Usuarios`): política **`SoloAdministrador`** (rol `Administrador`).
- **Resto de controladores** académicos: cualquier usuario **autenticado** (JWT válido).
- **Auth** (`/api/v1/Auth`): `programas` (lista pública de programas activos), `registro`, `login` y `refresh` son anónimos; `logout` exige JWT.

Para varias instancias de API, sustituya la lista negra en memoria por **Redis** (o `IDistributedCache` con Redis) implementando `IJwtListaNegra` de forma distribuida.

## Configuración

- `ConnectionStrings:Estudiantes` — cadena SQL Server.
- `Jwt:Emisor`, `Jwt:Audiencia`, `Jwt:ClaveSecreta` (mínimo **32 caracteres**), `Jwt:MinutosValidezAcceso`, `Jwt:DiasValidezRefresh`.
- `Api:*` — CORS y OpenTelemetry (en desarrollo pueden ir vacíos según `InstanciarConfiguracionApi`).
- Archivo `.env` en la raíz del API (cargado con `DotNetEnv`) si aplica.

## Ejecución local

```bash
dotnet restore Servicios_Estudiantes.sln
dotnet run --project Servicios_Estudiantes.Api
```

Swagger: ruta habitual del host en desarrollo (ver `launchSettings.json`).

## Excepciones por capa

| Tipo | Capa | Respuesta HTTP típica |
|------|------|-------------------------|
| `ExcepcionApi` | Api | 400 |
| `ExcepcionValidacion` | Aplicación (FluentValidation) | 400 + `Errores` |
| `ExcepcionAplicacion` | Aplicación / reglas de aplicación | 400 |
| `Servicios_Estudiantes.Dominio.Excepciones.Excepcion` | Dominio | 400 |
| `KeyNotFoundException` | Aplicación | 404 |
| Otras | — | 500 |

El middleware `ManejadorErroresIntermediario` serializa el envoltorio `Respuesta<T>` con `OperacionExitosa`, `Mensaje`, `Errores` y `Resultado`.

## Reglas de negocio del dominio académico

Estas reglas están modeladas en validaciones y procedimientos almacenados; los datos semilla (`Database/Scripts/02_Semillas.sql`) las respetan.

1. **Registro en línea (CRUD)**: el usuario puede darse de alta con `POST /api/v1/Auth/registro` (credenciales + nombre completo + programa de créditos); recibe JWT como en el login.
2. **Programa de créditos**: cada estudiante queda asociado a un `programaCreditoId`.
3. **Diez materias** en el catálogo semilla del programa base.
4. **Cada materia equivale a 3 créditos** (`creditos` = 3; validado en aplicación al crear/actualizar materia).
5. **Máximo 3 materias** por estudiante (`maxMateriasPorEstudiante` en programa; inscripción con tres IDs).
6. **Cinco profesores**, cada uno con **2 materias** asignadas en semilla.
7. **Sin dos materias con el mismo profesor** en la misma inscripción del estudiante (validación en base de datos).
8. **Visibilidad de registros**: cualquier usuario autenticado puede listar estudiantes (`GET /api/v1/Estudiantes`) y ver el detalle de inscripciones públicas del dominio.
9. **Compañeros de clase**: solo **nombres** de alumnos que comparten materia (`GET .../companeros` devuelve lista de strings, no datos personales extra).

## Referencia de API (`/api/v1`)

**Base**: `{origen}/api/v1` (ej. `https://localhost:7xxx/api/v1`). JSON en **camelCase**.

**Cabeceras habituales**

| Cabecera | Uso |
|----------|-----|
| `Authorization: Bearer {tokenAcceso}` | Rutas protegidas (JWT HS256). |
| `Content-Type: application/json` | Cuerpos POST/PUT. |

**Envoltorio `Respuesta<T>`** (cuerpo JSON):

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| `operacionExitosa` | boolean | `true` si la operación concluyó sin error de negocio. |
| `mensaje` | string \| null | Texto informativo o de error. |
| `errores` | string[] | Detalle (p. ej. validación). |
| `resultado` | T \| null | Payload específico del endpoint. |

En **401/403** el cuerpo sigue el mismo envoltorio (`resultado` suele ser `null`).

---

### Health (sin prefijo `/api`, sin JWT)

| Método | Ruta | Respuesta `resultado` |
|--------|------|------------------------|
| GET | `/health` | Informe agregado (`status`, `totalDurationMs`, `checks`). |
| GET | `/health/live` | Igual; solo comprobaciones con etiqueta `live`. |

---

### Auth — `/api/v1/Auth`

| Método | Ruta | Auth | Request body | Response `resultado` |
|--------|------|------|--------------|---------------------|
| GET | `programas` | No | — | `ProgramaCreditoDto[]` (solo programas **activos**; uso en formulario de registro). |
| POST | `registro` | No | Ver tabla | `TokenParDto` |
| POST | `login` | No | `CuerpoInicioSesion` | `TokenParDto` |
| POST | `refresh` | No | `CuerpoRefrescar` | `TokenParDto` |
| POST | `logout` | Sí | `CuerpoCerrarSesion` (opcional) | `boolean` |

**`CuerpoRegistroEstudiante`** (registro en línea)

| Campo JSON | Tipo | Descripción |
|------------|------|-------------|
| `nombreUsuario` | string | Máx. 120. |
| `email` | string | Email válido, máx. 256. |
| `password` | string | Mín. 8, máx. 256. |
| `nombreCompleto` | string | Máx. 120. |
| `programaCreditoId` | number | &gt; 0. |

**`CuerpoInicioSesion`**: `nombreUsuario`, `password`.

**`CuerpoRefrescar`**: `tokenRenovacion`.

**`CuerpoCerrarSesion`**: `tokenRenovacion` (opcional).

**`TokenParDto`**

| Campo | Tipo |
|-------|------|
| `tokenAcceso` | string (JWT; claim opcional `estudiante_id` si hay perfil académico) |
| `tokenRenovacion` | string |
| `expiraSegundosAcceso` | number |
| `tipoToken` | string (por defecto `"Bearer"`) |

---

### Usuarios — `/api/v1/Usuarios` (rol **Administrador**)

Cabecera: `Authorization: Bearer …`

| Método | Ruta | Query / ruta | Request body | Response `resultado` |
|--------|------|----------------|--------------|----------------------|
| GET | `` | `soloActivos` (bool) | — | `UsuarioListaDto[]` |
| GET | `{usuarioId}` | — | — | `UsuarioDetalleDto` |
| POST | `` | — | `CrearUsuarioCommand` | `number` (id); **201** + `Location` |
| PUT | `{usuarioId}` | — | `ActualizarUsuarioCuerpo` | `boolean` |
| DELETE | `{usuarioId}` | — | — | `boolean` |

**`CrearUsuarioCommand`**: `nombreUsuario`, `email`, `password`, `rol`.

**`ActualizarUsuarioCuerpo`**: `nombreUsuario`, `email`, `rol`, `estado` (byte 0–1), `nuevaPassword` (string \| null).

**`UsuarioListaDto` / `UsuarioDetalleDto`**: `usuarioId`, `nombreUsuario`, `email`, `rol`, `fechaRegistro`, `fechaModificacion`, `estado`.

---

### Estudiantes — `/api/v1/Estudiantes`

JWT requerido (cualquier rol autenticado).

| Método | Ruta | Parámetros | Request body | Response `resultado` |
|--------|------|--------------|--------------|----------------------|
| GET | `catalogo/materias` | `programaCreditoId` (opcional), `soloActivos` (bool) | — | `MateriaCatalogoDto[]` |
| GET | `{estudianteId}/materias/{materiaId}/companeros` | — | — | `string[]` (solo nombres de compañeros) |
| GET | `{estudianteId}/inscripcion` | `soloActivas` (bool) | — | `InscripcionEstudianteDto[]` |
| POST | `{estudianteId}/inscripcion/fila` | — | `InscripcionFilaCuerpo` | según caso de uso |
| DELETE | `{estudianteId}/inscripcion/materias/{materiaId}` | — | — | según caso de uso |
| PUT | `{estudianteId}/inscripcion/materia` | — | `CambiarMateriaCuerpo` | según caso de uso |
| GET | `{estudianteId}` | — | — | `EstudianteDetalleDto` |
| GET | `` | `soloActivos` (bool) | — | `EstudianteRegistroDto[]` |
| POST | `` | — | `CrearEstudianteCommand` | `number` (id); **201** |
| PUT | `{estudianteId}` | — | `ActualizarEstudianteCuerpo` | `boolean` |
| DELETE | `{estudianteId}` | — | — | `boolean` |
| POST | `{estudianteId}/inscripcion` | — | `InscripcionSolicitudApi` | `boolean` |

**`CrearEstudianteCommand`**: `nombre`, `email`, `programaCreditoId` (number \| null).

**`ActualizarEstudianteCuerpo`**: `nombre`, `email`, `programaCreditoId` (number \| null), `estado` (byte 0–1).

**`InscripcionSolicitudApi`**: `materiaId1`, `materiaId2`, `materiaId3` (tres distintas; reglas de profesor y programa en servidor).

**`InscripcionFilaCuerpo`**: `materiaId`.

**`CambiarMateriaCuerpo`**: `materiaIdAnterior`, `materiaIdNueva`.

**`EstudianteDetalleDto`**: `estudianteId`, `nombre`, `email`, `programaCreditoId`, `fechaRegistro`, `fechaModificacion`, `estado`, `usuarioId`.

**`EstudianteRegistroDto`**: igual que detalle sustituyendo `usuarioId` por agregado textual `materiasInscritas`.

**`InscripcionEstudianteDto`**: `materiaId`, `nombreMateria`, `creditos`, `profesorId`, `nombreProfesor`, `fechaRegistro`, `fechaModificacion`, `estado`.

**`MateriaCatalogoDto`**: `materiaId`, `nombre`, `creditos`, `profesorId`, `programaCreditoId`, `nombreProfesor`, `fechaRegistro`, `fechaModificacion`, `estado`.

---

### Programas de crédito — `/api/v1/ProgramasCredito`

JWT requerido.

| Método | Ruta | Query | Request body | Response `resultado` |
|--------|------|-------|--------------|----------------------|
| GET | `` | `soloActivos` (bool) | — | `ProgramaCreditoDto[]` |
| GET | `{id}` | — | — | `ProgramaCreditoDto` |
| POST | `` | — | `CrearProgramaCreditoCommand` | `number` (id); **201** |
| PUT | `{id}` | — | `ActualizarProgramaCreditoCuerpo` | `boolean` |
| DELETE | `{id}` | — | — | `boolean` |

**`CrearProgramaCreditoCommand`**: `nombre`, `creditosPorMateria`, `maxMateriasPorEstudiante`.

**`ActualizarProgramaCreditoCuerpo`**: `nombre`, `creditosPorMateria`, `maxMateriasPorEstudiante`, `estado`.

**`ProgramaCreditoDto`**: `programaCreditoId`, `nombre`, `creditosPorMateria`, `maxMateriasPorEstudiante`, `fechaRegistro`, `fechaModificacion`, `estado`.

---

### Profesores — `/api/v1/Profesores`

JWT requerido.

| Método | Ruta | Query | Request body | Response `resultado` |
|--------|------|-------|--------------|----------------------|
| GET | `` | `soloActivos` (bool) | — | `ProfesorDto[]` |
| GET | `{id}` | — | — | `ProfesorDto` |
| POST | `` | — | `CrearProfesorCommand` | `number` (id); **201** |
| PUT | `{id}` | — | `ActualizarProfesorCuerpo` | `boolean` |
| DELETE | `{id}` | — | — | `boolean` |

**`CrearProfesorCommand`**: `nombre`.

**`ActualizarProfesorCuerpo`**: `nombre`, `estado`.

**`ProfesorDto`**: `profesorId`, `nombre`, `fechaRegistro`, `fechaModificacion`, `estado`.

---

### Materias — `/api/v1/Materias`

JWT requerido.

| Método | Ruta | Query | Request body | Response `resultado` |
|--------|------|-------|--------------|----------------------|
| GET | `` | `programaCreditoId` (opcional), `soloActivos` (bool) | — | `MateriaCatalogoDto[]` |
| GET | `{id}` | — | — | `MateriaDetalleDto` |
| POST | `` | — | `CrearMateriaCommand` | `number` (id); **201** |
| PUT | `{id}` | — | `ActualizarMateriaCuerpo` | `boolean` |
| DELETE | `{id}` | — | — | `boolean` |

**`CrearMateriaCommand`**: `nombre`, `creditos` (debe ser **3**), `profesorId`, `programaCreditoId`.

**`ActualizarMateriaCuerpo`**: `nombre`, `creditos` (**3**), `profesorId`, `programaCreditoId`, `estado`.

**`MateriaDetalleDto`**: `materiaId`, `nombre`, `creditos`, `profesorId`, `programaCreditoId`, `fechaRegistro`, `fechaModificacion`, `estado`, `nombreProfesor`.

---

### Códigos HTTP frecuentes

| Código | Situación |
|--------|-----------|
| 401 | Sin JWT o JWT inválido/expirado |
| 403 | JWT válido pero sin permiso (p. ej. no administrador en `/Usuarios`) |
| 400 | Validación o regla de negocio |
| 404 | Recurso no encontrado |
| 500 | Error no controlado |

La integración detallada con ejemplos JSON adicionales sigue disponible en **`README_FRONTEND.md`** (auth y usuarios); la lista anterior es la referencia canónica de contratos en el repositorio.

## Despliegue (archivos en raíz del repo)

| Archivo | Uso |
|---------|-----|
| `Dockerfile` | Imagen multi-stage del API (puerto 8080). |
| `.dockerignore` | Reduce contexto de build Docker. |
| `azure-pipelines.yml` | Ejemplo de CI en Azure DevOps (restore, test, publish). |
| `coverlet.runsettings` | Plantilla para cobertura con Coverlet cuando exista proyecto de pruebas. |
| `NuGet.config` | Origen de paquetes NuGet.org. |

Ejemplo Docker:

```bash
docker build -t servicios-estudiantes-api .
docker run -p 8080:8080 -e ConnectionStrings__Estudiantes="..." -e Jwt__ClaveSecreta="..." servicios-estudiantes-api
```

## Más ayuda

- Contratos y OpenAPI: Swagger UI al ejecutar el API.
- Versiónado de rutas: `/api/v1/{controlador}/...` mediante `Map{nombreControlador}` en `Servicios_Estudiantes.Api.Controladores.V1`.
- **`README_FRONTEND.md`**: guía breve de integración (auth, usuarios, health); la referencia completa de endpoints y DTOs está en la sección **Referencia de API** de este README.
