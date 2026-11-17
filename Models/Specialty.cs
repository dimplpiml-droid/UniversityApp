using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Specialty
{
    public string Number { get; set; } = null!;

    public string Direction { get; set; } = null!;

    public string DeptCode { get; set; } = null!;

    public virtual Department DeptCodeNavigation { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Discipline> DisciplineCodes { get; set; } = new List<Discipline>();
}
