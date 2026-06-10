using FootballManager.Models;

namespace FootballManager.ViewModels
{
    public class AttendancePlayerItem
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public string PositionLabel { get; set; } = string.Empty;
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Unknown;
        public string? Note { get; set; }
    }

    public class MatchAttendanceViewModel
    {
        public int MatchId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public List<AttendancePlayerItem> Players { get; set; } = new();

        public int PresentCount => Players.Count(p => p.Status == AttendanceStatus.Present);
        public int AbsentCount => Players.Count(p => p.Status == AttendanceStatus.Absent);
        public int InjuredCount => Players.Count(p => p.Status == AttendanceStatus.Injured);
        public int UnknownCount => Players.Count(p => p.Status == AttendanceStatus.Unknown);
    }

    public class SaveAttendanceViewModel
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public List<AttendancePlayerItem> Players { get; set; } = new();
    }
}
