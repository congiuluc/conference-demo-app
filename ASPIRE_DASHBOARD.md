# .NET Aspire Dashboard Implementation

This repository now includes .NET Aspire Dashboard capabilities for monitoring the Conference Management System backend (API) and frontend (UI) applications.

> **Important**: The applications can run **both with and without** .NET Aspire. Aspire provides enhanced monitoring and orchestration, but is not required for basic functionality.

## What Was Added

### 1. ConferenceApp.ServiceDefaults Project
- **OpenTelemetry Integration**: Configured for metrics, tracing, and logging
- **Health Checks**: Added `/health` and `/alive` endpoints
- **Service Discovery**: HTTP client configuration with resilience patterns
- **Instrumentation**: ASP.NET Core, HTTP client, and runtime metrics

### 2. ConferenceApp.AppHost Project
- **Application Orchestrator**: Launches both API and UI applications
- **Monitoring Dashboard**: Provides centralized monitoring information
- **Service Coordination**: Manages communication between services

### 3. Enhanced API Project (ConferenceApp.API)
- **Health Endpoints**: `/health` and `/alive` for monitoring
- **OpenTelemetry**: Full observability integration
- **Service Defaults**: Uses shared observability configuration

### 4. Enhanced UI Project (ConferenceApp.UI)
- **Health Endpoints**: `/health` and `/alive` for monitoring
- **OpenTelemetry**: Full observability integration
- **Service Discovery**: HTTP client configured for API communication

## How to Use

The Conference Management System supports both standalone and Aspire modes:

### Option 1: Run with AppHost (Recommended for Development)
```bash
cd src
dotnet run --project ConferenceApp.AppHost
```

This will:
- Start both API and UI applications
- Display monitoring endpoints
- Show available application URLs
- Provide observability information

### Option 2: Run Applications Individually (Standalone Mode)
```bash
# Terminal 1 - API
cd src/ConferenceApp.API
dotnet run

# Terminal 2 - UI  
cd src/ConferenceApp.UI
dotnet run --urls "https://localhost:5000"
```

**Standalone Mode Notes:**
- Applications run independently without Aspire orchestration
- Health checks (`/health`, `/alive`) still work
- UI automatically connects to API using configured URL (`https://localhost:5001`)
- OpenTelemetry features are available but without centralized dashboard
- Perfect for production deployment without Aspire infrastructure

### Option 3: Production Deployment (Standalone)
```bash
# Build for production
dotnet publish src/ConferenceApp.API -c Release
dotnet publish src/ConferenceApp.UI -c Release

# Deploy as separate services
```

## Monitoring Endpoints

### API Monitoring
- **Health Check**: `https://localhost:5001/health`
- **Liveness Check**: `https://localhost:5001/alive`
- **API Documentation**: `https://localhost:5001/swagger`

### UI Monitoring
- **Health Check**: `https://localhost:5000/health`
- **Liveness Check**: `https://localhost:5000/alive`
- **Application**: `https://localhost:5000`

## Observability Features

### OpenTelemetry Integration
- **Metrics**: HTTP request metrics, runtime performance metrics
- **Tracing**: Distributed tracing across API and UI applications
- **Logging**: Structured logging with correlation IDs

### Health Checks
- **Health**: Complete application health including dependencies
- **Alive**: Basic liveness check for orchestration

### Service Discovery
- HTTP clients configured with:
  - Automatic retry policies
  - Circuit breaker patterns
  - Service endpoint resolution

## Architecture

### With Aspire (Orchestrated Mode)
```
ConferenceApp.AppHost (Orchestrator)
├── ConferenceApp.API (Backend)
│   ├── ServiceDefaults (Observability)
│   ├── Health Checks
│   └── OpenTelemetry
├── ConferenceApp.UI (Frontend)
│   ├── ServiceDefaults (Observability)
│   ├── Health Checks
│   └── Service Discovery
└── Monitoring Dashboard
```

### Standalone Mode
```
ConferenceApp.API (Backend) ← HTTP → ConferenceApp.UI (Frontend)
├── ServiceDefaults (Observability)     ├── ServiceDefaults (Observability)
├── Health Checks                       ├── Health Checks
└── OpenTelemetry                       └── Configured HTTP Client
```

### Key Differences

| Component | Standalone Mode | Aspire Mode |
|-----------|----------------|-------------|
| **Service Communication** | Direct HTTP with configured URLs | Service discovery |
| **Monitoring** | Individual health endpoints | Centralized dashboard |
| **Startup** | Manual (each app separately) | Orchestrated (single command) |
| **Dependencies** | Core ASP.NET + observability | Same + Aspire orchestration |
| **Production Ready** | ✅ Traditional deployment | ✅ Cloud-native deployment |

## Benefits

### Aspire Mode Benefits
1. **Centralized Monitoring**: Single place to monitor all applications
2. **Service Discovery**: Automatic service endpoint resolution
3. **Orchestration**: Single command to start entire system
4. **Development Experience**: Integrated monitoring during development

### Standalone Mode Benefits
1. **Simplicity**: Traditional ASP.NET Core deployment
2. **Independence**: No orchestration dependencies
3. **Production Flexibility**: Deploy to any environment
4. **Resource Efficiency**: No additional orchestration overhead

### Shared Benefits (Both Modes)
1. **Health Checks**: Automated health monitoring for both applications
2. **Observability**: Complete telemetry data for debugging and monitoring
3. **Resilience**: Built-in retry and circuit breaker patterns (when using HttpClient)
4. **Modern Architecture**: OpenTelemetry integration ready for production backends

## Next Steps

### For Aspire Mode (Production)
To fully leverage .NET Aspire Dashboard in production:

1. **Configure OTLP Exporter**: Set `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable
2. **Set up Telemetry Backend**: Use services like Azure Monitor, Jaeger, or Prometheus
3. **Configure Service Discovery**: Use service mesh or service registry in production
4. **Add Custom Metrics**: Implement application-specific metrics
5. **Configure Alerting**: Set up alerts based on health check failures

### For Standalone Mode (Production)
To deploy in standalone mode:

1. **Configure Load Balancer**: Point UI to API through load balancer
2. **Set API Base URL**: Configure `ApiSettings:BaseUrl` in UI appsettings
3. **Set up Monitoring**: Use existing APM tools with OpenTelemetry endpoints
4. **Configure Health Checks**: Use `/health` and `/alive` endpoints for orchestration
5. **Deploy Independently**: Each application can scale and deploy separately

## Troubleshooting

### Common Issues
- **Port Conflicts**: Ensure ports 5000 and 5001 are available
- **Certificate Warnings**: Use `dotnet dev-certs https --trust` for HTTPS certificates
- **CosmosDB Connection**: Configure CosmosDB connection string or use emulator

### Logs Location
- Application logs are output to console with structured formatting
- OpenTelemetry data is sent to configured OTLP endpoint
- Health check results are available via HTTP endpoints