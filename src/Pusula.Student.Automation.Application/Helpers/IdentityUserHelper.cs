using Microsoft.Extensions.Localization;
using Pusula.Student.Automation.Helper.Identity;
using Pusula.Student.Automation.Localization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace Pusula.Student.Automation.Helpers;

public class IdentityUserHelper(
        IdentityUserManager identityUserManager,
        IGuidGenerator guidGenerator,
        IStringLocalizer<AutomationResource> localizer) 
    : IIdentityUserHelper, IScopedDependency
{
    public virtual async Task<Guid> CreateIdentityUser(
        string firstName, 
        string lastName, 
        string userName, 
        string email, 
        string phoneNumber, 
        string password, 
        string role,
        Guid? currentTenantId = null)
    {

        if (await identityUserManager.FindByEmailAsync(email) is not null)
            throw new UserFriendlyException(localizer["EmailAlreadyTaken"]);

        if (await identityUserManager.FindByNameAsync(userName) is not null)
            throw new UserFriendlyException(localizer["UserNameAlreadyTaken"]);

        var user = new IdentityUser(guidGenerator.Create(), userName, email, currentTenantId)
        {
            Name = firstName,
            Surname = lastName
        };
        user.SetPhoneNumber(phoneNumber, true);
        user.SetEmailConfirmed(true);

        var createResult = await identityUserManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new BusinessException("IdentityUserCreationFailed")
                .WithData("Errors", string.Join(",", createResult.Errors.Select(e => e.Description)));
        }

        await identityUserManager.AddToRoleAsync(user, role);

        return user.Id;
    }
}
