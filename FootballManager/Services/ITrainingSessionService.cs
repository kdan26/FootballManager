using FootballManager.ViewModels;

namespace FootballManager.Services
{
    public interface ITrainingSessionService
    {
        Task<TrainingSessionIndexViewModel> GetSessionIndexAsync(int? teamId);
        Task<TrainingSessionDetailViewModel?> GetSessionDetailsAsync(int id);
        Task<TrainingSessionFormViewModel> GetSessionFormAsync(int? id, int? teamId);
        Task<int> SaveSessionAsync(TrainingSessionFormViewModel model, int createdByUserId);
        Task DeleteSessionAsync(int id);
        Task CompleteSessionAsync(int id);
        Task CancelSessionAsync(int id);
        Task RemoveDrillFromSessionAsync(int sessionDrillId);
    }
}
