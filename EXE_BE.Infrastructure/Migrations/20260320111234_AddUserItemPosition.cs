using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE_BE.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserItemPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserItemPositions",
                columns: table => new
                {
                    PositionId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    X = table.Column<float>(type: "float", nullable: false),
                    Y = table.Column<float>(type: "float", nullable: false),
                    Z = table.Column<float>(type: "float", nullable: false),
                    RoomMode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserItemPositions", x => x.PositionId);
                    table.ForeignKey(
                        name: "FK_UserItemPositions_GameItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "GameItems",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserItemPositions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserItemPositions_ItemId",
                table: "UserItemPositions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserItemPositions_UserId",
                table: "UserItemPositions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserItemPositions");
        }
    }
}
