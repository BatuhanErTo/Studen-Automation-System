using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Enrollments.GradeEntries;

public class GradeEntryDto : EntityDto<Guid>
{
    public Guid GradeComponentId { get; set; }
    public double Score { get; set; }
}
