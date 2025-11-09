using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Pusula.Student.Automation.Teachers;

public class TeacherCreateDto
{
    [Required]
    [StringLength(TeacherConsts.MaxFirstNameLength, MinimumLength = TeacherConsts.MinFirstNameLength)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(TeacherConsts.MaxLastNameLength, MinimumLength = TeacherConsts.MinLastNameLength)]
    public string LastName { get; set; } = null!;
    [Required]
    public EnumGender EnumGender { get; set; } = EnumGender.Unknown;
    [Required]
    [StringLength(TeacherConsts.MaxEmailAddressLength, MinimumLength = TeacherConsts.MinEmailAddressLength)]
    [EmailAddress]
    public string EmailAddress { get; set; } = null!;
    [Required]
    [StringLength(TeacherConsts.MaxPhoneNumberLength, MinimumLength = TeacherConsts.MinPhoneNumberLength)]
    public string PhoneNumber { get; set; } = null!;
    [DisableAuditing]
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public Guid DepartmentId { get; set; }
}
