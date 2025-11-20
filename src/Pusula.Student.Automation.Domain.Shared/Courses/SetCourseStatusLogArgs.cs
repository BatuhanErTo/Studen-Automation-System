using System;
using Volo.Abp.BackgroundJobs;

namespace Pusula.Student.Automation.Courses;

[BackgroundJobName("set-course-status-log")]
public class SetCourseStatusLogArgs
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public EnumCourseStatus NewEnumCourseStatus { get; set; }
}
