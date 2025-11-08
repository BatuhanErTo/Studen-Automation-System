using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Pusula.Student.Automation.GlobalExceptions;

public class StudentAutomationException : IStudentAutomationException
{
    private const string DEFAULT_ERROR_MESSAGE = "An unexpected error occurred.";
    private const string DEFAULT_ERROR_CODE = "SA-10000";


    public static void Throw(string? message) => ThrowIf(message);
    public static void ThrowIf(string? message, bool condition = true) => ThrowIf(message, DEFAULT_ERROR_CODE, condition);
    public static void ThrowIf(string? message, string? code, bool condition = true)
    {
        if (condition)
            ThrowException(message, code);
    }

    private static void ThrowException(string? message, string? code) => throw new UserFriendlyException(message ?? DEFAULT_ERROR_MESSAGE, code ?? DEFAULT_ERROR_CODE);
}
