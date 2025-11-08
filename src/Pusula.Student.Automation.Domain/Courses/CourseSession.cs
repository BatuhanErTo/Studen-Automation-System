using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Courses;

public sealed class CourseSession : Entity<Guid>
{
    public Guid CourseId { get; private set; }
    public EnumWeekDay Day { get; private set; }          
    public TimeRange Time { get; private set; }      

    protected CourseSession() { }

    internal CourseSession(Guid id, Guid courseId, EnumWeekDay day, TimeRange time)
    {
        Id = id;
        CourseId = courseId;
        SetDay(day);
        SetTime(time);
    }

    internal void SetDay(EnumWeekDay day) => Day = day;
    internal void SetTime(TimeRange time) => Time = time;
}
