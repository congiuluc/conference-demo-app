# .NET Aspire Dashboard Implementation

This repository now includes .NET Aspire Dashboard capabilities for monitoring the Conference Management System backend (API) and frontend (UI) applications.

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

### Option 1: Run with AppHost (Recommended)
```bash
cd src
dotnet run --project ConferenceApp.AppHost
```

This will:
- Start both API and UI applications
- Display monitoring endpoints
- Show available application URLs
- Provide observability information

### Option 2: Run Applications Individually
```bash
# Terminal 1 - API
cd src
ASPNETCORE_URLS=https://localhost:5001 dotnet run --project ConferenceApp.API

# Terminal 2 - UI  
cd src
ASPNETCORE_URLS=https://localhost:5000 dotnet run --project ConferenceApp.UI
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

## Benefits

1. **Centralized Monitoring**: Single place to monitor all applications
2. **Health Checks**: Automated health monitoring for both applications
3. **Observability**: Complete telemetry data for debugging and monitoring
4. **Service Discovery**: Automatic service endpoint resolution
5. **Resilience**: Built-in retry and circuit breaker patterns
6. **Development Experience**: Easy way to run and monitor the entire system

## Next Steps

To fully leverage .NET Aspire Dashboard in production:

1. **Configure OTLP Exporter**: Set `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable
2. **Set up Telemetry Backend**: Use services like Azure Monitor, Jaeger, or Prometheus
3. **Configure Service Discovery**: Use service mesh or service registry in production
4. **Add Custom Metrics**: Implement application-specific metrics
5. **Configure Alerting**: Set up alerts based on health check failures

## Troubleshooting

### Common Issues
- **Port Conflicts**: Ensure ports 5000 and 5001 are available
- **Certificate Warnings**: Use `dotnet dev-certs https --trust` for HTTPS certificates
- **CosmosDB Connection**: Configure CosmosDB connection string or use emulator

### Logs Location
- Application logs are output to console with structured formatting
- OpenTelemetry data is sent to configured OTLP endpoint
- Health check results are available via HTTP endpoints