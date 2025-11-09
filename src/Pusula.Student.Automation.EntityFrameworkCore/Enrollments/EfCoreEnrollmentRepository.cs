using Microsoft.EntityFrameworkCore;
using Pusula.Student.Automation.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Student.Automation.Enrollments;


public class EfCoreEnrollmentRepository(
    IDbContextProvider<AutomationDbContext> dbContextProvider
) : EfCoreRepository<AutomationDbContext, Enrollment, Guid>(dbContextProvider), IEnrollmentRepository
{
    public virtual async Task<long> GetCountAsync(
        Guid? courseId = null,
        Guid? studentId = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, courseId, studentId);
        return await query.LongCountAsync(cancellationToken);
    }

    public virtual async Task<List<Enrollment>> GetListAsync(
        Guid? courseId = null,
        Guid? studentId = null,
        string? sort = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        var query = ApplyFilter(queryable, courseId, studentId);
        //query = query.OrderBy(!string.IsNullOrWhiteSpace(sort) ? sort : "CreationTime desc");

        return await query
            .Include(e => e.TeacherComments)
            .Include(e => e.GradeEntries)
            .Include(e => e.AttendanceEntries)
            .AsSplitQuery()
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<Enrollment> GetWithDetailsAsync(
        Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        var query = (await GetDbSetAsync())
            .Where(e => e.Id == id)
            .Include(e => e.TeacherComments)
            .Include(e => e.GradeEntries)
            .Include(e => e.AttendanceEntries)
            .AsSplitQuery();

        if (asNoTracking) query = query.AsNoTracking();

        var entity = await query.FirstOrDefaultAsync(cancellationToken);
        return entity ?? throw new EntityNotFoundException(typeof(Enrollment), id);
    }

    public virtual async Task<Enrollment?> FindByStudentAndCourseAsync(
        Guid studentId, Guid courseId, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        return await queryable.FirstOrDefaultAsync(
            x => x.StudentId == studentId && x.CourseId == courseId, cancellationToken);
    }

    public virtual async Task<List<Guid>> GetStudentIdsByCourseAsync(
        Guid courseId, CancellationToken cancellationToken = default)
    {
        var queryable = await GetQueryableAsync();
        return await queryable
            .Where(e => e.CourseId == courseId)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<AttendanceEntry>> GetAttendanceAsync(
        Guid courseId,
        Guid studentId,
        DateOnly? dateFrom = null,
        DateOnly? dateTo = null,
        CancellationToken cancellationToken = default)
    {
        var ctx = await GetDbContextAsync();

        var query =
            from a in ctx.Set<AttendanceEntry>()
            join e in ctx.Set<Enrollment>() on a.EnrollmentId equals e.Id
            where e.CourseId == courseId
               && e.StudentId == studentId
               && (!dateFrom.HasValue || a.Date >= dateFrom.Value)
               && (!dateTo.HasValue || a.Date <= dateTo.Value)
            select a;

        return await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<List<GradeEntry>> GetGradesByStudentIdAndCourseIdAsync(
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var ctx = await GetDbContextAsync();

        var query =
            from g in ctx.Set<GradeEntry>()
            join e in ctx.Set<Enrollment>() on g.EnrollmentId equals e.Id
            where e.CourseId == courseId
              && e.StudentId == studentId
            select g;

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public virtual async Task<List<TeacherComment>> GetTeacherCommentsByStudentIdAndCourseIdAsync(
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var ctx = await GetDbContextAsync();

        var query =
            from t in ctx.Set<TeacherComment>()
            join e in ctx.Set<Enrollment>() on t.EnrollmentId equals e.Id
            where e.CourseId == courseId
              && e.StudentId == studentId
            select t;

        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    protected virtual IQueryable<Enrollment> ApplyFilter(
        IQueryable<Enrollment> query,
        Guid? courseId = null,
        Guid? studentId = null)
        => query
            .WhereIf(courseId.HasValue, e => e.CourseId == courseId)
            .WhereIf(studentId.HasValue, e => e.StudentId == studentId);
}
