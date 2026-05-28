using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfStrom.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReworkedWorker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Worker",
                newName: "Number");

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Worker",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Worker");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Worker",
                newName: "IsDeleted");
        }
    }
}
