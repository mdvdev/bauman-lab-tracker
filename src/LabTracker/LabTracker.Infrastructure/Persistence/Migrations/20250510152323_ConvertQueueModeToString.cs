using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertQueueModeToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QueueMode",
                table: "Courses",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "QueueMode",
                table: "Courses",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
