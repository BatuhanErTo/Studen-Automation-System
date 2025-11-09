using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.Enrollments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Students;

public class StudentWithNavigationPropertiesDto
{
    public StudentDto StudentDto { get; set; }
    public DepartmentDto DepartmentDto { get; set; }
    public List<EnrollmentDto>? EnrollmentDtos { get; set; }
}
