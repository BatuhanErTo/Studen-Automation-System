using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Departments;

public class DepartmentDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string DepartmentName { get; set; } = null!;
    public string ConcurrencyStamp { get; set; } = null!;
}
