using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Helper.Identity;
using Pusula.Student.Automation.Permissions;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Pusula.Student.Automation.Students;

[Authorize(AutomationPermissions.Students.Default)]
public class StudentAppService(
    IStudentRepository studentRepository,
    StudentManager studentManager,
    IIdentityUserHelper identityUserHelper) : AutomationAppService, IStudentAppService
{
    [Authorize(AutomationPermissions.Students.Create)]
    [UnitOfWork]
    public virtual async Task<StudentDto> CreateAsync(StudentCreateDto input)
    {
        Guid createdUserId = await identityUserHelper.CreateIdentityUser(
            input.FirstName,
            input.LastName,
            input.EmailAddress.Trim().ToLowerInvariant(),
            input.EmailAddress.Trim().ToLowerInvariant(),
            input.PhoneNumber,
            input.Password,
            Roles.StudentRole,
            currentTenantId: CurrentTenant.Id);

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

    public virtual async Task<List<StudentDto>> GetListAsync()
    {
        var items = await studentRepository.GetListAsync(
            sort: "FirstName asc, LastName asc",
            maxResultCount: int.MaxValue,
            skipCount: 0);

        return ObjectMapper.Map<List<StudentEntity>, List<StudentDto>>(items);
    }

    public virtual async Task<List<StudentWithNavigationPropertiesDto>> GetListWithNavigationAsync()
    {
        var items = await studentRepository.GetListWithNavigationPropertiesAsync();
        return ObjectMapper.Map<List<StudentWithNavigationProperties>, List<StudentWithNavigationPropertiesDto>>(items);
    }

    public virtual async Task<PagedResultDto<StudentWithNavigationPropertiesDto>> GetPagedListWithNavigationAsync(GetStudentsInput input)
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
}
