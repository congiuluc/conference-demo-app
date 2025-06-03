using ConferenceApp.Shared.Models;
using ConferenceApp.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceApp.UI.Controllers;

public class ConferencesController : Controller
{
    private readonly ApiService _apiService;
    private readonly ILogger<ConferencesController> _logger;

    public ConferencesController(ApiService apiService, ILogger<ConferencesController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // GET: Conferences
    public async Task<IActionResult> Index()
    {
        var conferences = await _apiService.GetConferencesAsync();
        return View(conferences);
    }

    // GET: Conferences/Active
    public async Task<IActionResult> Active()
    {
        var conferences = await _apiService.GetActiveConferencesAsync();
        return View("Index", conferences);
    }

    // GET: Conferences/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(id);
        if (conference == null)
        {
            return NotFound();
        }

        return View(conference);
    }

    // GET: Conferences/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Conferences/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Conference conference)
    {
        if (ModelState.IsValid)
        {
            var result = await _apiService.CreateConferenceAsync(conference);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to create conference. Please try again.");
        }
        return View(conference);
    }

    // GET: Conferences/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(id);
        if (conference == null)
        {
            return NotFound();
        }
        return View(conference);
    }

    // POST: Conferences/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Conference conference)
    {
        if (id != conference.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var result = await _apiService.UpdateConferenceAsync(id, conference);
            if (result != null)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("", "Failed to update conference. Please try again.");
        }
        return View(conference);
    }

    // GET: Conferences/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(id);
        if (conference == null)
        {
            return NotFound();
        }

        return View(conference);
    }

    // POST: Conferences/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var result = await _apiService.DeleteConferenceAsync(id);
        if (result)
        {
            return RedirectToAction(nameof(Index));
        }
        
        var conference = await _apiService.GetConferenceAsync(id);
        ModelState.AddModelError("", "Failed to delete conference. Please try again.");
        return View(conference);
    }

    // GET: Conferences/Sessions/5
    public async Task<IActionResult> Sessions(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(id);
        if (conference == null)
        {
            return NotFound();
        }

        var sessions = await _apiService.GetSessionsByConferenceAsync(id);
        ViewBag.Conference = conference;
        return View(sessions);
    }

    // GET: Conferences/Speakers/5
    public async Task<IActionResult> Speakers(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(id);
        if (conference == null)
        {
            return NotFound();
        }

        var speakers = await _apiService.GetSpeakersByConferenceAsync(id);
        ViewBag.Conference = conference;
        return View(speakers);
    }

    // GET: Conferences/Attendees/5
    public async Task<IActionResult> Attendees(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var conference = await _apiService.GetConferenceAsync(id);
        if (conference == null)
        {
            return NotFound();
        }

        var attendees = await _apiService.GetAttendeesByConferenceAsync(id);
        ViewBag.Conference = conference;
        return View(attendees);
    }
}
