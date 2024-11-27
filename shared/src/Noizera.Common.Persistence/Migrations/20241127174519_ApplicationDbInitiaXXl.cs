using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbInitiaXXl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedAsPreview",
                table: "MusicSets");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileType",
                table: "AlbumCredits",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowedAsPreview",
                table: "MusicSets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ProfileType",
                table: "AlbumCredits",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
