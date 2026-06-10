using FootballManager.Models;

namespace FootballManager.ViewModels
{
    /// <summary>
    /// Helper dùng chung trong ViewModels — tránh duplicate switch expression
    /// </summary>
    public static class ViewModelHelpers
    {
        public static string GetPositionLabel(PlayerPosition pos) => pos switch
        {
            PlayerPosition.GoalKeeper   => "Thủ môn",
            PlayerPosition.CenterBack   => "Trung vệ",
            PlayerPosition.FullBack     => "Hậu vệ biên",
            PlayerPosition.DefensiveMid => "Tiền vệ phòng ngự",
            PlayerPosition.CentralMid   => "Tiền vệ trung tâm",
            PlayerPosition.AttackingMid => "Tiền vệ công",
            PlayerPosition.Winger       => "Chạy cánh",
            PlayerPosition.Striker      => "Tiền đạo",
            _                           => pos.ToString()
        };

        public static string GetPositionShort(PlayerPosition pos) => pos switch
        {
            PlayerPosition.GoalKeeper   => "TM",
            PlayerPosition.CenterBack   => "TV",
            PlayerPosition.FullBack     => "HVB",
            PlayerPosition.DefensiveMid => "TVPN",
            PlayerPosition.CentralMid   => "TVTT",
            PlayerPosition.AttackingMid => "TVC",
            PlayerPosition.Winger       => "CC",
            PlayerPosition.Striker      => "TĐ",
            _                           => ""
        };

        public static string GetCategoryLabel(TrainingCategory cat) => cat switch
        {
            TrainingCategory.Physical  => "Thể lực",
            TrainingCategory.Technical => "Kỹ thuật",
            TrainingCategory.Tactical  => "Chiến thuật",
            _                          => cat.ToString()
        };

        public static string GetDifficultyLabel(int d) => d switch
        {
            1 => "★☆☆☆☆ Rất dễ",
            2 => "★★☆☆☆ Dễ",
            3 => "★★★☆☆ Trung bình",
            4 => "★★★★☆ Khó",
            5 => "★★★★★ Rất khó",
            _ => d.ToString()
        };

        public static string GetSessionStatusLabel(TrainingSessionStatus s) => s switch
        {
            TrainingSessionStatus.Scheduled => "Sắp diễn ra",
            TrainingSessionStatus.Completed => "Hoàn thành",
            TrainingSessionStatus.Cancelled => "Đã hủy",
            _                               => ""
        };

        public static string GetSessionStatusBadge(TrainingSessionStatus s) => s switch
        {
            TrainingSessionStatus.Scheduled => "bg-primary",
            TrainingSessionStatus.Completed => "bg-success",
            TrainingSessionStatus.Cancelled => "bg-secondary",
            _                               => "bg-light"
        };
    }
}
