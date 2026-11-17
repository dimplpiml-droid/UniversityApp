using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using UniversityApp.Models;

namespace UniversityApp.Views;

public partial class EditDialog : Window
{
    public EditDialog() : this("", new()) { }

    public EditDialog(string title, List<Field> fields)
    {
        InitializeComponent();
        Title = title;
        FieldsList.ItemsSource = fields;
    }

    private void Ok_Click(object? sender, RoutedEventArgs e) => Close(true);
    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close(false);
}
