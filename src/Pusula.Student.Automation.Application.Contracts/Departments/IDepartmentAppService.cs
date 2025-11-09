using Pusula.Student.Automation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Student.Automation.Departments;

public interface IDepartmentAppService : IApplicationService
{
    Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input);
    Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);
}
