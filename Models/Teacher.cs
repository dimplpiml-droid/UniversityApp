using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Teacher
{
    public int TabNumber { get; set; }

    public string AcademicRank { get; set; } = null!;

    public string? AcademicDegree { get; set; }

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual Employee TabNumberNavigation { get; set; } = null!;
}
