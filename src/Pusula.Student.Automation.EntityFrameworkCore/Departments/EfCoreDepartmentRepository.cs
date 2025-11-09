using Microsoft.EntityFrameworkCore;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.EntityFrameworkCore;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Student.Automation.Departments;

public class EfCoreDepartmentRepository(IDbContextProvider<AutomationDbContext> dbContextProvider) : EfCoreRepository<AutomationDbContext, Department, Guid>(dbContextProvider), IDepartmentRepository
{
    public virtual async Task<long> GetCountAsync(string? filterText = null, string? departmentName = null, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, filterText, departmentName);
        return await query.LongCountAsync();
    }

    public virtual async Task<List<Department>> GetListAsync(string? filterText = null, string? departmentName = null, string? sort = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, filterText,departmentName);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    protected virtual IQueryable<Department> ApplyFilter(
          IQueryable<Department> query,
          string? filterText = null,
          string? departmentName = null) => query
                  .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.DepartmentName.Contains(filterText!))
                  .WhereIf(!string.IsNullOrWhiteSpace(departmentName), e => e.DepartmentName.Contains(departmentName!));
}
