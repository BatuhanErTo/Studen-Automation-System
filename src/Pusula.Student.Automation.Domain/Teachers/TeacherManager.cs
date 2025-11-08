using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Student.Automation.Teachers;

public class TeacherManager(ITeacherRepository teacherRepository) : DomainService
{
    public virtual async Task<Teacher> CreateTeacherAsync(
        string firstName, 
        string lastName, 
        EnumGender enumGender, 
        string emailAddress, 
        string phoneNumber,
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        CheckValidate(firstName, lastName, emailAddress, phoneNumber, departmentId);

        var teacher = new Teacher(
                GuidGenerator.Create(),
                firstName,
                lastName,
                enumGender,
                emailAddress,
                phoneNumber, 
                departmentId);

        return await teacherRepository.InsertAsync(teacher, cancellationToken: cancellationToken);
    }

    public virtual async Task<Teacher> UpdateTeacherAsync(
        Guid id,
        string firstName,
        string lastName,
        EnumGender enumGender,
        string emailAddress,
        string phoneNumber,
        Guid departmentId,
        string? concurenyTimeStamp = null,
        CancellationToken cancellationToken = default)
    {
        CheckValidate(firstName, lastName, emailAddress, phoneNumber, departmentId);
        
        var teacher = await teacherRepository.GetAsync(id, cancellationToken: cancellationToken);

        teacher.SetFirstName(firstName);
        teacher.SetLastName(lastName);
        teacher.SetGender(enumGender);
        teacher.SetEmailAddress(emailAddress);
        teacher.SetPhoneNumber(phoneNumber);
        teacher.SetDepartmentId(departmentId);

        teacher.SetConcurrencyStampIfNotNull(concurenyTimeStamp);

        return await teacherRepository.UpdateAsync(teacher, cancellationToken: cancellationToken);
    }

    private static void CheckValidate(
        string firstName,
        string lastName,
        string emailAddress,
        string phoneNumber,
        Guid departmentId)
    {
        Check.NotNull(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), TeacherConsts.MaxFirstNameLength, TeacherConsts.MinFirstNameLength);
        Check.NotNull(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), TeacherConsts.MaxLastNameLength, TeacherConsts.MinLastNameLength);
        Check.NotNull(emailAddress, nameof(emailAddress));
        Check.Length(emailAddress, nameof(emailAddress), TeacherConsts.MaxEmailAddressLength, TeacherConsts.MinEmailAddressLength);
        Check.NotNull(phoneNumber, nameof(phoneNumber));
        Check.Length(phoneNumber, nameof(phoneNumber), TeacherConsts.MaxPhoneNumberLength, TeacherConsts.MinPhoneNumberLength);
        Check.NotNull(departmentId, nameof(departmentId));
    }
}
