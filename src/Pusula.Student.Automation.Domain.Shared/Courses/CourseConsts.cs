using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Courses;

public class CourseConsts
{
    private const string DefaultSorting = "{0}CourseName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Course." : string.Empty);
    }

    public const int MaxCourseNameLength = 256;
    public const int MinCourseNameLength = 4;

    public const int MinCredits = 1;
    public const int MaxCredits = 8;
}
