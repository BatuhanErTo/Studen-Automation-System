using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Teachers;

public class TeacherWithNavigationProperties
{
    public Teacher Teacher { get; set; } = null!;
    public Department Department { get; set; } = null!;
    public List<Course>? Courses { get; set; }
}
