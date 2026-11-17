using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversityApp.Models;

namespace UniversityApp.ViewModels;

// ===================== Студенты =====================
public class StudentsPageViewModel : TablePageViewModel<Student>
{
    public StudentsPageViewModel(bool canEdit) : base("Студенты", canEdit, canEdit, canEdit,
        new()
        {
            ["Рег. номер"] = s => s.RegNumber,
            ["Специальность"] = s => s.SpecialtyNumber,
            ["Фамилия"] = s => s.LastName
        })
    { }

    protected override List<Student> LoadItems()
    {
        using var db = DbConection.Create();
        return db.Set<Student>().AsNoTracking().ToList();
    }

    protected override string SearchStringOf(Student s) => $"{s.RegNumber} {s.SpecialtyNumber} {s.LastName}";

    protected override List<Field> FieldsFor(Student? s)
    {
        using var db = DbConection.Create();
        var specs = db.Set<Specialty>().OrderBy(x => x.Number).Select(x => x.Number + " — " + x.Direction).ToList();
        return new()
        {
            Fields.Text("reg", "Рег. номер", s?.RegNumber.ToString(), readOnly: s != null),
            Fields.Choice("spec", "Специальность", specs, s?.SpecialtyNumber),
            Fields.Text("name", "Фамилия", s?.LastName)
        };
    }

    protected override void SaveItem(Student? item, List<Field> f)
    {
        using var db = DbConection.Create();
        var st = item == null ? new Student { RegNumber = f.Int("reg") } : db.Set<Student>().Find(item.RegNumber)!;
        st.SpecialtyNumber = f.Str("spec");
        st.LastName = f.FullStr("name");
        if (item == null) db.Set<Student>().Add(st);
        db.SaveChanges();
    }

    protected override void RemoveItem(Student item)
    {
        using var db = DbConection.Create();
        db.Set<Student>().Where(x => x.RegNumber == item.RegNumber).ExecuteDelete();
    }
}

// ===================== Экзамены =====================
public class ExamsPageViewModel : TablePageViewModel<Exam>
{
    private readonly bool _onlyGrade;
    private readonly int? _studentFilter;

    // onlyGrade — преподаватель может менять только оценку
    // studentFilter — для студента показываем только его экзамены
    public ExamsPageViewModel(bool canAdd, bool canEdit, bool canDelete, bool onlyGrade, int? studentFilter)
        : base("Экзамены", canAdd, canEdit, canDelete,
        new()
        {
            ["Дата"] = e => e.ExamDate,
            ["Дисциплина"] = e => e.DisciplineName,
            ["Студент"] = e => e.StudentName,
            ["Преподаватель"] = e => e.TeacherName,
            ["Оценка"] = e => e.Grade
        })
    {
        _onlyGrade = onlyGrade;
        _studentFilter = studentFilter;
    }

    protected override List<Exam> LoadItems()
    {
        using var db = DbConection.Create();
        var disc = db.Set<Discipline>().ToDictionary(d => d.Code, d => d.Name);
        var studs = db.Set<Student>().ToDictionary(s => s.RegNumber, s => s.LastName);
        var emps = db.Set<Employee>().ToDictionary(e => e.TabNumber, e => e.LastName);

        IQueryable<Exam> q = db.Set<Exam>().AsNoTracking();
        if (_studentFilter != null)
            q = q.Where(e => e.StudentRegNumber == _studentFilter);

        var list = q.ToList();
        foreach (var e in list)
        {
            e.DisciplineName = disc.GetValueOrDefault(e.DisciplineCode, "");
            e.StudentName = studs.GetValueOrDefault(e.StudentRegNumber, "");
            e.TeacherName = emps.GetValueOrDefault(e.TeacherTabNumber, "");
        }
        return list;
    }

    protected override string SearchStringOf(Exam e)
        => $"{e.DateText} {e.DisciplineName} {e.StudentName} {e.TeacherName} {e.Room} {e.Grade}";

    protected override List<Field> FieldsFor(Exam? e)
    {
        using var db = DbConection.Create();
        var discs = db.Set<Discipline>().OrderBy(d => d.Code).Select(d => d.Code + " — " + d.Name).ToList();
        var studs = db.Set<Student>().OrderBy(s => s.RegNumber).Select(s => s.RegNumber + " — " + s.LastName).ToList();
        var teachers = (from t in db.Set<Teacher>()
                        join emp in db.Set<Employee>() on t.TabNumber equals emp.TabNumber
                        orderby t.TabNumber
                        select t.TabNumber + " — " + emp.LastName).ToList();
        var grades = new List<string> { "2", "3", "4", "5" };

        bool ro = _onlyGrade; // преподаватель: всё, кроме оценки, только для чтения
        return new()
        {
            Fields.Text("date", "Дата (дд.мм.гггг)", e?.DateText, ro),
            Fields.Choice("disc", "Дисциплина", discs, e?.DisciplineCode.ToString(), ro),
            Fields.Choice("stud", "Студент", studs, e?.StudentRegNumber.ToString(), ro),
            Fields.Choice("teach", "Преподаватель", teachers, e?.TeacherTabNumber.ToString(), ro),
            Fields.Text("room", "Аудитория", e?.Room, ro),
            Fields.Choice("grade", "Оценка", grades, e?.Grade.ToString())
        };
    }

    protected override void SaveItem(Exam? item, List<Field> f)
    {
        using var db = DbConection.Create();
        var ex = item == null ? new Exam() : db.Set<Exam>().Find(item.Id)!;
        ex.ExamDate = f.Date("date");
        ex.DisciplineCode = f.Int("disc");
        ex.StudentRegNumber = f.Int("stud");
        ex.TeacherTabNumber = f.Int("teach");
        ex.Room = f.Str("room");
        ex.Grade = f.Int("grade");
        if (item == null) db.Set<Exam>().Add(ex);
        db.SaveChanges();
    }

    protected override void RemoveItem(Exam item)
    {
        using var db = DbConection.Create();
        db.Set<Exam>().Where(x => x.Id == item.Id).ExecuteDelete();
    }
}

// ===================== Кафедры =====================
public class DepartmentsPageViewModel : TablePageViewModel<Department>
{
    public DepartmentsPageViewModel() : base("Кафедры", true, true, true,
        new()
        {
            ["Шифр"] = d => d.Code,
            ["Название"] = d => d.Name,
            ["Факультет"] = d => d.FacultyAbbr
        })
    { }

    protected override List<Department> LoadItems()
    {
        using var db = DbConection.Create();
        return db.Set<Department>().AsNoTracking().ToList();
    }

    protected override string SearchStringOf(Department d) => $"{d.Code} {d.Name} {d.FacultyAbbr}";

    protected override List<Field> FieldsFor(Department? d)
    {
        using var db = DbConection.Create();
        var facs = db.Set<Faculty>().OrderBy(x => x.Abbr).Select(x => x.Abbr + " — " + x.Name).ToList();
        return new()
        {
            Fields.Text("code", "Шифр", d?.Code, readOnly: d != null),
            Fields.Text("name", "Название", d?.Name),
            Fields.Choice("fac", "Факультет", facs, d?.FacultyAbbr)
        };
    }

    protected override void SaveItem(Department? item, List<Field> f)
    {
        using var db = DbConection.Create();
        var d = item == null ? new Department { Code = f.Str("code") } : db.Set<Department>().Find(item.Code)!;
        d.Name = f.FullStr("name");
        d.FacultyAbbr = f.Str("fac");
        if (item == null) db.Set<Department>().Add(d);
        db.SaveChanges();
    }

    protected override void RemoveItem(Department item)
    {
        using var db = DbConection.Create();
        db.Set<Department>().Where(x => x.Code == item.Code).ExecuteDelete();
    }
}

// ===================== Дисциплины (только просмотр) =====================
public class DisciplinesPageViewModel : TablePageViewModel<Discipline>
{
    public DisciplinesPageViewModel() : base("Дисциплины", false, false, false,
        new()
        {
            ["Код"] = d => d.Code,
            ["Название"] = d => d.Name,
            ["Объём (часы)"] = d => d.Hours,
            ["Кафедра"] = d => d.DeptCode
        })
    { }

    protected override List<Discipline> LoadItems()
    {
        using var db = DbConection.Create();
        return db.Set<Discipline>().AsNoTracking().ToList();
    }

    protected override string SearchStringOf(Discipline d) => $"{d.Code} {d.Name} {d.Hours} {d.DeptCode}";

    protected override List<Field> FieldsFor(Discipline? item) => new();
    protected override void SaveItem(Discipline? item, List<Field> fields) { }
    protected override void RemoveItem(Discipline item) { }
}

// ===================== Преподаватели (для админа) =====================
public class TeachersPageViewModel : TablePageViewModel<StaffRow>
{
    public TeachersPageViewModel() : base("Преподаватели", true, true, true,
        new()
        {
            ["Таб. №"] = r => r.TabNumber,
            ["Фамилия"] = r => r.LastName,
            ["Кафедра"] = r => r.DeptCode,
            ["Зарплата"] = r => r.Salary,
            ["Звание"] = r => r.Rank
        })
    { }

    protected override List<StaffRow> LoadItems()
    {
        using var db = DbConection.Create();
        return (from e in db.Set<Employee>()
                join t in db.Set<Teacher>() on e.TabNumber equals t.TabNumber
                where e.JobTitle == JobTitles.Teacher
                select new StaffRow
                {
                    TabNumber = e.TabNumber,
                    LastName = e.LastName,
                    DeptCode = e.DeptCode,
                    Salary = e.Salary,
                    Rank = t.AcademicRank,
                    Degree = t.AcademicDegree
                }).ToList();
    }

    protected override string SearchStringOf(StaffRow r)
        => $"{r.TabNumber} {r.LastName} {r.DeptCode} {r.Salary} {r.Rank} {r.Degree}";

    protected override List<Field> FieldsFor(StaffRow? r)
    {
        using var db = DbConection.Create();
        var depts = db.Set<Department>().OrderBy(d => d.Code).Select(d => d.Code + " — " + d.Name).ToList();
        return new()
        {
            Fields.Text("tab", "Табельный номер", r?.TabNumber.ToString(), readOnly: r != null),
            Fields.Text("name", "Фамилия", r?.LastName),
            Fields.Choice("dept", "Кафедра", depts, r?.DeptCode),
            Fields.Text("salary", "Зарплата", r?.Salary.ToString("0.##")),
            Fields.Text("rank", "Звание", r?.Rank),
            Fields.Text("degree", "Степень (необязательно)", r?.Degree)
        };
    }

    protected override void SaveItem(StaffRow? item, List<Field> f)
    {
        using var db = DbConection.Create();
        var dept = f.Str("dept");

        if (item == null)
        {
            int tab = f.Int("tab");
            // начальник нового преподавателя — заведующий его кафедры (если есть)
            int? boss = db.Set<Employee>()
                .Where(e => e.DeptCode == dept && e.JobTitle == JobTitles.Head)
                .Select(e => (int?)e.TabNumber).FirstOrDefault();

            db.Set<Employee>().Add(new Employee
            {
                TabNumber = tab, DeptCode = dept, LastName = f.FullStr("name"),
                JobTitle = JobTitles.Teacher, Salary = f.Dec("salary"), BossTabNumber = boss
            });
            db.Set<Teacher>().Add(new Teacher { TabNumber = tab, AcademicRank = f.FullStr("rank"), AcademicDegree = f.StrOrNull("degree") });
        }
        else
        {
            var emp = db.Set<Employee>().Find(item.TabNumber)!;
            emp.DeptCode = dept;
            emp.LastName = f.FullStr("name");
            emp.Salary = f.Dec("salary");
            var t = db.Set<Teacher>().Find(item.TabNumber)!;
            t.AcademicRank = f.FullStr("rank");
            t.AcademicDegree = f.StrOrNull("degree");
        }
        db.SaveChanges();
    }

    protected override void RemoveItem(StaffRow item)
    {
        using var db = DbConection.Create();
        // строки teacher и app_user удалятся каскадом
        db.Set<Employee>().Where(e => e.TabNumber == item.TabNumber).ExecuteDelete();
    }
}

// ===================== Заведующие кафедрами (для админа) =====================
public class HeadsPageViewModel : TablePageViewModel<StaffRow>
{
    public HeadsPageViewModel() : base("Заведующие", true, true, true,
        new()
        {
            ["Таб. №"] = r => r.TabNumber,
            ["Фамилия"] = r => r.LastName,
            ["Кафедра"] = r => r.DeptCode,
            ["Зарплата"] = r => r.Salary,
            ["Стаж (лет)"] = r => r.Experience
        })
    { }

    protected override List<StaffRow> LoadItems()
    {
        using var db = DbConection.Create();
        return (from e in db.Set<Employee>()
                join h in db.Set<DepartmentHead>() on e.TabNumber equals h.TabNumber
                select new StaffRow
                {
                    TabNumber = e.TabNumber,
                    LastName = e.LastName,
                    DeptCode = e.DeptCode,
                    Salary = e.Salary,
                    Experience = h.Experience
                }).ToList();
    }

    protected override string SearchStringOf(StaffRow r)
        => $"{r.TabNumber} {r.LastName} {r.DeptCode} {r.Salary} {r.Experience}";

    protected override List<Field> FieldsFor(StaffRow? r)
    {
        using var db = DbConection.Create();
        var depts = db.Set<Department>().OrderBy(d => d.Code).Select(d => d.Code + " — " + d.Name).ToList();
        return new()
        {
            Fields.Text("tab", "Табельный номер", r?.TabNumber.ToString(), readOnly: r != null),
            Fields.Text("name", "Фамилия", r?.LastName),
            Fields.Choice("dept", "Кафедра", depts, r?.DeptCode),
            Fields.Text("salary", "Зарплата", r?.Salary.ToString("0.##")),
            Fields.Text("exp", "Стаж (лет)", r?.Experience.ToString())
        };
    }

    protected override void SaveItem(StaffRow? item, List<Field> f)
    {
        using var db = DbConection.Create();
        if (item == null)
        {
            int tab = f.Int("tab");
            // заведующий — сам себе начальник (как в исходных данных)
            db.Set<Employee>().Add(new Employee
            {
                TabNumber = tab, DeptCode = f.Str("dept"), LastName = f.FullStr("name"),
                JobTitle = JobTitles.Head, Salary = f.Dec("salary"), BossTabNumber = tab
            });
            db.Set<DepartmentHead>().Add(new DepartmentHead { TabNumber = tab, Experience = f.Int("exp") });
        }
        else
        {
            var emp = db.Set<Employee>().Find(item.TabNumber)!;
            emp.DeptCode = f.Str("dept");
            emp.LastName = f.FullStr("name");
            emp.Salary = f.Dec("salary");
            db.Set<DepartmentHead>().Find(item.TabNumber)!.Experience = f.Int("exp");
        }
        db.SaveChanges();
    }

    protected override void RemoveItem(StaffRow item)
    {
        using var db = DbConection.Create();
        db.Set<Employee>().Where(e => e.TabNumber == item.TabNumber).ExecuteDelete();
    }
}
