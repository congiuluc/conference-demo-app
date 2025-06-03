using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ConferenceApp.Shared.Models;
using ConferenceApp.UI.Models;
using ConferenceApp.UI.Services;

namespace ConferenceApp.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApiService _apiService;

    public HomeController(ILogger<HomeController> logger, ApiService apiService)
    {
        _logger = logger;
        _apiService = apiService;
    }

    public async Task<IActionResult> Index()
    {
        var activeConferences = await _apiService.GetActiveConferencesAsync();
        var speakers = await _apiService.GetSpeakersAsync();
        var sessions = await _apiService.GetSessionsAsync();
        var attendees = await _apiService.GetAttendeesAsync();

        var model = new DashboardViewModel
        {
            ActiveConferences = activeConferences.Take(5).ToList(),            TotalConferences = activeConferences.Count,
            TotalSpeakers = speakers.Count,
            TotalSessions = sessions.Count,
            TotalAttendees = attendees.Count,
            UpcomingSessions = sessions
                .Where(s => s.StartTime > DateTime.UtcNow)
                .OrderBy(s => s.StartTime)
                .Take(5)
                .ToList()
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
