using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Student.Automation.Courses;

public interface ICourseRepository : IRepository<Course, Guid>
{
    Task<List<Course>> GetListAsync(
            string? filterText = null,
            string? courseName = null,
            int? credits = null,
            DateTime? startFrom = null,
            DateTime? endTo = null,
            Guid? teacherId = null,
            string? sort = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
            string? filterText = null,
            string? courseName = null,
            int? credits = null,
            DateTime? startFrom = null,
            DateTime? endTo = null,
            Guid? teacherId = null,
            CancellationToken cancellationToken = default);

    Task<Course> GetWithDetailsAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<List<Course>> GetCourseListExceedsEndDateAndStatusIsNotCompletedAsync();
}
