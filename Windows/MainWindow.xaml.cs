using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Controls;
using MHR_Editor.Util;
using Microsoft.Win32;

namespace MHR_Editor.Windows;

public partial class MainWindow {
#if DEBUG
    private const bool ENABLE_CHEAT_BUTTONS = true;
#else
    private const bool ENABLE_CHEAT_BUTTONS = false;
    public const  bool SHOW_RAW_BYTES = false;
#endif
    private const string TITLE = "MHR Editor";

    [CanBeNull] private CancellationTokenSource savedTimer;
    [CanBeNull] private ReDataFile              file;
    public              string                  targetFile { get; private set; }
    public readonly     string                  filter = $"MHR Data Files|{string.Join(";", Global.FILE_TYPES)}";

    public Global.LangIndex Locale {
        get => Global.locale;
        set {
            Global.locale = value;
            if (file?.rsz.objectData == null) return;
            foreach (var grid in GetAllDataGrids()) {
                grid.RefreshHeaderText();
            }
            foreach (var item in file!.rsz.objectData) {
                if (item is OnPropertyChangedBase io) {
                    foreach (var propertyInfo in io.GetType().GetProperties()) {
                        if (propertyInfo.Name == "Name"
                            || propertyInfo.Name.EndsWith("_button")) {
                            io.OnPropertyChanged(propertyInfo.Name);
                        }
                    }
                }
            }
        }
    }

    public bool ShowIdBeforeName {
        get => Global.showIdBeforeName;
        set {
            Global.showIdBeforeName = value;
            if (file?.rsz.objectData == null) return;
            foreach (var item in file.rsz.objectData) {
                if (item is OnPropertyChangedBase io) {
                    foreach (var propertyInfo in io.GetType().GetProperties()) {
                        if (propertyInfo.Name.EndsWith("_button")) {
                            io.OnPropertyChanged(propertyInfo.Name);
                        }
                    }
                }
            }
        }
    }

    public static bool SingleClickToEditMode { get; set; } = true;

    public MainWindow() {
        var args = Environment.GetCommandLineArgs();

        InitializeComponent();

        Title = TITLE;

        Width  = SystemParameters.MaximizedPrimaryScreenWidth * 0.8;
        Height = SystemParameters.MaximizedPrimaryScreenHeight * 0.5;

        cbx_localization.ItemsSource = Global.LANGUAGE_NAME_LOOKUP;

        SetupKeybind(new KeyGesture(Key.S, ModifierKeys.Control), (_,                      _) => Save());
        SetupKeybind(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift), (_, _) => Save(true));

        btn_test.Visibility = Debugger.IsAttached ? Visibility.Visible : Visibility.Collapsed;

        UpdateCheck.Run(this);

        TryLoad(args);
    }

    private async void TryLoad(IReadOnlyCollection<string> args) {
        if (args.Count >= 2) {
            var filePath = args.Last();
            if (filePath.StartsWith("-")) return;

            // Tiny delay so the UI is visible to the user before we load.
            await Task.Delay(10);
            Load(filePath);
        }
    }

    private void Load(string filePath = null) {
        try {
            var target = filePath ?? GetOpenTarget();
            if (string.IsNullOrEmpty(target)) return;

            targetFile = target;
            Title      = Path.GetFileName(targetFile);

            sub_grids.Children.Clear();
            sub_grids.UpdateLayout();

            ClearDataGrids(main_grid);
            ClearDataGrids(sub_grids);

            GC.Collect();

            file = ReDataFile.Read(targetFile!);

            var rszObjectData = file?.rsz.objectData;
            if (rszObjectData == null || rszObjectData.Count == 0) throw new("Error loading data; rszObjectData is null/empty.\n\nPlease report the path/name of the file you are trying to load.");
            var type          = rszObjectData[0].GetType();
            var getListOfType = typeof(Enumerable).GetMethod(nameof(Enumerable.OfType), BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)?.MakeGenericMethod(type);
            var items         = getListOfType?.Invoke(null, new object[] {rszObjectData}) ?? throw new("rsz.objectData.OfType failure.");
            var dataGrid      = MakeDataGrid((dynamic) items);
            AddMainDataGrid(dataGrid);
        } catch (Exception e) when (!Debugger.IsAttached) {
            ShowError(e, "Load Error");
        }
    }

    private string GetOpenTarget() {
        var ofdResult = new OpenFileDialog {
            Filter           = filter,
            Multiselect      = false,
            InitialDirectory = targetFile == null ? string.Empty : Path.GetDirectoryName(targetFile) ?? string.Empty
        };
        ofdResult.ShowDialog();

        return ofdResult.FileName;
    }

    private async void Save(bool saveAs = false) {
        if (file == null || targetFile == null) return;

        try {
            if (saveAs) {
                var target = GetSaveTarget();
                if (string.IsNullOrEmpty(target)) return;
                targetFile = target;
                Title      = Path.GetFileName(targetFile);
                file!.Write(targetFile!);
            } else {
                file!.Write(targetFile);
            }

            await ShowChangesSaved(true);
        } catch (Exception e) when (!Debugger.IsAttached) {
            ShowError(e, "Save Error");
        }
    }

    private string GetSaveTarget() {
        var sfdResult = new SaveFileDialog {
            FileName         = $"{Path.GetFileName(targetFile)}",
            InitialDirectory = targetFile == null ? string.Empty : Path.GetDirectoryName(targetFile) ?? string.Empty,
            AddExtension     = false
        };
        return sfdResult.ShowDialog() == true ? sfdResult.FileName : null;
    }

    private async Task ShowChangesSaved(bool changesSaved) {
        savedTimer?.Cancel();
        savedTimer                = new();
        lbl_saved.Visibility      = changesSaved.VisibleIfTrue();
        lbl_no_changes.Visibility = changesSaved ? Visibility.Collapsed : Visibility.Visible;
        try {
            await Task.Delay(3000, savedTimer.Token);
            lbl_saved.Visibility = lbl_no_changes.Visibility = Visibility.Hidden;
        } catch (TaskCanceledException) {
        }
    }

    public static AutoDataGridGeneric<T> MakeDataGrid<T>(IEnumerable<T> itemSource) {
        var dataGrid = new AutoDataGridGeneric<T>();
        dataGrid.SetItems(itemSource is ObservableCollection<T> source ? source : new(itemSource));
        return dataGrid;
    }

    public void AddMainDataGrid(AutoDataGrid dataGrid) {
        dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        dataGrid.VerticalScrollBarVisibility   = ScrollBarVisibility.Auto;

        main_grid.AddControl(dataGrid);

        Grid.SetRow(dataGrid, 1);
        Grid.SetColumn(dataGrid, 0);
        Grid.SetColumnSpan(dataGrid, 3);

        main_grid.UpdateLayout();
    }

    public void AddSubDataGrid(AutoDataGrid dataGrid) {
        sub_grids.AddControl(dataGrid);
    }

    public static void ClearDataGrids(Panel panel) {
        var grids = GetDataGrids(panel).ToList();

        // Remove them.
        foreach (var grid in grids) {
            grid.SetItems(null);
            panel.Children.Remove(grid);
        }

        // Cleanup if needed.
        if (grids.Count > 0) {
            panel.UpdateLayout();
        }
    }

    public IEnumerable<AutoDataGrid> GetAllDataGrids() {
        foreach (var grid in GetDataGrids(main_grid)) {
            yield return grid;
        }
        foreach (var grid in GetDataGrids(sub_grids)) {
            yield return grid;
        }
    }

    public static IEnumerable<AutoDataGrid> GetDataGrids(Panel panel) {
        foreach (UIElement child in panel.Children) {
            if (child is AutoDataGrid mhwGrid) {
                yield return mhwGrid;
            }
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
        var scv = (ScrollViewer) sender;
        scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    public static void ShowError(Exception err, string title) {
        const string errMsg = "Error occurred. Press Ctrl+C to copy the contents of this window and report to the developer.\r\n\r\n";

        MessageBox.Show(errMsg + err, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void SetupKeybind(InputGesture keyGesture, ExecutedRoutedEventHandler onPress) {
        var changeItemValues = new RoutedCommand();
        var ib               = new InputBinding(changeItemValues, keyGesture);
        InputBindings.Add(ib);
        // Bind handler.
        var cb = new CommandBinding(changeItemValues);
        cb.Executed += onPress;
        CommandBindings.Add(cb);
    }
}