using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Student.Automation.Enrollments;

public interface IEnrollmentRepository : IRepository<Enrollment, Guid>
{
    Task<List<Enrollment>> GetListAsync(
        Guid? courseId = null,
        Guid? studentId = null,
        string? sort = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        Guid? courseId = null,
        Guid? studentId = null,
        CancellationToken cancellationToken = default);

    Task<Enrollment> GetWithDetailsAsync(
        Guid id,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default);

    Task<Enrollment?> FindByStudentAndCourseAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken = default);

    Task<List<Guid>> GetStudentIdsByCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default);

    Task<List<AttendanceEntry>> GetAttendanceAsync(
        Guid courseId,
        Guid studentId,
        DateOnly? dateFrom = null,
        DateOnly? dateTo = null,
        CancellationToken cancellationToken = default);

    Task<List<GradeEntry>> GetGradesByStudentIdAndCourseIdAsync(
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<List<TeacherComment>> GetTeacherCommentsByStudentIdAndCourseIdAsync(
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<EnrollmentWithNavigationProperties> GetWithNavigationAsync(
      Guid id,
      CancellationToken cancellationToken = default);

    Task<List<EnrollmentWithNavigationProperties>> GetListWithNavigationAsync(
        Guid? courseId = null,
        Guid? studentId = null,
        string? sort = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}
