using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Student
{
    public int RegNumber { get; set; }

    public string SpecialtyNumber { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();

    public virtual Specialty SpecialtyNumberNavigation { get; set; } = null!;
}
