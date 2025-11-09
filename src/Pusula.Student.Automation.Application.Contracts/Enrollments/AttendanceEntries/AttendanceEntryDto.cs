using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Enrollments.AttendanceEntries;

public class AttendanceEntryDto : EntityDto<Guid>
{
    public DateOnly Date { get; set; }
    public Guid CourseSessionId { get; set; }
    public EnumAttendanceStatus AttendanceStatus { get; set; }
    public string? AbsentReason { get; set; }
}