# Servicios Estudiantes (arquitectura hexagonal)

API en **.NET 8** para dominio académico (programas, profesores, materias, estudiantes, inscripciones) y **gestión de usuarios con JWT** (acceso + renovación). La persistencia usa **SQL Server** exclusivamente mediante **procedimientos almacenados**.

## Estructura de la solución

| Proyecto | Rol |
|----------|-----|
| `Servicios_Estudiantes.Api` | Minimal APIs, pipeline, autenticación JWT, Swagger |
| `Servicios_Estudiantes.Aplicacion` | Casos de uso (MediatR), DTOs, puertos, validación |
| `Servicios_Estudiantes.Dominio` | Entidades y excepciones de dominio |
| `Servicios_Estudiantes.Infraestructura` | Adaptadores SQL (`RepositorioAcademicoSql`, `RepositorioUsuariosSql`) |

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
- **Auth** (`/api/v1/Auth`): `login` y `refresh` son anónimos; `logout` exige JWT.

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

## Despliegue (archivos en raíz del repo)

| Archivo | Uso |
|---------|-----|
| `Dockerfile` | Imagen multi-stage del API (puerto 8080). |
| `.dockerignore` | Reduce contexto de build Docker. |
| `azure-pipelines.yml` | Ejemplo de CI en Azure DevOps (restore, build, publish). |
| `coverlet.runsettings` | Plantilla para cobertura con Coverlet cuando exista proyecto de pruebas. |
| `NuGet.config` | Origen de paquetes NuGet.org. |

Ejemplo Docker:

```bash
docker build -t servicios-estudiantes-api .
docker run -p 8080:8080 -e ConnectionStrings__Estudiantes="..." -e Jwt__ClaveSecreta="..." servicios-estudiantes-api
```

## Documentación para frontend

Ver **`README_FRONTEND.md`**: rutas, cuerpos JSON, cabeceras y forma de `Respuesta<T>`.

## Más ayuda

- Contratos y OpenAPI: Swagger UI al ejecutar el API.
- Versiónado de rutas: `/api/v1/{controlador}/...` mediante `Map{nombreControlador}` en `Servicios_Estudiantes.Api.Controladores.V1`.
