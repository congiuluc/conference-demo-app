using ConferenceApp.Shared.Models;
using Newtonsoft.Json;
using System.Text;

namespace ConferenceApp.UI.Services;

/// <summary>
/// Service for making HTTP requests to the backend API
/// </summary>
public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly string? _baseApiUrl;

    public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // When running with Aspire, the HttpClient will be configured with service discovery
        // Fall back to configuration if not available
        if (_httpClient.BaseAddress == null)
        {
            _baseApiUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5001";
            _httpClient.BaseAddress = new Uri(_baseApiUrl);
        }
        
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    #region Conference Methods

    public async Task<List<Conference>> GetConferencesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/conferences");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Conference>>(jsonContent) ?? new List<Conference>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conferences");
            return new List<Conference>();
        }
    }

    public async Task<List<Conference>> GetActiveConferencesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/conferences/active");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Conference>>(jsonContent) ?? new List<Conference>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active conferences");
            return new List<Conference>();
        }
    }

    public async Task<Conference?> GetConferenceAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/conferences/{id}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Conference>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting conference {ConferenceId}", id);
            return null;
        }
    }

    public async Task<Conference?> CreateConferenceAsync(Conference conference)
    {
        try
        {
            var json = JsonConvert.SerializeObject(conference);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/conferences", content);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Conference>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conference");
            return null;
        }
    }

    public async Task<Conference?> UpdateConferenceAsync(string id, Conference conference)
    {
        try
        {
            var json = JsonConvert.SerializeObject(conference);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"api/conferences/{id}", content);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Conference>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating conference {ConferenceId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteConferenceAsync(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/conferences/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conference {ConferenceId}", id);
            return false;
        }
    }

    #endregion

    #region Speaker Methods

    public async Task<List<Speaker>> GetSpeakersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/speakers");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Speaker>>(jsonContent) ?? new List<Speaker>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting speakers");
            return new List<Speaker>();
        }
    }

    public async Task<Speaker?> GetSpeakerAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/speakers/{id}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Speaker>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting speaker {SpeakerId}", id);
            return null;
        }
    }

    public async Task<List<Speaker>> GetSpeakersByConferenceAsync(string conferenceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/speakers/by-conference/{conferenceId}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Speaker>>(jsonContent) ?? new List<Speaker>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting speakers for conference {ConferenceId}", conferenceId);
            return new List<Speaker>();
        }
    }

    public async Task<Speaker?> CreateSpeakerAsync(Speaker speaker)
    {
        try
        {
            var json = JsonConvert.SerializeObject(speaker);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/speakers", content);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Speaker>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating speaker");
            return null;
        }
    }

    #endregion

    #region Session Methods

    public async Task<List<Session>> GetSessionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/sessions");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Session>>(jsonContent) ?? new List<Session>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions");
            return new List<Session>();
        }
    }

    public async Task<Session?> GetSessionAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/sessions/{id}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Session>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session {SessionId}", id);
            return null;
        }
    }

    public async Task<List<Session>> GetSessionsByConferenceAsync(string conferenceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/sessions/by-conference/{conferenceId}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Session>>(jsonContent) ?? new List<Session>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions for conference {ConferenceId}", conferenceId);
            return new List<Session>();
        }
    }

    public async Task<Session?> CreateSessionAsync(Session session)
    {
        try
        {
            var json = JsonConvert.SerializeObject(session);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("api/sessions", content);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Session>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            return null;
        }
    }

    #endregion

    #region Attendee Methods

    public async Task<List<Attendee>> GetAttendeesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/attendees");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Attendee>>(jsonContent) ?? new List<Attendee>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendees");
            return new List<Attendee>();
        }
    }

    public async Task<Attendee?> GetAttendeeAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"attendees/{id}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Attendee>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendee {AttendeeId}", id);
            return null;
        }
    }

    public async Task<List<Attendee>> GetAttendeesByConferenceAsync(string conferenceId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/attendees/conference/{conferenceId}");
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Attendee>>(jsonContent) ?? new List<Attendee>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting attendees for conference {ConferenceId}", conferenceId);
            return new List<Attendee>();
        }
    }

    public async Task<Attendee?> CreateAttendeeAsync(Attendee attendee)
    {
        try
        {
            var json = JsonConvert.SerializeObject(attendee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("attendees", content);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Attendee>(jsonContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating attendee");
            return null;
        }
    }

    #endregion
}
