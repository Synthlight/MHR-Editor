using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Common.Models.List_Wrappers;
using RE_Editor.Controls;

namespace RE_Editor.Util;

public static class RowHelper {
    public static void AddKeybinds<T>(UIElement control, IList<T> items, AutoDataGridGeneric<T> dataGrid, RSZ rsz) {
        Utils.SetupKeybind(control, new KeyGesture(Key.I, ModifierKeys.Control), () => HandleAddRow(items, dataGrid, rsz));
        Utils.SetupKeybind(control, new KeyGesture(Key.R, ModifierKeys.Control), () => HandleRemoveRow(items, dataGrid));
    }

    public static void HandleAddRow<T>(IList<T> items, AutoDataGridGeneric<T> dataGrid, RSZ rsz) {
        if (items == null || dataGrid == null) return;
        try {
            if (typeof(T).GetNameWithoutGenericArity() == typeof(GenericWrapper<>).GetNameWithoutGenericArity()) {
                // Here, `T` is `GenericWrapper<*>`.
                var    innerType          = typeof(T).GenericTypeArguments[0];
                var    genericWrapperType = typeof(GenericWrapper<>).MakeGenericType(innerType);
                object itemValue;
                if (innerType.IsValueType) {
                    itemValue = Convert.ChangeType(0, innerType);
                } else if (innerType == typeof(string)) {
                    itemValue = "";
                } else {
                    // Should never happen.
                    throw new NotImplementedException($"Unknown value for type, not sure how to instance it: {innerType}");
                }
                var newItem = (T) Activator.CreateInstance(genericWrapperType, [-1, itemValue])!;
                items.Add(newItem);
                dataGrid.ScrollIntoView(newItem);
            } else if (typeof(T).IsAssignableTo(typeof(RszObject))) {
                var createMethod = typeof(T).GetMethod("Create", BindingFlags.Public | BindingFlags.Static, [typeof(RSZ)])!;
                var newItem      = (T) createMethod.Invoke(null, [rsz])!;
                items.Add(newItem);
                dataGrid.ScrollIntoView(newItem);
            } else {
                MessageBox.Show($"Unable to determine how to instance `{typeof(T)}` to add a row.", "Error Adding Rows", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } catch (Exception e) {
            MessageBox.Show($"Error adding a new row: {e}", "Error Adding Rows", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static void HandleRemoveRow<T>(IList<T> items, AutoDataGridGeneric<T> dataGrid) {
        if (items == null || dataGrid == null) return;
        try {
            if (items.Count == 0) return;

            var selectedItems = dataGrid.SelectedItems.Cast<object>().ToList();

            foreach (var cellItem in dataGrid.SelectedCells.Select(cell => cell.Item)) {
                if (selectedItems.Contains(cellItem)) continue;
                selectedItems.Add(cellItem);
            }

            foreach (var item in from selectedItem in selectedItems
                                 from item in new List<T>(items)
                                 where (object) item == selectedItem
                                 select item) {
                items.Remove(item);
            }
        } catch (Exception e) {
            MessageBox.Show($"Error adding a new row: {e}", "Error Adding Rows", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}