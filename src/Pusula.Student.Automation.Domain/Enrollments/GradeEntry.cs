using Pusula.Student.Automation.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Enrollments;

public sealed class GradeEntry : Entity<Guid>
{
    public Guid EnrollmentId { get; private set; }
    public Guid GradeComponentId { get; private set; }
    public double Score { get; private set; }
    protected GradeEntry() { }
    internal GradeEntry(Guid id, Guid enrollmentId, Guid gradeComponentId, double score) : base(id)
    {
        SetEnrollmentId(enrollmentId);
        SetGradeComponentId(gradeComponentId);
        SetScore(score);
    }
    internal void SetEnrollmentId(Guid enrollmentId)
    {
        if (enrollmentId == Guid.Empty) throw new BusinessException("GradeEntry.EnrollmentIdInvalid");
        EnrollmentId = enrollmentId;
    }
    internal void SetGradeComponentId(Guid gradeComponentId)
    {
        if (gradeComponentId == Guid.Empty) throw new BusinessException("GradeEntry.GradeComponentIdInvalid");

        GradeComponentId = gradeComponentId;
    }
    internal void SetScore(double score)
    {
        Check.Range(score, nameof(score), GradeEntryConsts.MinScore, GradeEntryConsts.MaxScore);
        Score = score;
    }
}
