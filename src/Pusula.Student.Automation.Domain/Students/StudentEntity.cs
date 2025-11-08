using JetBrains.Annotations;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enums;
using Scriban.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Student.Automation.Students;

// The name "StudentEntity" is used instead of "Student" to avoid conflicts with namespacing issue because of the solution's name.
public sealed class StudentEntity : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public string FirstName { get; private set; }
    [NotNull]
    public string LastName { get; private set; }
    [NotNull]
    public string IdentityNumber { get; private set; }
    public DateTime BirthDate { get; private set; }
    public EnumGradeYear GradeYear { get; private set; }
    public EnumGender Gender { get; private set; }
    [NotNull]
    public string Address { get; private set; }
    [NotNull]
    public string EmailAddress { get; private set; }
    [NotNull]
    public string PhoneNumber { get; private set; }
    public Guid DepartmentId { get; private set; }

    protected StudentEntity(){}

    public StudentEntity(
        Guid id,
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        EnumGradeYear gradeYear,
        EnumGender gender,
        string address,
        string emailAddress,
        string phoneNumber,
        Guid departmentId) : base(id)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetIdentityNumber(identityNumber);
        SetBirthDate(birthDate);
        SetGradeYear(gradeYear);
        SetGender(gender);
        SetAddress(address);
        SetEmailAddress(emailAddress);
        SetPhoneNumber(phoneNumber);
        SetDepartmentId(departmentId);
    }

    public void SetFirstName(string firstName)
    {
        Check.NotNull(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), StudentConsts.MaxFirstNameLength, StudentConsts.MinFirstNameLength);
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        Check.NotNull(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), StudentConsts.MaxLastNameLength, StudentConsts.MinLastNameLength);
        LastName = lastName;
    }

    public void SetIdentityNumber(string identityNumber)
    {
        Check.NotNull(identityNumber, nameof(identityNumber));
        Check.Length(identityNumber, nameof(identityNumber), StudentConsts.IdentityNumberMaxLength, 0);
        IdentityNumber = identityNumber;
    }

    public void SetBirthDate(DateTime birthDate)
    {
        Check.Range(birthDate.Year, nameof(birthDate), 1900, DateTime.UtcNow.Year);
        BirthDate = birthDate;
    }

    public void SetGradeYear(EnumGradeYear gradeYear) => GradeYear = gradeYear;

    public void SetGender(EnumGender gender) => Gender = gender;

    public void SetAddress(string address)
    {
        Check.NotNull(address, nameof(address));
        Check.Length(address, nameof(address), StudentConsts.MaxAddressLength, StudentConsts.MinAddressLength);
        Address = address;
    }

    public void SetEmailAddress(string emailAddress)
    {
        Check.NotNull(emailAddress, nameof(emailAddress));
        Check.Length(emailAddress, nameof(emailAddress), StudentConsts.MaxEmailAddressLength, StudentConsts.MinEmailAddressLength);
        EmailAddress = emailAddress;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        Check.NotNull(phoneNumber, nameof(phoneNumber));
        Check.Length(phoneNumber, nameof(phoneNumber), StudentConsts.MaxPhoneNumberLength, StudentConsts.MinPhoneNumberLength);
        PhoneNumber = phoneNumber;
    }

    public void SetDepartmentId(Guid departmentId)
    {
        if (departmentId == Guid.Empty) throw new BusinessException("Student.DepartmentIdInvalid");

        DepartmentId = departmentId;
    }
}