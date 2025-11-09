using Pusula.Student.Automation.Localization;
using Volo.Abp.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Blazor;

public abstract class AutomationComponentBase : AbpComponentBase
{
    protected AutomationComponentBase()
    {
        LocalizationResource = typeof(AutomationResource);
    }

      protected async Task ExecuteSafeAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }
}
