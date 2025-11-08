using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Courses.GradeComponents;

public class GradeComponentDto : EntityDto<Guid>
{
    public Guid CourseId { get; set; }
    public string GradeComponentName { get; set; } = null!;
    public int Order { get; set; }
    public int Weight { get; set; }
}
