using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Courses.GradeComponents;

public class GradeComponentCreateDto
{
    public Guid CourseId { get; set; }
    [Required]
    [StringLength(GradeComponentConsts.MaxGradeComponentNameLength, MinimumLength = GradeComponentConsts.MinGradeComponentNameLength)]
    public string GradeComponentName { get; set; } = null!;
    public int Order {  get; set; }
    public int Weight { get; set; }
}
