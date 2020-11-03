using Microsoft.EntityFrameworkCore.Migrations;

namespace RAMWebsite.Migrations.AppDb
{
    public partial class ExampleColumnInTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tasks",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4)",
                oldMaxLength: 4);

            migrationBuilder.AddColumn<string>(
                name: "ExampleInput",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExampleOutput",
                table: "Tasks",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SolvedNumber",
                table: "Tasks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExampleInput",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ExampleOutput",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SolvedNumber",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Tasks",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 8);
        }
    }
}
