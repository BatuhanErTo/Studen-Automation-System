using Blazorise;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enrollments.AttendanceEntries;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Students;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Users;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

[Authorize(Roles = Roles.TeacherRole)]
public partial class AttendanceEntry
{
    private Guid? CurrentTeacherId { get; set; }

    private IReadOnlyList<CourseDto> TeacherCourses { get; set; } = Array.Empty<CourseDto>();
    private List<StudentDto> EnrolledStudents { get; set; } = new();
    private List<CourseSessionDto> CourseSessions { get; set; } = new();

    private Guid SelectedCourseIdValue { get; set; } = Guid.Empty;
    private Guid SelectedStudentIdValue { get; set; } = Guid.Empty;
    private Guid SelectedCourseSessionId { get; set; } = Guid.Empty;

    private Guid? CurrentEnrollmentId { get; set; }

    private DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    private int SelectedAttendanceStatus { get; set; } = 0; // 0=Present, 1=Absent
    private string? AbsentReason { get; set; }

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

    private async Task OnCourseChanged(Guid value)
    {
        SelectedCourseIdValue = value;

        SelectedStudentIdValue = Guid.Empty;
        SelectedCourseSessionId = Guid.Empty;
        CurrentEnrollmentId = null;
        EnrolledStudents.Clear();
        CourseSessions.Clear();

        if (SelectedCourseIdValue != Guid.Empty)
        {
            await LoadStudentsForCourseAsync(SelectedCourseIdValue);
            await LoadCourseSessionsAsync(SelectedCourseIdValue);
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

    private async Task LoadCourseSessionsAsync(Guid courseId)
    {
        await ExecuteSafeAsync(async () =>
        {
            var course = await CourseAppService.GetAsync(courseId);
            CourseSessions = course.CourseSessions?.OrderBy(cs => cs.Day).ThenBy(cs => cs.Time.Start).ToList()
                             ?? new List<CourseSessionDto>();
        });
    }

    private async Task OnStudentChanged(Guid value)
    {
        SelectedStudentIdValue = value;
        SelectedCourseSessionId = Guid.Empty;
        CurrentEnrollmentId = null;

        await ExecuteSafeAsync(async () =>
        {
            if (SelectedCourseIdValue != Guid.Empty && SelectedStudentIdValue != Guid.Empty)
            {
                var enrollment = await EnrollmentAppService.GetEnrollmentByStudentAndCourseAsync(SelectedStudentIdValue, SelectedCourseIdValue);
                CurrentEnrollmentId = enrollment.Id;
            }
        });
    }

    private Task OnCourseSessionChanged(Guid value)
    {
        SelectedCourseSessionId = value;
        return Task.CompletedTask;
    }

    private Task OnAttendanceStatusChanged(int value)
    {
        SelectedAttendanceStatus = value;
        if (SelectedAttendanceStatus == 0) AbsentReason = null;
        return Task.CompletedTask;
    }

    private async Task SaveAttendanceAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (CurrentEnrollmentId is null || SelectedCourseSessionId == Guid.Empty)
                return;

            await EnrollmentAppService.AddAttendanceEntryAsync(new AttendanceEntryCreateDto
            {
                EnrollmentId = CurrentEnrollmentId.Value,
                Date = Date,
                CourseSessionId = SelectedCourseSessionId,
                AttendanceStatus = (EnumAttendanceStatus)SelectedAttendanceStatus,
                AbsentReason = AbsentReason
            });

            await UiMessageService.Success("Attendance saved");
            // Optionally reset just the status/reason
            SelectedAttendanceStatus = 0;
            AbsentReason = null;
        });
    }
}