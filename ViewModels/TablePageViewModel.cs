using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UniversityApp.Models;
using UniversityApp.Services;

namespace UniversityApp.ViewModels;

// Общая логика любой страницы с таблицей: поиск, сортировка, добавление, правка, удаление.
public abstract partial class TablePageViewModel : ViewModelBase
{
    private readonly Dictionary<string, Func<object, object>> _sorts;
    private List<object> _all = new();
    private string? _loadError;

    public string Title { get; }
    public bool CanAdd { get; }
    public bool CanEdit { get; }
    public bool CanDelete { get; }
    public List<ColumnDef> Columns { get; }
    public List<string> SortOptions { get; }
    public ObservableCollection<object> Items { get; } = new();

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private string? _selectedSort;
    [ObservableProperty] private bool _descending;
    [ObservableProperty] private object? _selectedItem;
    [ObservableProperty] private string _status = "";

    protected TablePageViewModel(string title, bool canAdd, bool canEdit, bool canDelete,
        List<ColumnDef> columns, Dictionary<string, Func<object, object>> sorts)
    {
        Title = title;
        CanAdd = canAdd;
        CanEdit = canEdit;
        CanDelete = canDelete;
        Columns = columns;
        _sorts = sorts;
        SortOptions = sorts.Keys.ToList();
        SelectedSort = SortOptions.FirstOrDefault();
    }

    protected abstract IEnumerable<object> LoadAll();
    protected abstract string SearchString(object item);
    protected abstract List<Field> BuildFields(object? item);
    protected abstract void Save(object? item, List<Field> fields);
    protected abstract void DeleteItem(object item);

    public void Reload()
    {
        try
        {
            _all = LoadAll().ToList();
            _loadError = null;
        }
        catch (Exception ex)
        {
            _all = new();
            _loadError = "Ошибка: " + ErrorText.Of(ex);
        }
        Apply();
    }

    partial void OnSearchTextChanged(string value) => Apply();
    partial void OnSelectedSortChanged(string? value) => Apply();
    partial void OnDescendingChanged(bool value) => Apply();

    private void Apply()
    {
        IEnumerable<object> q = _all;

        var s = SearchText?.Trim().ToLower();
        if (!string.IsNullOrEmpty(s))
            q = q.Where(o => SearchString(o).ToLower().Contains(s));

        if (SelectedSort != null && _sorts.TryGetValue(SelectedSort, out var key))
            q = Descending ? q.OrderByDescending(key) : q.OrderBy(key);

        Items.Clear();
        foreach (var o in q) Items.Add(o);

        Status = _loadError ?? $"Записей: {Items.Count}";
    }

    [RelayCommand]
    private async Task Add()
    {
        if (!CanAdd) return;
        var fields = BuildFields(null);
        if (await EditLoop("Добавление", fields, () => Save(null, fields)))
            Reload();
    }

    [RelayCommand]
    private async Task Edit()
    {
        if (!CanEdit) return;
        var item = SelectedItem;
        if (item == null)
        {
            await Dialogs.MessageAsync("Выберите запись в таблице");
            return;
        }
        var fields = BuildFields(item);
        if (await EditLoop("Редактирование", fields, () => Save(item, fields)))
            Reload();
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (!CanDelete) return;
        var item = SelectedItem;
        if (item == null)
        {
            await Dialogs.MessageAsync("Выберите запись в таблице");
            return;
        }
        if (!await Dialogs.ConfirmAsync("Удалить выбранную запись?")) return;
        try
        {
            DeleteItem(item);
            Reload();
        }
        catch (Exception ex)
        {
            await Dialogs.MessageAsync(ErrorText.Of(ex));
        }
    }

    [RelayCommand]
    private void Refresh() => Reload();

    // Показываем окно; если сохранить не получилось — показываем ошибку и открываем окно снова с теми же данными
    private static async Task<bool> EditLoop(string title, List<Field> fields, Action save)
    {
        while (true)
        {
            if (!await Dialogs.EditAsync(title, fields)) return false;
            try
            {
                save();
                return true;
            }
            catch (Exception ex)
            {
                await Dialogs.MessageAsync(ErrorText.Of(ex));
            }
        }
    }
}

// Типизированная обёртка, чтобы в страницах не приводить object вручную
public abstract class TablePageViewModel<T> : TablePageViewModel where T : class
{
    protected TablePageViewModel(string title, bool canAdd, bool canEdit, bool canDelete,
        List<ColumnDef> columns, Dictionary<string, Func<T, object>> sorts)
        : base(title, canAdd, canEdit, canDelete, columns, ConvertSorts(sorts))
    {
    }

    private static Dictionary<string, Func<object, object>> ConvertSorts(Dictionary<string, Func<T, object>> sorts)
        => sorts.ToDictionary(kv => kv.Key, kv => (Func<object, object>)(o => kv.Value((T)o)));

    protected abstract List<T> LoadItems();
    protected abstract string SearchStringOf(T item);
    protected abstract List<Field> FieldsFor(T? item);
    protected abstract void SaveItem(T? item, List<Field> fields);
    protected abstract void RemoveItem(T item);

    protected sealed override IEnumerable<object> LoadAll() => LoadItems();
    protected sealed override string SearchString(object item) => SearchStringOf((T)item);
    protected sealed override List<Field> BuildFields(object? item) => FieldsFor((T?)item);
    protected sealed override void Save(object? item, List<Field> fields) => SaveItem((T?)item, fields);
    protected sealed override void DeleteItem(object item) => RemoveItem((T)item);
}
