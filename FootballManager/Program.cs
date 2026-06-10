using FootballManager.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// EF Core + SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<FootballManager.Services.ITeamService, FootballManager.Services.TeamService>();
builder.Services.AddScoped<FootballManager.Services.IMatchService, FootballManager.Services.MatchService>();
builder.Services.AddScoped<FootballManager.Services.IStatisticsService, FootballManager.Services.StatisticsService>();
builder.Services.AddScoped<FootballManager.Services.IPlayerService, FootballManager.Services.PlayerService>();
builder.Services.AddScoped<FootballManager.Services.IAttendanceService, FootballManager.Services.AttendanceService>();
builder.Services.AddScoped<FootballManager.Services.IMatchEventService, FootballManager.Services.MatchEventService>();
builder.Services.AddScoped<FootballManager.Services.IPlayerStatsService, FootballManager.Services.PlayerStatsService>();
builder.Services.AddScoped<FootballManager.Services.IPlayerStatsDetailService, FootballManager.Services.PlayerStatsDetailService>();
builder.Services.AddScoped<FootballManager.Services.IPerformanceRatingService, FootballManager.Services.PerformanceRatingService>();
builder.Services.AddScoped<FootballManager.Services.IDrillService, FootballManager.Services.DrillService>();
builder.Services.AddScoped<FootballManager.Services.ITrainingSessionService, FootballManager.Services.TrainingSessionService>();
builder.Services.AddScoped<FootballManager.Services.ICalendarService, FootballManager.Services.CalendarService>();

// Cookie Authentication
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

// Seed dữ liệu ban đầu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    FootballManager.Data.SeedData.Initialize(db);
}

app.Run();
