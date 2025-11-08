using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Enrollments;

public sealed class TeacherComment : Entity<Guid>
{
    public Guid EnrollmentId { get; private set; }
    public string Comment { get; private set; }
    protected TeacherComment() { }
    internal TeacherComment(Guid id, Guid enrollmentId, string comment) : base(id)
    {
        SetEnrollmentId(enrollmentId);
        SetComment(comment);
    }
    internal void SetEnrollmentId(Guid enrollmentId)
    {
        if (enrollmentId == Guid.Empty) throw new BusinessException("TeacherComment.EnrollmentIdInvalid");
        EnrollmentId = enrollmentId;
    }
    internal void SetComment(string comment)
    {
        Check.NotNull(comment, nameof(comment));
        Check.Length(comment, nameof(comment), TeacherCommentConsts.MaxCommentLength, TeacherCommentConsts.MinCommentLength);
        Comment = comment;
    }
}
