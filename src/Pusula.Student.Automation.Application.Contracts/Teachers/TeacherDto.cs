using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Teachers;

public class TeacherDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public EnumGender EnumGender { get; set; } = EnumGender.Unknown;
    public string EmailAddress { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public Guid DepartmentId { get; set; }
    public string ConcurrencyStamp { get; set; } = null!;
}
