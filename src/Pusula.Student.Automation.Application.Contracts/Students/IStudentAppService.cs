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
    Task<PagedResultDto<StudentWithNavigationPropertiesDto>> GetListWithNavigationAsync(GetStudentsInput input);
    Task<StudentDto> GetAsync(Guid id);
    Task<StudentDto> CreateAsync(StudentCreateDto input);
    Task<StudentDto> UpdateAsync(Guid id, StudentUpdateDto input);
    Task DeleteAsync(Guid id);
}
