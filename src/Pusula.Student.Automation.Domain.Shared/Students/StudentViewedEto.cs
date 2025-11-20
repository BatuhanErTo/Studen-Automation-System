using System;
using Volo.Abp.Domain.Entities.Events.Distributed;

namespace Pusula.Student.Automation.Students;

public class StudentViewedEto : EtoBase
{
    public Guid StudentId { get; set; }
    public Guid? ViewerId { get; set; }
    public  DateTime ViewedAt { get; set; }
}
