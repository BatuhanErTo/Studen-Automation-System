namespace Pusula.Student.Automation.Permissions;

public static class AutomationPermissions
{
    public const string GroupName = "Automation";

    public static class Dashboard
    {
        public const string DashboardGroup = GroupName + ".Dashboard";
        public const string Host = DashboardGroup + ".Host";
        public const string Tenant = DashboardGroup + ".Tenant";
    }
    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Teachers
    {
        public const string Default = GroupName + ".Teachers";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
    public static class Students
    {
        public const string Default = GroupName + ".Students";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Courses
    {
        public const string Default = GroupName + ".Courses";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";

        public static class CourseSessions
        {
            public const string Default = Courses.Default + ".CourseSessions";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }

        public static class GradeComponents
        {
            public const string Default = Courses.Default + ".GradeComponents";
            public const string Edit = Default + ".Edit";
            public const string Create = Default + ".Create";
            public const string Delete = Default + ".Delete";
        }
    }

    public static class Enrollments
    {
        public const string Default = GroupName + ".Enrollments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }

    public static class Departments
    {
        public const string Default = GroupName + ".Departments";
        public const string Edit = Default + ".Edit";
        public const string Create = Default + ".Create";
        public const string Delete = Default + ".Delete";
    }
}
