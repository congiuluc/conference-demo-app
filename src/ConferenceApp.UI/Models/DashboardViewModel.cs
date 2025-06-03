using ConferenceApp.Shared.Models;

namespace ConferenceApp.UI.Models;

public class DashboardViewModel
{
    public List<Conference> ActiveConferences { get; set; } = new List<Conference>();
    public List<Session> UpcomingSessions { get; set; } = new List<Session>();
    public int TotalConferences { get; set; }
    public int TotalSpeakers { get; set; }
    public int TotalSessions { get; set; }
    public int TotalAttendees { get; set; }
}
