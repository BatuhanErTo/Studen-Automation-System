using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Departments;

public class GetDepartmentsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? DepartmentName { get; set; }
}
