using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Pusula.Student.Automation.Courses;

public sealed class GradeComponent : Entity<Guid>
{
    public Guid CourseId { get; private set; }
    public string GradeComponentName { get; private set; }
    public int Order { get; private set; }
    public int Weight { get; private set; }

    protected GradeComponent() { }

    internal GradeComponent(
        Guid id,
        Guid courseId,
        string gradeComponentName,
        int order,
        int weight
    ) : base(id)
    {
        SetCourseId(courseId);
        SetGradeComponentName(gradeComponentName);
        SetOrder(order);
        SetWeight(weight);
    }

    internal void SetCourseId(Guid courseId)
    {
        if (courseId == Guid.Empty) throw new BusinessException("GradeComponent.CourseIdInvalid");

        CourseId = courseId;
    }

    internal void SetGradeComponentName(string gradeComponentName)
    {
        Check.NotNull(gradeComponentName, nameof(gradeComponentName));
        Check.Length(gradeComponentName, nameof(gradeComponentName), GradeComponentConsts.MaxGradeComponentNameLength, GradeComponentConsts.MinGradeComponentNameLength);
        GradeComponentName = gradeComponentName;
    }

    internal void SetOrder(int order) => Order = order;
    internal void SetWeight(int weight)
    {
        Check.Range(weight, nameof(weight), GradeComponentConsts.MinWeight, GradeComponentConsts.MaxWeight);
        Weight = weight;
    }
}
