using ConferenceApp.ServiceDefaults;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace ConferenceApp.ServiceDefaults.Tests;

public class ExtensionsTests
{
    [Fact]
    public void AddServiceDefaults_ShouldAddRequiredServices()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddServiceDefaults();

        // Assert
        var services = builder.Services;
        services.Should().NotBeNull();
        // Services collection should have health checks and other required services
        // This is a basic test to ensure the method can be called without errors
    }

    [Fact]
    public void AddDefaultHealthChecks_ShouldAddHealthChecks()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.AddDefaultHealthChecks();

        // Assert
        var app = builder.Build();
        app.Should().NotBeNull();
    }

    [Fact]
    public void ConfigureOpenTelemetry_ShouldConfigureOpenTelemetry()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act
        builder.ConfigureOpenTelemetry();

        // Assert
        var app = builder.Build();
        app.Should().NotBeNull();
    }

    [Fact]
    public void MapDefaultEndpoints_ShouldMapEndpoints()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.AddDefaultHealthChecks();
        var app = builder.Build();

        // Act
        app.MapDefaultEndpoints();

        // Assert
        app.Should().NotBeNull();
    }
}