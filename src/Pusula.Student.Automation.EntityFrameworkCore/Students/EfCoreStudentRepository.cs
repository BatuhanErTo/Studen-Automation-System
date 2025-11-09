using Microsoft.EntityFrameworkCore;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.EntityFrameworkCore;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.GlobalExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Student.Automation.Students;

public class EfCoreStudentRepository(IDbContextProvider<AutomationDbContext> dbContextProvider) : EfCoreRepository<AutomationDbContext, StudentEntity, Guid>(dbContextProvider), IStudentRepository
{
    public async Task DeleteAllAsync(string? filterText = null, string? firstName = null, string? lastName = null, string? identityNumber = null, DateTime? birthDate = null, EnumGradeYear? gradeYear = null, EnumGender? gender = null, string? address = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryForNavigationProperties();
        var query = ApplyFilter(queryable, filterText, firstName, lastName, identityNumber, birthDate, gradeYear, gender, address, emailAddress, phoneNumber, departmentId);
        var studentIds = query.Select(x => x.Student.Id);
        await DeleteManyAsync(studentIds, cancellationToken: cancellationToken);
    }

    public async Task<long> GetCountAsync(string? filterText = null, string? firstName = null, string? lastName = null, string? identityNumber = null, DateTime? birthDate = null, EnumGradeYear? gradeYear = null, EnumGender? gender = null, string? address = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, CancellationToken cancellationToken = default)
    {
        var queryable = ApplyFilter(await GetDbSetAsync(), filterText, firstName, lastName, identityNumber, birthDate, gradeYear, gender, address, emailAddress, phoneNumber, departmentId);
        return await queryable.LongCountAsync(cancellationToken);
    }

    public async Task<List<StudentEntity>> GetListAsync(string? filterText = null, string? firstName = null, string? lastName = null, string? identityNumber = null, DateTime? birthDate = null, EnumGradeYear? gradeYear = null, EnumGender? gender = null, string? address = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, string? sort = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, filterText, firstName, lastName, identityNumber, birthDate, gradeYear, gender, address, emailAddress, phoneNumber, departmentId);
        query = query.OrderBy(!string.IsNullOrWhiteSpace(sort) ? sort : StudentConsts.GetDefaultSorting(false));
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public async Task<List<StudentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, string? firstName = null, string? lastName = null, string? identityNumber = null, DateTime? birthDate = null, EnumGradeYear? gradeYear = null, EnumGender? gender = null, string? address = null, string? emailAddress = null, string? phoneNumber = null, Guid? departmentId = null, string? sort = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryForNavigationProperties();
        var query = ApplyFilter(queryable, filterText, firstName, lastName, identityNumber, birthDate, gradeYear, gender, address, emailAddress, phoneNumber, departmentId);
        return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public async Task<StudentWithNavigationProperties> GetWithNavigationPropertiesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var students = await GetDbSetAsync();

        var student = await students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
        StudentAutomationException.ThrowIf("Student is not found", "S-0001", student is null);

        var department = await dbContext.Set<Department>().AsNoTracking().FirstOrDefaultAsync(d => d.Id == student!.DepartmentId, cancellationToken);
        StudentAutomationException.ThrowIf("Department is not found", "D-0001", department is null);


        var enrollments = await dbContext.Set<Enrollment>().AsNoTracking().Where(enrollment => enrollment.StudentId == studentId).ToListAsync(cancellationToken);

        return new StudentWithNavigationProperties
        {
            Student = student!,
            Department = department!,
            Enrollments = enrollments
        };
    }

    protected virtual async Task<IQueryable<StudentWithNavigationProperties>> GetQueryForNavigationProperties()
    {
        var dbContext = await GetDbContextAsync();
        var students = await GetDbSetAsync();
        var departments = dbContext.Set<Department>();
        var enrollments = dbContext.Set<Enrollment>();

        var query = from student in students
                    join department in departments on student.DepartmentId equals department.Id
                    join enrollment in enrollments on student.Id equals enrollment.StudentId into enrollmentGroup
                    select new StudentWithNavigationProperties
                    {
                        Student = student,
                        Department = department,
                        Enrollments = enrollmentGroup.ToList()
                    };
        return query;
    }

    protected virtual IQueryable<StudentEntity> ApplyFilter(
            IQueryable<StudentEntity> query,
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDate = null,
            EnumGradeYear? gradeYear = null,
            EnumGender? enumGender = null,
            string? address = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null) => query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FirstName.Contains(filterText!) || e.LastName.Contains(filterText!) || e.IdentityNumber.Contains(filterText!) || e.Address.Contains(filterText!) || e.EmailAddress.Contains(filterText!) || e.PhoneNumber.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName.Contains(firstName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName.Contains(lastName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.IdentityNumber.Contains(identityNumber!))
                    .WhereIf(birthDate.HasValue, e => e.BirthDate.Equals(birthDate!))
                    .WhereIf(gradeYear.HasValue, e => e.GradeYear.Equals(gradeYear!))
                    .WhereIf(enumGender.HasValue, e => e.Gender.Equals(enumGender))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.Address.Contains(address!))
                    .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.EmailAddress.Contains(emailAddress!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.PhoneNumber.Contains(phoneNumber!))
                    .WhereIf(departmentId.HasValue, e => e.DepartmentId.Equals(departmentId));

    protected virtual IQueryable<StudentWithNavigationProperties> ApplyFilter(
            IQueryable<StudentWithNavigationProperties> query,
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDate = null,
            EnumGradeYear? gradeYear = null,
            EnumGender? enumGender = null,
            string? address = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null) => query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Student.FirstName.Contains(filterText!) || e.Student.LastName.Contains(filterText!) || e.Student.IdentityNumber.Contains(filterText!) || e.Student.Address.Contains(filterText!) || e.Student.EmailAddress.Contains(filterText!) || e.Student.PhoneNumber.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.Student.FirstName.Contains(firstName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.Student.LastName.Contains(lastName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.Student.IdentityNumber.Contains(identityNumber!))
                    .WhereIf(birthDate.HasValue, e => e.Student.BirthDate.Equals(birthDate!))
                    .WhereIf(gradeYear.HasValue, e => e.Student.GradeYear.Equals(gradeYear!))
                    .WhereIf(enumGender.HasValue, e => e.Student.Gender.Equals(enumGender))
                    .WhereIf(!string.IsNullOrWhiteSpace(address), e => e.Student.Address.Contains(address!))
                    .WhereIf(!string.IsNullOrWhiteSpace(emailAddress), e => e.Student.EmailAddress.Contains(emailAddress!))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneNumber), e => e.Student.PhoneNumber.Contains(phoneNumber!))
                    .WhereIf(departmentId.HasValue, e => e.Department.Id.Equals(departmentId));
}
