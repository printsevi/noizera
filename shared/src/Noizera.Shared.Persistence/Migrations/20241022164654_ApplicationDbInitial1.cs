using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Shared.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbInitial1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Profiles",
                newName: "Bio");

            migrationBuilder.AddColumn<DateOnly>(
                name: "AlbumReleaseDate",
                table: "MusicSets",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumReleaseDate",
                table: "MusicSets");

            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "Profiles",
                newName: "Description");
        }
    }
}
