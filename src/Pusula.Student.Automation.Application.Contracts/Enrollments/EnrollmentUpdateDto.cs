using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Enrollments;

public class EnrollmentUpdateDto : IHasConcurrencyStamp
{
    [Required]
    public Guid StudentId { get; set; }

    [Required]
    public Guid CourseId { get; set; }

    public string ConcurrencyStamp { get; set; } = null!;
}
