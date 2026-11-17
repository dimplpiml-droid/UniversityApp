using System;
using System.Collections.Generic;

namespace UniversityApp.Models;

public partial class Exam
{
    public int Id { get; set; }

    public DateOnly ExamDate { get; set; }

    public int DisciplineCode { get; set; }

    public int StudentRegNumber { get; set; }

    public int TeacherTabNumber { get; set; }

    public string Room { get; set; } = null!;

    public int Grade { get; set; }

    public virtual Discipline DisciplineCodeNavigation { get; set; } = null!;

    public virtual Student StudentRegNumberNavigation { get; set; } = null!;

    public virtual Teacher TeacherTabNumberNavigation { get; set; } = null!;
}
