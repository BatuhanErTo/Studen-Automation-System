using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Teachers;

public class TeacherAppService(ITeacherRepository teacherRepository, TeacherManager teacherManager) : AutomationAppService, ITeacherAppService
{
    public virtual async Task<TeacherDto> CreateAsync(TeacherCreateDto input)
    {
        var teacher = await teacherManager.CreateTeacherAsync(
            input.FirstName,
            input.LastName,
            input.EnumGender,
            input.EmailAddress,
            input.PhoneNumber,
            input.DepartmentId);
        return ObjectMapper.Map<Teacher, TeacherDto>(teacher);
    }

    public virtual async Task DeletAsync(Guid id)
    {
        await teacherRepository.DeleteAsync(id);
    }

    public virtual async Task<TeacherDto> GetAsync(Guid id)
    {
        var teacher = await teacherRepository.GetAsync(id);
        return ObjectMapper.Map<Teacher, TeacherDto>(teacher);
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
}
