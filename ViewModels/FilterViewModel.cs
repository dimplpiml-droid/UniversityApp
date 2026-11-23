using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UniversityApp.ViewModels;

public partial class FilterViewModel : ObservableObject
{
    public const string All = "Все";

    public string Label { get; }
    public Func<object, string> Key { get; }
    public ObservableCollection<string> Options { get; } = new() { All };

    [ObservableProperty] private string? _selected = All;

    public event Action? Changed;

    public FilterViewModel(string label, Func<object, string> key)
    {
        Label = label;
        Key = key;
    }

    partial void OnSelectedChanged(string? value) => Changed?.Invoke();


    public void Refresh(IEnumerable<object> items)
    {
        var keep = Selected;
        var values = items.Select(Key).Where(v => !string.IsNullOrEmpty(v)).Distinct().OrderBy(v => v).ToList();

        Options.Clear();
        Options.Add(All);
        foreach (var v in values) Options.Add(v);

        Selected = keep != null && Options.Contains(keep) ? keep : All;
    }
}
