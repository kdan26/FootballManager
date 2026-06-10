using System.ComponentModel.DataAnnotations;

namespace FootballManager.Models
{
    public enum MatchStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }

    public class Match
    {
        public int Id { get; set; }

        [Required]
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        [Required]
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        [Required]
        public DateTime MatchDate { get; set; }

        [MaxLength(200)]
        public string? Venue { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<PlayerAttendance> PlayerAttendances { get; set; } = new List<PlayerAttendance>();
        public ICollection<MatchEvent> Events { get; set; } = new List<MatchEvent>();
    }
}
