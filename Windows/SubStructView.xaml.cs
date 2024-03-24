using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using RE_Editor.Common.Models;
using RE_Editor.Controls;

namespace RE_Editor.Windows {
    public partial class SubStructView {
        public SubStructView() {
            InitializeComponent();
        }
    }

    public sealed class SubStructViewDynamic<T> : SubStructView where T : RszObject {
        [CanBeNull] private readonly ObservableCollection<T> items;
        [CanBeNull] private readonly AutoDataGridGeneric<T>  dataGrid;

        [UsedImplicitly]
        public SubStructViewDynamic(Window window, string name, ObservableCollection<T> items, PropertyInfo sourceProperty) {
            this.items = items;
            Title      = name;
            Owner      = window;
            Width      = window.Width;
            Height     = window.Height * 0.8d;

            dataGrid = new() {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            };
            dataGrid.SetItems(items, sourceProperty);

            AddChild(dataGrid);

            Init();
        }

        [UsedImplicitly]
        public SubStructViewDynamic(Window window, string name, T item) {
            Title  = name;
            Owner  = window;
            Width  = window.Width;
            Height = window.Height * 0.8d;

            var structGrid = new StructGridGeneric<T> {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            };
            structGrid.SetItem(item);

            AddChild(structGrid);

            Init();
        }

        private void Init() {
            SetupKeybind(new KeyGesture(Key.I, ModifierKeys.Control), HandleAddRow);
        }

        private void HandleAddRow() {
            try {
                if (items is {Count: > 0}) {
                    var createMethod = items[0].GetType().GetMethod("Create", BindingFlags.Public | BindingFlags.Static, [typeof(RSZ)])!;
                    var newItem      = (T) createMethod.Invoke(null, [items[0].rsz])!;
                    items.Add(newItem);
                    dataGrid!.ScrollIntoView(newItem);
                }
            } catch (Exception e) {
                MessageBox.Show($"Error adding a new row: {e}", "Error Adding Rows", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetupKeybind(InputGesture gesture, Action command) {
            var changeItemValues = new RoutedCommand();
            var ib               = new InputBinding(changeItemValues, gesture);
            InputBindings.Add(ib);
            // Bind handler.
            var cb = new CommandBinding(changeItemValues);
            cb.Executed += (_, _) => command();
            CommandBindings.Add(cb);
        }
    }
}