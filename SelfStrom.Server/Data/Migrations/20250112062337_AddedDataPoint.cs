using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfStrom.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedDataPoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DataPoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataPoint_Device_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Device",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataPoint_DeviceId_Timestamp",
                table: "DataPoint",
                columns: new[] { "DeviceId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataPoint");
        }
    }
}
