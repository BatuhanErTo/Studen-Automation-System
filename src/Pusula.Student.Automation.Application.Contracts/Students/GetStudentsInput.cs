using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Students;

public class GetStudentsInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? IdentityNumber { get; set; }
    public DateTime? BirthDate { get; set; }
    public EnumGradeYear? GradeYear { get; set; }
    public EnumGender? Gender { get; set; }
    public string? Address { get; set; }
    public string? EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public Guid? DepartmentId { get; set; }
}