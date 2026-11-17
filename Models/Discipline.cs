using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Discipline
{
    public int Code { get; set; }

    public int Hours { get; set; }

    public string Name { get; set; } = null!;

    public string DeptCode { get; set; } = null!;

    public virtual Department DeptCodeNavigation { get; set; } = null!;

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual ICollection<Specialty> SpecialtyNumbers { get; set; } = new List<Specialty>();
}
