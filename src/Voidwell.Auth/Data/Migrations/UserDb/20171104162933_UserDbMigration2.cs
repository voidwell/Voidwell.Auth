using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.Auth.Data.Migrations.UserDb
{
    public partial class UserDbMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("5323942c-5f25-4758-97c1-15c6fc144cc4"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("28e3507d-6185-4e47-bac8-daebf44b002c"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "AspNetUsers",
                type: "timestamptz",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("e0b61e4e-13e5-4c45-86ac-eb6b98610179"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("9221348d-036a-4b0c-9180-12fb78a3bf1b"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new Guid("28e3507d-6185-4e47-bac8-daebf44b002c"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("5323942c-5f25-4758-97c1-15c6fc144cc4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: new Guid("9221348d-036a-4b0c-9180-12fb78a3bf1b"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("e0b61e4e-13e5-4c45-86ac-eb6b98610179"));
        }
    }
}
