using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Student.Automation.Migrations
{
    /// <inheritdoc />
    public partial class db_designV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppAttendanceEntries_AppEnrollments_EnrollmentId1",
                table: "AppAttendanceEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AppCourseSessions_AppCourses_CourseId1",
                table: "AppCourseSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_AppGradeComponents_AppCourses_CourseId1",
                table: "AppGradeComponents");

            migrationBuilder.DropForeignKey(
                name: "FK_AppGradeEntries_AppEnrollments_EnrollmentId1",
                table: "AppGradeEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTeacherComments_AppEnrollments_EnrollmentId1",
                table: "AppTeacherComments");

            migrationBuilder.DropIndex(
                name: "IX_AppTeacherComments_EnrollmentId1",
                table: "AppTeacherComments");

            migrationBuilder.DropIndex(
                name: "IX_AppGradeEntries_EnrollmentId1",
                table: "AppGradeEntries");

            migrationBuilder.DropIndex(
                name: "IX_AppGradeComponents_CourseId1",
                table: "AppGradeComponents");

            migrationBuilder.DropIndex(
                name: "IX_AppCourseSessions_CourseId1",
                table: "AppCourseSessions");

            migrationBuilder.DropIndex(
                name: "IX_AppAttendanceEntries_EnrollmentId1",
                table: "AppAttendanceEntries");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "AppTeacherComments");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "AppGradeEntries");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "AppGradeComponents");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "AppCourseSessions");

            migrationBuilder.DropColumn(
                name: "EnrollmentId1",
                table: "AppAttendanceEntries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId1",
                table: "AppTeacherComments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId1",
                table: "AppGradeEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId1",
                table: "AppGradeComponents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId1",
                table: "AppCourseSessions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EnrollmentId1",
                table: "AppAttendanceEntries",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppTeacherComments_EnrollmentId1",
                table: "AppTeacherComments",
                column: "EnrollmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeEntries_EnrollmentId1",
                table: "AppGradeEntries",
                column: "EnrollmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeComponents_CourseId1",
                table: "AppGradeComponents",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppCourseSessions_CourseId1",
                table: "AppCourseSessions",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttendanceEntries_EnrollmentId1",
                table: "AppAttendanceEntries",
                column: "EnrollmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AppAttendanceEntries_AppEnrollments_EnrollmentId1",
                table: "AppAttendanceEntries",
                column: "EnrollmentId1",
                principalTable: "AppEnrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppCourseSessions_AppCourses_CourseId1",
                table: "AppCourseSessions",
                column: "CourseId1",
                principalTable: "AppCourses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGradeComponents_AppCourses_CourseId1",
                table: "AppGradeComponents",
                column: "CourseId1",
                principalTable: "AppCourses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppGradeEntries_AppEnrollments_EnrollmentId1",
                table: "AppGradeEntries",
                column: "EnrollmentId1",
                principalTable: "AppEnrollments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTeacherComments_AppEnrollments_EnrollmentId1",
                table: "AppTeacherComments",
                column: "EnrollmentId1",
                principalTable: "AppEnrollments",
                principalColumn: "Id");
        }
    }
}
