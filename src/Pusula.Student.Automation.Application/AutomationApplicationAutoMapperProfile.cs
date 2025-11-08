using AutoMapper;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Teachers;
using Pusula.Student.Automation.ValueObjects;

namespace Pusula.Student.Automation;

public class AutomationApplicationAutoMapperProfile : Profile
{
    public AutomationApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<TimeRange, TimeRangeDto>();
        CreateMap<Course, CourseDto>();
        CreateMap<CourseSession, CourseSessionDto>();
        CreateMap<GradeComponent, GradeComponentDto>();

        CreateMap<Department, DepartmentDto>();

        CreateMap<Teacher, TeacherDto>();
        CreateMap<TeacherWithNavigationProperties, TeacherWithNavigationPropertiesDto>();
    }
}
