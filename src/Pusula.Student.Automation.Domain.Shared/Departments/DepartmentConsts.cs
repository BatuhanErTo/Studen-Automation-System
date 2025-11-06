using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Departments;

public class DepartmentConsts
{
    private const string DefaultSorting = "{0}DepartmentName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Department." : string.Empty);
    }

    public const int MaxDepartmentNameLength = 256;
    public const int MinDepartmentNameLength = 2;
}
