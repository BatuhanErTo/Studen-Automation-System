using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Pusula.Student.Automation.GlobalExceptions;

public interface IStudentAutomationException : IUserFriendlyException, ISingletonDependency
{
}
