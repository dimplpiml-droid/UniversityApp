using Avalonia.Controls;
using UniversityApp.Services;

namespace UniversityApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Dialogs.Owner = this;
    }
}
