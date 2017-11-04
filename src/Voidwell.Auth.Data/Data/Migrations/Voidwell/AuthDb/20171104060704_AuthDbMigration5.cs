using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.Auth.Data.Data.Migrations.Voidwell.AuthDb
{
    public partial class AuthDbMigration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Profile_DisplayName",
                schema: "public",
                table: "Profile",
                column: "DisplayName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profile_Email",
                schema: "public",
                table: "Profile",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Profile_DisplayName",
                schema: "public",
                table: "Profile");

            migrationBuilder.DropIndex(
                name: "IX_Profile_Email",
                schema: "public",
                table: "Profile");
        }
    }
}
