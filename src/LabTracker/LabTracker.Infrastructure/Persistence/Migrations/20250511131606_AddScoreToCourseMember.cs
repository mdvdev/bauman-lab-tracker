using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreToCourseMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "CourseMembers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "CourseMembers");
        }
    }
}
