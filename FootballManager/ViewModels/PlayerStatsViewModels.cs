namespace FootballManager.ViewModels
{
    /// <summary>
    /// Một điểm dữ liệu trên biểu đồ phong độ tập luyện
    /// </summary>
    public class TrainingChartPoint
    {
        public string Label { get; set; } = string.Empty;  // "15/05 - Chạy bền"
        public decimal? CoachRating { get; set; }           // điểm HLV (1-10)
        public decimal? PhysicalScore { get; set; }         // điểm thể lực tổng hợp
        public decimal? TechnicalScore { get; set; }        // điểm kỹ thuật
        public decimal? TacticalScore { get; set; }         // điểm chiến thuật
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// Một điểm dữ liệu trên biểu đồ chỉ số thi đấu
    /// </summary>
    public class MatchChartPoint
    {
        public string Label { get; set; } = string.Empty;  // "HomeTeam vs AwayTeam"
        public decimal? PassCompletionPct { get; set; }
        public int MinutesPlayed { get; set; }
        // Chỉ số nổi bật theo vị trí (nullable — chỉ có dữ liệu khi phù hợp vị trí)
        public decimal? KeyMetric1 { get; set; }            // vd SavePct cho GK, AerialDuelsPct cho CB...
        public string KeyMetric1Label { get; set; } = string.Empty;
        public decimal? KeyMetric2 { get; set; }
        public string KeyMetric2Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Gói toàn bộ dữ liệu chart cho view
    /// </summary>
    public class PerformanceChartViewModel
    {
        public List<TrainingChartPoint> TrainingPoints { get; set; } = new();
        public List<MatchChartPoint> MatchPoints { get; set; } = new();
        public decimal? AvgCoachRating { get; set; }
        public decimal? AvgPassCompletion { get; set; }
        public int TotalTrainingSessions { get; set; }
        public int TotalMatchesPlayed { get; set; }
        public decimal? RatingTrend { get; set; }
    }

    public class PlayerStatsItem
    {
        public int PlayerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public string PositionLabel { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public int Goals { get; set; }
        public int OwnGoals { get; set; }
        public int Penalties { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int MatchesPlayed { get; set; }
        /// <summary>Tổng bàn thắng (gồm penalty) — dùng để sắp xếp top scorer</summary>
        public int TotalGoals => Goals + Penalties;
    }

    public class PlayerStatsViewModel
    {
        public List<PlayerStatsItem> TopScorers { get; set; } = new();
        public List<PlayerStatsItem> MostYellowCards { get; set; } = new();
        public List<PlayerStatsItem> MostRedCards { get; set; } = new();
        public List<PlayerStatsItem> AllStats { get; set; } = new();
        public string? TeamFilter { get; set; }
        public List<string> Teams { get; set; } = new();
    }
}
