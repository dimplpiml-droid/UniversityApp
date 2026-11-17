using System.ComponentModel.DataAnnotations.Schema;

namespace UniversityApp.Models;

// Класс Exam генерирует scaffold (он partial), здесь только поля для показа в таблице.
// Этот файл scaffold не перезаписывает.
public partial class Exam
{
    [NotMapped] public string DateText => ExamDate.ToString("dd.MM.yyyy");
    [NotMapped] public string DisciplineName { get; set; } = "";
    [NotMapped] public string StudentName { get; set; } = "";
    [NotMapped] public string TeacherName { get; set; } = "";
}
