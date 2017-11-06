using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.Auth.Data.Data.Migrations.Voidwell.AuthDb
{
    public partial class AuthDbMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authentication_Users_UserId",
                schema: "public",
                table: "Authentication");

            migrationBuilder.DropForeignKey(
                name: "FK_Profile_Users_UserId",
                schema: "public",
                table: "Profile");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Authentication_Users_UserId",
                schema: "public",
                table: "Authentication",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Profile_Users_UserId",
                schema: "public",
                table: "Profile",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
