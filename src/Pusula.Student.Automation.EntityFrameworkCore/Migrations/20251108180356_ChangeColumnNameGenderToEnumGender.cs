using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pusula.Student.Automation.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameGenderToEnumGender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "AppTeachers",
                newName: "EnumGender");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EnumGender",
                table: "AppTeachers",
                newName: "Gender");
        }
    }
}
