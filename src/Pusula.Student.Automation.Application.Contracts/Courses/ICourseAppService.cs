using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Student.Automation.Courses;

public interface ICourseAppService : IApplicationService
{
    Task<PagedResultDto<CourseDto>> GetListAsync(GetCoursesInput input);
    Task<CourseDto> GetAsync(Guid id);
    Task<CourseDto> CreateAsync(CourseCreateDto input);
    Task<CourseDto> UpdateAsync(Guid id, CourseUpdateDto input);
    Task DeletAsync(Guid id);

    #region GradeComponent service methods
    Task<GradeComponentDto> AddGradeComponentAsync(
        GradeComponentCreateDto input,
        CancellationToken cancellationToken = default);

    Task<GradeComponentDto> UpdateGradeComponentAsync(
        GradeComponentUpdateDto input,
        Guid gradeComponentId,
        CancellationToken cancellationToken = default);

    Task RemoveGradeComponentAsync(
        Guid courseId,
        Guid gradeComponentId,
        CancellationToken cancellationToken = default);
    #endregion

    #region CourseSessions service methods
    Task<CourseSessionDto> AddCourseSessionAsync(
        CourseSessionCreateDto input,
        CancellationToken cancellationToken = default);

    Task<CourseSessionDto> UpdateCourseSessionAsync(
        Guid courseSessionId,
        CourseSessionUpdateDto input,
        CancellationToken cancellationToken = default);

    Task RemoveCourseSessionAsync(
        Guid courseId,
        Guid courseSessionId,
        CancellationToken cancellationToken = default);
    #endregion
}

