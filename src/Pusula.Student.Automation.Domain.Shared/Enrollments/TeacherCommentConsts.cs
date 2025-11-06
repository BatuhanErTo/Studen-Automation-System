using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Enrollments;

public class TeacherCommentConsts
{
    private const string DefaultSorting = "{0}Comment asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "TeacherComment." : string.Empty);
    }

    public const int MaxCommentLength = 1000;
    public const int MinCommentLength = 10;
}
