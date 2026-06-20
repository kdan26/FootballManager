using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db) => _db = db;

        // ─── Danh sách users ───
        public async Task<List<UserListItem>> GetAllUsersAsync()
        {
            return await _db.Users
                .Include(u => u.PlayerProfile)
                .OrderBy(u => u.Role).ThenBy(u => u.FullName)
                .Select(u => new UserListItem
                {
                    Id         = u.Id,
                    FullName   = u.FullName,
                    Username   = u.Username,
                    Role       = u.Role,
                    IsActive   = u.IsActive,
                    PlayerName = u.PlayerProfile != null ? u.PlayerProfile.FullName : null,
                    CreatedAt  = u.CreatedAt
                })
                .ToListAsync();
        }

        // ─── Form tạo mới ───
        public async Task<UserCreateViewModel> GetCreateFormAsync()
        {
            var vm = new UserCreateViewModel();
            vm.AvailablePlayers = await GetAvailablePlayersAsync(null);
            return vm;
        }

        // ─── Form chỉnh sửa ───
        public async Task<UserEditViewModel?> GetEditFormAsync(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u == null) return null;

            return new UserEditViewModel
            {
                Id               = u.Id,
                FullName         = u.FullName,
                Role             = u.Role,
                PlayerId         = u.PlayerId,
                IsActive         = u.IsActive,
                AvailablePlayers = await GetAvailablePlayersAsync(u.PlayerId)
            };
        }

        // ─── Tạo user ───
        public async Task<(bool Success, string? Error)> CreateAsync(UserCreateViewModel model)
        {
            if (await _db.Users.AnyAsync(u => u.Username == model.Username))
                return (false, $"Tên đăng nhập '{model.Username}' đã tồn tại");

            if (model.Role == "Player" && model.PlayerId == null)
                return (false, "Role Player phải liên kết với một cầu thủ");

            if (model.PlayerId.HasValue &&
                await _db.Users.AnyAsync(u => u.PlayerId == model.PlayerId))
                return (false, "Cầu thủ này đã có tài khoản");

            _db.Users.Add(new User
            {
                FullName     = model.FullName,
                Username     = model.Username,
                PasswordHash = SeedData.HashPassword(model.Password),
                Role         = model.Role,
                PlayerId     = model.Role == "Player" ? model.PlayerId : null,
                IsActive     = true,
                CreatedAt    = DateTime.Now
            });

            await _db.SaveChangesAsync();
            return (true, null);
        }

        // ─── Cập nhật user ───
        public async Task<(bool Success, string? Error)> UpdateAsync(UserEditViewModel model)
        {
            var user = await _db.Users.FindAsync(model.Id);
            if (user == null) return (false, "Không tìm thấy tài khoản");

            if (model.Role == "Player" && model.PlayerId == null)
                return (false, "Role Player phải liên kết với một cầu thủ");

            if (model.PlayerId.HasValue &&
                await _db.Users.AnyAsync(u => u.PlayerId == model.PlayerId && u.Id != model.Id))
                return (false, "Cầu thủ này đã có tài khoản khác");

            user.FullName = model.FullName;
            user.Role     = model.Role;
            user.PlayerId = model.Role == "Player" ? model.PlayerId : null;
            user.IsActive = model.IsActive;

            await _db.SaveChangesAsync();
            return (true, null);
        }

        // ─── Đổi mật khẩu ───
        public async Task<(bool Success, string? Error)> ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return (false, "Không tìm thấy tài khoản");

            user.PasswordHash = SeedData.HashPassword(newPassword);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        // ─── Xóa user ───
        public async Task<(bool Success, string? Error)> DeleteAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return (false, "Không tìm thấy tài khoản");

            if (user.Role == "Admin" &&
                await _db.Users.CountAsync(u => u.Role == "Admin") <= 1)
                return (false, "Không thể xóa tài khoản Admin cuối cùng");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return (true, null);
        }

        // ─── Player Portal ───
        public async Task<PlayerPortalViewModel?> GetPortalAsync(int userId)
        {
            var user = await _db.Users
                .Include(u => u.PlayerProfile).ThenInclude(p => p!.Team)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user?.PlayerProfile == null) return null;

            var player = user.PlayerProfile;
            var now    = DateTime.Now;

            // Buổi tập sắp tới (7 ngày)
            var sessions = await _db.TrainingSessions
                .Include(s => s.Attendances)
                .Where(s => s.TeamId == player.TeamId
                         && s.ScheduledAt >= now
                         && s.ScheduledAt <= now.AddDays(7)
                         && s.Status == TrainingSessionStatus.Scheduled)
                .OrderBy(s => s.ScheduledAt)
                .ToListAsync();

            var upcoming = sessions.Select(s =>
            {
                var att = s.Attendances.FirstOrDefault(a => a.PlayerId == player.Id);
                string status = att == null ? "Chưa xác nhận" : att.Status switch
                {
                    TrainingAttendanceStatus.Present => "Có mặt",
                    TrainingAttendanceStatus.Late    => "Đến muộn",
                    TrainingAttendanceStatus.Absent  => "Vắng mặt",
                    TrainingAttendanceStatus.Excused => "Xin phép",
                    TrainingAttendanceStatus.Injured => "Chấn thương",
                    _ => ""
                };
                string badge = att == null ? "bg-secondary" : att.Status switch
                {
                    TrainingAttendanceStatus.Present => "bg-success",
                    TrainingAttendanceStatus.Late    => "bg-warning text-dark",
                    TrainingAttendanceStatus.Absent  => "bg-danger",
                    TrainingAttendanceStatus.Excused => "bg-info text-dark",
                    TrainingAttendanceStatus.Injured => "bg-secondary",
                    _ => "bg-light"
                };
                return new UpcomingSessionItem
                {
                    SessionId   = s.Id,
                    Title       = s.Title,
                    ScheduledAt = s.ScheduledAt,
                    Location    = s.Location,
                    MyStatus    = status,
                    MyStatusBadge = badge
                };
            }).ToList();

            // Điểm danh gần nhất (10 buổi)
            var recentAtts = await _db.TrainingAttendances
                .Include(a => a.TrainingSession)
                .Where(a => a.PlayerId == player.Id)
                .OrderByDescending(a => a.TrainingSession.ScheduledAt)
                .Take(10)
                .ToListAsync();

            var recentList = recentAtts.Select(a => new RecentAttendanceItem
            {
                SessionTitle = a.TrainingSession.Title,
                Date         = a.TrainingSession.ScheduledAt,
                Status       = a.Status switch
                {
                    TrainingAttendanceStatus.Present => "Có mặt",
                    TrainingAttendanceStatus.Late    => $"Muộn {a.LateMinutes} phút",
                    TrainingAttendanceStatus.Absent  => "Vắng mặt",
                    TrainingAttendanceStatus.Excused => "Xin phép",
                    TrainingAttendanceStatus.Injured => "Chấn thương",
                    _ => ""
                },
                StatusBadge = a.Status switch
                {
                    TrainingAttendanceStatus.Present => "bg-success",
                    TrainingAttendanceStatus.Late    => "bg-warning text-dark",
                    TrainingAttendanceStatus.Absent  => "bg-danger",
                    TrainingAttendanceStatus.Excused => "bg-info text-dark",
                    _                                => "bg-secondary"
                },
                Note = a.Note
            }).ToList();

            // Điểm HLV
            var ratings = await _db.PlayerTrainingStats
                .Where(s => s.PlayerId == player.Id && s.CoachRating.HasValue)
                .OrderByDescending(s => s.TrainingDate)
                .Take(20)
                .ToListAsync();

            string healthLabel = player.HealthStatus switch
            {
                PlayerHealthStatus.Fit        => "Sẵn sàng",
                PlayerHealthStatus.Injured    => "Chấn thương",
                PlayerHealthStatus.Sick       => "Ốm đau",
                PlayerHealthStatus.Suspended  => "Treo giò",
                PlayerHealthStatus.Recovering => "Đang hồi phục",
                _ => ""
            };
            string healthBadge = player.HealthStatus switch
            {
                PlayerHealthStatus.Fit        => "bg-success",
                PlayerHealthStatus.Injured    => "bg-danger",
                PlayerHealthStatus.Sick       => "bg-warning text-dark",
                PlayerHealthStatus.Suspended  => "bg-secondary",
                PlayerHealthStatus.Recovering => "bg-info text-dark",
                _ => "bg-secondary"
            };

            return new PlayerPortalViewModel
            {
                PlayerId          = player.Id,
                FullName          = player.FullName,
                PositionLabel     = ViewModelHelpers.GetPositionLabel(player.Position),
                TeamName          = player.Team.Name,
                JerseyNumber      = player.JerseyNumber,
                HealthStatusLabel = healthLabel,
                HealthBadge       = healthBadge,
                UpcomingSessions  = upcoming,
                RecentAttendances = recentList,
                LastCoachRating   = ratings.FirstOrDefault()?.CoachRating,
                AvgCoachRating    = ratings.Any()
                    ? Math.Round((decimal)ratings.Average(r => (double)r.CoachRating!.Value), 1)
                    : null
            };
        }

        // ─── Helper: cầu thủ chưa có tài khoản ───
        private async Task<List<PlayerSelectItem>> GetAvailablePlayersAsync(int? currentPlayerId)
        {
            var linkedIds = await _db.Users
                .Where(u => u.PlayerId.HasValue && u.PlayerId != currentPlayerId)
                .Select(u => u.PlayerId!.Value)
                .ToListAsync();

            return await _db.Players
                .Include(p => p.Team)
                .Where(p => p.IsActive && !linkedIds.Contains(p.Id))
                .OrderBy(p => p.Team.Name).ThenBy(p => p.FullName)
                .Select(p => new PlayerSelectItem
                {
                    Id           = p.Id,
                    FullName     = p.FullName,
                    TeamName     = p.Team.Name,
                    JerseyNumber = p.JerseyNumber
                })
                .ToListAsync();
        }
    }
}
