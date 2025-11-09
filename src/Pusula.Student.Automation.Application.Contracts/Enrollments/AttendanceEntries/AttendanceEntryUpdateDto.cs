using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Enrollments.AttendanceEntries;

public class AttendanceEntryUpdateDto
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public EnumAttendanceStatus AttendanceStatus { get; set; }

    [StringLength(AttendanceEntryConsts.MaxReasonLength, MinimumLength = AttendanceEntryConsts.MinReasonLength)]
    public string? AbsentReason { get; set; }
}