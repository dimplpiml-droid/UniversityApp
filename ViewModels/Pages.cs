using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UniversityApp.Models;

namespace UniversityApp.ViewModels;


public class ExamsPageViewModel : TablePageViewModel<Exam>
{
    private readonly int? _defaultTeacher;

    public ExamsPageViewModel(int? defaultTeacher)
        : base("Экзамены", canAdd: true, canEdit: true, canDelete: false,
        new()
        {
            ["Дата"] = e => e.ExamDate,
            ["Дисциплина"] = e => e.DisciplineName,
            ["Студент"] = e => e.StudentName,
            ["Преподаватель"] = e => e.TeacherName,
            ["Оценка"] = e => e.Grade
        },
        new()
        {
            Filter("Дисциплина", e => e.DisciplineName),
            Filter("Преподаватель", e => e.TeacherName),
            Filter("Оценка", e => e.Grade.ToString())
        })
    {
        _defaultTeacher = defaultTeacher;
        AddText = "Добавить студента в экзамен";
        EditText = "Оценить";
    }

    protected override List<Exam> LoadItems()
    {
        using var db = DbConection.Create();
        var disc = db.Set<Discipline>().ToDictionary(d => d.Code, d => d.Name);
        var studs = db.Set<Student>().ToDictionary(s => s.RegNumber, s => s.LastName);
        var emps = db.Set<Employee>().ToDictionary(e => e.TabNumber, e => e.LastName);

        var list = db.Set<Exam>().AsNoTracking().ToList();
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

        // При оценивании (редактировании) менять можно только оценку
        bool ro = e != null;
        return new()
        {
            Fields.Text("date", "Дата (дд.мм.гггг)", e?.DateText ?? DateTime.Today.ToString("dd.MM.yyyy"), ro),
            Fields.Choice("disc", "Дисциплина", discs, e?.DisciplineCode.ToString(), ro),
            Fields.Choice("stud", "Студент", studs, e?.StudentRegNumber.ToString(), ro),
            Fields.Choice("teach", "Преподаватель", teachers, (e?.TeacherTabNumber ?? _defaultTeacher)?.ToString(), ro),
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

    protected override void RemoveItem(Exam item) { } 
}


public class DepartmentsPageViewModel : TablePageViewModel<Department>
{
    public DepartmentsPageViewModel() : base("Кафедры", canAdd: true, canEdit: true, canDelete: false,
        new()
        {
            ["Шифр"] = d => d.Code,
            ["Название"] = d => d.Name,
            ["Факультет"] = d => d.FacultyAbbr
        },
        new()
        {
            Filter("Факультет", d => d.FacultyAbbr)
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

    protected override void RemoveItem(Department item) { } 
}

public class DisciplinesPageViewModel : TablePageViewModel<Discipline>
{
    public DisciplinesPageViewModel() : base("Дисциплины", canAdd: false, canEdit: false, canDelete: false,
        new()
        {
            ["Код"] = d => d.Code,
            ["Название"] = d => d.Name,
            ["Объём (часы)"] = d => d.Hours,
            ["Кафедра"] = d => d.DeptCode
        },
        new()
        {
            Filter("Кафедра", d => d.DeptCode)
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



public class EmployeesPageViewModel : TablePageViewModel<Employee>
{
    public EmployeesPageViewModel() : base("Сотрудники", canAdd: true, canEdit: true, canDelete: true,
        new()
        {
            ["Таб. №"] = e => e.TabNumber,
            ["Фамилия"] = e => e.LastName,
            ["Кафедра"] = e => e.DeptCode,
            ["Должность"] = e => e.JobTitle,
            ["Оклад"] = e => e.Salary
        },
        new()
        {
            Filter("Кафедра", e => e.DeptCode),
            Filter("Должность", e => e.JobTitle)
        })
    { }

    protected override List<Employee> LoadItems()
    {
        using var db = DbConection.Create();
        return db.Set<Employee>().AsNoTracking().ToList();
    }

    protected override string SearchStringOf(Employee e)
        => $"{e.TabNumber} {e.LastName} {e.DeptCode} {e.JobTitle} {e.Salary}";

    protected override List<Field> FieldsFor(Employee? e)
    {
        using var db = DbConection.Create();
        var depts = db.Set<Department>().OrderBy(d => d.Code).Select(d => d.Code + " — " + d.Name).ToList();
        var jobs = new List<string> { JobTitles.Head, JobTitles.Teacher, JobTitles.Engineer };

        string extra = "";
        if (e != null)
        {
            extra = e.JobTitle switch
            {
                JobTitles.Teacher => db.Set<Teacher>().Where(t => t.TabNumber == e.TabNumber)
                                        .Select(t => t.AcademicRank).FirstOrDefault() ?? "",
                JobTitles.Head => db.Set<DepartmentHead>().Where(h => h.TabNumber == e.TabNumber)
                                        .Select(h => (int?)h.Experience).FirstOrDefault()?.ToString() ?? "",
                JobTitles.Engineer => db.Set<Engineer>().Where(g => g.TabNumber == e.TabNumber)
                                        .Select(g => g.Profession).FirstOrDefault() ?? "",
                _ => ""
            };
        }

        return new()
        {
            Fields.Text("tab", "Табельный номер", e?.TabNumber.ToString(), readOnly: e != null),
            Fields.Text("name", "Фамилия", e?.LastName),
            Fields.Choice("dept", "Кафедра", depts, e?.DeptCode),
            Fields.Choice("job", "Должность", jobs, e?.JobTitle),
            Fields.Text("salary", "Оклад", e?.Salary.ToString("0.##")),
            Fields.Text("extra", "Звание (преподаватель) / стаж в годах (заведующий) / специальность (инженер)", extra),
            Fields.Text("boss", "Табельный номер руководителя (необязательно)", e?.BossTabNumber?.ToString())
        };
    }

    protected override void SaveItem(Employee? item, List<Field> f)
    {
        using var db = DbConection.Create();

        int tab = item?.TabNumber ?? f.Int("tab");
        string dept = f.Str("dept");
        string job = f.Str("job");
        string name = f.FullStr("name");
        decimal salary = f.Dec("salary");
        int? boss = f.IntOrNull("boss");

        string? extra = f.StrOrNull("extra");
        int experience = 0;
        if (job == JobTitles.Teacher && extra == null) throw new Exception("Для преподавателя укажите звание");
        if (job == JobTitles.Engineer && extra == null) throw new Exception("Для инженера укажите специальность");
        if (job == JobTitles.Head && !int.TryParse(extra, out experience))
            throw new Exception("Для заведующего укажите стаж (целое число лет)");

        // Начальник по умолчанию: заведующий - сам себе, остальные - заведующий своей кафедры
        boss ??= job == JobTitles.Head
            ? tab
            : db.Set<Employee>().Where(x => x.DeptCode == dept && x.JobTitle == JobTitles.Head)
                                .Select(x => (int?)x.TabNumber).FirstOrDefault();

        Employee emp;
        if (item == null)
        {
            emp = new Employee { TabNumber = tab };
            db.Set<Employee>().Add(emp);
        }
        else
        {
            emp = db.Set<Employee>().Find(tab)!;
        }
        emp.DeptCode = dept;
        emp.LastName = name;
        emp.JobTitle = job;
        emp.Salary = salary;
        emp.BossTabNumber = boss;

        // При смене должности убираем запись прежней категории
        if (item != null && item.JobTitle != job)
        {
            if (item.JobTitle == JobTitles.Teacher && db.Set<Teacher>().Find(tab) is { } oldT) db.Remove(oldT);
            if (item.JobTitle == JobTitles.Head && db.Set<DepartmentHead>().Find(tab) is { } oldH) db.Remove(oldH);
            if (item.JobTitle == JobTitles.Engineer && db.Set<Engineer>().Find(tab) is { } oldE) db.Remove(oldE);
        }

        // Запись новой (или прежней) категории: добавляем или обновляем
        if (job == JobTitles.Teacher)
        {
            var t = db.Set<Teacher>().Find(tab);
            if (t == null) db.Set<Teacher>().Add(new Teacher { TabNumber = tab, AcademicRank = extra! });
            else t.AcademicRank = extra!;
        }
        else if (job == JobTitles.Head)
        {
            var h = db.Set<DepartmentHead>().Find(tab);
            if (h == null) db.Set<DepartmentHead>().Add(new DepartmentHead { TabNumber = tab, Experience = experience });
            else h.Experience = experience;
        }
        else
        {
            var g = db.Set<Engineer>().Find(tab);
            if (g == null) db.Set<Engineer>().Add(new Engineer { TabNumber = tab, Profession = extra! });
            else g.Profession = extra!;
        }

        db.SaveChanges();
    }

    protected override void RemoveItem(Employee item)
    {
        using var db = DbConection.Create();
        // строки teacher / engineer / department_head и учётная запись удалятся каскадом
        db.Set<Employee>().Where(e => e.TabNumber == item.TabNumber).ExecuteDelete();
    }
}
