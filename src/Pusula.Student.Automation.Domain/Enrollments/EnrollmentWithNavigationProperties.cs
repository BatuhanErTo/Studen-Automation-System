using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Students;

namespace Pusula.Student.Automation.Enrollments;

public class EnrollmentWithNavigationProperties
{
    public Enrollment Enrollment { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public StudentEntity Student { get; set; } = null!;
}