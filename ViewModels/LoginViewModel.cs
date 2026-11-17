using System;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniversityApp.Models;
using UniversityApp.Services;

namespace UniversityApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly Action<AppUser> _onSuccess;

    [ObservableProperty] private string _loginText = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _error = "";

    public LoginViewModel(Action<AppUser> onSuccess)
    {
        _onSuccess = onSuccess;
    }

    [RelayCommand]
    private void SignIn()
    {
        Error = "";
        try
        {
            using var db = DbConection.Create();
            var login = LoginText.Trim();
            var user = db.Set<AppUser>().FirstOrDefault(u => u.Login == login && u.Password == Password);
            if (user == null)
            {
                Error = "Неверный логин или пароль";
                return;
            }
            _onSuccess(user);
        }
        catch (Exception ex)
        {
            Error = "Баклажан: " + ErrorText.Of(ex);
        }
    }
}
