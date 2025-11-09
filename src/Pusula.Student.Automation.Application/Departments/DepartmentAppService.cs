using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Permissions;
using Pusula.Student.Automation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using static Pusula.Student.Automation.Permissions.AutomationPermissions;

namespace Pusula.Student.Automation.Departments;

[Authorize(AutomationPermissions.Departments.Default)]
public class DepartmentAppService(IDepartmentRepository departmentRepository) : AutomationAppService, IDepartmentAppService
{
    public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
    {
        var query = (await departmentRepository.GetQueryableAsync())
            .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), x => x.DepartmentName.Contains(input.Filter));

        var lookupData = query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicList<Department>();
        var totalCount = lookupData.Count;

        return new PagedResultDto<LookupDto<Guid>>(totalCount, ObjectMapper.Map<List<Department>, List<LookupDto<Guid>>>(lookupData));
    }

    public virtual async Task<PagedResultDto<DepartmentDto>> GetListAsync(GetDepartmentsInput input)
    {
        var totalCount = await departmentRepository.GetCountAsync(
            input.FilterText,
            input.DepartmentName);
        var departments = await departmentRepository.GetListAsync(
            input.FilterText,
            input.DepartmentName,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount);
        return new PagedResultDto<DepartmentDto>()
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Department>, List<DepartmentDto>>(departments)
        };
    }
}
