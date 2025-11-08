using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Courses;

public class GetCoursesInput : PagedAndSortedResultRequestDto
{
    public string? FilterText { get; set; }
    public string? CourseName { get; set; }
    public int? Credits { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? EndTo { get; set; }
    public Guid? TeacherId { get; set; }
}
