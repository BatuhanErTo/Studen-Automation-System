using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Helper.Identity;

public interface IIdentityUserHelper
{
    Task<Guid> CreateIdentityUser(
        string firstName,
        string lastName,
        string userName,
        string email,
        string phoneNumber,
        string password,
        string role,
        Guid? currentTenantId = null);
}
