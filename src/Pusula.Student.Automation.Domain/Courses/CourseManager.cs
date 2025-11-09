using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.GlobalExceptions;
using Pusula.Student.Automation.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Pusula.Student.Automation.Courses;

public class CourseManager(ICourseRepository courseRepository) : DomainService
{
    public virtual async Task<Course> CreateCourseAsync(
        string courseName,
        int credits,
        DateTime startFrom,
        DateTime endTo,
        Guid teacherId,
        EnumCourseStatus status,
        CancellationToken cancellationToken = default)
    {
        CheckValidateCourse(courseName, credits, startFrom, endTo, teacherId);
       
        var course = new Course(GuidGenerator.Create(), courseName, credits, startFrom, endTo, teacherId, status);
        
        return await courseRepository.InsertAsync(course, cancellationToken: cancellationToken);
    }

    public virtual async Task<Course> UpdateCourseAsync(
        Guid id,
        string courseName,
        int credits,
        DateTime startFrom,
        DateTime endTo,
        Guid teacherId,
        EnumCourseStatus status,
        string? concurenyTimeStamp = null,
        CancellationToken cancellationToken = default)
    {
        CheckValidateCourse(courseName, credits, startFrom, endTo, teacherId);

        var course = await courseRepository.GetAsync(id, cancellationToken: cancellationToken);

        course.SetCourseName(courseName);
        course.SetCredits(credits);
        course.SetStartFromAndEndTo(startFrom, endTo);
        course.SetTeacherId(teacherId);
        course.SetStatus(status);

        course.SetConcurrencyStampIfNotNull(concurenyTimeStamp);

        return await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);
    }

    public virtual async Task<GradeComponent> AddGradeComponentAsync(Guid courseId, string gradeComponentName, int order, int weight, CancellationToken cancellationToken = default)
    {
        CheckValidateGradeComponent(gradeComponentName, weight);

        var course = await courseRepository.GetWithDetailsAsync(courseId, cancellationToken: cancellationToken);

        var gradeComponent = course.AddGradeComponent(GuidGenerator.Create(), gradeComponentName, order, weight);

        await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);

        return gradeComponent;
    }

    public virtual async Task<GradeComponent> UpdateGradeComponentAsync(Guid courseId, Guid gradeComponentId, string gradeComponentName, int order, int weight, CancellationToken cancellationToken = default)
    {
        CheckValidateGradeComponent(gradeComponentName, weight);

        var course = await courseRepository.GetWithDetailsAsync(courseId, cancellationToken: cancellationToken);
        var updatedGradeComponent = course.UpdateGradeComponent(gradeComponentId, gradeComponentName, order, weight);

        await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);
        return updatedGradeComponent;
    }

    public virtual async Task RemoveGradeComponentAsync(Guid courseId, Guid gradeComponentId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetWithDetailsAsync(courseId, cancellationToken: cancellationToken);
        course.RemoveGradeComponent(gradeComponentId);

        await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);
    }

    public virtual async Task<CourseSession> AddCourseSessionAsync(Guid courseId, EnumWeekDay day, TimeRange time, CancellationToken cancellationToken = default)
    {
        //TODO: ilgili öğretmenin farklı bir kursu aynı vakit dilimlerinde çakışabilir bunun kontrolünü yap
        var course = await courseRepository.GetWithDetailsAsync(courseId, cancellationToken: cancellationToken);
        var courseSession = course.AddCourseSession(GuidGenerator.Create(), day, time);

        await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);

        return courseSession;
    }
    public virtual async Task<CourseSession> UpdateCourseSessionAsync(Guid courseId, Guid courseSessionId, EnumWeekDay day, TimeRange time, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetWithDetailsAsync(courseId, cancellationToken: cancellationToken);
        var updatedCourse = course.UpdateCourseSession(courseSessionId, day, time);

        await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);

        return updatedCourse;
    }

    public virtual async Task RemoveCourseSessionAsync(Guid courseId, Guid courseSessionId, CancellationToken cancellationToken = default)
    {
        var course = await courseRepository.GetWithDetailsAsync(courseId, cancellationToken: cancellationToken);
        course.RemoveCourseSession(courseSessionId);

        await courseRepository.UpdateAsync(course, cancellationToken: cancellationToken);
    }

    private static void CheckValidateCourse(
        string courseName,
        int credits,
        DateTime startFrom,
        DateTime endTo,
        Guid teacherId)
    {
        Check.NotNull(courseName, nameof(courseName));
        Check.Length(courseName, nameof(courseName), CourseConsts.MaxCourseNameLength, CourseConsts.MinCourseNameLength);
        Check.Range(credits, nameof(credits), CourseConsts.MinCredits, CourseConsts.MaxCredits);
        Check.NotNull(startFrom, nameof(startFrom));
        Check.NotNull(endTo, nameof(endTo));
        StudentAutomationException.ThrowIf("End date of course cannot be earlier than start date", "C-0002", endTo <= startFrom);
        Check.NotNull(teacherId, nameof(teacherId));
    }

    private static void CheckValidateGradeComponent(string gradeComponentName, int weight)
    {
        Check.NotNull(gradeComponentName, nameof(gradeComponentName));
        Check.Length(gradeComponentName, nameof(gradeComponentName), GradeComponentConsts.MaxGradeComponentNameLength, GradeComponentConsts.MinGradeComponentNameLength);
        Check.Range(weight, nameof(weight), GradeComponentConsts.MinWeight, GradeComponentConsts.MaxWeight);
    }
}
