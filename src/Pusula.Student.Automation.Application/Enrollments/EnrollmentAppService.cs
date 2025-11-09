using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Enrollments.AttendanceEntries;
using Pusula.Student.Automation.Enrollments.GradeEntries;
using Pusula.Student.Automation.Enrollments.TeacherComments;
using Pusula.Student.Automation.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace Pusula.Student.Automation.Enrollments;

[Authorize(AutomationPermissions.Enrollments.Default)]
public class EnrollmentAppService(
    IEnrollmentRepository enrollmentRepository,
    EnrollmentManager enrollmentManager
) : AutomationAppService, IEnrollmentAppService
{
    [Authorize(AutomationPermissions.Enrollments.Create)]
    public virtual async Task<EnrollmentDto> CreateAsync(EnrollmentCreateDto input)
    {
        var enrollment = await enrollmentManager.CreateEnrollmentAsync(input.StudentId, input.CourseId);
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }

    [Authorize(AutomationPermissions.Enrollments.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await enrollmentManager.DeleteEnrollmentAsync(id);
    }

    public virtual async Task<EnrollmentDto> GetAsync(Guid id)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(id, asNoTracking: true);
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }

    public virtual async Task<PagedResultDto<EnrollmentDto>> GetListAsync(GetEnrollmentsInput input)
    {
        var totalCount = await enrollmentRepository.GetCountAsync(input.CourseId, input.StudentId);
        var items = await enrollmentRepository.GetListAsync(input.CourseId, input.StudentId, input.Sorting, input.MaxResultCount, input.SkipCount);
        return new PagedResultDto<EnrollmentDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Enrollment>, List<EnrollmentDto>>(items)
        };
    }

    [Authorize(AutomationPermissions.Enrollments.Edit)]
    public virtual async Task<EnrollmentDto> UpdateAsync(Guid id, EnrollmentUpdateDto input)
    {
        var enrollment = await enrollmentRepository.GetAsync(id);
        enrollment.SetStudentId(input.StudentId);
        enrollment.SetCourseId(input.CourseId);
        enrollment.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        var updated = await enrollmentRepository.UpdateAsync(enrollment);
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(updated);
    }

    // Enroll/Unenroll
    [Authorize(AutomationPermissions.Enrollments.Create)]
    public virtual async Task<EnrollmentDto> EnrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentManager.CreateEnrollmentAsync(studentId, courseId, cancellationToken);
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(enrollment);
    }

    [Authorize(AutomationPermissions.Enrollments.Delete)]
    public virtual async Task UnenrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var exists = await enrollmentRepository.FindByStudentAndCourseAsync(studentId, courseId, cancellationToken);
        if (exists is not null)
        {
            await enrollmentManager.DeleteEnrollmentAsync(exists.Id, cancellationToken);
        }
    }

    // Comments
    [Authorize(AutomationPermissions.Enrollments.Edit)]
    public virtual async Task AddTeacherCommentAsync(TeacherCommentCreateDto input, CancellationToken cancellationToken = default)
    {
        await enrollmentManager.AddTeacherCommentAsync(input.EnrollmentId, input.Comment, cancellationToken);
    }

    [Authorize(AutomationPermissions.Enrollments.Delete)]
    public virtual async Task RemoveTeacherCommentAsync(Guid enrollmentId, Guid teacherCommentId, CancellationToken cancellationToken = default)
    {
        await enrollmentManager.RemoveTeacherCommentAsync(enrollmentId, teacherCommentId, cancellationToken);
    }

    public virtual async Task<List<TeacherCommentDto>> GetTeacherCommentsAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await enrollmentRepository.GetTeacherCommentsByStudentIdAndCourseIdAsync(courseId, studentId, cancellationToken);
        return ObjectMapper.Map<List<TeacherComment>, List<TeacherCommentDto>>(items);
    }

    // Grades
    [Authorize(AutomationPermissions.Enrollments.Create)]
    public virtual async Task<GradeEntryDto> AddGradeEntryAsync(GradeEntryCreateDto input, CancellationToken cancellationToken = default)
    {
        var entry = await enrollmentManager.AddGradeEntryAsync(input.EnrollmentId, input.GradeComponentId, input.Score, cancellationToken);
        return ObjectMapper.Map<GradeEntry, GradeEntryDto>(entry);
    }

    [Authorize(AutomationPermissions.Enrollments.Edit)]
    public virtual async Task UpdateGradeEntryAsync(Guid gradeEntryId, GradeEntryUpdateDto input, CancellationToken cancellationToken = default)
    {
        await enrollmentManager.UpdateGradeEntryAsync(input.EnrollmentId, gradeEntryId, input.Score, cancellationToken);
    }

    [Authorize(AutomationPermissions.Enrollments.Delete)]
    public virtual async Task RemoveGradeEntryAsync(Guid enrollmentId, Guid gradeEntryId, CancellationToken cancellationToken = default)
    {
        await enrollmentManager.RemoveGradeEntryAsync(enrollmentId, gradeEntryId, cancellationToken);
    }

    public virtual async Task<List<GradeEntryDto>> GetGradesAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default)
    {
        var items = await enrollmentRepository.GetGradesByStudentIdAndCourseIdAsync(courseId, studentId, cancellationToken);
        return ObjectMapper.Map<List<GradeEntry>, List<GradeEntryDto>>(items);
    }

    public virtual async Task<EnrollmentDto> GetEnrollmentByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.FindByStudentAndCourseAsync(studentId, courseId, cancellationToken)
            ?? throw new BusinessException("EnrollmentNotFound")
                .WithData("StudentId", studentId)
                .WithData("CourseId", courseId);

        var withDetails = await enrollmentRepository.GetWithDetailsAsync(enrollment.Id, asNoTracking: true, cancellationToken);
        return ObjectMapper.Map<Enrollment, EnrollmentDto>(withDetails);
    }

    public virtual async Task<double> GetTotalScoreAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await enrollmentManager.CalculateTotalScoreAsync(enrollmentId, cancellationToken);
    }

    // Attendance
    [Authorize(AutomationPermissions.Enrollments.Create)]
    public virtual async Task<AttendanceEntryDto> AddAttendanceEntryAsync(AttendanceEntryCreateDto input, CancellationToken cancellationToken = default)
    {
        var entry = await enrollmentManager.AddAttendanceEntryAsync(input.EnrollmentId, input.Date, input.CourseSessionId, input.AttendanceStatus, input.AbsentReason, cancellationToken);
        return ObjectMapper.Map<AttendanceEntry, AttendanceEntryDto>(entry);
    }

    [Authorize(AutomationPermissions.Enrollments.Edit)]
    public virtual async Task UpdateAttendanceEntryAsync(Guid attendanceEntryId, AttendanceEntryUpdateDto input, CancellationToken cancellationToken = default)
    {
        await enrollmentManager.UpdateAttendanceEntryAsync(input.EnrollmentId, attendanceEntryId, input.AttendanceStatus, input.AbsentReason, cancellationToken);
    }

    [Authorize(AutomationPermissions.Enrollments.Delete)]
    public virtual async Task RemoveAttendanceEntryAsync(Guid enrollmentId, Guid attendanceEntryId, CancellationToken cancellationToken = default)
    {
        await enrollmentManager.RemoveAttendanceEntryAsync(enrollmentId, attendanceEntryId, cancellationToken);
    }

    public virtual async Task<List<AttendanceEntryDto>> GetAttendanceAsync(Guid courseId, Guid studentId, DateOnly? dateFrom, DateOnly? dateTo, CancellationToken cancellationToken = default)
    {
        var entries = await enrollmentRepository.GetAttendanceAsync(courseId, studentId, dateFrom, dateTo, cancellationToken);
        return ObjectMapper.Map<List<AttendanceEntry>, List<AttendanceEntryDto>>(entries);
    }
}
