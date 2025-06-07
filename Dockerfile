# Usar la imagen base de ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Actividad2Arquitectura.csproj", "./"]
RUN dotnet restore "Actividad2Arquitectura.csproj"

COPY . .

RUN dotnet build "Actividad2Arquitectura.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Actividad2Arquitectura.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Actividad2Arquitectura.dll"]