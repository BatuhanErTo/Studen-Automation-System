using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Student.Automation.Migrations
{
    /// <inheritdoc />
    public partial class db_designV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDepartments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LastName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IdentityNumber = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GradeYear = table.Column<int>(type: "integer", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppStudents_AppDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "AppDepartments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    LastName = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    EmailAddress = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTeachers_AppDepartments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "AppDepartments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Credits = table.Column<int>(type: "integer", nullable: false),
                    StartFrom = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EndTo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCourses_AppTeachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "AppTeachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppCourseSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Day = table.Column<int>(type: "integer", nullable: false),
                    Start = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    End = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    CourseId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCourseSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCourseSessions_AppCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "AppCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppCourseSessions_AppCourses_CourseId1",
                        column: x => x.CourseId1,
                        principalTable: "AppCourses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppEnrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppEnrollments_AppCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "AppCourses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppEnrollments_AppStudents_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AppStudents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppGradeComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeComponentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    CourseId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGradeComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppGradeComponents_AppCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "AppCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppGradeComponents_AppCourses_CourseId1",
                        column: x => x.CourseId1,
                        principalTable: "AppCourses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppAttendanceEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    CourseSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttendanceStatus = table.Column<int>(type: "integer", nullable: false),
                    AbsentReason = table.Column<string>(type: "text", nullable: true),
                    EnrollmentId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppAttendanceEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppAttendanceEntries_AppCourseSessions_CourseSessionId",
                        column: x => x.CourseSessionId,
                        principalTable: "AppCourseSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppAttendanceEntries_AppEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "AppEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppAttendanceEntries_AppEnrollments_EnrollmentId1",
                        column: x => x.EnrollmentId1,
                        principalTable: "AppEnrollments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppTeacherComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EnrollmentId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppTeacherComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppTeacherComments_AppEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "AppEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppTeacherComments_AppEnrollments_EnrollmentId1",
                        column: x => x.EnrollmentId1,
                        principalTable: "AppEnrollments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppGradeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: false),
                    EnrollmentId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppGradeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppGradeEntries_AppEnrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "AppEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppGradeEntries_AppEnrollments_EnrollmentId1",
                        column: x => x.EnrollmentId1,
                        principalTable: "AppEnrollments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppGradeEntries_AppGradeComponents_GradeComponentId",
                        column: x => x.GradeComponentId,
                        principalTable: "AppGradeComponents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppAttendanceEntries_CourseSessionId",
                table: "AppAttendanceEntries",
                column: "CourseSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppAttendanceEntries_EnrollmentId_Date_CourseSessionId",
                table: "AppAttendanceEntries",
                columns: new[] { "EnrollmentId", "Date", "CourseSessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppAttendanceEntries_EnrollmentId1",
                table: "AppAttendanceEntries",
                column: "EnrollmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppCourses_CourseName",
                table: "AppCourses",
                column: "CourseName");

            migrationBuilder.CreateIndex(
                name: "IX_AppCourses_TeacherId_Status",
                table: "AppCourses",
                columns: new[] { "TeacherId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCourseSessions_CourseId",
                table: "AppCourseSessions",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCourseSessions_CourseId1",
                table: "AppCourseSessions",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppDepartments_DepartmentName",
                table: "AppDepartments",
                column: "DepartmentName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEnrollments_CourseId_StudentId",
                table: "AppEnrollments",
                columns: new[] { "CourseId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppEnrollments_StudentId",
                table: "AppEnrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeComponents_CourseId_GradeComponentName",
                table: "AppGradeComponents",
                columns: new[] { "CourseId", "GradeComponentName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeComponents_CourseId1",
                table: "AppGradeComponents",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeEntries_EnrollmentId_GradeComponentId",
                table: "AppGradeEntries",
                columns: new[] { "EnrollmentId", "GradeComponentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeEntries_EnrollmentId1",
                table: "AppGradeEntries",
                column: "EnrollmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppGradeEntries_GradeComponentId",
                table: "AppGradeEntries",
                column: "GradeComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStudents_DepartmentId",
                table: "AppStudents",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppStudents_EmailAddress",
                table: "AppStudents",
                column: "EmailAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppStudents_IdentityNumber",
                table: "AppStudents",
                column: "IdentityNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppTeacherComments_EnrollmentId",
                table: "AppTeacherComments",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppTeacherComments_EnrollmentId1",
                table: "AppTeacherComments",
                column: "EnrollmentId1");

            migrationBuilder.CreateIndex(
                name: "IX_AppTeachers_DepartmentId",
                table: "AppTeachers",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppAttendanceEntries");

            migrationBuilder.DropTable(
                name: "AppGradeEntries");

            migrationBuilder.DropTable(
                name: "AppTeacherComments");

            migrationBuilder.DropTable(
                name: "AppCourseSessions");

            migrationBuilder.DropTable(
                name: "AppGradeComponents");

            migrationBuilder.DropTable(
                name: "AppEnrollments");

            migrationBuilder.DropTable(
                name: "AppCourses");

            migrationBuilder.DropTable(
                name: "AppStudents");

            migrationBuilder.DropTable(
                name: "AppTeachers");

            migrationBuilder.DropTable(
                name: "AppDepartments");
        }
    }
}
