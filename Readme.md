## API SOAP - EnvíosExpress S.A.C.
### Seguimiento de Paquetes en una Empresa de Logística
**Descripción del Proyecto**

Este proyecto implementa una API SOAP para el seguimiento de paquetes de la empresa de logística **EnvíosExpress S.A.C.**, como parte de la Actividad de Aprendizaje 2 de Arquitectura de Software.
La API permite a los clientes consultar el estado actual de sus paquetes utilizando un número único de seguimiento, garantizando interoperabilidad, seguridad y estándares mediante el protocolo SOAP.

### Tecnologías Utilizadas

-   **.NET 8.0**: Framework principal
-   **SoapCore**: Implementación de servicios SOAP en .NET Core
-   **Entity Framework Core**: ORM para acceso a datos
-   **PostgreSQL**: Base de datos principal
-   **Docker**: Contenedorización 
-   **Swagger/OpenAPI**: Documentación de la API

### Estructura del Proyecto
```
Actividad2Arquitectura/
├── Controllers/          
├── Data/                 
│   ├── ApplicationDbContext.cs
│   └── DatabaseSeeder.cs
├── DTOs/                 
│   ├── BaseResponse.cs
│   ├── ErrorResponse.cs
│   ├── GetTrackingStatusRequest.cs
│   ├── GetTrackingStatusResponse.cs
│   ├── TrackingEventDto.cs
│   ├── TrackingError.cs
│   └── TrackingFault.cs
├── Models/          
│   ├── Package.cs
│   └── TrackingEvent.cs
├── Services/        
│   ├── ITrackingService.cs
│   └── TrackingService.cs
├── init-scripts/        
├── docker-compose.yml   
├── Dockerfile           
└── Program.cs           
```
### Instalación y Configuración

#### Prerrequisitos

-   .NET 8.0 SDK
-   Docker y Docker Compose
-   Git
#### Pasos de Instalación

1.  **Clonar el repositorio**
    
    bash
    
    ```bash
    git clone https://github.com/Ags-mites/net-soap.git
    cd net-soap
    ```
    
2. **Ejecutar con Docker Compose**

bash

```bash
docker-compose up -d
```

Esto iniciará:
-   API en el puerto `5190`
-   Base de datos PostgreSQL en el puerto `5432`
-   pgAdmin en el puerto `8080`

### Endpoints Disponibles

#### Información del Servicio

-   **URL Base**: `http://localhost:7061`
-   **WSDL**: `http://localhost:7061/TrackingService.asmx?wsdl`
-   **Endpoint SOAP**: `http://localhost:7061/TrackingService.asmx`
-   **Swagger UI**: `http://localhost:7061/swagger`



### 📊 Datos de Prueba
Datos de prueba precargados:
| Tracking Number | Estado | Ubicación actual |  
|--|--|--|
| PE1234567890 | En tránsito | Oficina EnvíosExpress S.A.C. Azuay |
| PE0987654321| Entregado | Quito - Ecuador |
| EC5555123456| Listo para retiro | Oficina EnvíosExpress S.A.C Ambato |
| PE2024001122| En proceso | Oficina EnvíosExpress S.A.C Ibarra |
| EC9999ERROR1| Incidencia - Dirección incorrecta | Centro de Atención al Cliente |

### Prueba de Servicio

#### Usando SoapUI

1.  Crear nuevo proyecto SOAP en SoapUI
2.  Usar WSDL: `http://localhost:7061/TrackingService.asmx?wsdl`
3.  Ejemplo de solicitud:

xml

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetTrackingStatus xmlns="http://tempuri.org/">
      <request>
        <TrackingNumber>EC9999ERROR1</TrackingNumber>
      </request>
    </GetTrackingStatus>
  </soap:Body>
</soap:Envelope>
```

#### Usando Postman

1.  Crear nueva solicitud POST
2.  URL: `http://localhost:5190/TrackingService.asmx`
3.  Headers:
    -   `Content-Type: text/xml; charset=utf-8`
4.  Body - raw XML:

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetTrackingStatus xmlns="http://tempuri.org/">
      <request>
        <TrackingNumber>EC9999ERROR1</TrackingNumber>
      </request>
    </GetTrackingStatus>
  </soap:Body>
</soap:Envelope>
```

### Configuración Docker

#### Variables de Entorno

yaml

```yaml
# Base de datos
POSTGRES_DB: envios_express_db
POSTGRES_USER: postgres
POSTGRES_PASSWORD: postgres123

# API
ASPNETCORE_ENVIRONMENT: Development
ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=envios_express_db;Username=postgres;Password=postgres123
```

### Autores

-   **Agustín Mites** 
-   **Milena Reyes** 
