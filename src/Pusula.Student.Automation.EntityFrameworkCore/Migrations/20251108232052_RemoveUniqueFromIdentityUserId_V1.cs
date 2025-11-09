using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Student.Automation.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueFromIdentityUserId_V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdentityUserId",
                table: "AppTeachers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "IdentityUserId",
                table: "AppStudents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppTeachers_IdentityUserId",
                table: "AppTeachers",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStudents_IdentityUserId",
                table: "AppStudents",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppStudents_AbpUsers_IdentityUserId",
                table: "AppStudents",
                column: "IdentityUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTeachers_AbpUsers_IdentityUserId",
                table: "AppTeachers",
                column: "IdentityUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppStudents_AbpUsers_IdentityUserId",
                table: "AppStudents");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTeachers_AbpUsers_IdentityUserId",
                table: "AppTeachers");

            migrationBuilder.DropIndex(
                name: "IX_AppTeachers_IdentityUserId",
                table: "AppTeachers");

            migrationBuilder.DropIndex(
                name: "IX_AppStudents_IdentityUserId",
                table: "AppStudents");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "AppTeachers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "AppStudents");
        }
    }
}
