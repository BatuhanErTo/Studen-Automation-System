using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Permissions;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

public partial class Teachers
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<TeacherWithNavigationPropertiesDto> TeacherList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private TeacherCreateDto NewTeacher { get; set; }
    private Validations NewTeacherValidations { get; set; } = new();
    private TeacherUpdateDto EditingTeacher { get; set; }
    private Validations EditingTeacherValidations { get; set; } = new();
    private Guid EditingTeacherId { get; set; }
    private Modal CreateTeacherModal { get; set; } = new();
    private Modal EditTeacherModal { get; set; } = new();
    private GetTeachersInput Filter { get; set; }
    private DataGridEntityActionsColumn<TeacherWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
    private bool CanCreateTeacher { get; set; }
    private bool CanEditTeacher { get; set; }
    private bool CanDeleteTeacher { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    private List<TeacherWithNavigationPropertiesDto> SelectedTeachers { get; set; } = [];
    private bool AllTeachersSelected { get; set; }
    protected string SelectedCreateTab = "teacher-create-tab";
    protected string SelectedEditTab = "teacher-edit-tab";
    public Teachers()
    {
        NewTeacher = new TeacherCreateDto();
        EditingTeacher = new TeacherUpdateDto();
        Filter = new GetTeachersInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        TeacherList = [];
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();

    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetDepartmentCollectionLookupAsync();
            await SetBreadcrumbItemsAsync();
            await SetToolbarItemsAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Teachers"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        //Toolbar.AddButton(L["ExportToExcel"], DownloadAsExcelAsync, IconName.Download);

        Toolbar.AddButton(L["NewTeacher"], OpenCreateTeacherModalAsync, IconName.Add, requiredPolicyName: AutomationPermissions.Teachers.Create);

        return ValueTask.CompletedTask;
    }
    private async Task SetPermissionsAsync()
    {
        CanCreateTeacher = await AuthorizationService
            .IsGrantedAsync(AutomationPermissions.Teachers.Create);
        CanEditTeacher = await AuthorizationService
                        .IsGrantedAsync(AutomationPermissions.Teachers.Edit);
        CanDeleteTeacher = await AuthorizationService
                        .IsGrantedAsync(AutomationPermissions.Teachers.Delete);
    }

    private async Task OpenCreateTeacherModalAsync()
    {
        NewTeacher = new TeacherCreateDto
        {
            DepartmentId = DepartmentsCollection.Select(i => i.Id).FirstOrDefault()
        };
        SelectedCreateTab = "teacher-create-tab";


        await NewTeacherValidations.ClearAll();
        await CreateTeacherModal.Show();
    }

    private async Task CloseCreateTeacherModalAsync()
    {
        NewTeacher = new TeacherCreateDto
        {
            DepartmentId = DepartmentsCollection.Select(i => i.Id).FirstOrDefault()
        };

        await CreateTeacherModal.Hide();
    }
    private async Task OpenEditTeacherModalAsync(TeacherWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "teacher-edit-tab";


        var teacherWithNav = await TeacherAppService.GetWithNavigationAsync(input.TeacherDto.Id);

        EditingTeacherId = teacherWithNav.TeacherDto.Id;
        EditingTeacher = ObjectMapper.Map<TeacherDto, TeacherUpdateDto>(teacherWithNav.TeacherDto);

        await EditingTeacherValidations.ClearAll();
        await EditTeacherModal.Show();
    }

    private async Task CloseEditTeacherModalAsync()
    {
        await EditTeacherModal.Hide();
    }
    private async Task UpdateTeacherAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (await EditingTeacherValidations.ValidateAll() == false)
            {
                return;
            }

            await TeacherAppService.UpdateAsync(EditingTeacherId, EditingTeacher);
            await GetTeachersAsync();
            await EditTeacherModal.Hide();
        });
    }
    private async Task CreateTeacherAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            if (await NewTeacherValidations.ValidateAll() == false)
            {
                return;
            }
            await TeacherAppService.CreateAsync(NewTeacher);
            await GetTeachersAsync();
            await CloseCreateTeacherModalAsync();
        });
    }
    private async Task DeleteTeacherAsync(TeacherWithNavigationPropertiesDto input)
    {
        await ExecuteSafeAsync(async () =>
        {
            await TeacherAppService.DeleteAsync(input.TeacherDto.Id);
            await GetTeachersAsync();
        });
    }

    private Task SelectedTeacherRowsChanged()
    {
        if (SelectedTeachers.Count != PageSize)
        {
            AllTeachersSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<TeacherWithNavigationPropertiesDto> e)
    {
        CurrentPage = e.Page;
        await GetTeachersAsync();
        await InvokeAsync(StateHasChanged);
    }
    private async Task GetTeachersAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await TeacherAppService.GetPagedListWithNavigationAsync(Filter);
        TeacherList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    private async Task GetDepartmentCollectionLookupAsync(string? newValue = null)
    {
        DepartmentsCollection = (await DepartmentAppService.GetDepartmentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }
    protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetTeachersAsync();
        await InvokeAsync(StateHasChanged);
    }
    protected virtual async Task OnFirstNameChangedAsync(string? firstName)
    {
        Filter.FirstName = firstName;
        await SearchAsync();
    }
    protected virtual async Task OnLastNameChangedAsync(string? lastName)
    {
        Filter.LastName = lastName;
        await SearchAsync();
    }

    protected virtual async Task OnEmailAddressChangedAsync(string? emailAddress)
    {
        Filter.EmailAddress = emailAddress;
        await SearchAsync();
    }
    protected virtual async Task OnPhoneNumberChangedAsync(string? mobilePhoneNumber)
    {
        Filter.PhoneNumber = mobilePhoneNumber;
        await SearchAsync();
    }
    private Task SelectAllItems()
    {
        AllTeachersSelected = true;

        return Task.CompletedTask;
    }
    private Task ClearSelection()
    {
        AllTeachersSelected = false;
        SelectedTeachers.Clear();

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedTeachersAsync()
    {
        await ExecuteSafeAsync(async () =>
        {
            var message = AllTeachersSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedTeachers.Count].Value;

            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllTeachersSelected)
                await TeacherAppService.DeleteAllAsync(Filter);
            else
                await TeacherAppService.DeleteByIdsAsync(SelectedTeachers.Select(x => x.TeacherDto.Id).ToList());

            SelectedTeachers.Clear();
            AllTeachersSelected = false;

            await GetTeachersAsync();
        });
    }
}