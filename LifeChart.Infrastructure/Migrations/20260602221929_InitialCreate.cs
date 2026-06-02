using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeChart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Mood = table.Column<int>(type: "INTEGER", nullable: false),
                    Functionality = table.Column<int>(type: "INTEGER", nullable: false),
                    SleepHours = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicationTaken = table.Column<bool>(type: "INTEGER", nullable: false),
                    MenstrualCycle = table.Column<bool>(type: "INTEGER", nullable: false),
                    Symptoms = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Dosage = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MinStock = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentStock = table.Column<int>(type: "INTEGER", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IntakeTime",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Time = table.Column<string>(type: "TEXT", nullable: false),
                    DoseCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MedicationId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntakeTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IntakeTime_Medications_MedicationId",
                        column: x => x.MedicationId,
                        principalTable: "Medications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyEntries_Date",
                table: "DailyEntries",
                column: "Date",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IntakeTime_MedicationId",
                table: "IntakeTime",
                column: "MedicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyEntries");

            migrationBuilder.DropTable(
                name: "IntakeTime");

            migrationBuilder.DropTable(
                name: "Medications");
        }
    }
}
