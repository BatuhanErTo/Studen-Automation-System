using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Teachers;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.BlazoriseUI.Components;
using Volo.Abp.Users;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

[Authorize(Roles = Roles.TeacherRole)]
public partial class MyCourses
{
    private DataGridEntityActionsColumn<CourseDto> EntityActionsColumn { get; set; } = new();
    private IReadOnlyList<CourseDto> CourseList { get; set; } = Array.Empty<CourseDto>();
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private Guid? CurrentTeacherId { get; set; }
    private GetCoursesInput Filter { get; set; }

    private Dictionary<Guid, EnumCourseStatus> EditingStatuses { get; set; } = new();
    private Dictionary<Guid, CourseSessionCreateDto> SessionInputs { get; set; } = new();
    private Dictionary<Guid, GradeComponentCreateDto> GradeInputs { get; set; } = new();

    // Modal state
    private Guid? SelectedCourseIdForSession { get; set; }
    private CourseSessionCreateDto CurrentSession { get; set; } = new()
    {
        Day = EnumWeekDay.Monday,
        Time = new TimeRangeDto { Start = new TimeOnly(9, 0), End = new TimeOnly(10, 0) }
    };
    private Modal AddSessionModal { get; set; } = new();
    private int SelectedDayValue { get; set; } = (int)EnumWeekDay.Monday;
    private Validations SessionValidations { get; set; } = new();
    private Guid? SelectedCourseIdForGrade { get; set; }
    private GradeComponentCreateDto CurrentGrade { get; set; } = new()
    {
        GradeComponentName = string.Empty,
        Order = 1,
        Weight = 10
    };
    private Modal AddGradeModal { get; set; } = new();
    private Validations GradeValidations { get; set; } = new();


    private Modal DetailsModal { get; set; } = new();
    private CourseDto? DetailsCourse { get; set; }

    // Edit Status modal
    private Modal EditStatusModal { get; set; } = new();
    private Guid? SelectedCourseIdForEdit { get; set; }
    private int SelectedStatusValue { get; set; } = (int)EnumCourseStatus.Planned;
    private Validations EditStatusValidations { get; set; } = new();

    // Enroll Student modal state
    private Modal EnrollModal { get; set; } = new();
    private Guid? SelectedCourseIdForEnroll { get; set; }
    private DataGrid<StudentDto> StudentsGridRef { get; set; } = new();
    private IReadOnlyList<StudentDto> AvailableStudents { get; set; } = Array.Empty<StudentDto>();
    private int StudentPageSize { get; set; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int StudentTotalCount { get; set; }
    private int StudentCurrentPage { get; set; } = 1;
    private string? StudentFilterText { get; set; }
    private Guid SelectedStudentIdValue { get; set; } = Guid.Empty;

    public MyCourses()
    {
        Filter = new GetCoursesInput
        {
            MaxResultCount = PageSize,
            SkipCount = 0,
            Sorting = CurrentSorting
        };
    }

    protected override async Task OnInitializedAsync()
    {
        var userEmail = CurrentUser.Email ?? string.Empty;

        var teachers = await TeacherAppService.GetPagedListAsync(new GetTeachersInput
        {
            EmailAddress = userEmail,
            MaxResultCount = 1
        });
        var teacher = teachers.Items.FirstOrDefault();
        CurrentTeacherId = teacher?.Id;

        await GetCoursesAsync();
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CourseDto> e)
    {
        CurrentPage = e.Page;
        await GetCoursesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task GetCoursesAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;
        Filter.TeacherId = CurrentTeacherId;

        if (CurrentTeacherId == null)
        {
            CourseList = Array.Empty<CourseDto>();
            TotalCount = 0;
            return;
        }

        var result = await CourseAppService.GetListAsync(Filter);
        CourseList = result.Items;
        TotalCount = (int)result.TotalCount;

        InitializeRowStates(CourseList);
    }

    private void InitializeRowStates(IEnumerable<CourseDto> courses)
    {
        foreach (var c in courses)
        {
            if (!EditingStatuses.ContainsKey(c.Id))
                EditingStatuses[c.Id] = c.Status;

            if (!SessionInputs.ContainsKey(c.Id))
                SessionInputs[c.Id] = new CourseSessionCreateDto
                {
                    CourseId = c.Id,
                    Day = EnumWeekDay.Monday,
                    Time = new TimeRangeDto { Start = new TimeOnly(9, 0), End = new TimeOnly(10, 0) }
                };

            if (!GradeInputs.ContainsKey(c.Id))
                GradeInputs[c.Id] = new GradeComponentCreateDto
                {
                    CourseId = c.Id,
                    GradeComponentName = string.Empty,
                    Order = 1,
                    Weight = 10
                };
        }
    }

    private void OnStatusChanged(Guid courseId, EnumCourseStatus newStatus)
    {
        EditingStatuses[courseId] = newStatus;
    }

    private Task OnSelectedStatusChanged(int value)
    {
        SelectedStatusValue = value;
        return Task.CompletedTask;
    }

    private async Task SaveStatusAsync(Guid courseId)
    {
        var dto = await CourseAppService.GetAsync(courseId);

        var input = new CourseUpdateDto
        {
            CourseName = dto.CourseName,
            Credits = dto.Credits,
            StartFrom = dto.StartFrom,
            EndTo = dto.EndTo,
            Status = EditingStatuses[courseId],
            TeacherId = dto.TeacherId,
            ConcurrencyStamp = dto.ConcurrencyStamp
        };

        await CourseAppService.UpdateAsync(courseId, input);
        await GetCoursesAsync();
    }

    private async Task OpenEditStatusModal(CourseDto course)
    {
        SelectedCourseIdForEdit = course.Id;
        // initialize proxy with current status
        if (!EditingStatuses.TryGetValue(course.Id, out var current))
        {
            current = course.Status;
            EditingStatuses[course.Id] = current;
        }
        SelectedStatusValue = (int)current;

        await EditStatusValidations.ClearAll();
        await EditStatusModal.Show();
    }

    private async Task CloseEditStatusModalAsync()
    {
        await EditStatusModal.Hide();
    }

    private async Task SaveStatusFromModalAsync()
    {
        if (SelectedCourseIdForEdit is null)
            return;

        var enumStatus = (EnumCourseStatus)SelectedStatusValue;
        EditingStatuses[SelectedCourseIdForEdit.Value] = enumStatus;

        await SaveStatusAsync(SelectedCourseIdForEdit.Value);
        await EditStatusModal.Hide();
    }

    // ----- Add Session Modal flows -----
    private Task OnSelectedDayChanged(int value)
    {
        SelectedDayValue = value;
        return Task.CompletedTask;
    }
    private async Task OpenAddSessionModal(Guid courseId)
    {
        SelectedCourseIdForSession = courseId;
        if (!SessionInputs.TryGetValue(courseId, out var input))
        {
            input = new CourseSessionCreateDto
            {
                CourseId = courseId,
                Day = EnumWeekDay.Monday,
                Time = new TimeRangeDto { Start = new TimeOnly(9, 0), End = new TimeOnly(10, 0) }
            };
            SessionInputs[courseId] = input;
        }

        CurrentSession = new CourseSessionCreateDto
        {
            CourseId = input.CourseId,
            Day = input.Day,
            Time = new TimeRangeDto { Start = input.Time.Start, End = input.Time.End }
        };

        // set proxy from enum
        SelectedDayValue = (int)CurrentSession.Day;

        await SessionValidations.ClearAll();
        await AddSessionModal.Show();
    }


    private async Task CloseAddSessionModalAsync()
    {
        await AddSessionModal.Hide();
    }
    private async Task SaveSessionAsync()
    {
        if (SelectedCourseIdForSession is null) return;

        CurrentSession.Day = (EnumWeekDay)SelectedDayValue;

        SessionInputs[SelectedCourseIdForSession.Value] = CurrentSession;
        await CourseAppService.AddCourseSessionAsync(CurrentSession);
        await AddSessionModal.Hide();
    }

    // ----- Add Grade Modal flows -----
    private async Task OpenAddGradeModal(Guid courseId)
    {
        SelectedCourseIdForGrade = courseId;
        if (!GradeInputs.TryGetValue(courseId, out var input))
        {
            input = new GradeComponentCreateDto
            {
                CourseId = courseId,
                GradeComponentName = string.Empty,
                Order = 1,
                Weight = 10
            };
            GradeInputs[courseId] = input;
        }

        CurrentGrade = new GradeComponentCreateDto
        {
            CourseId = input.CourseId,
            GradeComponentName = input.GradeComponentName,
            Order = input.Order,
            Weight = input.Weight
        };

        await GradeValidations.ClearAll();
        await AddGradeModal.Show();
    }

    private async Task CloseAddGradeModalAsync()
    {
        await AddGradeModal.Hide();
    }

    private async Task SaveGradeAsync()
    {
        if (SelectedCourseIdForGrade is null)
            return;

        GradeInputs[SelectedCourseIdForGrade.Value] = CurrentGrade;
        await CourseAppService.AddGradeComponentAsync(CurrentGrade);
        await AddGradeModal.Hide();
    }
    // ----- Details Modal flows -----
    private async Task OpenDetailsModalAsync(Guid courseId)
    {
        DetailsCourse = null;
        await DetailsModal.Show();
        // Fetch full details including CourseSessions and GradeComponents
        DetailsCourse = await CourseAppService.GetAsync(courseId);
    }

    private async Task CloseDetailsModalAsync()
    {
        await DetailsModal.Hide();
        DetailsCourse = null;
    }
    // ----- Enroll Student Modal flows -----
    private async Task OpenEnrollModal(Guid courseId)
    {
        SelectedCourseIdForEnroll = courseId;
        SelectedStudentIdValue = Guid.Empty;
        StudentFilterText = null;
        StudentCurrentPage = 1;

        await EnrollModal.Show();
    }

    private async Task CloseEnrollModalAsync()
    {
        await EnrollModal.Hide();
        SelectedCourseIdForEnroll = null;
        AvailableStudents = Array.Empty<StudentDto>();
        StudentTotalCount = 0;
        SelectedStudentIdValue = Guid.Empty;
    }

    private Task OnStudentFilterTextChangedAsync(string? value)
    {
        StudentFilterText = value;
        // Do not auto-query; let user click Search to avoid frequent calls
        return Task.CompletedTask;
    }

    private async Task SearchStudentsAsync()
    {
        // Force DataGrid to re-query with current filter
        await StudentsGridRef.Reload();
    }

    private async Task OnStudentDataGridReadAsync(DataGridReadDataEventArgs<StudentDto> e)
    {
        if (SelectedCourseIdForEnroll is null)
        {
            AvailableStudents = Array.Empty<StudentDto>();
            StudentTotalCount = 0;
            return;
        }

        StudentCurrentPage = e.Page;
        var skip = (StudentCurrentPage - 1) * StudentPageSize;
        var result = await StudentAppService.GetAvailableStudentListAsync(
            SelectedCourseIdForEnroll.Value,
            StudentFilterText,
            skip,
            StudentPageSize);

        AvailableStudents = result.Items;
        StudentTotalCount = (int)result.TotalCount;
    }

    private void OnStudentRowClicked(DataGridRowMouseEventArgs<StudentDto> e)
    {
        SelectedStudentIdValue = e.Item.Id;
    }

    private void GetStudentRowStyling(StudentDto s, DataGridRowStyling styling)
    {
        if (s.Id == SelectedStudentIdValue)
        {
            styling.Class = "table-active";
        }
    }
    private async Task CreateEnrollmentAsync()
    {
        if (SelectedCourseIdForEnroll is null || SelectedStudentIdValue == Guid.Empty)
            return;

        await EnrollmentAppService.CreateAsync(new EnrollmentCreateDto
        {
            CourseId = SelectedCourseIdForEnroll.Value,
            StudentId = SelectedStudentIdValue
        });

        await UiMessageService.Success(L["SuccessfullyCompleted"].Value);
        await CloseEnrollModalAsync();
    }
}