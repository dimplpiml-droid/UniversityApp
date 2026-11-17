using CommunityToolkit.Mvvm.ComponentModel;

namespace UniversityApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ViewModelBase? _current;

    public MainWindowViewModel()
    {
        ShowLogin();
    }

    private void ShowLogin()
    {
        Current = new LoginViewModel(user => Current = new ShellViewModel(user, ShowLogin));
    }
}
