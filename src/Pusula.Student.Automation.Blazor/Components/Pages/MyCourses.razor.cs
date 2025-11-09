using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Users;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

[Authorize(Roles = Roles.TeacherRole)]
public partial class MyCourses
{
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
        // Resolve teacher by current user’s email
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

    private void OnSessionDayChanged(Guid courseId, EnumWeekDay day)
    {
        SessionInputs[courseId].Day = day;
    }

    private void OnSessionStartChanged(Guid courseId, TimeOnly start)
    {
        SessionInputs[courseId].Time.Start = start;
    }

    private void OnSessionEndChanged(Guid courseId, TimeOnly end)
    {
        SessionInputs[courseId].Time.End = end;
    }

    private async Task AddSessionAsync(Guid courseId)
    {
        var input = SessionInputs[courseId];
        await CourseAppService.AddCourseSessionAsync(input);
        // reset default session values (keeps Day)
        SessionInputs[courseId].Time = new TimeRangeDto { Start = new TimeOnly(9, 0), End = new TimeOnly(10, 0) };
    }

    private void OnGradeNameChanged(Guid courseId, string name)
    {
        GradeInputs[courseId].GradeComponentName = name ?? string.Empty;
    }

    private void OnGradeOrderChanged(Guid courseId, int order)
    {
        GradeInputs[courseId].Order = order;
    }

    private void OnGradeWeightChanged(Guid courseId, int weight)
    {
        GradeInputs[courseId].Weight = weight;
    }

    private async Task AddGradeComponentAsync(Guid courseId)
    {
        var input = GradeInputs[courseId];
        await CourseAppService.AddGradeComponentAsync(input);
        // reset name/order/weight
        GradeInputs[courseId] = new GradeComponentCreateDto
        {
            CourseId = courseId,
            GradeComponentName = string.Empty,
            Order = 1,
            Weight = 10
        };
    }
}