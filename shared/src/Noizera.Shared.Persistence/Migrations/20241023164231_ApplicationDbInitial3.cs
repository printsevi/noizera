using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Shared.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbInitial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "MusicSets");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "AlbumReleaseDate",
                table: "MusicSets",
                type: "DATE",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "AlbumReleaseDate",
                table: "MusicSets",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "DATE",
                oldNullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ReleaseDate",
                table: "MusicSets",
                type: "date",
                nullable: true);
        }
    }
}
