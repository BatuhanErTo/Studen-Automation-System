using Blazorise;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Authorization;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Permissions;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;

namespace Pusula.Student.Automation.Blazor.Components.Pages;

public partial class Students
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = [];
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
    private IReadOnlyList<StudentWithNavigationPropertiesDto> StudentList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private StudentCreateDto NewStudent { get; set; }
    private Validations NewStudentValidations { get; set; } = new();
    private StudentUpdateDto EditingStudent { get; set; }
    private Validations EditingStudentValidations { get; set; } = new();
    private Guid EditingStudentId { get; set; }
    private Modal CreateStudentModal { get; set; } = new();
    private Modal EditStudentModal { get; set; } = new();
    private GetStudentsInput Filter { get; set; }
    private DataGridEntityActionsColumn<StudentWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
    private bool CanCreateStudent { get; set; }
    private bool CanEditStudent { get; set; }
    private bool CanDeleteStudent { get; set; }
    private IReadOnlyList<LookupDto<Guid>> DepartmentsCollection { get; set; } = [];
    private List<StudentWithNavigationPropertiesDto> SelectedStudents { get; set; } = [];
    private bool AllStudentsSelected { get; set; }
    protected string SelectedCreateTab = "student-create-tab";
    protected string SelectedEditTab = "student-edit-tab";

    public Students()
    {
        NewStudent = new StudentCreateDto();
        EditingStudent = new StudentUpdateDto();
        Filter = new GetStudentsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        StudentList = [];
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
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Students"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewStudent"], OpenCreateStudentModalAsync, IconName.Add, requiredPolicyName: AutomationPermissions.Students.Create);
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateStudent = await AuthorizationService.IsGrantedAsync(AutomationPermissions.Students.Create);
        CanEditStudent = await AuthorizationService.IsGrantedAsync(AutomationPermissions.Students.Edit);
        CanDeleteStudent = await AuthorizationService.IsGrantedAsync(AutomationPermissions.Students.Delete);
    }

    private async Task OpenCreateStudentModalAsync()
    {
        NewStudent = new StudentCreateDto
        {
            DepartmentId = DepartmentsCollection.Select(i => i.Id).FirstOrDefault()
        };
        SelectedCreateTab = "student-create-tab";

        await NewStudentValidations.ClearAll();
        await CreateStudentModal.Show();
    }

    private async Task CloseCreateStudentModalAsync()
    {
        NewStudent = new StudentCreateDto
        {
            DepartmentId = DepartmentsCollection.Select(i => i.Id).FirstOrDefault()
        };

        await CreateStudentModal.Hide();
    }

    private async Task OpenEditStudentModalAsync(StudentWithNavigationPropertiesDto input)
    {
        SelectedEditTab = "student-edit-tab";

        var withNav = await StudentAppService.GetWithNavigationAsync(input.StudentDto.Id);

        EditingStudentId = withNav.StudentDto.Id;
        EditingStudent = ObjectMapper.Map<StudentDto, StudentUpdateDto>(withNav.StudentDto);

        await EditingStudentValidations.ClearAll();
        await EditStudentModal.Show();
    }

    private async Task CloseEditStudentModalAsync()
    {
        await EditStudentModal.Hide();
    }

    private async Task UpdateStudentAsync()
    {
        try
        {
            if (await EditingStudentValidations.ValidateAll() == false)
            {
                return;
            }

            await StudentAppService.UpdateAsync(EditingStudentId, EditingStudent);
            await GetStudentsAsync();
            await EditStudentModal.Hide();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CreateStudentAsync()
    {
        try
        {
            if (await NewStudentValidations.ValidateAll() == false)
            {
                return;
            }
            await StudentAppService.CreateAsync(NewStudent);
            await GetStudentsAsync();
            await CloseCreateStudentModalAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task DeleteStudentAsync(StudentWithNavigationPropertiesDto input)
    {
        await StudentAppService.DeleteAsync(input.StudentDto.Id);
        await GetStudentsAsync();
    }

    private Task SelectedStudentRowsChanged()
    {
        if (SelectedStudents.Count != PageSize)
        {
            AllStudentsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<StudentWithNavigationPropertiesDto> e)
    {
        CurrentPage = e.Page;
        await GetStudentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task GetStudentsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await StudentAppService.GetListWithNavigationAsync(Filter);
        StudentList = result.Items;
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
        await GetStudentsAsync();
        await InvokeAsync(StateHasChanged);
    }

    protected virtual async Task OnFirstNameChangedAsync(string? firstName)
    {
        Filter.FirstName = firstName ?? string.Empty;
        await SearchAsync();
    }

    protected virtual async Task OnLastNameChangedAsync(string? lastName)
    {
        Filter.LastName = lastName ?? string.Empty;
        await SearchAsync();
    }

    protected virtual async Task OnEmailAddressChangedAsync(string? emailAddress)
    {
        Filter.EmailAddress = emailAddress ?? string.Empty;
        await SearchAsync();
    }

    protected virtual async Task OnPhoneNumberChangedAsync(string? mobilePhoneNumber)
    {
        Filter.PhoneNumber = mobilePhoneNumber ?? string.Empty;
        await SearchAsync();
    }

    private Task SelectAllItems()
    {
        AllStudentsSelected = true;
        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllStudentsSelected = false;
        SelectedStudents.Clear();

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedStudentsAsync()
    {
        var message = AllStudentsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedStudents.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        // No bulk delete API on students: delete one by one based on current selection or by current page when 'all' toggled.
        if (AllStudentsSelected)
        {
            foreach (var s in StudentList.ToList())
            {
                await StudentAppService.DeleteAsync(s.StudentDto.Id);
            }
        }
        else
        {
            foreach (var s in SelectedStudents.ToList())
            {
                await StudentAppService.DeleteAsync(s.StudentDto.Id);
            }
        }

        SelectedStudents.Clear();
        AllStudentsSelected = false;

        await GetStudentsAsync();
    }
}