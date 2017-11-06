using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Voidwell.Auth.Data.Migrations.UserDb
{
    public partial class UserDbMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("988b234b-0dc2-44a7-ab60-643c83a1e8b4"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("5323942c-5f25-4758-97c1-15c6fc144cc4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("04f1b139-ced0-47f5-8500-389446361d14"),
                oldClrType: typeof(Guid),
                oldDefaultValue: new Guid("e0b61e4e-13e5-4c45-86ac-eb6b98610179"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new Guid("5323942c-5f25-4758-97c1-15c6fc144cc4"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("988b234b-0dc2-44a7-ab60-643c83a1e8b4"));

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetRoles",
                nullable: false,
                defaultValue: new Guid("e0b61e4e-13e5-4c45-86ac-eb6b98610179"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValue: new Guid("04f1b139-ced0-47f5-8500-389446361d14"));
        }
    }
}
