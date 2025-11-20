using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enrollments.TeacherComments;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Users;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

[Authorize(Roles = Roles.StudentRole)]
public partial class MessageBox
{
    private Guid? _studentId;
    private bool _loaded;

    private readonly List<CourseCommentsVm> _items = new();

    private class CourseCommentsVm
    {
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<TeacherCommentDto> Comments { get; set; } = new();
    }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            // Resolve student by current user email
            var email = CurrentUser.Email ?? string.Empty;

            var students = await StudentAppService.GetPagedListWithNavigationAsync(new GetStudentsInput
            {
                EmailAddress = email,
                MaxResultCount = 1
            });

            var student = students.Items.FirstOrDefault();
            _studentId = student?.StudentDto.Id;

            if (_studentId.HasValue)
            {
                await LoadAsync(_studentId.Value);
            }
        });

        _loaded = true;
    }

    private async Task LoadAsync(Guid studentId)
    {
        await ExecuteSafeAsync(async () =>
        {
            _items.Clear();

            var enrollments = await EnrollmentAppService.GetListWithNavigationAsync(new GetEnrollmentsInput
            {
                StudentId = studentId,
                MaxResultCount = 1000
            });

            foreach (var e in enrollments.Items)
            {
                var comments = await EnrollmentAppService.GetTeacherCommentsAsync(e.CourseDto.Id, studentId);

                _items.Add(new CourseCommentsVm
                {
                    CourseId = e.CourseDto.Id,
                    CourseName = e.CourseDto.CourseName,
                    Comments = comments.OrderByDescending(c => c.Id).ToList()
                });
            }
        });
    }
}