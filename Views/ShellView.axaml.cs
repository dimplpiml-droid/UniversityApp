using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using UniversityApp.ViewModels;

namespace UniversityApp.Views;

public partial class ShellView : UserControl
{
    public ShellView()
    {
        InitializeComponent();
    }

    // Двойной щелчок по строке таблицы = редактирование (если у роли есть такое право)
    private void Grid_DoubleTapped(object? sender, TappedEventArgs e)
    {
        var onRow = (e.Source as Visual)?.FindAncestorOfType<DataGridRow>() != null;
        if (onRow && DataContext is ShellViewModel { SelectedPage: { CanEdit: true } page })
            page.EditCommand.Execute(null);
    }
}
