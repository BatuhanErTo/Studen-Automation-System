using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Logging;

public class LoggingConsts
{
    public const string SystemLogIndexFormatDefinitionName = "Pusula-Student-Automation-log-";
    public const string SystemLogIndexFormatGroupingByYearAndMonth = "{0:yyyy.MM}";
    public const string SystemLogGroupContextName = "PusulaStudentAutomation";

    public const string SystemLogIndexFormat =
       SystemLogIndexFormatDefinitionName + SystemLogIndexFormatGroupingByYearAndMonth;

    public const string SystemLogPropertiesContextRequestBody = "RequestBody";
    public const string SystemLogPropertiesContextLoginUser = "LoginUser";
    public const string SystemLogPropertiesContextLoginUserId = "LoginUserId";
    public const string SystemLogPropertiesContextMethod = "Method";
    public const string SystemLogPropertiesContextPath = "Path";
    public const string SystemLogPropertiesContextQueryString = "QueryString";
    public const string SystemLogPropertiesContextGroup = "Group";
    public const string SystemLogPropertiesContextResponseBody = "ResponseBody";
    public const string SystemLogPropertiesContextStatusCode = "StatusCode";
    public const string SystemLogPropertiesContextElapsedMilliseconds = "ElapsedMilliseconds";
}
