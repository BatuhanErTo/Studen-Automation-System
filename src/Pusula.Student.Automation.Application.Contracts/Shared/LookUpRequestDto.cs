using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Student.Automation.Shared;
public class LookupRequestDto : PagedResultRequestDto
{
    public string? Filter { get; set; }

    public LookupRequestDto()
    {
        MaxResultCount = MaxMaxResultCount;
    }
}
