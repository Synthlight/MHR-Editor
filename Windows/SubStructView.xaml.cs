using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using RE_Editor.Controls;

namespace RE_Editor.Windows {
    public partial class SubStructView {
        public SubStructView() {
            InitializeComponent();
        }
    }

    public sealed class SubStructViewDynamic<T> : SubStructView {
        public SubStructViewDynamic(Window window, string name, ObservableCollection<T> items, PropertyInfo sourceProperty) {
            Title  = name;
            Owner  = window;
            Width  = window.Width;
            Height = window.Height * 0.8d;

            var dataGrid = new AutoDataGridGeneric<T> {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            };
            dataGrid.SetItems(items, sourceProperty);

            AddChild(dataGrid);
        }

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
        }
    }
}