# Usar la imagen base de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Usar la imagen del SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto
COPY ["Actividad2Arquitectura.csproj", "./"]
RUN dotnet restore "Actividad2Arquitectura.csproj"

# Copiar todo el código fuente
COPY . .

# Compilar la aplicación
RUN dotnet build "Actividad2Arquitectura.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "Actividad2Arquitectura.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Crear la imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "Actividad2Arquitectura.dll"]