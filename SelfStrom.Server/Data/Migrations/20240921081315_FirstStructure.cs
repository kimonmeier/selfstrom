using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SelfStrom.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class FirstStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Home",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Number = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Home_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Worker",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    WorkerName = table.Column<string>(type: "TEXT", maxLength: 254, nullable: false),
                    HomeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Worker_Home_HomeId",
                        column: x => x.HomeId,
                        principalTable: "Home",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Home_UserId",
                table: "Home",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Worker_HomeId",
                table: "Worker",
                column: "HomeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Worker");

            migrationBuilder.DropTable(
                name: "Home");
        }
    }
}
