using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Students;

public class StudentDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string IdentityNumber { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public EnumGradeYear GradeYear { get; set; } = EnumGradeYear.FirstYear;
    public EnumGender Gender { get; set; } = EnumGender.Unknown;
    public string Address { get; set; } = null!;
    public string EmailAddress { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public Guid DepartmentId { get; set; }
    public string ConcurrencyStamp { get; set; } = null!;
}
