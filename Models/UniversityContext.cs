using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UniversityApp.Models;

public partial class UniversityContext : DbContext
{
    public UniversityContext()
    {
    }

    public UniversityContext(DbContextOptions<UniversityContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentHead> DepartmentHeads { get; set; }

    public virtual DbSet<Discipline> Disciplines { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Engineer> Engineers { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=university;Username=postgres;Password=1234");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("app_user_pkey");

            entity.ToTable("app_user");

            entity.HasIndex(e => e.Login, "app_user_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.StudentRegNumber).HasColumnName("student_reg_number");
            entity.Property(e => e.TabNumber).HasColumnName("tab_number");

            entity.HasOne(d => d.StudentRegNumberNavigation).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.StudentRegNumber)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("app_user_student_reg_number_fkey");

            entity.HasOne(d => d.TabNumberNavigation).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.TabNumber)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("app_user_tab_number_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("department_pkey");

            entity.ToTable("department");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.FacultyAbbr)
                .HasMaxLength(10)
                .HasColumnName("faculty_abbr");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.FacultyAbbrNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FacultyAbbr)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("department_faculty_abbr_fkey");
        });

        modelBuilder.Entity<DepartmentHead>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("department_head_pkey");

            entity.ToTable("department_head");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.Experience).HasColumnName("experience");

            entity.HasOne(d => d.TabNumberNavigation).WithOne(p => p.DepartmentHead)
                .HasForeignKey<DepartmentHead>(d => d.TabNumber)
                .HasConstraintName("department_head_tab_number_fkey");
        });

        modelBuilder.Entity<Discipline>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("discipline_pkey");

            entity.ToTable("discipline");

            entity.Property(e => e.Code)
                .ValueGeneratedNever()
                .HasColumnName("code");
            entity.Property(e => e.DeptCode)
                .HasMaxLength(10)
                .HasColumnName("dept_code");
            entity.Property(e => e.Hours).HasColumnName("hours");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.DeptCodeNavigation).WithMany(p => p.Disciplines)
                .HasForeignKey(d => d.DeptCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("discipline_dept_code_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.BossTabNumber).HasColumnName("boss_tab_number");
            entity.Property(e => e.DeptCode)
                .HasMaxLength(10)
                .HasColumnName("dept_code");
            entity.Property(e => e.JobTitle)
                .HasMaxLength(50)
                .HasColumnName("job_title");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Salary)
                .HasPrecision(10, 2)
                .HasColumnName("salary");

            entity.HasOne(d => d.BossTabNumberNavigation).WithMany(p => p.InverseBossTabNumberNavigation)
                .HasForeignKey(d => d.BossTabNumber)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("employee_boss_tab_number_fkey");

            entity.HasOne(d => d.DeptCodeNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DeptCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_dept_code_fkey");
        });

        modelBuilder.Entity<Engineer>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("engineer_pkey");

            entity.ToTable("engineer");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.Profession)
                .HasMaxLength(50)
                .HasColumnName("profession");

            entity.HasOne(d => d.TabNumberNavigation).WithOne(p => p.Engineer)
                .HasForeignKey<Engineer>(d => d.TabNumber)
                .HasConstraintName("engineer_tab_number_fkey");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("exam_pkey");

            entity.ToTable("exam");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DisciplineCode).HasColumnName("discipline_code");
            entity.Property(e => e.ExamDate).HasColumnName("exam_date");
            entity.Property(e => e.Grade).HasColumnName("grade");
            entity.Property(e => e.Room)
                .HasMaxLength(10)
                .HasColumnName("room");
            entity.Property(e => e.StudentRegNumber).HasColumnName("student_reg_number");
            entity.Property(e => e.TeacherTabNumber).HasColumnName("teacher_tab_number");

            entity.HasOne(d => d.DisciplineCodeNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.DisciplineCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("exam_discipline_code_fkey");

            entity.HasOne(d => d.StudentRegNumberNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.StudentRegNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("exam_student_reg_number_fkey");

            entity.HasOne(d => d.TeacherTabNumberNavigation).WithMany(p => p.Exams)
                .HasForeignKey(d => d.TeacherTabNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("exam_teacher_tab_number_fkey");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Abbr).HasName("faculty_pkey");

            entity.ToTable("faculty");

            entity.Property(e => e.Abbr)
                .HasMaxLength(10)
                .HasColumnName("abbr");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Number).HasName("specialty_pkey");

            entity.ToTable("specialty");

            entity.Property(e => e.Number)
                .HasMaxLength(10)
                .HasColumnName("number");
            entity.Property(e => e.DeptCode)
                .HasMaxLength(10)
                .HasColumnName("dept_code");
            entity.Property(e => e.Direction)
                .HasMaxLength(150)
                .HasColumnName("direction");

            entity.HasOne(d => d.DeptCodeNavigation).WithMany(p => p.Specialties)
                .HasForeignKey(d => d.DeptCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("specialty_dept_code_fkey");

            entity.HasMany(d => d.DisciplineCodes).WithMany(p => p.SpecialtyNumbers)
                .UsingEntity<Dictionary<string, object>>(
                    "Application",
                    r => r.HasOne<Discipline>().WithMany()
                        .HasForeignKey("DisciplineCode")
                        .HasConstraintName("application_discipline_code_fkey"),
                    l => l.HasOne<Specialty>().WithMany()
                        .HasForeignKey("SpecialtyNumber")
                        .HasConstraintName("application_specialty_number_fkey"),
                    j =>
                    {
                        j.HasKey("SpecialtyNumber", "DisciplineCode").HasName("application_pkey");
                        j.ToTable("application");
                        j.IndexerProperty<string>("SpecialtyNumber")
                            .HasMaxLength(10)
                            .HasColumnName("specialty_number");
                        j.IndexerProperty<int>("DisciplineCode").HasColumnName("discipline_code");
                    });
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.RegNumber).HasName("student_pkey");

            entity.ToTable("student");

            entity.Property(e => e.RegNumber)
                .ValueGeneratedNever()
                .HasColumnName("reg_number");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.SpecialtyNumber)
                .HasMaxLength(10)
                .HasColumnName("specialty_number");

            entity.HasOne(d => d.SpecialtyNumberNavigation).WithMany(p => p.Students)
                .HasForeignKey(d => d.SpecialtyNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("student_specialty_number_fkey");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TabNumber).HasName("teacher_pkey");

            entity.ToTable("teacher");

            entity.Property(e => e.TabNumber)
                .ValueGeneratedNever()
                .HasColumnName("tab_number");
            entity.Property(e => e.AcademicDegree)
                .HasMaxLength(50)
                .HasColumnName("academic_degree");
            entity.Property(e => e.AcademicRank)
                .HasMaxLength(50)
                .HasColumnName("academic_rank");

            entity.HasOne(d => d.TabNumberNavigation).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.TabNumber)
                .HasConstraintName("teacher_tab_number_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
