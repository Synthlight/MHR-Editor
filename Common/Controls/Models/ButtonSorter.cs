﻿using System.ComponentModel;
using System.Reflection;

#pragma warning disable CS8600
#pragma warning disable CS8618

namespace RE_Editor.Common.Controls.Models;

public class ButtonSorter : ICustomSorterWithPropertyName {
    public ListSortDirection SortDirection { get; set; } = ListSortDirection.Ascending;
    public string            PropertyName  { get; set; }

    public int Compare(object? x, object? y) {
        if (Global.showIdBeforeName) {
            dynamic propXValue = x?.GetType().GetProperty(PropertyName.Replace("_button", ""), BindingFlags.Public | BindingFlags.Instance)?.GetValue(x);
            dynamic propYValue = y?.GetType().GetProperty(PropertyName.Replace("_button", ""), BindingFlags.Public | BindingFlags.Instance)?.GetValue(y);

            if (propXValue == null || propYValue == null) return 0;

            return propXValue?.CompareTo(propYValue) * (SortDirection == ListSortDirection.Ascending ? 1 : -1);
        } else {
            var propXValue = (string) x?.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(x);
            var propYValue = (string) y?.GetType().GetProperty(PropertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(y);

            if (propXValue == null || propYValue == null) return 0;

            return string.Compare(propXValue, propYValue, StringComparison.Ordinal) * (SortDirection == ListSortDirection.Ascending ? 1 : -1);
        }
    }
}