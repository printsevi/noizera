using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Noizera.Common.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationDbInitia222222l : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConnectedAccountStripeId",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConnectedAccountStripeId",
                table: "Users");
        }
    }
}
