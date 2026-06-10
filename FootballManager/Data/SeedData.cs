using FootballManager.Models;
using System.Security.Cryptography;
using System.Text;

namespace FootballManager.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext db)
        {
            if (db.Users.Any()) return;

            db.Users.Add(new User
            {
                FullName = "Administrator",
                Username = "admin",
                PasswordHash = HashPassword("admin123"),
                Role = "Admin"
            });

            db.SaveChanges();
        }

        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
