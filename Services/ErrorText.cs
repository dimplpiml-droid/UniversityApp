using System;
using Npgsql;

namespace UniversityApp.Services;

public static class ErrorText
{
    // Превращает исключение в понятное сообщение
    public static string Of(Exception ex)
    {
        Exception last = ex;
        for (Exception? e = ex; e != null; e = e.InnerException)
        {
            last = e;
            if (e is PostgresException pg)
            {
                return pg.SqlState switch
                {
                    "23503" => "Операция невозможна: запись связана с другими таблицами (или указан несуществующий код).",
                    "23505" => "Запись с таким ключом уже существует.",
                    "23514" => "Значение не проходит проверку (например, оценка должна быть от 2 до 5).",
                    _ => pg.MessageText
                };
            }
        }
        return last.Message;
    }
}
