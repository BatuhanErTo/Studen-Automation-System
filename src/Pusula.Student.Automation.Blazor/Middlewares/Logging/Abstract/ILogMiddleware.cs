using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace Pusula.Student.Automation.Blazor.Middlewares.Logging.Abstract;

public interface ILogMiddleware : IMiddleware, ISingletonDependency
{
}
