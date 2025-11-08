using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Courses;

public class CourseCreateDto
{
    [Required]
    [StringLength(CourseConsts.MaxCourseNameLength, MinimumLength = CourseConsts.MinCourseNameLength)]
    public string CourseName { get; set; } = null!;
    public int Credits { get; set; }
    public DateTime StartFrom { get; set; }
    public DateTime EndTo { get; set; } 
    public EnumCourseStatus Status { get; set; }
    public Guid TeacherId { get; set; } 
}
