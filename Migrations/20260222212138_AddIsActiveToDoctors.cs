using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hastane_Otomasyon.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToDoctors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "doctor",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "doctor");
        }
    }
}
