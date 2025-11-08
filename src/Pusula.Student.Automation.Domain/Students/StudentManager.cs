using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;

namespace Pusula.Student.Automation.Students;

public class StudentManager(IStudentRepository studentRepository) : DomainService
{
    public virtual async Task<StudentEntity> CreateStudentAsync(
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        EnumGradeYear gradeYear,
        EnumGender enumGender,
        string address,
        string emailAddress,
        string phoneNumber,
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        CheckValidate(firstName, lastName, identityNumber, birthDate, address, emailAddress, phoneNumber, departmentId);

        var student = new StudentEntity(
                GuidGenerator.Create(),
                firstName, 
                lastName, 
                identityNumber, 
                birthDate, 
                gradeYear, 
                enumGender, 
                address, 
                emailAddress, 
                phoneNumber, 
                departmentId);

        return await studentRepository.InsertAsync(student, cancellationToken: cancellationToken);
    }

    public virtual async Task<StudentEntity> UpdateStudentAsync(
        Guid id,
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        EnumGradeYear gradeYear,
        EnumGender enumGender,
        string address,
        string emailAddress,
        string phoneNumber,
        Guid departmentId,
        string? concurenyTimeStamp = null,
        CancellationToken cancellationToken = default)
    {
        CheckValidate(firstName, lastName, identityNumber, birthDate, address, emailAddress, phoneNumber, departmentId);

        var student = await studentRepository.GetAsync(id, cancellationToken: cancellationToken);

        student.SetFirstName(firstName);
        student.SetLastName(lastName);
        student.SetIdentityNumber(identityNumber);
        student.SetBirthDate(birthDate);
        student.SetGradeYear(gradeYear);
        student.SetGender(enumGender);
        student.SetAddress(address);
        student.SetEmailAddress(emailAddress);
        student.SetPhoneNumber(phoneNumber);
        student.SetDepartmentId(departmentId);

        student.SetConcurrencyStampIfNotNull(concurenyTimeStamp);

        return await studentRepository.UpdateAsync(student, cancellationToken: cancellationToken);
    }

    private static void CheckValidate(
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        string address,
        string emailAddress,
        string phoneNumber,
        Guid departmentId)
    {
        Check.NotNull(firstName, nameof(firstName));
        Check.Length(firstName, nameof(firstName), StudentConsts.MaxFirstNameLength, StudentConsts.MinFirstNameLength);
        Check.NotNull(lastName, nameof(lastName));
        Check.Length(lastName, nameof(lastName), StudentConsts.MaxLastNameLength, StudentConsts.MinLastNameLength);
        Check.NotNull(identityNumber, nameof(identityNumber));
        Check.Length(identityNumber, nameof(identityNumber), StudentConsts.IdentityNumberMaxLength, 0);
        Check.Range(birthDate.Year, nameof(birthDate), 1900, DateTime.UtcNow.Year);
        Check.NotNull(address, nameof(address));
        Check.Length(address, nameof(address), StudentConsts.MaxAddressLength, StudentConsts.MinAddressLength);
        Check.NotNull(emailAddress, nameof(emailAddress));
        Check.Length(emailAddress, nameof(emailAddress), StudentConsts.MaxEmailAddressLength, StudentConsts.MinEmailAddressLength);
        Check.NotNull(phoneNumber, nameof(phoneNumber));
        Check.Length(phoneNumber, nameof(phoneNumber), StudentConsts.MaxPhoneNumberLength, StudentConsts.MinPhoneNumberLength);
        Check.NotNull(departmentId, nameof(departmentId));
    }
}
