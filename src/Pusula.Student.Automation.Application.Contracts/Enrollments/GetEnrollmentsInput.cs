using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Enrollments;

public class GetEnrollmentsInput : PagedAndSortedResultRequestDto
{
    public Guid? CourseId { get; set; }
    public Guid? StudentId { get; set; }
}
