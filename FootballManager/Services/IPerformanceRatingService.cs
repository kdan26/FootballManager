using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface IPerformanceRatingService
    {
        /// <summary>Lấy toàn bộ danh sách + dữ liệu chart cho 1 cầu thủ</summary>
        Task<PerformanceRatingIndexViewModel?> GetIndexAsync(int playerId);

        /// <summary>Lấy form thêm mới (playerId đã biết)</summary>
        Task<PerformanceRatingFormViewModel?> GetFormAsync(int playerId, int? ratingId = null);

        /// <summary>Lưu (thêm mới hoặc cập nhật)</summary>
        Task<int> SaveAsync(PerformanceRatingFormViewModel model, int ratedByUserId);

        /// <summary>Xóa 1 đánh giá</summary>
        Task DeleteAsync(int ratingId);

        /// <summary>Bật/tắt công bố cho cầu thủ</summary>
        Task TogglePublishAsync(int ratingId);
    }
}
