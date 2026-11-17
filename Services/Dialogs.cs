using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using UniversityApp.Models;
using UniversityApp.Views;

namespace UniversityApp.Services;

// Простой помощник для показа модальных окон из ViewModel
public static class Dialogs
{
    public static Window? Owner { get; set; }

    public static async Task<bool> EditAsync(string title, List<Field> fields)
        => await new EditDialog(title, fields).ShowDialog<bool>(Owner!);

    public static async Task MessageAsync(string text)
        => await new MessageDialog(text, false).ShowDialog<bool>(Owner!);

    public static async Task<bool> ConfirmAsync(string text)
        => await new MessageDialog(text, true).ShowDialog<bool>(Owner!);
}
