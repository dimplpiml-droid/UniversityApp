using Avalonia.Controls;
using Avalonia.Interactivity;

namespace UniversityApp.Views;

public partial class MessageDialog : Window
{
    public MessageDialog() : this("", false) { }

    public MessageDialog(string text, bool confirm)
    {
        InitializeComponent();
        MessageText.Text = text;
        CancelButton.IsVisible = confirm;
        if (confirm) Title = "Подтверждение";
    }

    private void Ok_Click(object? sender, RoutedEventArgs e) => Close(true);
    private void Cancel_Click(object? sender, RoutedEventArgs e) => Close(false);
}
