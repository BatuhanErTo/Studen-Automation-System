using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.Enums;
using Pusula.Student.Automation.Students;
using Pusula.Student.Automation.Teachers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Pusula.Student.Automation;

public class StudentAutomationDataSeederContributor(
    IGuidGenerator guidGenerator,
    ITeacherRepository teacherRepository,
    IStudentRepository studentRepository, 
    IDepartmentRepository departmentRepository,
    IdentityUserManager identityUserManager) : IDataSeedContributor, ITransientDependency
{
    public async Task SeedAsync(DataSeedContext context)
    {
        // 1) Departments (3 adet)
        var csDeptId = await EnsureDepartmentAsync("Computer Science");
        var mathDeptId = await EnsureDepartmentAsync("Mathematics");
        var litDeptId = await EnsureDepartmentAsync("Literature");

        // 2) Teachers (3 adet)
        await EnsureTeacherAsync(
            firstName: "Muharrem",
            lastName: "Özkan",
            gender: EnumGender.Male,
            email: "muharrem@university.edu",
            phone: "+90 555 000 1000",
            departmentId: csDeptId,
            password: "Muharrem123!"
        );

        await EnsureTeacherAsync(
            firstName: "Ahmet",
            lastName: "Satar",
            gender: EnumGender.Male,
            email: "ahmet@university.edu",
            phone: "+90 555 000 1002",
            departmentId: mathDeptId,
            password: "Ahmet123!"
        );

        await EnsureTeacherAsync(
            firstName: "İlayda",
            lastName: "Çabuk",
            gender: EnumGender.Female,
            email: "ilayda@university.edu",
            phone: "+90 555 004 0003",
            departmentId: litDeptId,
            password: "Ilayda123!"
        );

        // 3) Students (3 adet)
        // Not: EnumGradeYear değerlerini projendeki isimlerle eşleştir (ör: FirstYear/Freshman, SecondYear/Sophomore vb.)
        await EnsureStudentAsync(
            firstName: "Ahmet",
            lastName: "Ertürk",
            identityNumber: "12000000000",
            birthDate: new DateTime(2003, 5, 12),
            gradeYear: EnumGradeYear.FirstYear,     // gerekirse değiştir
            gender: EnumGender.Female,
            address: "İstanbul, Beşiktaş",
            email: "ahmet@example.com",
            phone: "+90 555 211 1111",
            departmentId: csDeptId,
            password: "Ahmet123!"
        );

        await EnsureStudentAsync(
            firstName: "Ali",
            lastName: "Kırıcı",
            identityNumber: "10000060001",
            birthDate: new DateTime(2002, 9, 20),
            gradeYear: EnumGradeYear.SecondYear,    // gerekirse değiştir
            gender: EnumGender.Male,
            address: "Ankara, Çankaya",
            email: "ali.kirici@example.com",
            phone: "+90 555 611 1112",
            departmentId: mathDeptId,
            password: "AliPassword123!"
        );

        await EnsureStudentAsync(
            firstName: "Feride",
            lastName: "Kaya",
            identityNumber: "10006000002",
            birthDate: new DateTime(2001, 1, 15),
            gradeYear: EnumGradeYear.ThirdYear,     // gerekirse değiştir
            gender: EnumGender.Female,
            address: "İzmir, Karşıyaka",
            email: "feride.kaya@example.com",
            phone: "+90 755 111 1113",
            departmentId: litDeptId,
            password: "Feride123!"
        );
    }

    // -------- Helpers --------

    async Task<Guid> EnsureDepartmentAsync(string name)
    {
        var exists = await departmentRepository.AnyAsync(x => x.DepartmentName == name);
        if (exists)
        {
            var dep = await departmentRepository.FirstOrDefaultAsync(x => x.DepartmentName == name);
            return dep!.Id;
        }

        var id = guidGenerator.Create();
        var department = new Department(id, name);
        await departmentRepository.InsertAsync(department, autoSave: true);
        return id;
    }

    async Task EnsureTeacherAsync(
        string firstName,
        string lastName,
        EnumGender gender,
        string email,
        string phone,
        Guid departmentId,
        string password)
    {
        var exists = await teacherRepository.AnyAsync(
            x => x.EmailAddress == email || (x.FirstName == firstName && x.LastName == lastName && x.DepartmentId == departmentId));

        if (exists) return;

        var userId = await CreateIdentityUser(
            firstName,
            lastName,
            email,
            email,
            phone,
            password,
            Roles.TeacherRole);

        var teacher = new Teacher(
            id: guidGenerator.Create(),
            firstName: firstName,
            lastName: lastName,
            gender: gender,
            emailAddress: email,
            phoneNumber: phone,
            departmentId: departmentId,
            identityUserId: userId
        );

        await teacherRepository.InsertAsync(teacher, autoSave: true);
    }

    async Task EnsureStudentAsync(
        string firstName,
        string lastName,
        string identityNumber,
        DateTime birthDate,
        EnumGradeYear gradeYear,
        EnumGender gender,
        string address,
        string email,
        string phone,
        string password,
        Guid departmentId)
    {
        var exists = await studentRepository.AnyAsync(
            x => x.IdentityNumber == identityNumber || x.EmailAddress == email);

        if (exists) return;

        var userId = await CreateIdentityUser(
            firstName, 
            lastName, 
            email, 
            email, 
            phone,
            password,
            Roles.StudentRole);

        var student = new StudentEntity(
            id: guidGenerator.Create(),
            firstName: firstName,
            lastName: lastName,
            identityNumber: identityNumber,
            birthDate: birthDate,
            gradeYear: gradeYear,
            gender: gender,
            address: address,
            emailAddress: email,
            phoneNumber: phone,
            departmentId: departmentId,
            identityUserId: userId
        );

        await studentRepository.InsertAsync(student, autoSave: true);
    }

    private async Task<Guid> CreateIdentityUser(
       string firstName,
       string lastName,
       string userName,
       string email,
       string phoneNumber,
       string password,
       string role)
    {
        var user = new IdentityUser(guidGenerator.Create(), userName, email)
        {
            Name = firstName,
            Surname = lastName
        };
        user.SetPhoneNumber(phoneNumber, true);
        user.SetEmailConfirmed(true);

        var createResult = await identityUserManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            throw new BusinessException("IdentityUserCreationFailed")
                .WithData("Errors", string.Join(",", createResult.Errors.Select(e => e.Description)));
        }

        await identityUserManager.AddToRoleAsync(user, role);

        return user.Id;
    }
}
