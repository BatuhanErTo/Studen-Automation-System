using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enrollments.GradeEntries;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Students;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Blazor.DropDowns;

namespace Pusula.Student.Automation.Blazor.Components.Pages;
[Authorize(Roles = Roles.TeacherRole)]
public partial class Grading
{
    private Guid? CurrentTeacherId { get; set; }

    private IReadOnlyList<CourseDto> TeacherCourses { get; set; } = Array.Empty<CourseDto>();
    private List<StudentDto> EnrolledStudents { get; set; } = new();
    private List<GradeComponentDto> GradeComponents { get; set; } = new();

    private Guid SelectedCourseIdValue { get; set; } = Guid.Empty;
    private Guid SelectedStudentIdValue { get; set; } = Guid.Empty;
    private Guid SelectedGradeComponentId { get; set; } = Guid.Empty;

    private Guid? CurrentEnrollmentId { get; set; }
    private double Score { get; set; } = 0;

    protected override async Task OnInitializedAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            // Resolve teacher by current user email
            var userEmail = CurrentUser.Email ?? string.Empty;
            var teachers = await TeacherAppService.GetPagedListAsync(new GetTeachersInput
            {
                EmailAddress = userEmail,
                MaxResultCount = 1
            });
            CurrentTeacherId = teachers.Items.FirstOrDefault()?.Id;

            if (CurrentTeacherId.HasValue)
            {
                await LoadCoursesAsync(CurrentTeacherId.Value);
            }
        });
    }

    private async Task LoadCoursesAsync(Guid teacherId)
    {
        await ExecuteSafeAsync(async () =>
        {
            var result = await CourseAppService.GetListAsync(new GetCoursesInput
            {
                TeacherId = teacherId,
                MaxResultCount = 1000
            });

            TeacherCourses = result.Items;
        });
    }

    private async Task OnCourseChanged(ChangeEventArgs<Guid, CourseDto> courseChangeEventArgs)
    {
        SelectedCourseIdValue = courseChangeEventArgs.Value;

        SelectedStudentIdValue = Guid.Empty;
        SelectedGradeComponentId = Guid.Empty;
        CurrentEnrollmentId = null;
        EnrolledStudents.Clear();
        GradeComponents.Clear();
        CurrentEnrollmentId = null;

        if (SelectedCourseIdValue != Guid.Empty)
        {
            await LoadStudentsForCourseAsync(SelectedCourseIdValue);
            await LoadGradeComponentsForCourseAsync(SelectedCourseIdValue);
        }
    }

    private async Task LoadStudentsForCourseAsync(Guid courseId)
    {
        await ExecuteSafeAsync(async () =>
        {
            var enrollments = await EnrollmentAppService.GetListWithNavigationAsync(new GetEnrollmentsInput
            {
                CourseId = courseId,
                MaxResultCount = 1000
            });

            EnrolledStudents = enrollments.Items
                .Select(x => x.StudentDto)
                .DistinctBy(s => s.Id)
                .ToList();
        });
    }

    private async Task LoadGradeComponentsForCourseAsync(Guid courseId)
    {
        await ExecuteSafeAsync(async () =>
        {
            var course = await CourseAppService.GetAsync(courseId);
            GradeComponents = course.GradeComponents?.OrderBy(gc => gc.Order).ToList() ?? new List<GradeComponentDto>();
        });
    }

    private async Task OnStudentChanged(ChangeEventArgs<Guid, StudentDto> studentChangeEventArgs)
    {
        SelectedStudentIdValue = studentChangeEventArgs.Value;
        SelectedGradeComponentId = Guid.Empty;
        CurrentEnrollmentId = null;

        await ExecuteSafeAsync(async () =>
        {
            if (SelectedCourseIdValue != Guid.Empty && SelectedStudentIdValue != Guid.Empty)
            {
                // Fetch or resolve the enrollment id
                var enrollment = await EnrollmentAppService.GetEnrollmentByStudentAndCourseAsync(SelectedStudentIdValue, SelectedCourseIdValue);
                CurrentEnrollmentId = enrollment.Id;
            }
        });
    }

    private Task OnGradeComponentChanged(ChangeEventArgs<Guid, GradeComponentDto> gradeComponentChangeEventArgs)
    {
        SelectedGradeComponentId = gradeComponentChangeEventArgs.Value;
        return Task.CompletedTask;
    }

    private async Task SaveGradeEntryAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (CurrentEnrollmentId is null || SelectedGradeComponentId == Guid.Empty)
                return;

            await EnrollmentAppService.AddGradeEntryAsync(new GradeEntryCreateDto
            {
                EnrollmentId = CurrentEnrollmentId.Value,
                GradeComponentId = SelectedGradeComponentId,
                Score = Score
            });

            await UiMessageService.Success("Grade entry saved");
            // Optionally reset score
            Score = 0;
        });
    }
}