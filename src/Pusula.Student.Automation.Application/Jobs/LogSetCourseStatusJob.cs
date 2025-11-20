using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace Pusula.Student.Automation.Jobs;

public class LogSetCourseStatusJob : AsyncBackgroundJob<SetCourseStatusLogArgs>, ITransientDependency
{
    public override Task ExecuteAsync(SetCourseStatusLogArgs args)
    {

        using (Logger.BeginScope(new Dictionary<string, object>
        {
            [LoggingConsts.SystemLogPropertiesContextGroup] = LoggingConsts.SystemLogGroupContextName
        }))
        {
            Logger.LogInformation(
                "BACKGROUND JOB -> Course's Status with ID {CourseId} with name {CourseName} set to {NewEnumCourseStatus}.",
                args.CourseId,
                args.CourseName,
                args.NewEnumCourseStatus
            );
        }
        return Task.CompletedTask;
    }
}
