using Pusula.Student.Automation.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Student.Automation.Teachers;

public interface ITeacherRepository : IRepository<Teacher, Guid>
{
    Task<List<Teacher>> GetListAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            EnumGender? enumGender = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            string? sort = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

    Task<List<TeacherWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            EnumGender? enumGender = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            string? sort = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

    Task<TeacherWithNavigationProperties> GetWithNavigationPropertiesByTeacherIdAsync(
        Guid teacherId,
        CancellationToken cancellationToken = default);

    Task DeleteAllAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            EnumGender? enumGender = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            EnumGender? enumGender = null,
            string? emailAddress = null,
            string? phoneNumber = null,
            Guid? departmentId = null,
            CancellationToken cancellationToken = default);
}
