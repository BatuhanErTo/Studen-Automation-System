using Pusula.Student.Automation.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Pusula.Student.Automation.Permissions;

public class AutomationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AutomationPermissions.GroupName);
        myGroup.AddPermission(AutomationPermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        myGroup.AddPermission(AutomationPermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(AutomationPermissions.MyPermission1, L("Permission:MyPermission1"));
        var teacherPermission = myGroup.AddPermission(AutomationPermissions.Teachers.Default, L("Permission:Teachers"));
        teacherPermission.AddChild(AutomationPermissions.Teachers.Create, L("Permission:Create"));
        teacherPermission.AddChild(AutomationPermissions.Teachers.Edit, L("Permission:Edit"));
        teacherPermission.AddChild(AutomationPermissions.Teachers.Delete, L("Permission:Delete"));
       
        var studentPermission = myGroup.AddPermission(AutomationPermissions.Students.Default, L("Permission:Students"));
        studentPermission.AddChild(AutomationPermissions.Students.Create, L("Permission:Create"));
        studentPermission.AddChild(AutomationPermissions.Students.Edit, L("Permission:Edit"));
        studentPermission.AddChild(AutomationPermissions.Students.Delete, L("Permission:Delete"));
        
        var coursePermission = myGroup.AddPermission(AutomationPermissions.Courses.Default, L("Permission:Courses"));
        coursePermission.AddChild(AutomationPermissions.Courses.Create, L("Permission:Create"));
        coursePermission.AddChild(AutomationPermissions.Courses.Edit, L("Permission:Edit"));
        coursePermission.AddChild(AutomationPermissions.Courses.Delete, L("Permission:Delete"));

        // Courses -> CourseSessions (child)
        var courseSessionsPermission =
            coursePermission.AddChild(
                AutomationPermissions.Courses.CourseSessions.Default,
                L("Permission:CourseSessions"));
        courseSessionsPermission.AddChild(AutomationPermissions.Courses.CourseSessions.Create, L("Permission:Create"));
        courseSessionsPermission.AddChild(AutomationPermissions.Courses.CourseSessions.Edit, L("Permission:Edit"));
        courseSessionsPermission.AddChild(AutomationPermissions.Courses.CourseSessions.Delete, L("Permission:Delete"));

        // Courses -> GradeComponents (child)
        var gradeComponentsPermission =
            coursePermission.AddChild(
                AutomationPermissions.Courses.GradeComponents.Default,
                L("Permission:GradeComponents"));
        gradeComponentsPermission.AddChild(AutomationPermissions.Courses.GradeComponents.Create, L("Permission:Create"));
        gradeComponentsPermission.AddChild(AutomationPermissions.Courses.GradeComponents.Edit, L("Permission:Edit"));
        gradeComponentsPermission.AddChild(AutomationPermissions.Courses.GradeComponents.Delete, L("Permission:Delete"));


        var enrollmentPermission = myGroup.AddPermission(AutomationPermissions.Enrollments.Default, L("Permission:Enrollments"));
        enrollmentPermission.AddChild(AutomationPermissions.Enrollments.Create, L("Permission:Create"));
        enrollmentPermission.AddChild(AutomationPermissions.Enrollments.Edit, L("Permission:Edit"));
        enrollmentPermission.AddChild(AutomationPermissions.Enrollments.Delete, L("Permission:Delete"));


        // Enrollments -> AttendanceEntries (child)
        var attendanceEntriesPermission =
            enrollmentPermission.AddChild(
                AutomationPermissions.Enrollments.AttendanceEntries.Default,
                L("Permission:AttendanceEntries"));
        attendanceEntriesPermission.AddChild(AutomationPermissions.Enrollments.AttendanceEntries.Create, L("Permission:Create"));
        attendanceEntriesPermission.AddChild(AutomationPermissions.Enrollments.AttendanceEntries.Edit, L("Permission:Edit"));
        attendanceEntriesPermission.AddChild(AutomationPermissions.Enrollments.AttendanceEntries.Delete, L("Permission:Delete"));

        // Enrollments -> GradeEntries (child)
        var gradeEntriesPermission =
            enrollmentPermission.AddChild(
                AutomationPermissions.Enrollments.GradeEntries.Default,
                L("Permission:GradeEntries"));
        gradeEntriesPermission.AddChild(AutomationPermissions.Enrollments.GradeEntries.Create, L("Permission:Create"));
        gradeEntriesPermission.AddChild(AutomationPermissions.Enrollments.GradeEntries.Edit, L("Permission:Edit"));
        gradeEntriesPermission.AddChild(AutomationPermissions.Enrollments.GradeEntries.Delete, L("Permission:Delete"));

        // Enrollments -> TeacherComments (child)
        var teacherCommentsPermission=
            enrollmentPermission.AddChild(
                AutomationPermissions.Enrollments.TeacherComments.Default,
                L("Permission:TeacherComments"));
        teacherCommentsPermission.AddChild(AutomationPermissions.Enrollments.TeacherComments.Create, L("Permission:Create"));
        teacherCommentsPermission.AddChild(AutomationPermissions.Enrollments.TeacherComments.Edit, L("Permission:Edit"));
        teacherCommentsPermission.AddChild(AutomationPermissions.Enrollments.TeacherComments.Delete, L("Permission:Delete"));

        var departmentPermission = myGroup.AddPermission(AutomationPermissions.Departments.Default, L("Permission:Departments"));
        departmentPermission.AddChild(AutomationPermissions.Departments.Create, L("Permission:Create"));
        departmentPermission.AddChild(AutomationPermissions.Departments.Edit, L("Permission:Edit"));
        departmentPermission.AddChild(AutomationPermissions.Departments.Delete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AutomationResource>(name);
    }
}
