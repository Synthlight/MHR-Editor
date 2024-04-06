using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using RE_Editor.Controls;
using RE_Editor.Util;

namespace RE_Editor.Windows {
    public partial class SubStructView {
        public SubStructView() {
            InitializeComponent();
        }
    }

    public sealed class SubStructViewDynamic<T> : SubStructView {
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
            RowHelper.AddKeybinds(this, items, dataGrid);
        }
    }
}