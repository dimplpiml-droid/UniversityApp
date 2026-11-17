using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace UniversityApp.Models;

// Поле в окне добавления / редактирования
public abstract class Field
{
    public string Key { get; set; } = "";
    public string Label { get; set; } = "";
    public string? Value { get; set; } = "";
    public bool ReadOnly { get; set; }
    public bool IsEditable => !ReadOnly;
}

public class TextField : Field { }

public class ChoiceField : Field
{
    public List<string> Options { get; set; } = new();
}

public static class Fields
{
    public static TextField Text(string key, string label, string? value = "", bool readOnly = false)
        => new() { Key = key, Label = label, Value = value ?? "", ReadOnly = readOnly };

    // Value должно совпадать с одним из элементов options (подбираем по коду перед " — ")
    public static ChoiceField Choice(string key, string label, List<string> options, string? code = null, bool readOnly = false)
        => new()
        {
            Key = key, Label = label, Options = options, ReadOnly = readOnly,
            Value = code == null ? null : options.FirstOrDefault(o => o == code || o.StartsWith(code + " — "))
        };

    private static Field Get(this List<Field> f, string key) => f.First(x => x.Key == key);

    // Код = часть строки до " — " ("102 — Информатика" -> "102")
    private static string Raw(this List<Field> f, string key)
    {
        var v = (f.Get(key).Value ?? "").Trim();
        var i = v.IndexOf(" — ", StringComparison.Ordinal);
        return i >= 0 ? v[..i].Trim() : v;
    }

    public static string Str(this List<Field> f, string key)
    {
        var v = f.Raw(key);
        if (v == "") throw new Exception($"Заполните поле «{f.Get(key).Label}»");
        return v;
    }

    // Для текстовых полей, где нужно сохранить строку целиком (фамилия может содержать пробелы)
    public static string FullStr(this List<Field> f, string key)
    {
        var v = (f.Get(key).Value ?? "").Trim();
        if (v == "") throw new Exception($"Заполните поле «{f.Get(key).Label}»");
        return v;
    }

    public static string? StrOrNull(this List<Field> f, string key)
    {
        var v = (f.Get(key).Value ?? "").Trim();
        return v == "" ? null : v;
    }

    public static int Int(this List<Field> f, string key)
    {
        if (!int.TryParse(f.Str(key), out var n))
            throw new Exception($"Поле «{f.Get(key).Label}» должно быть целым числом");
        return n;
    }

    public static decimal Dec(this List<Field> f, string key)
    {
        var s = f.Str(key).Replace(',', '.');
        if (!decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var n))
            throw new Exception($"Поле «{f.Get(key).Label}» должно быть числом");
        return n;
    }

    public static DateOnly Date(this List<Field> f, string key)
    {
        if (!DateOnly.TryParseExact(f.Str(key), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
            throw new Exception($"Поле «{f.Get(key).Label}»: дата в формате дд.мм.гггг");
        return d;
    }
}

public record ColumnDef(string Header, string Path);
