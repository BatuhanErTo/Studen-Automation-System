using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Student.Automation.Students;

public interface IStudentRepository : IRepository<StudentEntity, Guid>
{
    Task<List<StudentEntity>> GetListAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDate = null,
            EnumGradeYear? gradeYear = null,
            EnumGender? gender = null,
            string? address = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            string? sort = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

    Task<List<StudentWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDate = null,
            EnumGradeYear? gradeYear = null,
            EnumGender? gender = null,
            string? address = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            string? sort = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

    Task<StudentWithNavigationProperties> GetWithNavigationPropertiesByStudentIdAsync(
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task DeleteAllAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDate = null,
            EnumGradeYear? gradeYear = null,
            EnumGender? gender = null,
            string? address = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDate = null,
            EnumGradeYear? gradeYear = null,
            EnumGender? gender = null,
            string? address = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

    // Available for enrollment (not enrolled to courseId and no session overlaps with that course)
    Task<long> GetAvailableForCourseCountAsync(
        Guid courseId,
        string? filterText = null,
        CancellationToken cancellationToken = default);

    Task<List<StudentEntity>> GetAvailableForCourseListAsync(
        Guid courseId,
        string? filterText = null,
        string? sort = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default);
}
