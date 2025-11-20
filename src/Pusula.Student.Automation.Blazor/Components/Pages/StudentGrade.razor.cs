using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

[Authorize(Roles = Roles.StudentRole)]
public partial class StudentGrade
{
    private Guid? _studentId;
    private bool _loaded;

    private readonly List<CourseGradesVm> _courseGrades = new();

    private class CourseGradesVm
    {
        public Guid EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<GradeRowVm> Grades { get; set; } = new();
        public double? TotalScore { get; set; }
    }

    private class GradeRowVm
    {
        public Guid GradeComponentId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public int Weight { get; set; }
        public double Score { get; set; }
    }

    protected override async Task OnInitializedAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
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
                await LoadGradesAsync(_studentId.Value);
            }
        });

        _loaded = true;
    }

    private async Task LoadGradesAsync(Guid studentId)
    {
        await ExecuteSafeAsync(async () =>
        {
            _courseGrades.Clear();

            var enrollments = await EnrollmentAppService.GetListWithNavigationAsync(new GetEnrollmentsInput
            {
                StudentId = studentId,
                MaxResultCount = 1000
            });

            foreach (var enrollment in enrollments.Items)
            {
                var course = await CourseAppService.GetAsync(enrollment.CourseDto.Id);
                var gradeComponents = course.GradeComponents ?? new List<GradeComponentDto>();

                var gradeEntryList = await EnrollmentAppService.GetGradesAsync(enrollment.CourseDto.Id, studentId);

                var courseGradesVm = new CourseGradesVm
                {
                    EnrollmentId = enrollment.EnrollmentDto.Id,
                    CourseId = enrollment.CourseDto.Id,
                    CourseName = enrollment.CourseDto.CourseName
                };

                foreach (var gradeEntry in gradeEntryList)
                {
                    var comp = gradeComponents.FirstOrDefault(gc => gc.Id == gradeEntry.GradeComponentId);
                    courseGradesVm.Grades.Add(new GradeRowVm
                    {
                        GradeComponentId = gradeEntry.GradeComponentId,
                        ComponentName = comp?.GradeComponentName ?? gradeEntry.GradeComponentId.ToString(),
                        Weight = comp?.Weight ?? 0,
                        Score = gradeEntry.Score
                    });
                }

                courseGradesVm.TotalScore = await EnrollmentAppService.GetTotalScoreAsync(courseGradesVm.EnrollmentId);

                _courseGrades.Add(courseGradesVm);
            }
        });
    }
}