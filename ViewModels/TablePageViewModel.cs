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

public abstract partial class TablePageViewModel : ViewModelBase
{
    private readonly Dictionary<string, Func<object, object>> _sorts;
    private List<object> _all = new();
    private string? _loadError;
    private bool _refreshingFilters;

    public string Title { get; }
    public bool CanAdd { get; }
    public bool CanEdit { get; }
    public bool CanDelete { get; }
    public List<string> SortOptions { get; }
    public List<FilterViewModel> Filters { get; }

    public string AddText { get; protected set; } = "Добавить";
    public string EditText { get; protected set; } = "Изменить";
    public ObservableCollection<object> Items { get; } = new();

    [ObservableProperty] private string _searchText = "";
    [ObservableProperty] private string? _selectedSort;
    [ObservableProperty] private bool _descending;
    [ObservableProperty] private object? _selectedItem;
    [ObservableProperty] private string _status = "";

    protected TablePageViewModel(string title, bool canAdd, bool canEdit, bool canDelete,
        Dictionary<string, Func<object, object>> sorts, List<FilterViewModel> filters)
    {
        Title = title;
        CanAdd = canAdd;
        CanEdit = canEdit;
        CanDelete = canDelete;
        _sorts = sorts;
        SortOptions = sorts.Keys.ToList();
        Filters = filters;
        foreach (var f in Filters)
            f.Changed += () => { if (!_refreshingFilters) Apply(); };
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

            // варианты в списках фильтров берутся из загруженных данных
            _refreshingFilters = true;
            foreach (var f in Filters) f.Refresh(_all);
            _refreshingFilters = false;
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

        // Фильтры и поиск работают одновременно: запись должна пройти все условия
        foreach (var f in Filters)
        {
            var value = f.Selected;
            if (!string.IsNullOrEmpty(value) && value != FilterViewModel.All)
                q = q.Where(o => f.Key(o) == value);
        }

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

    [RelayCommand]
    private void ClearFilters()
    {
        SearchText = "";
        foreach (var f in Filters) f.Selected = FilterViewModel.All;
    }

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

public abstract class TablePageViewModel<T> : TablePageViewModel where T : class
{
    protected TablePageViewModel(string title, bool canAdd, bool canEdit, bool canDelete,
        Dictionary<string, Func<T, object>> sorts, List<FilterViewModel> filters)
        : base(title, canAdd, canEdit, canDelete, ConvertSorts(sorts), filters)
    {
    }

    protected static FilterViewModel Filter(string label, Func<T, string> key)
        => new(label, o => key((T)o));

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
