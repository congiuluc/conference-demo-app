using System.Diagnostics;

Console.WriteLine("=== Conference App .NET Aspire Dashboard Demo ===");
Console.WriteLine();

// Check if ports are available
var apiPort = 5001;
var uiPort = 5000;

Console.WriteLine("Starting Conference Management System with monitoring...");
Console.WriteLine();

try
{
    // Start API
    Console.WriteLine($"🔗 Starting API on https://localhost:{apiPort}");
    var apiProcess = StartProject("ConferenceApp.API", apiPort, "ASPNETCORE_URLS=https://localhost:5001;OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317");
    
    // Wait a moment for API to start
    await Task.Delay(3000);
    
    // Start UI  
    Console.WriteLine($"🌐 Starting UI on https://localhost:{uiPort}");
    var uiProcess = StartProject("ConferenceApp.UI", uiPort, $"ASPNETCORE_URLS=https://localhost:5000;ApiSettings__BaseUrl=https://localhost:5001;OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317");
    
    Console.WriteLine();
    Console.WriteLine("✅ Applications started successfully!");
    Console.WriteLine();
    Console.WriteLine("📊 Monitoring endpoints:");
    Console.WriteLine($"   API Health: https://localhost:{apiPort}/health");
    Console.WriteLine($"   API Alive:  https://localhost:{apiPort}/alive");
    Console.WriteLine($"   UI Health:  https://localhost:{uiPort}/health");
    Console.WriteLine($"   UI Alive:   https://localhost:{uiPort}/alive");
    Console.WriteLine();
    Console.WriteLine("🌐 Application endpoints:");
    Console.WriteLine($"   Conference API: https://localhost:{apiPort}/swagger");
    Console.WriteLine($"   Conference UI:  https://localhost:{uiPort}");
    Console.WriteLine();
    Console.WriteLine("📈 OpenTelemetry is configured for:");
    Console.WriteLine("   - HTTP request/response metrics");
    Console.WriteLine("   - Application logs with structured data");
    Console.WriteLine("   - Distributed tracing");
    Console.WriteLine("   - Runtime metrics");
    Console.WriteLine();
    Console.WriteLine("Press Ctrl+C to stop all services...");
    
    // Handle cancellation
    var cancellationToken = new CancellationTokenSource();
    Console.CancelKeyPress += (_, e) =>
    {
        e.Cancel = true;
        cancellationToken.Cancel();
    };
    
    // Wait for cancellation
    try
    {
        await Task.Delay(-1, cancellationToken.Token);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine();
        Console.WriteLine("🛑 Stopping services...");
        
        try { apiProcess?.Kill(); } catch { }
        try { uiProcess?.Kill(); } catch { }
        
        Console.WriteLine("✅ All services stopped.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    return 1;
}

return 0;

Process StartProject(string projectName, int port, string environmentVars)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = $"run --project {projectName}/{projectName}.csproj",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };
    
    // Add environment variables
    foreach (var envVar in environmentVars.Split(';'))
    {
        var parts = envVar.Split('=', 2);
        if (parts.Length == 2)
        {
            startInfo.EnvironmentVariables[parts[0]] = parts[1];
        }
    }
    
    var process = Process.Start(startInfo);
    
    // Async output reading to prevent deadlock
    _ = Task.Run(async () =>
    {
        while (!process!.StandardOutput.EndOfStream)
        {
            var line = await process.StandardOutput.ReadLineAsync();
            if (!string.IsNullOrEmpty(line))
            {
                Console.WriteLine($"[{projectName}] {line}");
            }
        }
    });
    
    _ = Task.Run(async () =>
    {
        while (!process!.StandardError.EndOfStream)
        {
            var line = await process.StandardError.ReadLineAsync();
            if (!string.IsNullOrEmpty(line))
            {
                Console.WriteLine($"[{projectName}] ERROR: {line}");
            }
        }
    });
    
    return process!;
}