using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Student.Automation.Teachers;

public interface ITeacherAppService : IApplicationService
{
    Task<TeacherWithNavigationPropertiesDto> GetWithNavigationAsync(Guid id);
    Task<PagedResultDto<TeacherWithNavigationPropertiesDto>> GetListWithNavigationAsync(GetTeachersInput input);
    Task<PagedResultDto<TeacherDto>> GetPagedListAsync(GetTeachersInput input);
    Task<List<TeacherDto>> GetListAsync();
    Task<TeacherDto> GetAsync(Guid id);
    Task<TeacherDto> CreateAsync(TeacherCreateDto input);
    Task<TeacherDto> UpdateAsync(Guid id, TeacherUpdateDto input);
    Task DeleteAsync(Guid id);
    Task DeleteByIdsAsync(List<Guid> teacherIds);
    Task DeleteAllAsync(GetTeachersInput input);
}