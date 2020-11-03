using Microsoft.EntityFrameworkCore.Migrations;

namespace RAMWebsite.Migrations.AppDb
{
    public partial class CodeColumnInTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Tasks",
                maxLength: 4,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Tasks");
        }
    }
}
