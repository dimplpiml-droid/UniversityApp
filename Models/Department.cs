using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Department
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string FacultyAbbr { get; set; } = null!;

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual Faculty FacultyAbbrNavigation { get; set; } = null!;

    public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
}
