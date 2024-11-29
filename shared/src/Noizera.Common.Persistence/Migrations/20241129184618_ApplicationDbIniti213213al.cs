using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbIniti213213al : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedMusicSets",
                table: "SavedMusicSets");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "SavedMusicSets",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedMusicSets",
                table: "SavedMusicSets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SavedMusicSets_UserId",
                table: "SavedMusicSets",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SavedMusicSets",
                table: "SavedMusicSets");

            migrationBuilder.DropIndex(
                name: "IX_SavedMusicSets_UserId",
                table: "SavedMusicSets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SavedMusicSets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavedMusicSets",
                table: "SavedMusicSets",
                columns: new[] { "UserId", "MusicSetId" });
        }
    }
}
