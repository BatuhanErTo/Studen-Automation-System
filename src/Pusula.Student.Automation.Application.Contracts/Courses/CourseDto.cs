using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Courses;

public class CourseDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public string CourseName { get; set; } = null!;
    public int Credits { get; set; }
    public DateTime StartFrom { get; set; }
    public DateTime EndTo { get; set; }
    public EnumCourseStatus Status { get; set; }
    public Guid TeacherId { get; set; }
    public List<GradeComponentDto> GradeComponents { get; set; } = new();
    public List<CourseSessionDto> CourseSessions { get; set; } = new();
    public string ConcurrencyStamp { get; set; }
}
