using Pusula.Student.Automation.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Students;
public class StudentWithNavigationProperties
{
    public Student Student { get; set; } = null!;
    public Department Department { get; set; } = null!;
}
