using FootballManager.Models;
using System.ComponentModel.DataAnnotations;

namespace FootballManager.ViewModels
{
    // ─── Dùng để hiển thị danh sách ───
    public class DrillListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryLabel { get; set; } = string.Empty;
        public TrainingCategory Category { get; set; }
        public int? DurationMinutes { get; set; }
        public int Difficulty { get; set; }
        public string DifficultyLabel => ViewModelHelpers.GetDifficultyLabel(Difficulty);
        public string? TargetSkill { get; set; }
        public string? VideoUrl { get; set; }
        public bool IsShared { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
    }

    // ─── Dùng để tạo / chỉnh sửa ───
    public class DrillFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bài tập")]
        [MaxLength(200)]
        [Display(Name = "Tên bài tập")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Loại bài tập")]
        public TrainingCategory Category { get; set; } = TrainingCategory.Physical;

        [MaxLength(500)]
        [Display(Name = "Mô tả ngắn")]
        public string? Description { get; set; }

        [Display(Name = "Hướng dẫn chi tiết")]
        public string? Instructions { get; set; }

        [Range(1, 120, ErrorMessage = "Từ 1 đến 120 phút")]
        [Display(Name = "Thời lượng (phút)")]
        public int? DurationMinutes { get; set; }

        [Range(1, 5, ErrorMessage = "Từ 1 đến 5")]
        [Display(Name = "Độ khó (1–5)")]
        public int Difficulty { get; set; } = 3;

        [MaxLength(200)]
        [Display(Name = "Kỹ năng mục tiêu")]
        public string? VideoUrl { get; set; }

        [Display(Name = "Chia sẻ với tất cả HLV")]
        public bool IsShared { get; set; } = false;
    }

    // ─── Chi tiết (xem) ───
    public class DrillDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TrainingCategory Category { get; set; }
        public string CategoryLabel { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Instructions { get; set; }
        public int? DurationMinutes { get; set; }
        public int Difficulty { get; set; }
        public string DifficultyLabel => ViewModelHelpers.GetDifficultyLabel(Difficulty);
        public string? VideoUrl { get; set; }
        public bool IsShared { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UsageCount { get; set; }
    }

    // ─── Dùng trong dropdown chọn drill khi nhập Training Stats ───
    public class DrillSelectItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TrainingCategory Category { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Description { get; set; }
    }
}
