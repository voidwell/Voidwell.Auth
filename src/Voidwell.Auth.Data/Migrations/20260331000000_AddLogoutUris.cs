using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voidwell.Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoutUris : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FrontChannelLogoutUri",
                table: "OpenIddictApplications",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackChannelLogoutUri",
                table: "OpenIddictApplications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrontChannelLogoutUri",
                table: "OpenIddictApplications");

            migrationBuilder.DropColumn(
                name: "BackChannelLogoutUri",
                table: "OpenIddictApplications");
        }
    }
}
