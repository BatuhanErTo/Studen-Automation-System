using Microsoft.Extensions.Logging;
using Pusula.Student.Automation.Logging;
using Pusula.Student.Automation.Students;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Student.Automation.Handlers;

public class StudentViewedEventHandler(ILogger<StudentViewedEventHandler> logger) : IDistributedEventHandler<StudentViewedEto>, ITransientDependency
{
    public Task HandleEventAsync(StudentViewedEto eventData)
    {
        using (logger.BeginScope(new Dictionary<string, object>
        {
            [LoggingConsts.SystemLogPropertiesContextGroup] = LoggingConsts.SystemLogGroupContextName
        }))
        {
            logger.LogInformation(
                "HANDLER -> Student {StudentId} viewed at {ViewedAt} by {ViewerId}.",
                eventData.StudentId,
                eventData.ViewedAt,
                eventData.ViewerId
            );
        }
        return Task.CompletedTask;
    }
}
