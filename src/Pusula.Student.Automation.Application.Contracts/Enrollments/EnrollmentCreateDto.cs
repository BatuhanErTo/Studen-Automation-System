using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Enrollments;

public class EnrollmentCreateDto
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid CourseId { get; set; }
}
