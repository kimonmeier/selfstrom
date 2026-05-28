using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfStrom.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsDeletedFlagForDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Device",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Device");
        }
    }
}
