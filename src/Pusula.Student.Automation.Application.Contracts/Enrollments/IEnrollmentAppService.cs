using Pusula.Student.Automation.Enrollments.AttendanceEntries;
using Pusula.Student.Automation.Enrollments.GradeEntries;
using Pusula.Student.Automation.Enrollments.TeacherComments;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pusula.Student.Automation.Enrollments;

public interface IEnrollmentAppService : IApplicationService
{
    // CRUD
    Task<PagedResultDto<EnrollmentDto>> GetListAsync(GetEnrollmentsInput input);
    Task<EnrollmentDto> GetAsync(Guid id);
    Task<EnrollmentDto> CreateAsync(EnrollmentCreateDto input);
    Task<EnrollmentDto> UpdateAsync(Guid id, EnrollmentUpdateDto input);
    Task DeleteAsync(Guid id);
    Task UnenrollStudentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    // Comments
    Task AddTeacherCommentAsync(TeacherCommentCreateDto input, CancellationToken cancellationToken = default);
    Task RemoveTeacherCommentAsync(Guid enrollmentId, Guid teacherCommentId, CancellationToken cancellationToken = default);
    Task<List<TeacherCommentDto>> GetTeacherCommentsAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);

    // Grades
    Task<GradeEntryDto> AddGradeEntryAsync(GradeEntryCreateDto input, CancellationToken cancellationToken = default);
    Task UpdateGradeEntryAsync(Guid gradeEntryId, GradeEntryUpdateDto input, CancellationToken cancellationToken = default);
    Task RemoveGradeEntryAsync(Guid enrollmentId, Guid gradeEntryId, CancellationToken cancellationToken = default);
    Task<List<GradeEntryDto>> GetGradesAsync(Guid courseId, Guid studentId, CancellationToken cancellationToken = default);
    Task<EnrollmentDto> GetEnrollmentByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken = default);
    Task<double> GetTotalScoreAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    // Attendance
    Task<AttendanceEntryDto> AddAttendanceEntryAsync(AttendanceEntryCreateDto input, CancellationToken cancellationToken = default);
    Task UpdateAttendanceEntryAsync(Guid attendanceEntryId, AttendanceEntryUpdateDto input, CancellationToken cancellationToken = default);
    Task RemoveAttendanceEntryAsync(Guid enrollmentId, Guid attendanceEntryId, CancellationToken cancellationToken = default);
    Task<List<AttendanceEntryDto>> GetAttendanceAsync(Guid courseId, Guid studentId, DateOnly? dateFrom, DateOnly? dateTo, CancellationToken cancellationToken = default);

    // Navigation
    Task<EnrollmentWithNavigationPropertiesDto> GetWithNavigationAsync(Guid id);
    Task<PagedResultDto<EnrollmentWithNavigationPropertiesDto>> GetListWithNavigationAsync(GetEnrollmentsInput input);
}
