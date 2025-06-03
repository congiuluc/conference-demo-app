# Conference Demo App

A comprehensive conference management system built with .NET 8, featuring a REST API backend and MVC frontend for managing tech conferences, speakers, sessions, attendees, and related activities.

## Overview

This application provides a complete solution for managing tech conferences, including:

- **Conference Management**: Create and manage conference details, venues, and schedules
- **Speaker Management**: Manage speaker profiles and their sessions
- **Session Management**: Handle session proposals, reviews, and approvals
- **Attendee Management**: Track conference attendees and registrations
- **Call for Papers**: Manage speaking proposals and review processes
- **Agenda Management**: Organize conference schedules and daily agendas

## Architecture

The solution consists of these main projects:

- **ConferenceApp.API**: ASP.NET Core 8 Web API with REST endpoints
- **ConferenceApp.UI**: ASP.NET Core 8 MVC web application
- **ConferenceApp.Shared**: Shared models and validation logic
- **ConferenceApp.ServiceDefaults**: Shared observability and health check configuration
- **ConferenceApp.AppHost**: .NET Aspire application orchestrator (optional)

### Deployment Options

The application supports two deployment patterns:

1. **Standalone Mode**: Run API and UI independently without orchestration
2. **Aspire Mode**: Run with .NET Aspire Dashboard for enhanced monitoring and orchestration

## Prerequisites

Before running the application locally, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) or Azure Cosmos DB account
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/) (optional)

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/congiuluc/conference-demo-app.git
cd conference-demo-app
```

### 2. Set Up Cosmos DB

#### Option A: Using Cosmos DB Emulator (Recommended for Development)

1. Install and start the [Azure Cosmos DB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
2. The emulator will run on `https://localhost:8081` by default
3. The application is already configured to use the emulator with default settings

#### Option B: Using Azure Cosmos DB

1. Create an Azure Cosmos DB account
2. Update the connection settings in `src/ConferenceApp.API/appsettings.json`:

```json
{
  "CosmosDb": {
    "EndpointUrl": "https://your-account.documents.azure.com:443/",
    "PrimaryKey": "your-primary-key",
    "DatabaseName": "ConferenceDb",
    "ContainerName": "ConferenceContainer"
  }
}
```

### 3. Build and Restore Dependencies

```bash
cd src
dotnet restore
dotnet build
```

### 4. Run the Applications

You can run the applications in two ways:

#### Option 1: Standalone Mode (No Aspire Required)

This is the traditional way to run ASP.NET Core applications independently:

##### Start the API (Backend)

```bash
cd src/ConferenceApp.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`
- Health Check: `https://localhost:5001/health`
- Liveness Check: `https://localhost:5001/alive`

##### Start the UI (Frontend)

In a new terminal:

```bash
cd src/ConferenceApp.UI
dotnet run --launch-profile https
```

The web application will be available at:
- HTTPS: `https://localhost:7064`
- HTTP: `http://localhost:5096`
- Health Check: `https://localhost:7064/health`
- Liveness Check: `https://localhost:7064/alive`

**Note**: The UI is configured to automatically connect to the API at `https://localhost:5001` when running standalone.

#### Option 2: Aspire Mode (With Monitoring Dashboard)

For enhanced monitoring and orchestration, use .NET Aspire:

```bash
cd src
dotnet run --project ConferenceApp.AppHost
```

This will:
- Start both API and UI applications automatically
- Provide a centralized monitoring dashboard
- Display application URLs and health status
- Enable distributed tracing and metrics collection

Applications will be available at:
- **API**: `https://localhost:5001` (with observability)
- **UI**: `https://localhost:5000` (with service discovery)
- **Monitoring**: Check console output for dashboard URLs

**Note**: When using Aspire mode, the UI automatically discovers the API through service discovery.

## API Documentation

The API includes comprehensive Swagger/OpenAPI documentation available at `https://localhost:5001/swagger` when running in development mode.

### Main API Endpoints

- **Conferences**: `/api/conferences` - Manage conference information
- **Speakers**: `/api/speakers` - Manage speaker profiles
- **Sessions**: `/api/sessions` - Handle session proposals and management
- **Attendees**: `/api/attendees` - Track conference attendees
- **Venues**: `/api/venues` - Manage conference venues
- **Call for Papers**: `/api/callforpapers` - Handle speaking proposals
- **Agenda**: `/api/agenda` - Manage conference schedules
- **Session Management**: `/api/session-management` - Advanced session review and approval workflows

## Project Structure

```
conference-demo-app/
├── src/
│   ├── ConferenceApp.API/          # Web API project
│   │   ├── Endpoints/              # API endpoint definitions
│   │   ├── Services/               # Business logic and data services
│   │   ├── Config/                 # Configuration classes
│   │   └── Program.cs              # API startup and configuration
│   ├── ConferenceApp.UI/           # MVC Web application
│   │   ├── Controllers/            # MVC controllers
│   │   ├── Views/                  # Razor views
│   │   ├── Models/                 # View models
│   │   ├── Services/               # API client services
│   │   └── wwwroot/                # Static web assets
│   ├── ConferenceApp.Shared/       # Shared library
│   │   ├── Models/                 # Domain models
│   │   └── Validators/             # FluentValidation validators
│   ├── ConferenceApp.ServiceDefaults/ # Observability configuration
│   │   └── Extensions.cs           # Health checks, OpenTelemetry, service discovery
│   ├── ConferenceApp.AppHost/      # Aspire orchestrator (optional)
│   │   └── Program.cs              # Application host configuration
│   └── ConferenceApp.sln           # Solution file
├── ASPIRE_DASHBOARD.md             # Aspire monitoring documentation
└── README.md                       # This file
```

## Key Features

### Conference Management
- Create, update, and manage conference details
- Track conference status and dates
- Associate venues with conferences

### Speaker & Session Management
- Maintain speaker profiles and contact information
- Submit and review session proposals
- Approve/reject sessions with review notes
- Track session status throughout the review process

### Attendee Registration
- Register attendees for conferences
- Track registration status and contact details

### Agenda Planning
- Create daily agendas for conferences
- Schedule sessions and speakers
- Organize conference timelines

## Technology Stack

- **Backend**: ASP.NET Core 8 Web API
- **Frontend**: ASP.NET Core 8 MVC
- **Database**: Azure Cosmos DB
- **Validation**: FluentValidation
- **Documentation**: Swagger/OpenAPI
- **Monitoring**: OpenTelemetry with health checks
- **Orchestration**: .NET Aspire (optional)
- **Dependency Injection**: Built-in ASP.NET Core DI

## Configuration

The application uses standard ASP.NET Core configuration. Key settings in `appsettings.json`:

- **CosmosDb**: Database connection settings
- **Logging**: Application logging configuration
- **AllowedHosts**: CORS and host restrictions

## Development Notes

- The API uses minimal APIs pattern with endpoint classes for organization
- All models inherit from `BaseEntity` for consistent ID and partition key handling
- FluentValidation ensures data integrity across all operations
- The application follows repository pattern with Cosmos DB service abstraction
- CORS is configured to allow cross-origin requests for development

### Standalone vs Aspire Mode

| Feature | Standalone Mode | Aspire Mode |
|---------|----------------|-------------|
| **Startup** | Run each app individually | Single command starts both |
| **Service Discovery** | Uses configured API URL | Automatic service discovery |
| **Monitoring** | Basic health endpoints | Full dashboard with metrics |
| **Dependencies** | Core .NET packages only | Additional observability packages |
| **Production** | Traditional deployment | Enhanced monitoring/observability |
| **Development** | Familiar ASP.NET Core experience | Integrated development environment |

**When to use Standalone Mode:**
- Simple development scenarios
- Production deployments with existing monitoring
- When you don't need .NET Aspire features
- Minimal dependencies preferred

**When to use Aspire Mode:**
- Local development with multiple services
- Want integrated monitoring and observability
- Building cloud-native applications
- Need distributed tracing and service discovery

For detailed information about .NET Aspire features, see [ASPIRE_DASHBOARD.md](ASPIRE_DASHBOARD.md).

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Build and test locally
5. Submit a pull request

## License

This project is a demo application for educational and demonstration purposes.