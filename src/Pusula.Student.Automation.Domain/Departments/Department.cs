using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Student.Automation.Departments;

public sealed class Department : FullAuditedAggregateRoot<Guid>
{
    public string DepartmentName { get; private set; }

    protected Department() { }
    public Department(
        Guid id,
        string departmentName) : base(id)
    {
        SetDepartmentName(departmentName);
    }
    public void SetDepartmentName(string departmentName)
    {
        Check.NotNull(departmentName, nameof(departmentName));
        Check.Length(departmentName, nameof(departmentName), DepartmentConsts.MaxDepartmentNameLength, DepartmentConsts.MinDepartmentNameLength);
        DepartmentName = departmentName;
    }
}
