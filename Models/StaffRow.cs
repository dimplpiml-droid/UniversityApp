namespace UniversityApp.Models;

// Строка для страниц "Преподаватели" и "Заведующие" (employee + teacher / department_head).
// Это не таблица БД, а просто объект для DataGrid.
public class StaffRow
{
    public int TabNumber { get; set; }
    public string LastName { get; set; } = "";
    public string DeptCode { get; set; } = "";
    public decimal Salary { get; set; }
    public int Experience { get; set; }
    public string Rank { get; set; } = "";
    public string? Degree { get; set; }
}
