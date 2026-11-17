using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniversityApp.Models;

namespace UniversityApp.ViewModels;

// Рабочая область после входа: меню слева + текущая таблица справа
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
            Roles.Admin => "Администратор",
            Roles.Head => "Заведующий кафедрой",
            Roles.Teacher => "Преподаватель",
            _ => "Студент"
        };
        UserInfo = $"{user.Login} — {roleName}";

        // Права: (добавление, редактирование, удаление)
        Pages = user.Role switch
        {
            // Админ: Преподаватели д/у/р, Заведующие д/у/р
            Roles.Admin => new()
            {
                new TeachersPageViewModel(),
                new HeadsPageViewModel()
            },
            // Заведующий: Студенты д/у/р, Экзамены д/у/р, Кафедры д/у/р
            Roles.Head => new()
            {
                new StudentsPageViewModel(canEdit: true),
                new ExamsPageViewModel(canAdd: true, canEdit: true, canDelete: true, onlyGrade: false, studentFilter: null),
                new DepartmentsPageViewModel()
            },
            // Преподаватель: Студенты с, Экзамены р/с (меняет оценку), Дисциплины с
            Roles.Teacher => new()
            {
                new StudentsPageViewModel(canEdit: false),
                new ExamsPageViewModel(canAdd: false, canEdit: true, canDelete: false, onlyGrade: true, studentFilter: null),
                new DisciplinesPageViewModel()
            },
            // Студент: Экзамены с (только свои), Дисциплины с
            _ => new()
            {
                new ExamsPageViewModel(canAdd: false, canEdit: false, canDelete: false, onlyGrade: false, studentFilter: user.StudentRegNumber ?? -1),
                new DisciplinesPageViewModel()
            }
        };

        SelectedPage = Pages[0];
    }

    partial void OnSelectedPageChanged(TablePageViewModel? value) => value?.Reload();

    [RelayCommand]
    private void Logout() => _logout();
}
