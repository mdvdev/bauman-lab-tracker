using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Group",
                table: "AspNetUsers",
                column: "Group");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Group",
                table: "AspNetUsers");
            
            migrationBuilder.DropColumn(
                name: "Group",
                table: "AspNetUsers");
        }
    }
}
