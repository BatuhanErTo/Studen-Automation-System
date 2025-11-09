using Microsoft.EntityFrameworkCore;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.EntityFrameworkCore;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.GlobalExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pusula.Student.Automation.Teachers;

public class EfCoreTeacherRepository(IDbContextProvider<AutomationDbContext> dbContextProvider) : EfCoreRepository<AutomationDbContext, Teacher, Guid>(dbContextProvider), ITeacherRepository
{
    public virtual async Task<List<Teacher>> GetListAsync(string? filterText = null, string? firstName = null, string? lastName = null, EnumGender? enumGender = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, string? sort = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, filterText, firstName, lastName, enumGender, emailAddress, phoneNumber, departmentId);
        query = query.OrderBy(!string.IsNullOrWhiteSpace(sort) ? sort : TeacherConsts.GetDefaultSorting(false));
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<List<TeacherWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? firstName = null, string? lastName = null, EnumGender? enumGender = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, string? sort = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryForNavigationProperties();
        var query = ApplyFilter(queryable, filterText, firstName, lastName, enumGender, emailAddress, phoneNumber, departmentId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<TeacherWithNavigationProperties> GetWithNavigationPropertiesByTeacherIdAsync(Guid teacherId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var teachers = await GetDbSetAsync();

        var teacher = await teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);
        StudentAutomationException.ThrowIf("Teacher is not found", "T-0001", teacher is null);

        var department = await dbContext.Set<Department>().AsNoTracking().FirstOrDefaultAsync(d => d.Id == teacher!.DepartmentId, cancellationToken);
        StudentAutomationException.ThrowIf("Department is not found", "D-0001", department is null);


        var courses = await dbContext.Set<Course>().AsNoTracking().Where(course => course.TeacherId == teacherId).ToListAsync(cancellationToken);

        return new TeacherWithNavigationProperties
        {
            Teacher = teacher!,
            Department = department!,
            Courses = courses
        };
    }
    public virtual async Task DeleteAllAsync(string? filterText = null, string? firstName = null, string? lastName = null, EnumGender? enumGender = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryForNavigationProperties();
        var query = ApplyFilter(queryable, filterText, firstName, lastName, enumGender, emailAddress, phoneNumber, departmentId);
        var teacherIds = query.Select(x => x.Teacher.Id);
        await DeleteManyAsync(teacherIds, cancellationToken: cancellationToken);
    }

    public virtual async Task<long> GetCountAsync(string? filterText = null, string? firstName = null, string? lastName = null, EnumGender? enumGender = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        var queryable = ApplyFilter(await GetQueryForNavigationProperties(), filterText, firstName, lastName, enumGender, emailAddress, phoneNumber, departmentId);
        return await queryable.LongCountAsync(cancellationToken);
    }

    protected virtual async Task<IQueryable<TeacherWithNavigationProperties>> GetQueryForNavigationProperties()
    {
        var dbContext = await GetDbContextAsync();
        var teachers = await GetDbSetAsync();
        var departments = dbContext.Set<Department>();
        var courses = dbContext.Set<Course>();

        var query = from teacher in teachers
                    join department in departments on teacher.DepartmentId equals department.Id
                    join course in courses on teacher.Id equals course.TeacherId into courseGroup
                    select new TeacherWithNavigationProperties
                    {
                        Teacher = teacher,
                        Department = department,
                        Courses = courseGroup.ToList()
                    };
        return query;
    }

    protected virtual IQueryable<Teacher> ApplyFilter(
            IQueryable<Teacher> query,
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            EnumGender? enumGender = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null) => query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FirstName.Contains(filterText!) || e.LastName.Contains(filterText!) || e.EmailAddress.Contains(filterText!) || e.PhoneNumber.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.Contains(firstName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.Contains(lastName!))
                    .WhereIf(enumGender.HasValue, e => e.EnumGender.Equals(enumGender))
                    .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress.Contains(emailAddress!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber!))
                    .WhereIf(departmentId.HasValue, e => e.DepartmentId.Equals(departmentId));

    protected virtual IQueryable<TeacherWithNavigationProperties> ApplyFilter(
            IQueryable<TeacherWithNavigationProperties> query,
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            EnumGender? enumGender = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null) => query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Teacher.FirstName.Contains(filterText!) || e.Teacher.LastName.Contains(filterText!) || e.Teacher.EmailAddress.Contains(filterText!) || e.Teacher.PhoneNumber.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.Teacher.FirstName.Contains(firstName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.Teacher.LastName.Contains(lastName!))
                    .WhereIf(enumGender.HasValue, e => e.Teacher.EnumGender.Equals(enumGender))
                    .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.Teacher.EmailAddress.Contains(emailAddress!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.Teacher.PhoneNumber.Contains(phoneNumber!))
                    .WhereIf(departmentId.HasValue, e => e.Department.Id.Equals(departmentId));
}
