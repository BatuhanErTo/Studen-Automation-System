using JetBrains.Annotations;
using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Student.Automation.Teachers;

public class Teacher : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public string FirstName { get; private set; }
    [NotNull]
    public string LastName { get; private set; }
    public EnumGender Gender { get; private set; }
    [NotNull]
    public string EmailAddress { get; private set; }
    [NotNull]
    public string PhoneNumber { get; private set; }
    public Guid DepartmentId { get; private set; }

    protected Teacher(){}

    public Teacher(
        Guid id,
        string firstName,
        string lastName,
        EnumGender gender,
        string emailAddress,
        string phoneNumber,
        Guid departmentId) : base(id)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
        SetGender(gender);
        SetEmailAddress(emailAddress);
        SetPhoneNumber(phoneNumber);
        SetDepartmentId(departmentId);
    }

    public void SetFirstName(string firstName)
    {
        Check.NotNull(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), TeacherConsts.MaxFirstNameLength, TeacherConsts.MinFirstNameLength);
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        Check.NotNull(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), TeacherConsts.MaxLastNameLength, TeacherConsts.MinLastNameLength);
        LastName = lastName;
    }

    public void SetGender(EnumGender gender) => Gender = gender;

    public void SetEmailAddress(string emailAddress)
    {
        Check.NotNull(emailAddress, nameof(emailAddress));
        Check.Length(emailAddress, nameof(emailAddress), TeacherConsts.MaxEmailAddressLength, TeacherConsts.MinEmailAddressLength);
        EmailAddress = emailAddress;
    }

    public void SetPhoneNumber(string phoneNumber)
    {
        Check.NotNull(phoneNumber, nameof(phoneNumber));
        Check.Length(phoneNumber, nameof(phoneNumber), TeacherConsts.MaxPhoneNumberLength, TeacherConsts.MinPhoneNumberLength);
        PhoneNumber = phoneNumber;
    }

    public void SetDepartmentId(Guid departmentId)
    {
        if (departmentId == Guid.Empty) throw new BusinessException("Teacher.DepartmentIdInvalid");

        DepartmentId = departmentId;
    }

}

