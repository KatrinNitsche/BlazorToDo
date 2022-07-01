using Microsoft.EntityFrameworkCore.Migrations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Migrations
{
    public partial class AddedRepitionTypeToToDoListEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RepetitionType",
                table: "ToDoList",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepetitionType",
                table: "ToDoList");
        }
    }
}
