using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbInitia111111l : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MusicSets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MusicSets",
                type: "text",
                nullable: true);
        }
    }
}
