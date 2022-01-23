using System.Collections.ObjectModel;
using System.Windows.Controls;
using MHR_Editor.Controls;

namespace MHR_Editor.Windows {
    public partial class SubStructView {
        public SubStructView() {
            InitializeComponent();
        }
    }

    public sealed class SubStructViewDynamic<T> : SubStructView where T : class {
        public SubStructViewDynamic(MainWindow mainWindow, string name, ObservableCollection<T> items) {
            Title  = name;
            Owner  = mainWindow;
            Width  = mainWindow.Width;
            Height = mainWindow.Height * 0.8d;

            var dataGrid = new AutoDataGridGeneric<T> {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            };
            dataGrid.SetItems(items);

            AddChild(dataGrid);
        }
    }
}