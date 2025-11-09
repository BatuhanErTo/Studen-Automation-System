using Microsoft.Extensions.DependencyInjection;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Localization;
using Pusula.Student.Automation.MultiTenancy;
using Pusula.Student.Automation.Permissions;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;

namespace Pusula.Student.Automation.Blazor.Menus;

public class AutomationMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<AutomationResource>();
        
        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                AutomationMenus.Home,
                l["Menu:Home"],
                "/",
                icon: "fas fa-home",
                order: 1
            )
        );

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;
    
        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 3);

        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

        if (currentUser.IsInRole(Roles.AdminRole))
        {
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    AutomationMenus.Teachers,
                    l["Menu:Teachers"],
                    url: "/teachers",
                    icon: "fa fa-file-alt")
            );

            context.Menu.AddItem(
                new ApplicationMenuItem(
                    AutomationMenus.Courses,
                    l["Menu:Courses"],
                    url: "/courses",
                    icon: "fa fa-book")
            );
        }

        if (currentUser.IsInRole(Roles.TeacherRole))
        {
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    AutomationMenus.MyCourses,
                    l["Menu:MyCourses"],
                    url: "/my-courses",
                    icon: "fa fa-list")
            );
        }

        if (currentUser.IsInRole(Roles.AdminRole) || currentUser.IsInRole(Roles.TeacherRole))
        {
            context.Menu.AddItem(
                new ApplicationMenuItem(
                    AutomationMenus.Students,
                    l["Menu:Students"],
                    url: "/students",
                    icon: "fa fa-user-graduate")
            );
        }

        return Task.CompletedTask;
    }
}
