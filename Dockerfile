# Imagen de ejecución del API Servicios_Estudiantes (.NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Servicios_Estudiantes.sln ./
COPY Servicios_Estudiantes.Api/Servicios_Estudiantes.Api.csproj Servicios_Estudiantes.Api/
COPY Servicios_Estudiantes.Aplicacion/Servicios_Estudiantes.Aplicacion.csproj Servicios_Estudiantes.Aplicacion/
COPY Servicios_Estudiantes.Dominio/Servicios_Estudiantes.Dominio.csproj Servicios_Estudiantes.Dominio/
COPY Servicios_Estudiantes.Infraestructura/Servicios_Estudiantes.Infraestructura.csproj Servicios_Estudiantes.Infraestructura/

RUN dotnet restore Servicios_Estudiantes.Api/Servicios_Estudiantes.Api.csproj

COPY Servicios_Estudiantes.Api/ Servicios_Estudiantes.Api/
COPY Servicios_Estudiantes.Aplicacion/ Servicios_Estudiantes.Aplicacion/
COPY Servicios_Estudiantes.Dominio/ Servicios_Estudiantes.Dominio/
COPY Servicios_Estudiantes.Infraestructura/ Servicios_Estudiantes.Infraestructura/

RUN dotnet publish Servicios_Estudiantes.Api/Servicios_Estudiantes.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Servicios_Estudiantes.Api.dll"]
