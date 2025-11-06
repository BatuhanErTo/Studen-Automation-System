using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Enrollments;

public class AttendanceEntry : Entity<Guid>
{
    public Guid EnrollmentId { get; private set; }
    public DateOnly Date { get; private set; }            
    public Guid CourseSessionId { get; private set; }  
    public EnumAttendanceStatus AttendanceStatus { get; private set; }
    public string? AbsentReason { get; private set; }
    protected AttendanceEntry() { }

    internal AttendanceEntry(
        Guid id, 
        Guid enrollmentId, 
        DateOnly date,
        Guid courseSessionId,
        EnumAttendanceStatus attendanceStatus,
        string? absentReason = null) : base(id)
    {
        SetEnrollmentId(enrollmentId);
        SetDate(date);
        SetCourseSessionId(courseSessionId);
        SetEnumAttendanceStatus(attendanceStatus);
        SetAbsentReason(absentReason);
    }

    internal void SetEnrollmentId(Guid enrollmentId)
    {
        if (enrollmentId == Guid.Empty) throw new BusinessException("Attendance.EnrollmentIdInvalid");
        EnrollmentId = enrollmentId;
    }

    internal void SetDate(DateOnly date)
    {
        Date = date;
    }

    internal void SetCourseSessionId(Guid courseSessionId)
    {
        if (courseSessionId == Guid.Empty) throw new BusinessException("Attendance.CourseSessionIdInvalid");
        CourseSessionId = courseSessionId;
    }

    internal void SetEnumAttendanceStatus(EnumAttendanceStatus attendanceStatus) => AttendanceStatus = attendanceStatus;

    internal void SetAbsentReason(string? absentReason)
    {
        if (absentReason is not null)
        {
            Check.Length(absentReason, nameof(absentReason), AttendanceEntryConsts.MaxReasonLength, AttendanceEntryConsts.MinReasonLength);
            AbsentReason = absentReason;
        }
    }
}
