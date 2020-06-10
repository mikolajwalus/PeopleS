using Microsoft.EntityFrameworkCore.Migrations;

namespace PeopleS.API.Migrations
{
    public partial class ValueUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Word",
                table: "Values",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Word",
                table: "Values");
        }
    }
}
