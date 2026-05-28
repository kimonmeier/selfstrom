using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfStrom.Worker.Migrations
{
    /// <inheritdoc />
    public partial class InitialDesign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConnectedDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MacAdress = table.Column<string>(type: "TEXT", nullable: false),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectedDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataPoint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeviceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Value = table.Column<double>(type: "REAL", nullable: false),
                    IsSynced = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataPoint_ConnectedDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "ConnectedDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConnectedDevices_MacAdress",
                table: "ConnectedDevices",
                column: "MacAdress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataPoint_DeviceId",
                table: "DataPoint",
                column: "DeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataPoint");

            migrationBuilder.DropTable(
                name: "ConnectedDevices");
        }
    }
}
