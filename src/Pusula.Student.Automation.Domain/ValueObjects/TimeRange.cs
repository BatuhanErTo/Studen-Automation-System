using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Values;

namespace Pusula.Student.Automation.ValueObjects;

public class TimeRange : ValueObject
{
    public TimeOnly Start { get; private set; }
    public TimeOnly End { get; private set; }

    private TimeRange() { } 
    public TimeRange(TimeOnly start, TimeOnly end)
    {
        if (end <= start) throw new BusinessException("TimeRange.Invalid");
        Start = start;
        End = end;
    }

    public bool Overlaps(TimeRange other) =>
        Start < other.End && other.Start < End;

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Start;
        yield return End;
    }
}
