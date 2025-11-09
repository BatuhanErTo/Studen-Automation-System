using Pusula.Student.Automation.Enrollments.AttendanceEntries;
using Pusula.Student.Automation.Enrollments.GradeEntries;
using Pusula.Student.Automation.Enrollments.TeacherComments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Enrollments;

public class EnrollmentDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }

    public List<TeacherCommentDto> TeacherComments { get; set; } = new();
    public List<GradeEntryDto> GradeEntries { get; set; } = new();
    public List<AttendanceEntryDto> AttendanceEntries { get; set; } = new();
    public string ConcurrencyStamp { get; set; } = null!;
}
