using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Enrollments.AttendanceEntries;

public class GetAttendanceInput
{
    public Guid? CourseId { get; set; }
    public Guid? StudentId { get; set; }
    public DateOnly? DateFrom { get; set; }
    public DateOnly? DateTo { get; set; }
}
