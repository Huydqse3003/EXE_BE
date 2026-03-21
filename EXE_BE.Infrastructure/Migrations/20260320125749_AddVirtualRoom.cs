using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_BE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVirtualRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionState",
                table: "UserItemPositions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "VirtualRoomId",
                table: "UserItemPositions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "VirtualRooms",
                columns: table => new
                {
                    VirtualRoomId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackgroundImageUrl = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualRooms", x => x.VirtualRoomId);
                    table.ForeignKey(
                        name: "FK_VirtualRooms_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoomCharacters",
                columns: table => new
                {
                    CharacterId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VirtualRoomId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    X = table.Column<float>(type: "float", nullable: false),
                    Y = table.Column<float>(type: "float", nullable: false),
                    Z = table.Column<float>(type: "float", nullable: false),
                    ActionState = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direction = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomCharacters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_RoomCharacters_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoomCharacters_VirtualRooms_VirtualRoomId",
                        column: x => x.VirtualRoomId,
                        principalTable: "VirtualRooms",
                        principalColumn: "VirtualRoomId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserItemPositions_VirtualRoomId",
                table: "UserItemPositions",
                column: "VirtualRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomCharacters_UserId",
                table: "RoomCharacters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomCharacters_VirtualRoomId",
                table: "RoomCharacters",
                column: "VirtualRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_VirtualRooms_UserId",
                table: "VirtualRooms",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserItemPositions_VirtualRooms_VirtualRoomId",
                table: "UserItemPositions",
                column: "VirtualRoomId",
                principalTable: "VirtualRooms",
                principalColumn: "VirtualRoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserItemPositions_VirtualRooms_VirtualRoomId",
                table: "UserItemPositions");

            migrationBuilder.DropTable(
                name: "RoomCharacters");

            migrationBuilder.DropTable(
                name: "VirtualRooms");

            migrationBuilder.DropIndex(
                name: "IX_UserItemPositions_VirtualRoomId",
                table: "UserItemPositions");

            migrationBuilder.DropColumn(
                name: "ActionState",
                table: "UserItemPositions");

            migrationBuilder.DropColumn(
                name: "VirtualRoomId",
                table: "UserItemPositions");
        }
    }
}
