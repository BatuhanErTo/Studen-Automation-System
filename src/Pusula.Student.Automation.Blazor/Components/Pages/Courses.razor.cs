using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Permissions;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

public partial class Courses
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }

    private IReadOnlyList<CourseDto> CourseList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }

    private CourseCreateDto NewCourse { get; set; }
    private Validations NewCourseValidations { get; set; } = new();
    private CourseUpdateDto EditingCourse { get; set; }
    private Validations EditingCourseValidations { get; set; } = new();
    private Guid EditingCourseId { get; set; }

    private Modal CreateCourseModal { get; set; } = new();
    private Modal EditCourseModal { get; set; } = new();

    private GetCoursesInput Filter { get; set; }

    private DataGridEntityActionsColumn<CourseDto> EntityActionsColumn { get; set; } = new();
    private bool CanCreateCourse { get; set; }
    private bool CanEditCourse { get; set; }
    private bool CanDeleteCourse { get; set; }

    private IReadOnlyList<LookupDto<Guid>> TeachersCollection { get; set; } = [];
    private Dictionary<Guid, string> TeacherNameLookup { get; set; } = new();

    private List<CourseDto> SelectedCourses { get; set; } = [];
    private bool AllCoursesSelected { get; set; }

    protected string SelectedCreateTab = "course-create-tab";
    protected string SelectedEditTab = "course-edit-tab";

    public Courses()
    {
        NewCourse = new CourseCreateDto { Status = EnumCourseStatus.Planned };
        EditingCourse = new CourseUpdateDto { Status = EnumCourseStatus.Planned, ConcurrencyStamp = string.Empty };
        Filter = new GetCoursesInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        CourseList = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetTeacherCollectionLookupAsync();
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Courses"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewCourse"], OpenCreateCourseModalAsync, IconName.Add, requiredPolicyName: AutomationPermissions.Courses.Create);
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateCourse = await AuthorizationService.IsGrantedAsync(AutomationPermissions.Courses.Create);
        CanEditCourse = await AuthorizationService.IsGrantedAsync(AutomationPermissions.Courses.Edit);
        CanDeleteCourse = await AuthorizationService.IsGrantedAsync(AutomationPermissions.Courses.Delete);
    }

    private async Task OpenCreateCourseModalAsync()
    {
        await EnsureTeachersLoadedForFormAsync();

        NewCourse = new CourseCreateDto
        {
            Status = EnumCourseStatus.Planned,
            StartFrom = DateTime.Today,
            EndTo = DateTime.Today.AddDays(1),
            TeacherId = TeachersCollection.Select(i => i.Id).FirstOrDefault()
        };
        SelectedCreateTab = "course-create-tab";

        await NewCourseValidations.ClearAll();
        await CreateCourseModal.Show();
    }

    private async Task CloseCreateCourseModalAsync()
    {
        await CreateCourseModal.Hide();
    }

    private async Task OpenEditCourseModalAsync(CourseDto input)
    {
        SelectedEditTab = "course-edit-tab";

        await EnsureTeachersLoadedForFormAsync();

        var dto = await CourseAppService.GetAsync(input.Id);
        EditingCourseId = dto.Id;
        EditingCourse = new CourseUpdateDto
        {
            CourseName = dto.CourseName,
            Credits = dto.Credits,
            StartFrom = dto.StartFrom,
            EndTo = dto.EndTo,
            Status = dto.Status,
            TeacherId = dto.TeacherId,
            ConcurrencyStamp = dto.ConcurrencyStamp
        };

        await EditingCourseValidations.ClearAll();
        await EditCourseModal.Show();
    }

    private async Task CloseEditCourseModalAsync()
    {
        await EditCourseModal.Hide();
    }

    private async Task UpdateCourseAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (await EditingCourseValidations.ValidateAll() == false)
            {
                return;
            }

            await CourseAppService.UpdateAsync(EditingCourseId, EditingCourse);
            await GetCoursesAsync();
            await EditCourseModal.Hide();
        });
    }

    private async Task CreateCourseAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (await NewCourseValidations.ValidateAll() == false)
            {
                return;
            }

            await CourseAppService.CreateAsync(NewCourse);
            await GetCoursesAsync();
            await CloseCreateCourseModalAsync();
        });
    }

    private async Task DeleteCourseAsync(CourseDto input)
    {
        await ExecuteSafeAsync(async () =>
        {
            await CourseAppService.DeleteAsync(input.Id);
            await GetCoursesAsync();
        });
    }

    private Task SelectedCourseRowsChanged()
    {
        if (SelectedCourses.Count != PageSize)
        {
            AllCoursesSelected = false;
        }
        return Task.CompletedTask;
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

        var result = await CourseAppService.GetListAsync(Filter);
        CourseList = result.Items;
        TotalCount = (int)result.TotalCount;

        BuildTeacherNameLookup(CourseList.Select(x => x.TeacherId).Distinct().ToList());

        await ClearSelection();
    }

    private async Task EnsureTeachersLoadedForFormAsync()
    {
        var all = await TeacherAppService.GetListAsync();

        TeachersCollection = all
            .Select(t => new LookupDto<Guid> { Id = t.Id, DisplayName = $"{t.FirstName} {t.LastName}" })
            .ToList();

        foreach (var t in TeachersCollection)
        {
            TeacherNameLookup[t.Id] = t.DisplayName;
        }
    }

    private void BuildTeacherNameLookup(IEnumerable<Guid> teacherIds)
    {
        // Keep existing names; only add missing ids using current TeachersCollection.
        foreach (var id in teacherIds)
        {
            if (!TeacherNameLookup.ContainsKey(id))
            {
                var found = TeachersCollection.FirstOrDefault(x => x.Id == id);
                if (found != null)
                {
                    TeacherNameLookup[id] = found.DisplayName;
                }
            }
        }
    }

    private async Task GetTeacherCollectionLookupAsync(string? newValue = null)
    {
        var teachers = await TeacherAppService.GetPagedListAsync(new GetTeachersInput
        {
            FilterText = newValue,
            MaxResultCount = 50
        });

        TeachersCollection = teachers.Items
            .Select(t => new LookupDto<Guid> { Id = t.Id, DisplayName = $"{t.FirstName} {t.LastName}" })
            .ToList();

        foreach (var t in TeachersCollection)
        {
            TeacherNameLookup[t.Id] = t.DisplayName;
        }
    }

    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetCoursesAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OnCourseNameChangedAsync(string? courseName)
    {
        Filter.CourseName = courseName ?? string.Empty;
        await SearchAsync();
    }

    protected virtual async Task OnCreditsChangedAsync(int? credits)
    {
        Filter.Credits = credits;
        await SearchAsync();
    }

    protected virtual async Task OnStartFromChangedAsync(DateTime? startFrom)
    {
        Filter.StartFrom = startFrom;
        await SearchAsync();
    }

    protected virtual async Task OnEndToChangedAsync(DateTime? endTo)
    {
        Filter.EndTo = endTo;
        await SearchAsync();
    }
    protected virtual async Task OnTeacherFilterChangedAsync(Guid? teacherId)
    {
        Filter.TeacherId = teacherId;
        await SearchAsync();
    }

    private Task SelectAllItems()
    {
        AllCoursesSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllCoursesSelected = false;
        SelectedCourses.Clear();
        return Task.CompletedTask;
    }

    private async Task DeleteSelectedCoursesAsync()
    {
        var message = AllCoursesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedCourses.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllCoursesSelected)
        {
            foreach (var c in CourseList.ToList())
            {
                await CourseAppService.DeleteAsync(c.Id);
            }
        }
        else
        {
            foreach (var c in SelectedCourses.ToList())
            {
                await CourseAppService.DeleteAsync(c.Id);
            }
        }

        SelectedCourses.Clear();
        AllCoursesSelected = false;

        await GetCoursesAsync();
    }
}