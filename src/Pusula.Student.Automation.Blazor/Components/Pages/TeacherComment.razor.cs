using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enrollments.TeacherComments;
using Pusula.Student.Automation.Teachers;
using Pusula.Student.Automation.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;
using Pusula.Student.Automation.Enums;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

[Authorize(Roles = Roles.TeacherRole)]
public partial class TeacherComment
{
    private Guid? CurrentTeacherId { get; set; }

    private IReadOnlyList<CourseDto> TeacherCourses { get; set; } = Array.Empty<CourseDto>();
    private List<StudentDto> EnrolledStudents { get; set; } = new();
    private List<TeacherCommentDto> Comments { get; set; } = new();

    private Guid SelectedCourseIdValue { get; set; } = Guid.Empty;
    private Guid SelectedStudentIdValue { get; set; } = Guid.Empty;

    private Guid? CurrentEnrollmentId { get; set; }
    private string NewCommentText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
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
        CurrentEnrollmentId = null;
        EnrolledStudents.Clear();
        Comments.Clear();
        NewCommentText = string.Empty;

        if (SelectedCourseIdValue != Guid.Empty)
        {
            await LoadStudentsForCourseAsync(SelectedCourseIdValue);
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

    private async Task OnStudentChanged(ChangeEventArgs<Guid, StudentDto> studentChangeEventArgs)
    {
        SelectedStudentIdValue = studentChangeEventArgs.Value;
        CurrentEnrollmentId = null;
        Comments.Clear();
        NewCommentText = string.Empty;

        await ExecuteSafeAsync(async () =>
        {
            if (SelectedCourseIdValue != Guid.Empty && SelectedStudentIdValue != Guid.Empty)
            {
                var enrollment = await EnrollmentAppService.GetEnrollmentByStudentAndCourseAsync(SelectedStudentIdValue, SelectedCourseIdValue);
                CurrentEnrollmentId = enrollment.Id;

                await LoadCommentsAsync();
            }
        });
    }

    private async Task LoadCommentsAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (SelectedCourseIdValue == Guid.Empty || SelectedStudentIdValue == Guid.Empty)
                return;

            var list = await EnrollmentAppService.GetTeacherCommentsAsync(SelectedCourseIdValue, SelectedStudentIdValue);
            Comments = list.OrderByDescending(c => c.Id).ToList();
        });
    }

    private async Task SaveTeacherCommentAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (CurrentEnrollmentId is null || string.IsNullOrWhiteSpace(NewCommentText))
                return;

            await EnrollmentAppService.AddTeacherCommentAsync(new TeacherCommentCreateDto
            {
                EnrollmentId = CurrentEnrollmentId.Value,
                Comment = NewCommentText.Trim()
            });

            NewCommentText = string.Empty;
            await LoadCommentsAsync();
            await UiMessageService.Success("Comment added");
        });
    }

    private async Task RemoveCommentAsync(Guid commentId)
    {
        await ExecuteSafeAsync(async () =>
        {
            if (CurrentEnrollmentId is null)
                return;

            await EnrollmentAppService.RemoveTeacherCommentAsync(CurrentEnrollmentId.Value, commentId);
            await LoadCommentsAsync();
        });
    }
}