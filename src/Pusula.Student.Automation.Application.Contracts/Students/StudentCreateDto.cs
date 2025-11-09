using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;

namespace Pusula.Student.Automation.Students;

public class StudentCreateDto
{
    [Required]
    [StringLength(StudentConsts.MaxFirstNameLength, MinimumLength = StudentConsts.MinFirstNameLength)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(StudentConsts.MaxLastNameLength, MinimumLength = StudentConsts.MinLastNameLength)]
    public string LastName { get; set; } = null!;
    [Required]
    [StringLength(StudentConsts.IdentityNumberMaxLength)]
    public string IdentityNumber { get; set; } = null!;
    public DateTime BirthDate { get;  set; }
    public EnumGradeYear GradeYear { get;  set; }
    public EnumGender Gender { get;  set; }
    [Required]
    [StringLength(StudentConsts.MaxAddressLength, MinimumLength = StudentConsts.MinAddressLength)]
    public string Address { get;  set; } = null!;
    [Required]
    [StringLength(StudentConsts.MaxEmailAddressLength, MinimumLength = StudentConsts.MinEmailAddressLength)]
    [EmailAddress]
    public string EmailAddress { get;  set; } = null!;
    [Required]
    [StringLength(StudentConsts.MaxPhoneNumberLength, MinimumLength = StudentConsts.MinPhoneNumberLength)]
    public string PhoneNumber { get;  set; } = null!;
    [Required]
    [DisableAuditing]
    public string Password { get;  set; } = null!;
    public Guid DepartmentId { get;  set; }
}
