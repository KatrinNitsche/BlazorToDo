using Microsoft.EntityFrameworkCore.Migrations;

namespace CollaborateSoftware.MyLittleHelpers.Backend.Migrations
{
    public partial class AddedCategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ToDoList",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Icon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoList_CategoryId",
                table: "ToDoList",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoList_Category_CategoryId",
                table: "ToDoList",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoList_Category_CategoryId",
                table: "ToDoList");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_ToDoList_CategoryId",
                table: "ToDoList");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ToDoList");
        }
    }
}
