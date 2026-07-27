using FootballManager.Models;
using System.Security.Cryptography;
using System.Text;

namespace FootballManager.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext db)
        {
            // Chỉ seed nếu chưa có user nào
            if (db.Users.Any()) return;

            // ══════════════════════════════════════
            //  USERS
            // ══════════════════════════════════════
            var admin = new User { FullName = "Administrator",  Username = "admin",   PasswordHash = HashPassword("admin123"),  Role = "Admin",   IsActive = true };
            var coach1 = new User { FullName = "Nguyễn Văn Hùng", Username = "coach1",  PasswordHash = HashPassword("coach123"),  Role = "Coach",   IsActive = true };
            var coach2 = new User { FullName = "Trần Minh Tuấn",  Username = "coach2",  PasswordHash = HashPassword("coach123"),  Role = "Coach",   IsActive = true };
            var analyst = new User { FullName = "Lê Thị Hà",      Username = "analyst", PasswordHash = HashPassword("analyst123"), Role = "Analyst", IsActive = true };
            var member  = new User { FullName = "Phạm Văn Thành", Username = "member",  PasswordHash = HashPassword("member123"),  Role = "Member",  IsActive = true };

            db.Users.AddRange(admin, coach1, coach2, analyst, member);
            db.SaveChanges();

            // ══════════════════════════════════════
            //  TEAMS
            // ══════════════════════════════════════
            var teamA = new Team { Name = "FC Rồng Vàng",  HomeGround = "Sân Mỹ Đình",     Description = "Đội bóng mạnh nhất khu vực phía Bắc" };
            var teamB = new Team { Name = "FC Sao Biển",   HomeGround = "Sân Thống Nhất",   Description = "Câu lạc bộ truyền thống miền Nam" };
            var teamC = new Team { Name = "FC Bão Lửa",    HomeGround = "Sân Hàng Đẫy",     Description = "Đội trẻ đầy tiềm năng" };

            db.Teams.AddRange(teamA, teamB, teamC);
            db.SaveChanges();

            // ══════════════════════════════════════
            //  PLAYERS — Team A (FC Rồng Vàng)
            // ══════════════════════════════════════
            var playersA = new List<Player>
            {
                new Player { FullName="Nguyễn Thanh Long",   JerseyNumber=1,  Position=PlayerPosition.GoalKeeper,   TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Trần Văn Bình",       JerseyNumber=4,  Position=PlayerPosition.CenterBack,   TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Lê Hoàng Nam",        JerseyNumber=5,  Position=PlayerPosition.CenterBack,   TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Phạm Đức Huy",        JerseyNumber=2,  Position=PlayerPosition.FullBack,     TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Ngô Minh Chiến",      JerseyNumber=3,  Position=PlayerPosition.FullBack,     TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Vũ Quang Hải",        JerseyNumber=6,  Position=PlayerPosition.DefensiveMid, TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Đinh Văn Tài",        JerseyNumber=8,  Position=PlayerPosition.CentralMid,   TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Hoàng Anh Tuấn",      JerseyNumber=10, Position=PlayerPosition.AttackingMid, TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Lương Xuân Trường",   JerseyNumber=7,  Position=PlayerPosition.Winger,       TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Công Phượng",  JerseyNumber=9,  Position=PlayerPosition.Striker,      TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Đoàn Văn Hậu",        JerseyNumber=11, Position=PlayerPosition.Striker,      TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Bùi Tiến Dũng",       JerseyNumber=16, Position=PlayerPosition.GoalKeeper,   TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Recovering, HealthNote="Chấn thương vai nhẹ" },
                new Player { FullName="Phan Văn Đức",         JerseyNumber=14, Position=PlayerPosition.Winger,       TeamId=teamA.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
            };

            // ══════════════════════════════════════
            //  PLAYERS — Team B (FC Sao Biển)
            // ══════════════════════════════════════
            var playersB = new List<Player>
            {
                new Player { FullName="Đặng Văn Lâm",        JerseyNumber=1,  Position=PlayerPosition.GoalKeeper,   TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Quế Ngọc Hải",        JerseyNumber=5,  Position=PlayerPosition.CenterBack,   TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Phong Hồng Duy",JerseyNumber=4, Position=PlayerPosition.CenterBack,   TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Trần Đình Trọng",     JerseyNumber=2,  Position=PlayerPosition.FullBack,     TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Injured, HealthNote="Chấn thương gối" },
                new Player { FullName="Hồ Tấn Tài",          JerseyNumber=3,  Position=PlayerPosition.FullBack,     TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Tuấn Anh",     JerseyNumber=8,  Position=PlayerPosition.CentralMid,   TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Trần Minh Vương",     JerseyNumber=6,  Position=PlayerPosition.DefensiveMid, TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Văn Quyết",   JerseyNumber=10, Position=PlayerPosition.AttackingMid, TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Hà Đức Chinh",        JerseyNumber=9,  Position=PlayerPosition.Striker,      TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Tiến Linh",   JerseyNumber=11, Position=PlayerPosition.Striker,      TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Vũ Văn Thanh",        JerseyNumber=7,  Position=PlayerPosition.Winger,       TeamId=teamB.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
            };

            // ══════════════════════════════════════
            //  PLAYERS — Team C (FC Bão Lửa)
            // ══════════════════════════════════════
            var playersC = new List<Player>
            {
                new Player { FullName="Trần Nguyên Mạnh",   JerseyNumber=1,  Position=PlayerPosition.GoalKeeper,   TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Hữu Tuấn",   JerseyNumber=5,  Position=PlayerPosition.CenterBack,   TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Đỗ Duy Mạnh",        JerseyNumber=4,  Position=PlayerPosition.CenterBack,   TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Lê Phạm Thành Long", JerseyNumber=2,  Position=PlayerPosition.FullBack,     TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Trần Văn Kiên",      JerseyNumber=6,  Position=PlayerPosition.DefensiveMid, TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Nguyễn Quang Hải",  JerseyNumber=19, Position=PlayerPosition.AttackingMid, TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Trịnh Duy Quang",   JerseyNumber=8,  Position=PlayerPosition.CentralMid,   TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Ngô Hoàng Thịnh",  JerseyNumber=9,  Position=PlayerPosition.Striker,      TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Trần Bình Trọng",   JerseyNumber=7,  Position=PlayerPosition.Winger,       TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Fit },
                new Player { FullName="Vũ Minh Tuấn",      JerseyNumber=11, Position=PlayerPosition.Striker,      TeamId=teamC.Id, Nationality="Việt Nam", IsActive=true, HealthStatus=PlayerHealthStatus.Suspended, HealthNote="Treo giò 1 trận" },
            };

            db.Players.AddRange(playersA);
            db.Players.AddRange(playersB);
            db.Players.AddRange(playersC);
            db.SaveChanges();

            // ══════════════════════════════════════
            //  MATCHES
            // ══════════════════════════════════════
            var now = DateTime.Now;

            var match1 = new Match
            {
                HomeTeamId = teamA.Id, AwayTeamId = teamB.Id,
                MatchDate  = now.AddDays(-14), Venue = "Sân Mỹ Đình",
                Status     = MatchStatus.Completed,
                HomeScore  = 2, AwayScore = 1
            };
            var match2 = new Match
            {
                HomeTeamId = teamB.Id, AwayTeamId = teamC.Id,
                MatchDate  = now.AddDays(-7), Venue = "Sân Thống Nhất",
                Status     = MatchStatus.Completed,
                HomeScore  = 0, AwayScore = 0
            };
            var match3 = new Match
            {
                HomeTeamId = teamA.Id, AwayTeamId = teamC.Id,
                MatchDate  = now.AddDays(3), Venue = "Sân Mỹ Đình",
                Status     = MatchStatus.Scheduled
            };
            var match4 = new Match
            {
                HomeTeamId = teamC.Id, AwayTeamId = teamA.Id,
                MatchDate  = now.AddDays(10), Venue = "Sân Hàng Đẫy",
                Status     = MatchStatus.Scheduled
            };
            var match5 = new Match
            {
                HomeTeamId = teamB.Id, AwayTeamId = teamA.Id,
                MatchDate  = now.AddDays(17), Venue = "Sân Thống Nhất",
                Status     = MatchStatus.Scheduled
            };

            db.Matches.AddRange(match1, match2, match3, match4, match5);
            db.SaveChanges();

            // ══════════════════════════════════════
            //  MATCH EVENTS (trận đã hoàn thành)
            // ══════════════════════════════════════
            var striker1 = playersA.First(p => p.Position == PlayerPosition.Striker);
            var mid1     = playersA.First(p => p.Position == PlayerPosition.AttackingMid);
            var striker2 = playersB.First(p => p.Position == PlayerPosition.Striker);

            db.MatchEvents.AddRange(
                new MatchEvent { MatchId=match1.Id, PlayerId=striker1.Id, EventType=MatchEventType.Goal,       Minute=23, Description="Đánh đầu từ pha tạt biên" },
                new MatchEvent { MatchId=match1.Id, PlayerId=mid1.Id,     EventType=MatchEventType.Goal,       Minute=67, Description="Sút xa ngoài vòng cấm" },
                new MatchEvent { MatchId=match1.Id, PlayerId=striker2.Id, EventType=MatchEventType.Goal,       Minute=78, Description="Phạt đền" },
                new MatchEvent { MatchId=match1.Id, PlayerId=playersB.First(p => p.Position == PlayerPosition.DefensiveMid).Id,
                                                                           EventType=MatchEventType.YellowCard, Minute=45 },
                new MatchEvent { MatchId=match2.Id, PlayerId=playersB.First(p => p.Position == PlayerPosition.Striker).Id,
                                                                           EventType=MatchEventType.YellowCard, Minute=30 }
            );
            db.SaveChanges();

            // ══════════════════════════════════════
            //  DRILLS (thư viện bài tập)
            // ══════════════════════════════════════
            db.Drills.AddRange(
                new Drill { Name="Chạy bền 5km",         Category=TrainingCategory.Physical,  Difficulty=2, DurationMinutes=40, Description="Chạy bền cải thiện sức bền aerobic", IsShared=true, CreatedByUserId=coach1.Id },
                new Drill { Name="Tăng tốc 30m x 10",   Category=TrainingCategory.Physical,  Difficulty=3, DurationMinutes=20, Description="Bài tập tốc độ và sức mạnh bùng phát", IsShared=true, CreatedByUserId=coach1.Id },
                new Drill { Name="Rondo 5v2",            Category=TrainingCategory.Technical, Difficulty=2, DurationMinutes=20, Description="Giữ bóng vòng tròn 5 chống 2", IsShared=true, CreatedByUserId=coach1.Id },
                new Drill { Name="Dẫn bóng qua cọc",    Category=TrainingCategory.Technical, Difficulty=2, DurationMinutes=15, Description="Kỹ năng dẫn bóng và thay đổi hướng", IsShared=true, CreatedByUserId=coach1.Id },
                new Drill { Name="Sút cố định 20 quả",  Category=TrainingCategory.Technical, Difficulty=3, DurationMinutes=25, Description="Luyện kỹ thuật sút bóng chính xác", IsShared=true, CreatedByUserId=coach1.Id },
                new Drill { Name="Pressing cao 4-4-2",  Category=TrainingCategory.Tactical,  Difficulty=4, DurationMinutes=35, Description="Tổ chức pressing cao với sơ đồ 4-4-2", IsShared=true, CreatedByUserId=coach2.Id },
                new Drill { Name="Phản công nhanh",     Category=TrainingCategory.Tactical,  Difficulty=4, DurationMinutes=30, Description="Chuyển trạng thái phòng thủ sang tấn công", IsShared=true, CreatedByUserId=coach2.Id },
                new Drill { Name="Bóng chết đá phạt",  Category=TrainingCategory.Tactical,  Difficulty=3, DurationMinutes=20, Description="Các tình huống đá phạt cố định", IsShared=true, CreatedByUserId=coach2.Id }
            );
            db.SaveChanges();

            // ══════════════════════════════════════
            //  TEAM TACTICS
            // ══════════════════════════════════════
            db.TeamTactics.AddRange(
                new TeamTactics { TeamId=teamA.Id, Formation="4-3-3",   Notes="Tấn công biên, pressing cao, chuyền ngắn" },
                new TeamTactics { TeamId=teamB.Id, Formation="4-4-2",   Notes="Phòng thủ chắc, phản công nhanh" },
                new TeamTactics { TeamId=teamC.Id, Formation="4-2-3-1", Notes="Kiểm soát bóng, kiên nhẫn xây dựng" }
            );
            db.SaveChanges();

            // ══════════════════════════════════════
            //  TRAINING SESSIONS
            // ══════════════════════════════════════
            db.TrainingSessions.AddRange(
                new TrainingSession { TeamId=teamA.Id, Title="Tập thể lực trước trận",   ScheduledAt=now.AddDays(1).Date.AddHours(7),  Location="Sân tập A", Status=TrainingSessionStatus.Scheduled, CreatedByUserId=coach1.Id },
                new TrainingSession { TeamId=teamA.Id, Title="Chiến thuật vs Bão Lửa",  ScheduledAt=now.AddDays(2).Date.AddHours(16), Location="Phòng họp", Status=TrainingSessionStatus.Scheduled, CreatedByUserId=coach1.Id },
                new TrainingSession { TeamId=teamB.Id, Title="Hồi phục sau trận",        ScheduledAt=now.AddDays(1).Date.AddHours(9),  Location="Sân tập B", Status=TrainingSessionStatus.Scheduled, CreatedByUserId=coach2.Id }
            );
            db.SaveChanges();
        }

        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
