using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Teachers;

public class TeacherConsts
{
    private const string DefaultSorting = "{0}FirstName asc";

    public static string GetDefaultSorting(bool withEntityName)
    {
        return string.Format(DefaultSorting, withEntityName ? "Teacher." : string.Empty);
    }

    public const int MaxFirstNameLength = 256;
    public const int MinFirstNameLength = 2;
    public const int MaxLastNameLength = 32;
    public const int MinLastNameLength = 2;


    public const int MaxAddressLength = 512;
    public const int MinAddressLength = 10;

    public const int MaxEmailAddressLength = 128;
    public const int MinEmailAddressLength = 12;

    public const int MaxPhoneNumberLength = 32;
    public const int MinPhoneNumberLength = 3;
}
