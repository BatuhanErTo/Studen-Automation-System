using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Courses.CourseSessionComponents;

public class CourseSessionUpdateDto : IHasConcurrencyStamp
{
    public Guid CourseId { get; private set; }
    public EnumWeekDay Day { get; private set; }
    public TimeRangeDto Time { get; private set; }
    public string ConcurrencyStamp { get; set; }
}
