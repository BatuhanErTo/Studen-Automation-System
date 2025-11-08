using Pusula.Student.Automation.Departments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Student.Automation.Teachers;

public class TeacherWithNavigationPropertiesDto
{
    public TeacherDto TeacherDto { get; set; }
    public DepartmentDto DepartmentDto {  get; set; }
}
