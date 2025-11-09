using AutoMapper;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Courses.CourseSessionComponents;
using Pusula.Student.Automation.Courses.GradeComponents;
using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Enrollments.AttendanceEntries;
using Pusula.Student.Automation.Enrollments.GradeEntries;
using Pusula.Student.Automation.Enrollments.TeacherComments;
using Pusula.Student.Automation.Shared;
using Pusula.Student.Automation.Students;
using Pusula.Student.Automation.Teachers;
using Pusula.Student.Automation.ValueObjects;
using System;

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
        CreateMap<CourseDto, Course>();
        CreateMap<CourseSession, CourseSessionDto>();
        CreateMap<GradeComponent, GradeComponentDto>();
        CreateMap<AttendanceEntry, AttendanceEntryDto>();
        CreateMap<GradeEntry, GradeEntryDto>();
        CreateMap<TeacherComment, TeacherCommentDto>();
        CreateMap<Enrollment, EnrollmentDto>();
        CreateMap<EnrollmentWithNavigationProperties, EnrollmentWithNavigationPropertiesDto>()
           .ForMember(d => d.EnrollmentDto, opt => opt.MapFrom(s => s.Enrollment))
           .ForMember(d => d.CourseDto, opt => opt.MapFrom(s => s.Course))
           .ForMember(d => d.StudentDto, opt => opt.MapFrom(s => s.Student));
        CreateMap<Department, DepartmentDto>();
        CreateMap<Department, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DepartmentName));
        CreateMap<Teacher, TeacherDto>();
        CreateMap<TeacherDto, TeacherUpdateDto>();
        CreateMap<TeacherWithNavigationProperties, TeacherWithNavigationPropertiesDto>();
        CreateMap<TeacherWithNavigationProperties, TeacherWithNavigationPropertiesDto>()
            .ForMember(d => d.TeacherDto, opt => opt.MapFrom(s => s.Teacher))
            .ForMember(d => d.DepartmentDto, opt => opt.MapFrom(s => s.Department))
            .ForMember(d => d.CourseDtos, opt => opt.MapFrom(s => s.Courses));
        CreateMap<StudentEntity, StudentDto>();
        CreateMap<StudentWithNavigationProperties, StudentWithNavigationPropertiesDto>();
        CreateMap<StudentWithNavigationProperties, StudentWithNavigationPropertiesDto>()
            .ForMember(d => d.StudentDto, opt => opt.MapFrom(s => s.Student))
            .ForMember(d => d.DepartmentDto, opt => opt.MapFrom(s => s.Department))
            .ForMember(d => d.EnrollmentDtos, opt => opt.MapFrom(s => s.Enrollments));

    }
}
