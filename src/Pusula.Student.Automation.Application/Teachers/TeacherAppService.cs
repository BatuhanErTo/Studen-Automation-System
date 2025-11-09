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

namespace Pusula.Student.Automation.Teachers;

[Authorize(AutomationPermissions.Teachers.Default)]
public class TeacherAppService(
    ITeacherRepository teacherRepository, 
    TeacherManager teacherManager,
    IdentityUserManager identityUserManager) : AutomationAppService, ITeacherAppService
{
    [Authorize(AutomationPermissions.Teachers.Create)]
    [UnitOfWork]
    public virtual async Task<TeacherDto> CreateAsync(TeacherCreateDto input)
    {
        Guid createdUserId = await CreateIdentityUser(
            input.FirstName,
            input.LastName,
            input.EmailAddress.Trim().ToLowerInvariant(),
            input.EmailAddress.Trim().ToLowerInvariant(),
            input.PhoneNumber,
            input.Password);

        var teacher = await teacherManager.CreateTeacherAsync(
            input.FirstName,
            input.LastName,
            input.EnumGender,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId,
            createdUserId);
        return ObjectMapper.Map<Teacher, TeacherDto>(teacher);
    }

    [Authorize(AutomationPermissions.Teachers.Delete)]
    public virtual async Task DeleteAllAsync(GetTeachersInput input)
    {
        await teacherRepository.DeleteAllAsync(input.FilterText, input.FirstName, input.LastName, input.EnumGender, input.EmailAddress, input.PhoneNumber, input.DepartmentId);
    }

    [Authorize(AutomationPermissions.Teachers.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await teacherRepository.DeleteAsync(id);
    }

    [Authorize(AutomationPermissions.Teachers.Delete)]
    public virtual async Task DeleteByIdsAsync(List<Guid> teacherIds)
    {
        await teacherRepository.DeleteManyAsync(teacherIds);
    }

    public virtual async Task<TeacherDto> GetAsync(Guid id)
    {
        var teacher = await teacherRepository.GetAsync(id);
        return ObjectMapper.Map<Teacher, TeacherDto>(teacher);
    }

    public virtual async Task<PagedResultDto<TeacherDto>> GetPagedListAsync(GetTeachersInput input)
    {
        long totalCount = await teacherRepository.GetCountAsync(
            input.FilterText,
            input.FirstName,
            input.LastName,
            input.EnumGender,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId);

        var items = await teacherRepository.GetListAsync(
           input.FilterText,
           input.FirstName,
           input.LastName,
           input.EnumGender,
           input.EmailAddress,
           input.PhoneNumber,
           input.DepartmentId,
           input.Sorting,
           input.MaxResultCount,
           input.SkipCount);

        return new PagedResultDto<TeacherDto>
        {
            Items = ObjectMapper.Map<List<Teacher>, List<TeacherDto>>(items),
            TotalCount = totalCount
        };
    }

    public virtual async Task<PagedResultDto<TeacherWithNavigationPropertiesDto>> GetListWithNavigationAsync(GetTeachersInput input)
    {
        long totalCount = await teacherRepository.GetCountAsync(
            input.FilterText, 
            input.FirstName, 
            input.LastName,
            input.EnumGender,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId);

        var items = await teacherRepository.GetListWithNavigationPropertiesAsync(
            input.FilterText,
            input.FirstName,
            input.LastName,
            input.EnumGender,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId, 
            input.Sorting, 
            input.MaxResultCount, 
            input.SkipCount);

        return new PagedResultDto<TeacherWithNavigationPropertiesDto>
        {
            Items = ObjectMapper.Map<List<TeacherWithNavigationProperties>,List<TeacherWithNavigationPropertiesDto>>(items),
            TotalCount = totalCount
        };
    }

    public virtual async Task<TeacherWithNavigationPropertiesDto> GetWithNavigationAsync(Guid id)
    {
        var teacherWithNavigationItem = await teacherRepository.GetWithNavigationPropertiesByTeacherIdAsync(id);
        return ObjectMapper.Map<TeacherWithNavigationProperties, TeacherWithNavigationPropertiesDto>(teacherWithNavigationItem);
    }
    [Authorize(AutomationPermissions.Teachers.Edit)]
    public virtual async Task<TeacherDto> UpdateAsync(Guid id, TeacherUpdateDto input)
    {
        var updatedTeacher = await teacherManager.UpdateTeacherAsync(
            id, 
            input.FirstName,
            input.LastName,
            input.EnumGender,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId,
            input.ConcurrencyStamp);
        return ObjectMapper.Map<Teacher, TeacherDto>(updatedTeacher);
    }

    //TODO: bunu ortak bir yardımcı sınıfa al
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

        await identityUserManager.AddToRoleAsync(user, Roles.TeacherRole);

        return user.Id;
    }
    // give better naming to make the purpose more clear
    public virtual async Task<List<TeacherDto>> GetListAsync()
    {
        var items = await teacherRepository.GetListAsync(
            sort: "FirstName asc, LastName asc",
            maxResultCount: int.MaxValue,
            skipCount: 0);

        return ObjectMapper.Map<List<Teacher>, List<TeacherDto>>(items);
    }
}
