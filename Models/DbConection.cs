namespace UniversityApp.Models;

// UniversityContext и все сущности (Student, Exam, ...) создаёт scaffold (см. README).
internal static class DbConection
{
    // Новый контекст на каждую операцию: если сохранение упало, "битые" записи
    // не остаются в контексте и не мешают следующей операции.
    public static UniversityContext Create() => new UniversityContext();
}
