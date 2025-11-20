using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Student.Automation.Students;

public interface IStudentAppService : IApplicationService
{
    Task<StudentWithNavigationPropertiesDto> GetWithNavigationAsync(Guid id);
    Task<PagedResultDto<StudentWithNavigationPropertiesDto>> GetPagedListWithNavigationAsync(GetStudentsInput input);
    Task<List<StudentDto>> GetListAsync();
    Task<List<StudentWithNavigationPropertiesDto>> GetListWithNavigationAsync();
    Task<StudentDto> GetAsync(Guid id);
    Task<StudentDto> CreateAsync(StudentCreateDto input);
    Task<StudentDto> UpdateAsync(Guid id, StudentUpdateDto input);
    Task DeleteAsync(Guid id);

    // Returns students eligible to enroll to the given course (paged)
    Task<PagedResultDto<StudentDto>> GetAvailableStudentListAsync(
        Guid courseId,
        string? filterText = null,
        int skip = 0,
        int take = 20);
}
