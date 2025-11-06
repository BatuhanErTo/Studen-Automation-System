using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Courses;

public class GradeComponentConsts
{
    private const string DefaultSorting = "{0}GradeComponentName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "GradeComponent." : string.Empty);
    }

    public const int MaxGradeComponentNameLength = 50;
    public const int MinGradeComponentNameLength = 5;

    public const int MinWeight = 1;
    public const int MaxWeight = 100;

    public const int MinScore = 0;
    public const int MaxScore = 100;
}
