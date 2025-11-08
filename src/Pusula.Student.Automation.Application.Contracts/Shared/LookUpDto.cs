using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Shared;

public class LookupDto<TKey>
{
    public TKey Id { get; set; } = default!;

    public string DisplayName { get; set; } = null!;
}
