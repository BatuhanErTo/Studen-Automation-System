using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Teachers;

public class GetTeachersInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public EnumGender? EnumGender { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
}
