using FootballManager.Data;
using FootballManager.Models;
using FootballManager.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Services
{
    public class DrillService : IDrillService
    {
        private readonly ApplicationDbContext _db;

        public DrillService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<DrillListItem>> GetAllAsync(string? category = null, string? search = null)
        {
            var query = _db.Drills
                .Include(d => d.CreatedByUser)
                .Include(d => d.TrainingStats)
                .Where(d => d.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<TrainingCategory>(category, out var cat))
            {
                query = query.Where(d => d.Category == cat);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d => d.Name.Contains(search) ||
                                         (d.Description != null && d.Description.Contains(search)));
            }

            var list = await query.OrderBy(d => d.Category).ThenBy(d => d.Name).ToListAsync();

            return list.Select(d => new DrillListItem
            {
                Id            = d.Id,
                Name          = d.Name,
                Category      = d.Category,
                CategoryLabel = d.Category switch
                {
                    TrainingCategory.Physical  => "Thể lực",
                    TrainingCategory.Technical => "Kỹ thuật",
                    TrainingCategory.Tactical  => "Chiến thuật",
                    _ => d.Category.ToString()
                },
                DurationMinutes = d.DurationMinutes,
                Difficulty      = d.Difficulty,
                VideoUrl        = d.VideoUrl,
                IsShared        = d.IsShared,
                IsActive        = d.IsActive,
                UsageCount      = d.TrainingStats.Count,
                CreatedByName   = d.CreatedByUser.Username
            }).ToList();
        }

        public async Task<DrillDetailViewModel?> GetDetailAsync(int id)
        {
            var d = await _db.Drills
                .Include(x => x.CreatedByUser)
                .Include(x => x.TrainingStats)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (d == null) return null;

            return new DrillDetailViewModel
            {
                Id            = d.Id,
                Name          = d.Name,
                Category      = d.Category,
                CategoryLabel = d.Category switch
                {
                    TrainingCategory.Physical  => "Thể lực",
                    TrainingCategory.Technical => "Kỹ thuật",
                    TrainingCategory.Tactical  => "Chiến thuật",
                    _ => d.Category.ToString()
                },
                Description    = d.Description,
                Instructions   = d.Instructions,
                DurationMinutes = d.DurationMinutes,
                Difficulty     = d.Difficulty,
                VideoUrl       = d.VideoUrl,
                IsShared       = d.IsShared,
                IsActive       = d.IsActive,
                CreatedByName  = d.CreatedByUser.Username,
                CreatedAt      = d.CreatedAt,
                UpdatedAt      = d.UpdatedAt,
                UsageCount     = d.TrainingStats.Count
            };
        }

        public async Task<DrillFormViewModel?> GetFormAsync(int id)
        {
            if (id == 0) return new DrillFormViewModel();

            var d = await _db.Drills.FindAsync(id);
            if (d == null) return null;

            return new DrillFormViewModel
            {
                Id              = d.Id,
                Name            = d.Name,
                Category        = d.Category,
                Description     = d.Description,
                Instructions    = d.Instructions,
                DurationMinutes = d.DurationMinutes,
                Difficulty      = d.Difficulty,
                VideoUrl        = d.VideoUrl,
                IsShared        = d.IsShared
            };
        }

        public async Task<int> CreateAsync(DrillFormViewModel model, int createdByUserId)
        {
            var drill = new Drill
            {
                Name            = model.Name,
                Category        = model.Category,
                Description     = model.Description,
                Instructions    = model.Instructions,
                DurationMinutes = model.DurationMinutes,
                Difficulty      = model.Difficulty,
                VideoUrl        = model.VideoUrl,
                IsShared        = model.IsShared,
                CreatedByUserId = createdByUserId,
                CreatedAt       = DateTime.Now,
                UpdatedAt       = DateTime.Now
            };

            _db.Drills.Add(drill);
            await _db.SaveChangesAsync();
            return drill.Id;
        }

        public async Task UpdateAsync(DrillFormViewModel model)
        {
            var drill = await _db.Drills.FindAsync(model.Id);
            if (drill == null) return;

            drill.Name            = model.Name;
            drill.Category        = model.Category;
            drill.Description     = model.Description;
            drill.Instructions    = model.Instructions;
            drill.DurationMinutes = model.DurationMinutes;
            drill.Difficulty      = model.Difficulty;
            drill.VideoUrl        = model.VideoUrl;
            drill.IsShared        = model.IsShared;
            drill.UpdatedAt       = DateTime.Now;

            await _db.SaveChangesAsync();
        }

        public async Task ToggleActiveAsync(int id)
        {
            var drill = await _db.Drills.FindAsync(id);
            if (drill == null) return;

            drill.IsActive  = !drill.IsActive;
            drill.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        public async Task<List<DrillSelectItem>> GetSelectListAsync(string? category = null)
        {
            var query = _db.Drills.Where(d => d.IsActive).AsQueryable();

            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<TrainingCategory>(category, out var cat))
            {
                query = query.Where(d => d.Category == cat);
            }

            return await query
                .OrderBy(d => d.Name)
                .Select(d => new DrillSelectItem
                {
                    Id              = d.Id,
                    Name            = d.Name,
                    Category        = d.Category,
                    DurationMinutes = d.DurationMinutes,
                    Description     = d.Description
                })
                .ToListAsync();
        }
    }
}
