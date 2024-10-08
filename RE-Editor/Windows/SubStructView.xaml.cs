﻿using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using JetBrains.Annotations;
using RE_Editor.Common.Models;
using RE_Editor.Controls;
using RE_Editor.Util;

namespace RE_Editor.Windows {
    public partial class SubStructView {
        public SubStructView() {
            InitializeComponent();
        }
    }

    public sealed class SubStructViewDynamic<T> : SubStructView where T : class {
        [CanBeNull] private readonly ObservableCollection<T> items;
        [CanBeNull] private readonly AutoDataGridGeneric<T>  dataGrid;

        [UsedImplicitly]
        public SubStructViewDynamic(Window window, string name, ObservableCollection<T> items, PropertyInfo sourceProperty, RSZ rsz) {
            this.items = items;
            Title      = name;
            Owner      = window;
            Width      = window.Width;
            Height     = window.Height * 0.8d;

            dataGrid = new(rsz) {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            };
            dataGrid.SetItems(items, sourceProperty);

            AddChild(dataGrid);

            Init(rsz);
        }

        [UsedImplicitly]
        public SubStructViewDynamic(Window window, string name, T item, RSZ rsz) {
            Title  = name;
            Owner  = window;
            Width  = window.Width;
            Height = window.Height * 0.8d;

            var structGrid = MakeStructGrid((dynamic) item, rsz); // Hack so it gets called with the concrete type of the item and I don't have to fuck with `MakeGenericMethod`.
            structGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            structGrid.VerticalScrollBarVisibility   = ScrollBarVisibility.Auto;
            structGrid.SetItem(item);

            AddChild(structGrid);

            Init(rsz);
        }

        private void Init(RSZ rsz) {
            RowHelper<T>.AddKeybinds(this, [items], dataGrid, rsz);
        }

        public static StructGridGeneric<F> MakeStructGrid<F>(F item, RSZ rsz) where F : RszObject {
            var dataGrid = new StructGridGeneric<F>(rsz);
            dataGrid.SetItem(item);
            return dataGrid;
        }
    }
}