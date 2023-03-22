using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Controls.Models;
using RE_Editor.Windows;

namespace RE_Editor.Controls;

public interface IStructGrid {
    void SetItem(object item);
    void Refresh();
}

public interface IStructGrid<T> : IStructGrid {
    T    Item { get; }
    void SetItem(T item);
}

public abstract partial class StructGrid : IStructGrid {
    protected StructGrid() {
        InitializeComponent();
    }

    public abstract void SetItem(object item);
    public abstract void Refresh();
}

public class StructGridGeneric<T> : StructGrid, IStructGrid<T> {
    private T item;
    public T Item {
        get => item;
        set {
            ClearContents();
            item = value;
            SetupRows();
        }
    }

    // ReSharper disable once ParameterHidesMember
    public void SetItem(T item) {
        Item = item;
    }

    // ReSharper disable once ParameterHidesMember
    public override void SetItem(object item) {
        Item = (T) item;
    }

    private void ClearContents() {
        if (Item == null) return;

        grid.Children.Clear();
        grid.RowDefinitions.Clear();
    }

    private void SetupRows() {
        if (Item == null) return;

        var properties = typeof(T).GetProperties();
        var rows       = new List<Row>(properties.Length);

        foreach (var propertyInfo in properties) {
            var propertyName = propertyInfo.Name;
            if (properties.Any(prop => prop.Name == $"{propertyName}_button")) continue;
            if (propertyName == "Index") continue;

            var displayName    = ((DisplayNameAttribute) propertyInfo.GetCustomAttribute(typeof(DisplayNameAttribute), true))?.DisplayName;
            var sortOrder      = ((SortOrderAttribute) propertyInfo.GetCustomAttribute(typeof(SortOrderAttribute), true))?.sortOrder ?? 0;
            var isList         = (IsListAttribute) propertyInfo.GetCustomAttribute(typeof(IsListAttribute), true) != null;
            var showAsHex      = (ShowAsHexAttribute) propertyInfo.GetCustomAttribute(typeof(ShowAsHexAttribute), true) != null;
            var genericTypeDef = propertyInfo.PropertyType.IsGenericType ? propertyInfo.PropertyType.GetGenericTypeDefinition() : null;
            //var genericParamType    = propertyInfo.PropertyType.IsGenericType ? propertyInfo.PropertyType.GenericTypeArguments[0] : null;
            //var genericParamTypeDef = genericParamType?.IsGenericType == true ? genericParamType.GetGenericTypeDefinition() : null;

            if (displayName is "") continue;
            displayName ??= propertyName;

            var row = new Row {sortOrder = sortOrder};

            var headerInfo = new HeaderInfo(displayName, propertyName);
            var header     = new TextBlock {Padding = new(4)};

            header.SetBinding(TextBlock.TextProperty, new Binding(nameof(HeaderInfo.OriginalText)) {Source = headerInfo});
            row.name = header;

            // TODO: Handle DataSourceWrapper types.

            if (propertyInfo.PropertyType.IsEnum) {
                var comboBox = new ComboBox();
                comboBox.SetBinding(ItemsControl.ItemsSourceProperty, new Binding {Source            = Enum.GetValues(propertyInfo.PropertyType)});
                comboBox.SetBinding(Selector.SelectedItemProperty, new Binding(propertyName) {Source = Item, ValidatesOnExceptions = true});

                row.value = comboBox;
            } else if (propertyInfo.PropertyType == typeof(bool)) {
                var checkBox = new CheckBox();
                checkBox.SetBinding(ToggleButton.IsCheckedProperty, new Binding(propertyName) {Source = Item, ValidatesOnExceptions = true});

                row.value = checkBox;
            } else if (isList || genericTypeDef?.Is(typeof(ObservableCollection<>)) == true) {
                var button = new Button {Content = "Open"};
                button.Click += (_, _) => OpenGrid(propertyInfo, displayName, isList);

                row.value = button;
            } else {
                var textBox = new TextBox();

                var binding = new Binding(propertyName) {Source = Item, ValidatesOnExceptions = true};
                if (propertyInfo.PropertyType == typeof(DateTime)) {
                    binding.StringFormat = "{0:yyyy-MM-dd}";
                } else if (showAsHex) {
                    binding.StringFormat = "0x{0:X}";
                }
                if (!propertyInfo.CanWrite) {
                    binding.Mode       = BindingMode.OneWay;
                    textBox.IsReadOnly = true;
                }

                textBox.SetBinding(TextBox.TextProperty, binding);

                row.value = textBox;
            }

            rows.Add(row);
        }

        var rowIndex     = 0;
        var shadeThisRow = false;

        foreach (var row in rows.OrderBy(row => row.sortOrder)) {
            grid.RowDefinitions.Add(new() {Height = GridLength.Auto});

            Grid.SetRow(row.name, rowIndex);
            Grid.SetColumn(row.name, 0);
            grid.Children.Add(row.name);
            AddBorder(rowIndex, 0);

            Grid.SetRow(row.value, rowIndex);
            Grid.SetColumn(row.value, 1);
            grid.Children.Add(row.value);
            AddBorder(rowIndex, 1);

            if (shadeThisRow) {
                row.name.Background = AutoDataGrid.ALT_ROW_BRUSH;
                row.value.SetValue(BackgroundProperty, AutoDataGrid.ALT_ROW_BRUSH);
            }
            shadeThisRow = !shadeThisRow;

            rowIndex++;
        }
    }

    private void OpenGrid(PropertyInfo propertyInfo, string displayName, bool isList) {
        var list     = propertyInfo.GetGetMethod()?.Invoke(Item, null);
        var listType = list?.GetType().GenericTypeArguments[0];
        var viewType = typeof(SubStructViewDynamic<>).MakeGenericType(listType ?? throw new InvalidOperationException());
        var subStructView = isList switch {
            true => (SubStructView) Activator.CreateInstance(viewType, Window.GetWindow(this), displayName, list, propertyInfo),
            false => (SubStructView) Activator.CreateInstance(viewType, Window.GetWindow(this), displayName, ((dynamic) list)[0])
        };
        subStructView?.ShowDialog();
    }

    private void AddBorder(int row, int col) {
        const float borderWidth = 0.5f;
        var border = new Border {
            BorderThickness = new(borderWidth, row == 0 ? borderWidth : 0, col == 1 ? borderWidth : 0, borderWidth),
            BorderBrush     = Brushes.Black
        };
        Grid.SetRow(border, row);
        Grid.SetColumn(border, col);
        grid.Children.Add(border);
    }

    public override void Refresh() {
    }

    private class Row {
        public int              sortOrder;
        public TextBlock        name;
        public FrameworkElement value;
    }
}