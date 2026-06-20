using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootballManager.Migrations
{
    /// <inheritdoc />
    public partial class FixIsActiveDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fix: tất cả user hiện có bị IsActive=false do migration cũ
            // set defaultValue: false. Update về true cho toàn bộ.
            migrationBuilder.Sql("UPDATE [Users] SET [IsActive] = 1 WHERE [IsActive] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Không rollback — không thể biết user nào vốn dĩ đã inactive
        }
    }
}
