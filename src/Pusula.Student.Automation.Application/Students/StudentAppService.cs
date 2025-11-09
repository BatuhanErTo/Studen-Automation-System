using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Pusula.Student.Automation.Students;

[Authorize(AutomationPermissions.Students.Default)]
public class StudentAppService(
    IStudentRepository studentRepository,
    StudentManager studentManager,
    IdentityUserManager identityUserManager) : AutomationAppService, IStudentAppService
{
    [Authorize(AutomationPermissions.Students.Create)]
    [UnitOfWork]
    public virtual async Task<StudentDto> CreateAsync(StudentCreateDto input)
    {
        Guid createdUserId = await CreateIdentityUser(
            input.FirstName,
            input.LastName,
            input.EmailAddress.Trim().ToLowerInvariant(),
            input.EmailAddress.Trim().ToLowerInvariant(),
            input.PhoneNumber,
            input.Password);

        var student = await studentManager.CreateStudentAsync(
            input.FirstName,
            input.LastName,
            input.IdentityNumber,
            input.BirthDate,
            input.GradeYear,
            input.Gender,
            input.Address,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId,
            createdUserId);
        return ObjectMapper.Map<StudentEntity, StudentDto>(student);

    }
    [Authorize(AutomationPermissions.Students.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await studentRepository.DeleteAsync(id);
    }

    public async Task<StudentDto> GetAsync(Guid id)
    {
        var student = await studentRepository.GetAsync(id);
        return ObjectMapper.Map<StudentEntity, StudentDto>(student);
    }

    [Authorize(AutomationPermissions.Enrollments.Create)]
    public virtual async Task<PagedResultDto<StudentDto>> GetAvailableStudentListAsync(Guid courseId, string? filterText = null, int skip = 0, int take = 20)
    {
        var totalCount = await studentRepository.GetAvailableForCourseCountAsync(courseId, filterText);
        var items = await studentRepository.GetAvailableForCourseListAsync(
            courseId,
            filterText,
            StudentConsts.GetDefaultSorting(false),
            take,
            skip);

        return new PagedResultDto<StudentDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<StudentEntity>, List<StudentDto>>(items)
        };
    }

    public virtual async Task<PagedResultDto<StudentWithNavigationPropertiesDto>> GetListWithNavigationAsync(GetStudentsInput input)
    {
        long totalCount = await studentRepository.GetCountAsync(
            input.FilterText,
            input.FirstName,
            input.LastName,
            input.IdentityNumber,
            input.BirthDate,
            input.GradeYear,
            input.Gender,
            input.Address,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId);

        var items = await studentRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText,
            input.FirstName,
            input.LastName,
            input.IdentityNumber,
            input.BirthDate,
            input.GradeYear,
            input.Gender,
            input.Address,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId);

        return new PagedResultDto<StudentWithNavigationPropertiesDto>
        {
            Items = ObjectMapper.Map<List<StudentWithNavigationProperties>, List<StudentWithNavigationPropertiesDto>>(items),
            TotalCount = totalCount
        };
    }

    public virtual async Task<StudentWithNavigationPropertiesDto> GetWithNavigationAsync(Guid id)
    {
        var studentWithNavigationProperty = await studentRepository.GetWithNavigationPropertiesByStudentIdAsync(id);

        return ObjectMapper.Map<StudentWithNavigationProperties, StudentWithNavigationPropertiesDto>(studentWithNavigationProperty);
    }

    [Authorize(AutomationPermissions.Students.Edit)]
    public virtual async Task<StudentDto> UpdateAsync(Guid id, StudentUpdateDto input)
    {
        var updatedStudent = await studentManager.UpdateStudentAsync(
            id,
            input.FirstName,
            input.LastName,
            input.IdentityNumber,
            input.BirthDate,
            input.GradeYear,
            input.Gender,
            input.Address,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId,
            input.ConcurrencyStamp);
        return ObjectMapper.Map<StudentEntity, StudentDto>(updatedStudent);
    }

    private async Task<Guid> CreateIdentityUser(
        string firstName,
        string lastName,
        string userName,
        string email,
        string phoneNumber,
        string password)
    {
        if (await identityUserManager.FindByEmailAsync(email) is not null)
            throw new UserFriendlyException(L["EmailAlreadyTaken"]);

        if (await identityUserManager.FindByNameAsync(userName) is not null)
            throw new UserFriendlyException(L["UserNameAlreadyTaken"]);

        var currentTenantId = CurrentTenant.Id;
        var user = new IdentityUser(GuidGenerator.Create(), userName, email, currentTenantId)
        {
            Name = firstName,
            Surname = lastName
        };
        user.SetPhoneNumber(phoneNumber, true);
        user.SetEmailConfirmed(true);

        var createResult = await identityUserManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new BusinessException("IdentityUserCreationFailed")
                .WithData("Errors", string.Join(",", createResult.Errors.Select(e => e.Description)));
        }

        await identityUserManager.AddToRoleAsync(user, Roles.StudentRole);

        return user.Id;
    }
}
