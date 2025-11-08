using Pusula.Student.Automation.Students;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Student.Automation.Enrollments;

public sealed class Enrollment : FullAuditedAggregateRoot<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }

    private List<TeacherComment> _teacherComments = new();
    public IReadOnlyList<TeacherComment> TeacherComments => _teacherComments.AsReadOnly();

    private List<GradeEntry> _gradeEntries = new();
    public IReadOnlyList<GradeEntry> GradeEntries => _gradeEntries.AsReadOnly();

    private List<AttendanceEntry> _attendanceEntries = new();
    public IReadOnlyList<AttendanceEntry> AttendanceEntries => _attendanceEntries.AsReadOnly();

    protected Enrollment() { }
    public Enrollment(Guid id, Guid studentId,  Guid courseId) : base(id)
    {
        SetStudentId(studentId);
        SetCourseId(courseId);
    }
    public void SetStudentId(Guid studentId)
    {
        if (studentId == Guid.Empty) throw new BusinessException("Enrollment.StudentIdInvalid");

        StudentId = studentId;
    }
    public void SetCourseId(Guid courseId)
    {
        if (courseId == Guid.Empty) throw new BusinessException("Enrollment.CourseIdInvalid");

        CourseId = courseId;
    }

#region TeacherComments Management Methods
    public void AddTeacherComment(string comment)
    {
        Check.NotNull(comment, nameof(comment));
        var teacherComment = new TeacherComment(Guid.NewGuid(), Id, comment);
        _teacherComments.Add(teacherComment);
    }

    public void RemoveTeacherComment(Guid teacherCommentId)
    {
        var teacherComment = _teacherComments.FirstOrDefault(x => x.Id == teacherCommentId);
        if (teacherComment == null)
        {
            throw new BusinessException("TeacherCommentNotFound", $"No teacher comment found with ID '{teacherCommentId}'.");
        }
        _teacherComments.Remove(teacherComment);
    }
#endregion

#region GradeEntry Management Methods
    public GradeEntry AddGradeEntry(Guid gradeComponentId, double score)
    {
        if (_gradeEntries.Any(x => x.GradeComponentId == gradeComponentId))
        {
            throw new BusinessException("GradeEntryAlreadyExists", $"A grade entry for the grade component '{gradeComponentId}' already exists.");
        }
        var gradeEntry = new GradeEntry(Guid.NewGuid(), Id, gradeComponentId, score);
        _gradeEntries.Add(gradeEntry);
        return gradeEntry;
    }

    public void UpdateGradeEntry(Guid gradeEntryId, double newScore)
    {
        var gradeEntry = _gradeEntries.FirstOrDefault(x => x.Id == gradeEntryId);
        if (gradeEntry == null)
        {
            throw new BusinessException("GradeEntryNotFound", $"No grade entry found with ID '{gradeEntryId}'.");
        }


        gradeEntry.SetScore(newScore);
    }

    public void RemoveGradeEntry(Guid gradeEntryId)
    {
        var gradeEntry = _gradeEntries.FirstOrDefault(x => x.Id == gradeEntryId);
        if (gradeEntry == null)
        {
            throw new BusinessException("GradeEntryNotFound", $"No grade entry found with ID '{gradeEntryId}'.");
        }
        _gradeEntries.Remove(gradeEntry);
    }

    public double CalculateTotalScore(IEnumerable<(Guid gradeComponentId, int weight)> gradeComponentIdAndWeightEnumerable)
    {
        var weights = gradeComponentIdAndWeightEnumerable
        .ToDictionary(x => x.gradeComponentId, x => x.weight);

        return _gradeEntries.Sum(entry =>
            weights.TryGetValue(entry.GradeComponentId, out var weight)
                ? (entry.Score * weight) / 100.0
                : 0);
    }
#endregion

#region AttendanceEntry Management Methods
    public AttendanceEntry AddAttendanceEntry(
        DateOnly date, 
        Guid courseSessionId, 
        EnumAttendanceStatus attendanceStatus, 
        string? absentReason = null)
    {
        if (_attendanceEntries.Any(x => x.Date == date && x.CourseSessionId == courseSessionId))
            throw new BusinessException("Attendance.AlreadyExists")
                .WithData("Date", date)
                .WithData("CourseSessionId", courseSessionId);

        var attendanceEntry = new AttendanceEntry(Guid.NewGuid(), Id, date, courseSessionId, attendanceStatus, absentReason);
        _attendanceEntries.Add(attendanceEntry);
        return attendanceEntry;
    }

    public void UpdateAttendanceEntry(
        Guid attendanceEntryId,
        EnumAttendanceStatus attendanceStatus,
        string? absentReason = null)
    {
        var attendanceEntry = _attendanceEntries.FirstOrDefault(x => x.Id == attendanceEntryId);
        if (attendanceEntry == null)
        {
            throw new BusinessException("AttendanceEntryNotFound", $"No attendance entry found with ID '{attendanceEntryId}'.");
        }
        attendanceEntry.SetEnumAttendanceStatus(attendanceStatus);
        attendanceEntry.SetAbsentReason(absentReason);
    }
    public void RemoveAttendanceEntry(Guid attendanceEntryId)
    {
        var attendanceEntry = _attendanceEntries.FirstOrDefault(x => x.Id == attendanceEntryId);
        if (attendanceEntry == null)
        {
            throw new BusinessException("AttendanceEntryNotFound", $"No attendance entry found with ID '{attendanceEntryId}'.");
        }
        _attendanceEntries.Remove(attendanceEntry);
    }

#endregion
}
