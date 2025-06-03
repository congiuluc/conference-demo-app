namespace ConferenceApp.API.Endpoints;

/// <summary>
/// Interface for endpoint mapping classes
/// </summary>
public interface IEndpoints
{
    /// <summary>
    /// Maps endpoints to the web application
    /// </summary>
    /// <param name="app">Web application</param>
    void MapEndpoints(WebApplication app);
}
