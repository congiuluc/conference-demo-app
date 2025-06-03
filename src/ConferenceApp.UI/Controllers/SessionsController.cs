using ConferenceApp.Shared.Models;
using ConferenceApp.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.UI.Controllers;

public class SessionsController : Controller
{
    private readonly ApiService _apiService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(ApiService apiService, ILogger<SessionsController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // GET: Sessions
    public async Task<IActionResult> Index()
    {
        var sessions = await _apiService.GetSessionsAsync();
        return View(sessions);
    }

    // GET: Sessions/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var session = await _apiService.GetSessionAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        return View(session);
    }

    // GET: Sessions/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Conferences = await _apiService.GetConferencesAsync();
        ViewBag.Speakers = await _apiService.GetSpeakersAsync();
        return View();
    }

    // POST: Sessions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Session session)
    {
        if (ModelState.IsValid)
        {
            var result = await _apiService.CreateSessionAsync(session);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to create session. Please try again.");
        }
        
        ViewBag.Conferences = await _apiService.GetConferencesAsync();
        ViewBag.Speakers = await _apiService.GetSpeakersAsync();
        return View(session);
    }

    // GET: Sessions/ByConference/5
    public async Task<IActionResult> ByConference(string conferenceId)
    {
        if (string.IsNullOrEmpty(conferenceId))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(conferenceId);
        if (conference == null)
        {
            return NotFound();
        }

        var sessions = await _apiService.GetSessionsByConferenceAsync(conferenceId);
        ViewBag.Conference = conference;
        return View(sessions);
    }
}
