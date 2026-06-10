namespace FootballManager.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalTeams { get; set; }
        public int TotalMatches { get; set; }
        public int CompletedMatches { get; set; }
        public int UpcomingMatchCount { get; set; }
        public List<MatchListItemViewModel> UpcomingMatches { get; set; } = new();
    }
}
