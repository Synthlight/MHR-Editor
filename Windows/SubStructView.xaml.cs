using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MHR_Editor.Controls;

namespace MHR_Editor.Windows {
    public partial class SubStructView {
        public SubStructView() {
            InitializeComponent();
        }
    }

    public sealed class SubStructViewDynamic<T> : SubStructView {
        public SubStructViewDynamic(Window window, string name, ObservableCollection<T> items) {
            Title  = name;
            Owner  = window;
            Width  = window.Width;
            Height = window.Height * 0.8d;

            var dataGrid = new AutoDataGridGeneric<T> {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            };
            dataGrid.SetItems(items);

            AddChild(dataGrid);
        }
    }
}