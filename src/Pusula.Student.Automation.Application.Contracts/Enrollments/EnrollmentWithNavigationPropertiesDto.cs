using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Students;

namespace Pusula.Student.Automation.Enrollments;

public class EnrollmentWithNavigationPropertiesDto
{
    public EnrollmentDto EnrollmentDto { get; set; } = null!;
    public CourseDto CourseDto { get; set; } = null!;
    public StudentDto StudentDto { get; set; } = null!;
}