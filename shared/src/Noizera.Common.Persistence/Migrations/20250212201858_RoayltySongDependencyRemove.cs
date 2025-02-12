using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RoayltySongDependencyRemove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Royalties_Songs_SongId",
                table: "Royalties");

            migrationBuilder.DropIndex(
                name: "IX_Royalties_SongId",
                table: "Royalties");

            migrationBuilder.DropColumn(
                name: "SongId",
                table: "Royalties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SongId",
                table: "Royalties",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Royalties_SongId",
                table: "Royalties",
                column: "SongId");

            migrationBuilder.AddForeignKey(
                name: "FK_Royalties_Songs_SongId",
                table: "Royalties",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id");
        }
    }
}
