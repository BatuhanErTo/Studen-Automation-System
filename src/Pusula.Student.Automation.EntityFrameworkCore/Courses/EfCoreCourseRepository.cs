using Microsoft.EntityFrameworkCore;
using Pusula.Student.Automation.EntityFrameworkCore;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.GlobalExceptions;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Student.Automation.Courses;

public class EfCoreCourseRepository(IDbContextProvider<AutomationDbContext> dbContextProvider) : EfCoreRepository<AutomationDbContext, Course, Guid>(dbContextProvider), ICourseRepository
{
    public virtual async Task<long> GetCountAsync(string? filterText = null, string? courseName = null, int? credits = null, DateTime? startFrom = null, DateTime? endTo = null, Guid? teacherId = null, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, filterText, courseName, credits, startFrom, endTo, teacherId);
        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<Course>> GetListAsync(string? filterText = null, string? courseName = null, int? credits = null, DateTime? startFrom = null, DateTime? endTo = null, Guid? teacherId = null, string? sort = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, filterText, courseName, credits, startFrom, endTo, teacherId);
        query = query.OrderBy(!string.IsNullOrWhiteSpace(sort) ? sort : CourseConsts.GetDefaultSorting(false));
        return await query
            .Include(c => c.GradeComponents)
            .Include(c => c.CourseSessions)
            .PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
    }

    public virtual async Task<Course> GetWithDetailsAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync())
            .Where(c => c.Id == id)
            .Include(c => c.GradeComponents)
            .Include(c => c.CourseSessions)
            .AsSplitQuery(); 

        if (asNoTracking)
            query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(cancellationToken);

        return entity ?? throw new EntityNotFoundException(typeof(Course), id);
    }

    protected virtual IQueryable<Course> ApplyFilter(
           IQueryable<Course> query,
           string? filterText = null, 
           string? courseName = null, 
           int? credits = null, 
           DateTime? startFrom = null, 
           DateTime? endTo = null, 
           Guid? teacherId = null) => query
                   .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.CourseName.Contains(filterText!))
                   .WhereIf(!string.IsNullOrWhiteSpace(courseName), e => e.CourseName.Contains(courseName!))
                   .WhereIf(credits.HasValue, e => e.Credits.Equals(credits))
                   .WhereIf(startFrom.HasValue, e => e.StartFrom.Equals(startFrom!))
                   .WhereIf(endTo.HasValue, e => e.EndTo.Equals(endTo!))
                   .WhereIf(teacherId.HasValue, e => e.TeacherId.Equals(teacherId));
}
