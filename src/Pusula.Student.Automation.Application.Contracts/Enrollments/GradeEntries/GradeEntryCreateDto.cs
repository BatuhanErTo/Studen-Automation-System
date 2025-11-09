using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Enrollments.GradeEntries;

public class GradeEntryCreateDto
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    public Guid GradeComponentId { get; set; }

    [Range(GradeEntryConsts.MinScore, GradeEntryConsts.MaxScore)]
    public double Score { get; set; }
}