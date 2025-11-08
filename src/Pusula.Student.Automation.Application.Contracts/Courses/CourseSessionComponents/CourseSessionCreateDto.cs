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
    public Guid CourseId { get; private set; }
    public EnumWeekDay Day { get; private set; }
    public TimeRangeDto Time { get; private set; }
}
