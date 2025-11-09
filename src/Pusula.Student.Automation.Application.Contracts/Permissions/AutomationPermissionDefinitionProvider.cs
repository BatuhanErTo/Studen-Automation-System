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
        var enrollmentPermission = myGroup.AddPermission(AutomationPermissions.Enrollments.Default, L("Permission:Enrollments"));
        enrollmentPermission.AddChild(AutomationPermissions.Enrollments.Create, L("Permission:Create"));
        enrollmentPermission.AddChild(AutomationPermissions.Enrollments.Edit, L("Permission:Edit"));
        enrollmentPermission.AddChild(AutomationPermissions.Enrollments.Delete, L("Permission:Delete"));
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
