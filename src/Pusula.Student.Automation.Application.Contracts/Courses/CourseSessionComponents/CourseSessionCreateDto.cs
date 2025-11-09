using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Courses.CourseSessionComponents;

public class CourseSessionCreateDto
{
    public Guid CourseId { get; set; }
    public EnumWeekDay Day { get; set; }
    public TimeRangeDto Time { get; set; }
}
