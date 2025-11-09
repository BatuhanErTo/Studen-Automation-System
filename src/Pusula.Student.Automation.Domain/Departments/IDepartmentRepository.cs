using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Student.Automation.Departments;

public interface IDepartmentRepository : IRepository<Department, Guid>
{
    Task<List<Department>> GetListAsync(
            string? filterText = null,
            string? departmentName = null,
            string? sort = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
            string? filterText = null,
            string? departmentName = null,
            CancellationToken cancellationToken = default);
}
