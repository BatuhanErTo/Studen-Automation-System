using Pusula.Student.Automation.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Pusula.Student.Automation.Enrollments;

public class EnrollmentManager(
    IEnrollmentRepository enrollmentRepository,
    ICourseRepository courseRepository
) : DomainService
{
    public virtual async Task<Enrollment> CreateEnrollmentAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        if (studentId == Guid.Empty) throw new BusinessException("Enrollment.StudentIdInvalid");
        if (courseId == Guid.Empty) throw new BusinessException("Enrollment.CourseIdInvalid");

        var exists = await enrollmentRepository.FindByStudentAndCourseAsync(studentId, courseId, cancellationToken);
        if (exists is not null)
        {
            throw new BusinessException("EnrollmentAlreadyExists")
                .WithData("StudentId", studentId)
                .WithData("CourseId", courseId);
        }

        var enrollment = new Enrollment(GuidGenerator.Create(), studentId, courseId);
        return await enrollmentRepository.InsertAsync(enrollment, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteEnrollmentAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        await enrollmentRepository.DeleteAsync(enrollmentId, cancellationToken: cancellationToken);
    }

    public virtual async Task AddTeacherCommentAsync(
        Guid enrollmentId,
        string comment,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);
        enrollment.AddTeacherComment(comment);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
    }

    public virtual async Task RemoveTeacherCommentAsync(
        Guid enrollmentId,
        Guid teacherCommentId,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);
        enrollment.RemoveTeacherComment(teacherCommentId);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
    }

    public virtual async Task<GradeEntry> AddGradeEntryAsync(
        Guid enrollmentId,
        Guid gradeComponentId,
        double score,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);

        // Validate gradeComponent belongs to the course
        var course = await courseRepository.GetWithDetailsAsync(enrollment.CourseId, asNoTracking: true, cancellationToken: cancellationToken);
        if (!course.GradeComponents.Any(gc => gc.Id == gradeComponentId))
            throw new BusinessException("GradeComponentInvalidForCourse")
                .WithData("CourseId", enrollment.CourseId)
                .WithData("GradeComponentId", gradeComponentId);

        var entry = enrollment.AddGradeEntry(gradeComponentId, score);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
        return entry;
    }

    public virtual async Task UpdateGradeEntryAsync(
        Guid enrollmentId,
        Guid gradeEntryId,
        double score,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);
        enrollment.UpdateGradeEntry(gradeEntryId, score);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
    }

    public virtual async Task RemoveGradeEntryAsync(
        Guid enrollmentId,
        Guid gradeEntryId,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);
        enrollment.RemoveGradeEntry(gradeEntryId);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
    }

    public virtual async Task<double> CalculateTotalScoreAsync(
        Guid enrollmentId,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, asNoTracking: true, cancellationToken: cancellationToken);
        var course = await courseRepository.GetWithDetailsAsync(enrollment.CourseId, asNoTracking: true, cancellationToken: cancellationToken);
        var weights = course.GradeComponents.Select(gc => (gc.Id, gc.Weight));
        return enrollment.CalculateTotalScore(weights);
    }

    public virtual async Task<AttendanceEntry> AddAttendanceEntryAsync(
        Guid enrollmentId,
        DateOnly date,
        Guid courseSessionId,
        EnumAttendanceStatus attendanceStatus,
        string? absentReason = null,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);

        // Validate session belongs to the course
        var course = await courseRepository.GetWithDetailsAsync(enrollment.CourseId, asNoTracking: true, cancellationToken: cancellationToken);
        if (!course.CourseSessions.Any(cs => cs.Id == courseSessionId))
            throw new BusinessException("CourseSessionInvalidForCourse")
                .WithData("CourseId", enrollment.CourseId)
                .WithData("CourseSessionId", courseSessionId);

        if (date < DateOnly.FromDateTime(course.StartFrom) || date > DateOnly.FromDateTime(course.EndTo))
            throw new BusinessException("Attendance.DateOutOfCourseRange")
                .WithData("CourseId", enrollment.CourseId)
                .WithData("CourseStartFrom", course.StartFrom)
                .WithData("CourseEndTo", course.EndTo)
                .WithData("AttendanceDate", date);

        var entry = enrollment.AddAttendanceEntry(date, courseSessionId, attendanceStatus, absentReason);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
        return entry;
    }

    public virtual async Task UpdateAttendanceEntryAsync(
        Guid enrollmentId,
        Guid attendanceEntryId,
        EnumAttendanceStatus attendanceStatus,
        string? absentReason = null,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);
        enrollment.UpdateAttendanceEntry(attendanceEntryId, attendanceStatus, absentReason);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
    }

    public virtual async Task RemoveAttendanceEntryAsync(
        Guid enrollmentId,
        Guid attendanceEntryId,
        CancellationToken cancellationToken = default)
    {
        var enrollment = await enrollmentRepository.GetWithDetailsAsync(enrollmentId, cancellationToken: cancellationToken);
        enrollment.RemoveAttendanceEntry(attendanceEntryId);
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken: cancellationToken);
    }
}
