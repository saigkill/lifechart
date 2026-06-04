using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeChart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsHypomanic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHypomanic",
                table: "DailyEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHypomanic",
                table: "DailyEntries");
        }
    }
}
