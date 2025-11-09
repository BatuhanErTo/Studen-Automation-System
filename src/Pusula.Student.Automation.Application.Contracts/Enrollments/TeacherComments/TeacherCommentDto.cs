using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Enrollments.TeacherComments;

public class TeacherCommentDto : EntityDto<Guid>
{
    public string Comment { get; set; } = null!;
}
