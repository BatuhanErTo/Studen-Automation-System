using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Pusula.Student.Automation.Workers;

public class PeriodicSetCourseStatusWorker : AsyncPeriodicBackgroundWorkerBase
{
    public PeriodicSetCourseStatusWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory)
        : base(timer, serviceScopeFactory)
    {
        Timer.Period = (int)TimeSpan.FromHours(12).TotalMilliseconds;
    }

    [UnitOfWork]
    protected async override Task DoWorkAsync(
        PeriodicBackgroundWorkerContext workerContext)
    {
        using (Logger.BeginScope(new Dictionary<string, object>
        {
            [LoggingConsts.SystemLogPropertiesContextGroup] = LoggingConsts.SystemLogGroupContextName
        }))
        {
            Logger.LogInformation("Starting: PeriodicPatientViewerWorker...");
        }    

        var courseRepository = workerContext.ServiceProvider.GetRequiredService<ICourseRepository>();

        var overDueCourses = await courseRepository.GetCourseListExceedsEndDateAndStatusIsNotCompletedAsync();

        if (overDueCourses != null)
        {
            var backgroundJobManager = workerContext.ServiceProvider.GetRequiredService<IBackgroundJobManager>();
            overDueCourses.ForEach(async course =>
            {
                course.SetStatus(EnumCourseStatus.Completed);
                await courseRepository.UpdateAsync(course);
                await backgroundJobManager.EnqueueAsync(new SetCourseStatusLogArgs { CourseId = course.Id, CourseName = course.CourseName, NewEnumCourseStatus = course.Status });
            });            
        }

        using (Logger.BeginScope(new Dictionary<string, object>
        {
            [LoggingConsts.SystemLogPropertiesContextGroup] = LoggingConsts.SystemLogGroupContextName
        }))
        {
            Logger.LogInformation("Completed: PeriodicPatientViewerWorker...");
        }
    }
}
