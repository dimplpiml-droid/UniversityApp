using Avalonia.Controls;
using Avalonia.Data;
using UniversityApp.ViewModels;

namespace UniversityApp.Views;

public partial class TablePageView : UserControl
{
    public TablePageView()
    {
        InitializeComponent();

        // Колонки у каждой таблицы свои — создаём их из списка Columns во ViewModel
        DataContextChanged += (_, _) => BuildColumns();

        // Двойной щелчок по строке = редактирование (если разрешено)
        MainGrid.DoubleTapped += (_, _) =>
        {
            if (DataContext is TablePageViewModel vm && vm.CanEdit)
                vm.EditCommand.Execute(null);
        };
    }

    private void BuildColumns()
    {
        MainGrid.Columns.Clear();
        if (DataContext is not TablePageViewModel vm) return;

        foreach (var c in vm.Columns)
        {
            MainGrid.Columns.Add(new DataGridTextColumn
            {
                Header = c.Header,
                Binding = new Binding(c.Path),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
        }
    }
}
