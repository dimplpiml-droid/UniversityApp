using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniversityApp.Models;

namespace UniversityApp.ViewModels;

public partial class ShellViewModel : ViewModelBase
{
    private readonly Action _logout;

    public string UserInfo { get; }
    public List<TablePageViewModel> Pages { get; }

    [ObservableProperty] private TablePageViewModel? _selectedPage;

    public ShellViewModel(AppUser user, Action logout)
    {
        _logout = logout;

        string roleName = user.Role switch
        {
            Roles.Head => "Заведующий кафедрой",
            Roles.Teacher => "Учитель",
            Roles.Engineer => "Инженер",
            _ => "Гость"
        };
        UserInfo = user.Role == Roles.Guest ? roleName : $"{user.Login} — {roleName}";

        // Что доступно каждой роли 
        Pages = user.Role switch
        {
            // Учитель: список экзаменов, добавление студента в экзамен, оценивание
            Roles.Teacher => new() { new ExamsPageViewModel(user.TabNumber) },
            // Заведующий: добавление и изменение кафедр
            Roles.Head => new() { new DepartmentsPageViewModel() },
            // Инженер: просмотр, добавление, изменение, удаление сотрудников
            Roles.Engineer => new() { new EmployeesPageViewModel() },
            // Гость: просмотр списка дисциплин
            _ => new() { new DisciplinesPageViewModel() }
        };

        SelectedPage = Pages[0];
    }

    partial void OnSelectedPageChanged(TablePageViewModel? value) => value?.Reload();

    [RelayCommand]
    private void Logout() => _logout();
}
