using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.Auth.Data.Data.Migrations.Voidwell.AuthDb
{
    public partial class AuthDbMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SecurityQuestions_Users_Id",
                schema: "public",
                table: "SecurityQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Authentication_Id",
                schema: "public",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Profile_Id",
                schema: "public",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_SecurityQuestions_Id",
                schema: "public",
                table: "SecurityQuestions");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "public",
                table: "SecurityQuestions");

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

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityQuestions_Users_UserId",
                schema: "public",
                table: "SecurityQuestions",
                column: "UserId",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authentication_Users_UserId",
                schema: "public",
                table: "Authentication");

            migrationBuilder.DropForeignKey(
                name: "FK_Profile_Users_UserId",
                schema: "public",
                table: "Profile");

            migrationBuilder.DropForeignKey(
                name: "FK_SecurityQuestions_Users_UserId",
                schema: "public",
                table: "SecurityQuestions");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "public",
                table: "SecurityQuestions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecurityQuestions_Id",
                schema: "public",
                table: "SecurityQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SecurityQuestions_Users_Id",
                schema: "public",
                table: "SecurityQuestions",
                column: "Id",
                principalSchema: "public",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Authentication_Id",
                schema: "public",
                table: "Users",
                column: "Id",
                principalSchema: "public",
                principalTable: "Authentication",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Profile_Id",
                schema: "public",
                table: "Users",
                column: "Id",
                principalSchema: "public",
                principalTable: "Profile",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
