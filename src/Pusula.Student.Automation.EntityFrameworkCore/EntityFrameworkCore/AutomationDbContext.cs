using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Pusula.Student.Automation.Teachers;
using Pusula.Student.Automation.Departments;
using Pusula.Student.Automation.Courses;
using Pusula.Student.Automation.Enrollments;
using Pusula.Student.Automation.Students;
using Pusula.Student.Automation.ValueObjects;

namespace Pusula.Student.Automation.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class AutomationDbContext :
    AbpDbContext<AutomationDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    DbSet<Teacher> Teachers { get; set; } = null!;
    DbSet<StudentEntity> Students { get; set; } = null!;
    DbSet<Department> Departments { get; set; } = null!;
    DbSet<Course> Courses { get; set; } = null!;
    DbSet<Enrollment> Enrollments { get; set; } = null!;



    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public AutomationDbContext(DbContextOptions<AutomationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        if (builder.IsHostDatabase()) 
        {
            builder.Entity<Department>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "Departments", AutomationConsts.DbSchema);
                b.ConfigureByConvention();
                b.Property(x => x.DepartmentName)
                    .HasColumnName(nameof(Department.DepartmentName))
                    .IsRequired()
                    .HasMaxLength(DepartmentConsts.MaxDepartmentNameLength);

                b.HasIndex(x => x.DepartmentName).IsUnique();
            });

            builder.Entity<Teacher>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "Teachers", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.FirstName)
                    .HasColumnName(nameof(Teacher.FirstName))
                    .IsRequired()
                    .HasMaxLength(TeacherConsts.MaxFirstNameLength);

                b.Property(x => x.LastName)
                    .HasColumnName(nameof(Teacher.LastName))
                    .IsRequired()
                    .HasMaxLength(TeacherConsts.MaxLastNameLength);

                b.Property(x => x.EmailAddress)
                    .HasColumnName(nameof(Teacher.EmailAddress))
                    .IsRequired()
                    .HasMaxLength(TeacherConsts.MaxEmailAddressLength);

                b.Property(x => x.PhoneNumber)
                    .HasColumnName(nameof(Teacher.PhoneNumber))
                    .IsRequired()
                    .HasMaxLength(TeacherConsts.MaxPhoneNumberLength);

                b.Property(x => x.EnumGender)
                    .HasColumnName(nameof(Teacher.EnumGender))
                    .IsRequired();

                b.HasOne<Department>()
                 .WithMany()
                 .HasForeignKey(x => x.DepartmentId)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(x => x.IdentityUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<StudentEntity>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "Students", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.FirstName)
                    .HasColumnName(nameof(StudentEntity.FirstName))
                    .IsRequired()
                    .HasMaxLength(StudentConsts.MaxFirstNameLength);

                b.Property(x => x.LastName)
                    .HasColumnName(nameof(StudentEntity.LastName))
                    .IsRequired()
                    .HasMaxLength(StudentConsts.MaxLastNameLength);

                b.Property(x => x.IdentityNumber)
                    .HasColumnName(nameof(StudentEntity.IdentityNumber))
                    .IsRequired()
                    .HasMaxLength(StudentConsts.IdentityNumberMaxLength);

                b.Property(x => x.EmailAddress)
                    .HasColumnName(nameof(StudentEntity.EmailAddress))
                    .IsRequired()
                    .HasMaxLength(StudentConsts.MaxEmailAddressLength);

                b.Property(x => x.PhoneNumber)
                    .HasColumnName(nameof(StudentEntity.PhoneNumber))
                    .IsRequired()
                    .HasMaxLength(StudentConsts.MaxPhoneNumberLength);

                b.Property(x => x.Address)
                    .HasColumnName(nameof(StudentEntity.Address))
                    .IsRequired()
                    .HasMaxLength(StudentConsts.MaxAddressLength);

                b.Property(x => x.BirthDate)
                    .HasColumnName(nameof(StudentEntity.BirthDate))
                    .IsRequired();
                b.Property(x => x.GradeYear)
                    .HasColumnName(nameof(StudentEntity.GradeYear))
                    .IsRequired();
                b.Property(x => x.Gender)
                    .HasColumnName(nameof(StudentEntity.Gender))
                    .IsRequired();

                b.HasOne<Department>()
                 .WithMany()
                 .HasForeignKey(x => x.DepartmentId)
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<IdentityUser>()
                 .WithMany()
                 .HasForeignKey(x => x.IdentityUserId)
                 .IsRequired()
                 .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => x.IdentityNumber).IsUnique();
                b.HasIndex(x => x.EmailAddress).IsUnique();
            });

            builder.Entity<Course>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "Courses", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.CourseName)
                    .HasColumnName(nameof(Course.CourseName))
                    .IsRequired()
                    .HasMaxLength(CourseConsts.MaxCourseNameLength);
                b.Property(x => x.Credits)
                    .HasColumnName(nameof(Course.Credits))
                    .IsRequired();

                b.Property(x => x.StartFrom)
                    .HasColumnName(nameof(Course.StartFrom))
                    .IsRequired();
                b.Property(x => x.EndTo)
                    .HasColumnName(nameof(Course.EndTo))
                    .IsRequired();
                b.Property(x => x.Status)
                    .HasColumnName(nameof(Course.Status))
                    .IsRequired();

                b.HasOne<Teacher>()
                    .WithMany()
                    .HasForeignKey(c => c.TeacherId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasMany(c => c.CourseSessions)
                    .WithOne()                         
                    .HasForeignKey(s => s.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(c => c.GradeComponents)
                    .WithOne()
                    .HasForeignKey(gc => gc.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.Navigation(x => x.CourseSessions)
                    .HasField("_courseSessions")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                b.Navigation(x => x.GradeComponents)
                    .HasField("_gradeComponents")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                b.HasIndex(x => new { x.TeacherId, x.Status });
                b.HasIndex(x => x.CourseName);
            });

            builder.Entity<CourseSession>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "CourseSessions", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Day).HasColumnName(nameof(CourseSession.Day)).IsRequired();

                b.OwnsOne(x => x.Time, tr =>
                {
                    tr.Property(p => p.Start).HasColumnName(nameof(TimeRange.Start)).IsRequired();
                    tr.Property(p => p.End).HasColumnName(nameof(TimeRange.End)).IsRequired();
                });
            });

            builder.Entity<GradeComponent>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "GradeComponents", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.GradeComponentName)
                    .HasColumnName(nameof(GradeComponent.GradeComponentName))
                    .IsRequired()
                    .HasMaxLength(GradeComponentConsts.MaxGradeComponentNameLength);
                b.Property(x => x.Order)
                    .HasColumnName(nameof(GradeComponent.Order))
                    .IsRequired();
                b.Property(x => x.Weight)
                    .HasColumnName(nameof(GradeComponent.Weight))
                    .IsRequired();

                b.HasIndex(x => new { x.CourseId, x.GradeComponentName }).IsUnique();
            });

            builder.Entity<Enrollment>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "Enrollments", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.HasOne<StudentEntity>()
                    .WithMany()
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasMany(e => e.TeacherComments)
                    .WithOne()
                    .HasForeignKey(x => x.EnrollmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(e => e.GradeEntries)
                    .WithOne()
                    .HasForeignKey(x => x.EnrollmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasMany(e => e.AttendanceEntries)
                    .WithOne()
                    .HasForeignKey(x => x.EnrollmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.Navigation(x => x.TeacherComments)
                    .HasField("_teacherComments")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                b.Navigation(x => x.GradeEntries)
                    .HasField("_gradeEntries")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                b.Navigation(x => x.AttendanceEntries)
                    .HasField("_attendanceEntries")
                    .UsePropertyAccessMode(PropertyAccessMode.Field);

                b.HasIndex(x => new { x.CourseId, x.StudentId }).IsUnique();
            });

            builder.Entity<TeacherComment>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "TeacherComments", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Comment)
                    .HasColumnName(nameof(TeacherComment.Comment))
                    .IsRequired()
                    .HasMaxLength(TeacherCommentConsts.MaxCommentLength);
            });

            builder.Entity<GradeEntry>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "GradeEntries", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Score)
                    .HasColumnName(nameof(GradeEntry.Score))
                    .IsRequired();

                b.HasOne<GradeComponent>()
                    .WithMany()
                    .HasForeignKey(x => x.GradeComponentId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new { x.EnrollmentId, x.GradeComponentId }).IsUnique();
            });

            builder.Entity<AttendanceEntry>(b =>
            {
                b.ToTable(AutomationConsts.DbTablePrefix + "AttendanceEntries", AutomationConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(x => x.Date)
                    .HasColumnName(nameof(AttendanceEntry.Date))
                    .IsRequired();
                b.Property(x => x.AttendanceStatus)
                    .HasColumnName(nameof(AttendanceEntry.AttendanceStatus))
                    .IsRequired();
                b.Property(x => x.AbsentReason)
                    .HasColumnName(nameof(AttendanceEntry.AbsentReason));

                b.HasOne<CourseSession>()
                    .WithMany()
                    .HasForeignKey(x => x.CourseSessionId)
                    .OnDelete(DeleteBehavior.NoAction);

                b.HasIndex(x => new { x.EnrollmentId, x.Date, x.CourseSessionId }).IsUnique();
            });

        }   
    }
}
