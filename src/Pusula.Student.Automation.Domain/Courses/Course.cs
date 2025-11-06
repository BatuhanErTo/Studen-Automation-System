using JetBrains.Annotations;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;

namespace Pusula.Student.Automation.Courses;

public class Course : FullAuditedAggregateRoot<Guid>
{
    [NotNull]
    public string CourseName { get; private set; }
    [NotNull]
    public int Credits { get; private set; }
    public DateTime StartFrom { get; private set; }
    public DateTime EndTo { get; private set; }
    public EnumCourseStatus Status { get; private set; }
    public Guid TeacherId { get; private set; }


    private List<GradeComponent> _gradeComponents = new();
    public IReadOnlyList<GradeComponent> GradeComponents => _gradeComponents.AsReadOnly();

    private List<CourseSession> _courseSessions = new();
    public IReadOnlyList<CourseSession> CourseSessions => _courseSessions.AsReadOnly();


    protected Course() { }

    public Course(
        Guid id,
        string courseName,
        int credits,
        DateTime startFrom,
        DateTime endTo,
        Guid teacherId,
        EnumCourseStatus status = EnumCourseStatus.Planned)
        : base(id)
    {
        SetCourseName(courseName);
        SetCredits(credits);
        SetStartFromAndEndTo(startFrom, endTo);
        SetTeacherId(teacherId);
        SetStatus(status);
    }

    public void SetCourseName(string courseName)
    {
        Check.NotNull(courseName, nameof(courseName));
        Check.Length(courseName, nameof(courseName), CourseConsts.MaxCourseNameLength, CourseConsts.MinCourseNameLength);
        CourseName = courseName;
    }

    public void SetCredits(int credits)
    {
        Check.Range(credits, nameof(credits), CourseConsts.MinCredits, CourseConsts.MaxCredits);
        Credits = credits;
    }

    public void SetStartFromAndEndTo(DateTime startFrom, DateTime endTo)
    {
        if (startFrom == default) throw new BusinessException("Course.StartFromDefault");
        if (endTo == default) throw new BusinessException("Course.EndToDefault");
        if (endTo <= startFrom) throw new BusinessException("Course.EndToMustBeGreaterThanStartFrom");
        StartFrom = startFrom;
        EndTo = endTo;
    }

    public void SetStatus(EnumCourseStatus enumCourseStatus) => Status = enumCourseStatus;

    public void SetTeacherId(Guid teacherId)
    {
        if (teacherId == Guid.Empty) throw new BusinessException("Course.TeacherIdInvalid");
        TeacherId = teacherId;
    }

    #region Grade Components Management Methods
    public GradeComponent AddGradeComponent(Guid gradeComponentId, string name, int order, int weight)
    {
        if (_gradeComponents.Any(x => x.GradeComponentName == name))
            throw new BusinessException("Course.GradeComponent.DuplicateName");

        var gradeComponent = new GradeComponent(gradeComponentId, Id, name, order, weight);
        _gradeComponents.Add(gradeComponent);

        EnsureGradeWeightsValid();
        return gradeComponent;
    }

    public void UpdateGradeComponent(Guid id, string name, int order, int weight)
    {
        var gradeComponent = _gradeComponents.FirstOrDefault(x => x.Id == id)
                 ?? throw new BusinessException("Course.GradeComponent.NotFound");

        if (_gradeComponents.Any(x => x.Id != id && x.GradeComponentName == name))
            throw new BusinessException("Course.GradeComponent.DuplicateName");

        gradeComponent.SetGradeComponentName(name);
        gradeComponent.SetOrder(order);
        gradeComponent.SetWeight(weight);

        EnsureGradeWeightsValid();
    }

    public void RemoveGradeComponent(Guid id)
    {
        var gradeComponent = _gradeComponents.FirstOrDefault(x => x.Id == id);
        if (gradeComponent != null)
        {
            _gradeComponents.Remove(gradeComponent);
            EnsureGradeWeightsValid();
        }
    }
    private void EnsureGradeWeightsValid()
    {
        var total = _gradeComponents.Sum(x => x.Weight);
        if (total != 100)
            throw new BusinessException("Course.GradeComponent.TotalWeightMustBe100");
    }
    #endregion

    #region Course Sessions Management Methods
    public CourseSession AddSession(Guid courseSessionId, EnumWeekDay day, TimeRange time)
    {
        if (_courseSessions.Any(s => s.Day == day && s.Time.Overlaps(time)))
            throw new BusinessException("Course.SessionOverlap");

        var courseSession = new CourseSession(courseSessionId, Id, day, time);
        _courseSessions.Add(courseSession);
        return courseSession;
    }

    public void UpdateSession(Guid sessionId, EnumWeekDay day, TimeRange time)
    {
        var courseSession = _courseSessions.FirstOrDefault(x => x.Id == sessionId)
                ?? throw new BusinessException("Course.SessionNotFound");

        if (_courseSessions.Any(x => x.Id != sessionId && x.Day == day && x.Time.Overlaps(time)))
            throw new BusinessException("Course.SessionOverlap");

        courseSession.SetDay(day);
        courseSession.SetTime(time);
    }

    public void RemoveSession(Guid sessionId)
    {
        var courseSession = _courseSessions.FirstOrDefault(x => x.Id == sessionId);
        if (courseSession != null) _courseSessions.Remove(courseSession);
    }
    #endregion
}
