using FootballManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<TeamTactics> TeamTactics { get; set; }
        public DbSet<PlayerAttendance> PlayerAttendances { get; set; }
        public DbSet<MatchEvent> MatchEvents { get; set; }
        public DbSet<PlayerMatchStats> PlayerMatchStats { get; set; }
        public DbSet<PlayerTrainingStats> PlayerTrainingStats { get; set; }
        public DbSet<PerformanceRating> PerformanceRatings { get; set; }
        public DbSet<Drill> Drills { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<TrainingSessionDrill> TrainingSessionDrills { get; set; }
        public DbSet<TrainingAttendance> TrainingAttendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Match → HomeTeam: Restrict để tránh cascade conflict
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // Match → AwayTeam: Restrict
            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Player (optional FK cho role Player)
            modelBuilder.Entity<User>()
                .HasOne(u => u.PlayerProfile)
                .WithMany()
                .HasForeignKey(u => u.PlayerId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            // Unique: mỗi Player chỉ link với 1 tài khoản
            modelBuilder.Entity<User>()
                .HasIndex(u => u.PlayerId)
                .IsUnique()
                .HasFilter("[PlayerId] IS NOT NULL");

            // Unique index cho Team.Name
            modelBuilder.Entity<Team>()
                .HasIndex(t => t.Name)
                .IsUnique();

            // MatchStatus lưu dưới dạng string
            modelBuilder.Entity<Match>()
                .Property(m => m.Status)
                .HasConversion<string>();

            // PlayerAttendance → Match: Cascade
            modelBuilder.Entity<PlayerAttendance>()
                .HasOne(pa => pa.Match)
                .WithMany(m => m.PlayerAttendances)
                .HasForeignKey(pa => pa.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlayerAttendance → Player: Restrict
            modelBuilder.Entity<PlayerAttendance>()
                .HasOne(pa => pa.Player)
                .WithMany(p => p.Attendances)
                .HasForeignKey(pa => pa.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique: mỗi cầu thủ chỉ có 1 bản ghi điểm danh / trận
            modelBuilder.Entity<PlayerAttendance>()
                .HasIndex(pa => new { pa.MatchId, pa.PlayerId })
                .IsUnique();

            // AttendanceStatus lưu dạng string
            modelBuilder.Entity<PlayerAttendance>()
                .Property(pa => pa.Status)
                .HasConversion<string>();

            // MatchEvent → Match: Cascade
            modelBuilder.Entity<MatchEvent>()
                .HasOne(e => e.Match)
                .WithMany(m => m.Events)
                .HasForeignKey(e => e.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // MatchEvent → Player: Restrict
            modelBuilder.Entity<MatchEvent>()
                .HasOne(e => e.Player)
                .WithMany(p => p.Events)
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // MatchEvent → SubstitutePlayer: NoAction
            modelBuilder.Entity<MatchEvent>()
                .HasOne(e => e.SubstitutePlayer)
                .WithMany()
                .HasForeignKey(e => e.SubstitutePlayerId)
                .OnDelete(DeleteBehavior.NoAction);

            // EventType lưu dạng string
            modelBuilder.Entity<MatchEvent>()
                .Property(e => e.EventType)
                .HasConversion<string>();

            // PlayerMatchStats → Player: Restrict
            modelBuilder.Entity<PlayerMatchStats>()
                .HasOne(s => s.Player)
                .WithMany(p => p.MatchStats)
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PlayerMatchStats → Match: Cascade
            modelBuilder.Entity<PlayerMatchStats>()
                .HasOne(s => s.Match)
                .WithMany()
                .HasForeignKey(s => s.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique: mỗi cầu thủ chỉ có 1 bản ghi stats / trận
            modelBuilder.Entity<PlayerMatchStats>()
                .HasIndex(s => new { s.PlayerId, s.MatchId })
                .IsUnique();

            // TeamTactics → Team: Cascade
            modelBuilder.Entity<TeamTactics>()
                .HasOne(t => t.Team)
                .WithMany()
                .HasForeignKey(t => t.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique: mỗi đội chỉ có 1 bản ghi tactics
            modelBuilder.Entity<TeamTactics>()
                .HasIndex(t => t.TeamId)
                .IsUnique();

            // PlayerTrainingStats → Player: Cascade
            modelBuilder.Entity<PlayerTrainingStats>()
                .HasOne(s => s.Player)
                .WithMany(p => p.TrainingStats)
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // PlayerTrainingStats → Drill: Restrict (optional FK)
            modelBuilder.Entity<PlayerTrainingStats>()
                .HasOne(s => s.Drill)
                .WithMany(d => d.TrainingStats)
                .HasForeignKey(s => s.DrillId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Enum conversions
            modelBuilder.Entity<Player>()
                .Property(p => p.HealthStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Player>()
                .Property(p => p.Position)
                .HasConversion<string>();

            modelBuilder.Entity<PlayerTrainingStats>()
                .Property(s => s.Category)
                .HasConversion<string>();

            // PerformanceRating → Player: Restrict
            modelBuilder.Entity<PerformanceRating>()
                .HasOne(r => r.Player)
                .WithMany()
                .HasForeignKey(r => r.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PerformanceRating → RatedByUser: Restrict
            modelBuilder.Entity<PerformanceRating>()
                .HasOne(r => r.RatedByUser)
                .WithMany()
                .HasForeignKey(r => r.RatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // PerformanceRating → Match: Restrict (optional FK)
            modelBuilder.Entity<PerformanceRating>()
                .HasOne(r => r.Match)
                .WithMany()
                .HasForeignKey(r => r.MatchId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // PerformanceRating → PlayerTrainingStats: Restrict (optional FK)
            modelBuilder.Entity<PerformanceRating>()
                .HasOne(r => r.TrainingStats)
                .WithMany()
                .HasForeignKey(r => r.TrainingStatsId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // RatingType lưu dạng string
            modelBuilder.Entity<PerformanceRating>()
                .Property(r => r.RatingType)
                .HasConversion<string>();

            // Index tìm kiếm nhanh theo cầu thủ + ngày
            modelBuilder.Entity<PerformanceRating>()
                .HasIndex(r => new { r.PlayerId, r.RatingDate });

            // ===================== DRILL LIBRARY =====================

            // Drill → CreatedByUser: Restrict
            modelBuilder.Entity<Drill>()
                .HasOne(d => d.CreatedByUser)
                .WithMany()
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Drill.Category lưu dạng string
            modelBuilder.Entity<Drill>()
                .Property(d => d.Category)
                .HasConversion<string>();

            // TrainingSession → Team: Restrict
            modelBuilder.Entity<TrainingSession>()
                .HasOne(s => s.Team)
                .WithMany()
                .HasForeignKey(s => s.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingSession → CreatedByUser: Restrict
            modelBuilder.Entity<TrainingSession>()
                .HasOne(s => s.CreatedByUser)
                .WithMany()
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainingSessionStatus lưu dạng string
            modelBuilder.Entity<TrainingSession>()
                .Property(s => s.Status)
                .HasConversion<string>();

            // TrainingSessionDrill → TrainingSession: Cascade
            modelBuilder.Entity<TrainingSessionDrill>()
                .HasOne(sd => sd.TrainingSession)
                .WithMany(s => s.SessionDrills)
                .HasForeignKey(sd => sd.TrainingSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrainingSessionDrill → Drill: Restrict
            modelBuilder.Entity<TrainingSessionDrill>()
                .HasOne(sd => sd.Drill)
                .WithMany(d => d.SessionDrills)
                .HasForeignKey(sd => sd.DrillId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique: mỗi drill chỉ xuất hiện 1 lần / buổi tập
            modelBuilder.Entity<TrainingSessionDrill>()
                .HasIndex(sd => new { sd.TrainingSessionId, sd.DrillId })
                .IsUnique();

            // Index tìm kiếm buổi tập theo đội + ngày
            modelBuilder.Entity<TrainingSession>()
                .HasIndex(s => new { s.TeamId, s.ScheduledAt });

            // ===================== TRAINING ATTENDANCE =====================

            // TrainingAttendance → TrainingSession: Cascade
            modelBuilder.Entity<TrainingAttendance>()
                .HasOne(a => a.TrainingSession)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.TrainingSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // TrainingAttendance → Player: Restrict
            modelBuilder.Entity<TrainingAttendance>()
                .HasOne(a => a.Player)
                .WithMany()
                .HasForeignKey(a => a.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique: mỗi cầu thủ chỉ có 1 bản ghi / buổi tập
            modelBuilder.Entity<TrainingAttendance>()
                .HasIndex(a => new { a.TrainingSessionId, a.PlayerId })
                .IsUnique();

            // Status lưu dạng string
            modelBuilder.Entity<TrainingAttendance>()
                .Property(a => a.Status)
                .HasConversion<string>();
        }
    }
}
