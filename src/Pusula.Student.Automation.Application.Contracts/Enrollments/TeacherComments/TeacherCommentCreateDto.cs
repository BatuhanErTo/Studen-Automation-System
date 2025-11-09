using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Enrollments.TeacherComments;

public class TeacherCommentCreateDto
{
    [Required]
    public Guid EnrollmentId { get; set; }

    [Required]
    [StringLength(TeacherCommentConsts.MaxCommentLength, MinimumLength = TeacherCommentConsts.MinCommentLength)]
    public string Comment { get; set; } = null!;
}
