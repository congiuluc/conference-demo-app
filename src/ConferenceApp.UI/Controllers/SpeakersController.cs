using ConferenceApp.Shared.Models;
using ConferenceApp.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.UI.Controllers;

public class SpeakersController : Controller
{
    private readonly ApiService _apiService;
    private readonly ILogger<SpeakersController> _logger;

    public SpeakersController(ApiService apiService, ILogger<SpeakersController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // GET: Speakers
    public async Task<IActionResult> Index()
    {
        var speakers = await _apiService.GetSpeakersAsync();
        return View(speakers);
    }

    // GET: Speakers/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var speaker = await _apiService.GetSpeakerAsync(id);
        if (speaker == null)
        {
            return NotFound();
        }

        return View(speaker);
    }

    // GET: Speakers/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Conferences = await _apiService.GetConferencesAsync();
        return View();
    }

    // POST: Speakers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Speaker speaker)
    {
        if (ModelState.IsValid)
        {
            var result = await _apiService.CreateSpeakerAsync(speaker);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to create speaker. Please try again.");
        }
        
        ViewBag.Conferences = await _apiService.GetConferencesAsync();
        return View(speaker);
    }

    // GET: Speakers/ByConference/5
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

        var speakers = await _apiService.GetSpeakersByConferenceAsync(conferenceId);
        ViewBag.Conference = conference;
        return View(speakers);
    }
}
