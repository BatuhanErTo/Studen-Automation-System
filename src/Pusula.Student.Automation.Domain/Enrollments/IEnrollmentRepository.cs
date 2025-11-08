using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Student.Automation.Enrollments;

public interface IEnrollmentRepository : IRepository<Enrollment, Guid>
{
}
