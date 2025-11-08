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
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public EnumGender Gender { get; private set; } = EnumGender.Unknown;
    public string EmailAddress { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public Guid DepartmentId { get; private set; }
    public string ConcurrencyStamp { get; set; } = null!;
}
