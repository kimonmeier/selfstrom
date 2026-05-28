using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfStrom.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMissingRoomNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Room",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Number",
                table: "Room");
        }
    }
}
