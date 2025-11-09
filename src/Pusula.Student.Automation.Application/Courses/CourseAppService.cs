using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Permissions;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.ValueObjects;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Courses;

[Authorize(AutomationPermissions.Courses.Default)]
public class CourseAppService(ICourseRepository courseRepository, CourseManager courseManager) : AutomationAppService, ICourseAppService
{
    [Authorize(AutomationPermissions.Courses.Create)]
    public virtual async Task<CourseDto> CreateAsync(CourseCreateDto input)
    {
        var course = await courseManager.CreateCourseAsync(
                input.CourseName,
                input.Credits,
                input.StartFrom,
                input.EndTo,
                input.TeacherId,
                input.Status);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    [Authorize(AutomationPermissions.Courses.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await courseRepository.DeleteAsync(id);
    }

    public virtual async Task<CourseDto> GetAsync(Guid id)
    {
        var course = await courseRepository.GetWithDetailsAsync(id, asNoTracking: true);   
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    public virtual async Task<PagedResultDto<CourseDto>> GetListAsync(GetCoursesInput input)
    {
        var totalCount = await courseRepository.GetCountAsync(
            input.FilterText,
            input.CourseName,
            input.Credits,
            input.StartFrom,
            input.EndTo,
            input.TeacherId);
        var courses = await courseRepository.GetListAsync(
            input.FilterText,
            input.CourseName,
            input.Credits,
            input.StartFrom,
            input.EndTo,
            input.TeacherId,
            input.Sorting, 
            input.MaxResultCount, 
            input.SkipCount);

        return new PagedResultDto<CourseDto>()
        {
           TotalCount = totalCount,
           Items = ObjectMapper.Map<List<Course>, List<CourseDto>>(courses)
        };

    }

    [Authorize(AutomationPermissions.Courses.Edit)]
    public virtual async Task<CourseDto> UpdateAsync(Guid id, CourseUpdateDto input)
    {
        var updatedCourse = await courseManager.UpdateCourseAsync(
            id,
            input.CourseName,
            input.Credits,
            input.StartFrom,
            input.EndTo,
            input.TeacherId,
            input.Status,
            input.ConcurrencyStamp);
        return ObjectMapper.Map<Course, CourseDto>(updatedCourse);
    }

#region GradeComponent service methods
    [Authorize(AutomationPermissions.Courses.GradeComponents.Create)]
    public virtual async Task<GradeComponentDto> AddGradeComponentAsync(
            GradeComponentCreateDto input,
            CancellationToken cancellationToken = default)
    {
        var gradeComponent = await courseManager.AddGradeComponentAsync(
            input.CourseId, input.GradeComponentName, input.Order, input.Weight, cancellationToken);

        return ObjectMapper.Map<GradeComponent, GradeComponentDto>(gradeComponent);
    }

    [Authorize(AutomationPermissions.Courses.GradeComponents.Edit)]
    public virtual async Task<GradeComponentDto> UpdateGradeComponentAsync(
        GradeComponentUpdateDto input,
        Guid gradeComponentId,
        CancellationToken cancellationToken = default)
    {
        var updatedGradeComponent = await courseManager.UpdateGradeComponentAsync(
            input.CourseId, gradeComponentId, input.GradeComponentName, input.Order, input.Weight, cancellationToken);
        return ObjectMapper.Map<GradeComponent, GradeComponentDto>(updatedGradeComponent);
    }

    [Authorize(AutomationPermissions.Courses.GradeComponents.Delete)]
    public virtual async Task RemoveGradeComponentAsync(
        Guid courseId,
        Guid gradeComponentId,
        CancellationToken cancellationToken = default)
    {
        await courseManager.RemoveGradeComponentAsync(
            courseId, gradeComponentId, cancellationToken);
    }
#endregion

#region CourseSessions service methods
    [Authorize(AutomationPermissions.Courses.CourseSessions.Create)]
    public virtual async Task<CourseSessionDto> AddCourseSessionAsync(
        CourseSessionCreateDto input,
        CancellationToken cancellationToken = default)
    {
        var session = await courseManager.AddCourseSessionAsync(
            input.CourseId, input.Day, ToTimeRange(input.Time), cancellationToken);

        return ObjectMapper.Map<CourseSession, CourseSessionDto>(session);
    }

    [Authorize(AutomationPermissions.Courses.CourseSessions.Edit)]
    public virtual async Task<CourseSessionDto> UpdateCourseSessionAsync(
        Guid courseSessionId,
        CourseSessionUpdateDto input,
        CancellationToken cancellationToken = default)
    {
       var updateCourseSession =  await courseManager.UpdateCourseSessionAsync(
           input.CourseId, courseSessionId, input.Day, ToTimeRange(input.Time), cancellationToken);
        return ObjectMapper.Map<CourseSession, CourseSessionDto>(updateCourseSession);
    }

    [Authorize(AutomationPermissions.Courses.CourseSessions.Delete)]
    public virtual async Task RemoveCourseSessionAsync(
        Guid courseId,
        Guid courseSessionId,
        CancellationToken cancellationToken = default)
    {
        await courseManager.RemoveCourseSessionAsync(
            courseId, courseSessionId, cancellationToken);
    }
#endregion
    private static TimeRange ToTimeRange(TimeRangeDto dto)
    {
        return new TimeRange(dto.Start, dto.End);
    }
}
