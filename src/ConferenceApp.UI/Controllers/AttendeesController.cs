using ConferenceApp.Shared.Models;
using ConferenceApp.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.UI.Controllers;

public class AttendeesController : Controller
{
    private readonly ApiService _apiService;
    private readonly ILogger<AttendeesController> _logger;

    public AttendeesController(ApiService apiService, ILogger<AttendeesController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // GET: Attendees
    public async Task<IActionResult> Index()
    {
        var attendees = await _apiService.GetAttendeesAsync();
        return View(attendees);
    }

    // GET: Attendees/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var attendee = await _apiService.GetAttendeeAsync(id);
        if (attendee == null)
        {
            return NotFound();
        }

        return View(attendee);
    }

    // GET: Attendees/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Conferences = await _apiService.GetConferencesAsync();
        return View();
    }

    // POST: Attendees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Attendee attendee)
    {
        if (ModelState.IsValid)
        {
            var result = await _apiService.CreateAttendeeAsync(attendee);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to create attendee. Please try again.");
        }
        
        ViewBag.Conferences = await _apiService.GetConferencesAsync();
        return View(attendee);
    }

    // GET: Attendees/ByConference/5
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

        var attendees = await _apiService.GetAttendeesByConferenceAsync(conferenceId);
        ViewBag.Conference = conference;
        return View(attendees);
    }
}
