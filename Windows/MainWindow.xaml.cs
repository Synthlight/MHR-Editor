using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JetBrains.Annotations;
using MHR_Editor.Controls;
using MHR_Editor.Models;
using MHR_Editor.Models.Structs;
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

    public static bool SingleClickToEditMode { get; set; } = true;

    public MainWindow() {
        var args = Environment.GetCommandLineArgs();

        InitializeComponent();

        Title = TITLE;

        Width  = SystemParameters.MaximizedPrimaryScreenWidth * 0.8;
        Height = SystemParameters.MaximizedPrimaryScreenHeight * 0.5;

        SetupKeybind(new KeyGesture(Key.S, ModifierKeys.Control), (_,                      _) => Save());
        SetupKeybind(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift), (_, _) => Save(true));

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
            var target = filePath ?? GetOpenTarget($"MHR Data Files|{string.Join(";", Global.FILE_TYPES)}");
            if (string.IsNullOrEmpty(target)) return;

            targetFile = target;
            Title      = Path.GetFileName(targetFile);

            sub_grids.Children.Clear();
            sub_grids.UpdateLayout();

            ClearDataGrids(sub_grids);

            GC.Collect();

            file = ReDataFile.Read(targetFile);

            var type = GetFileType();
            AddDataGrid(file?.rsz.objectData.Where(o => o.GetType().Is(type)));
        } catch (Exception e) when (!Debugger.IsAttached) {
            ShowError(e, "Load Error");
        }
    }

    private string GetOpenTarget(string filter) {
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
            file.Write(targetFile);

            await ShowChangesSaved(true);
        } catch (Exception e) when (!Debugger.IsAttached) {
            ShowError(e, "Save Error");
        }
    }

    private string GetSaveTarget() {
        var sfdResult = new SaveFileDialog {
            Filter           = $"MHR Data Files|{string.Join(";", Global.FILE_TYPES)}",
            FileName         = $"{Path.GetFileNameWithoutExtension(targetFile)}",
            InitialDirectory = targetFile == null ? string.Empty : Path.GetDirectoryName(targetFile) ?? string.Empty,
            AddExtension     = true
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

    public AutoDataGrid AddDataGrid<T>(IEnumerable<T> itemSource) {
        var dataGrid = new AutoDataGridGeneric<T>();
        dataGrid.SetItems(itemSource is ObservableCollection<T> source ? source : new(itemSource));
        sub_grids.AddControl(dataGrid);
        return dataGrid;
    }

    public void ClearDataGrids(Panel panel) {
        var grids = new List<UIElement>();

        // Find them all.
        foreach (UIElement child in sub_grids.Children) {
            if (child is AutoDataGrid mhwGrid) {
                grids.Add(child);
                mhwGrid.SetItems(null);
            }
        }

        // Remove them.
        foreach (var grid in grids) {
            panel.Children.Remove(grid);
        }

        // Cleanup if needed.
        if (grids.Count > 0) {
            panel.UpdateLayout();
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
        var scv = (ScrollViewer) sender;
        scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    public static void ShowError(Exception err, string title) {
        var errMsg = "Error occurred. Press Ctrl+C to copy the contents of th.s window and report to the developer.\r\n\r\n";

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

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public Type GetFileType() {
        var fileName = Path.GetFileName(targetFile); //?.ToLower();

        return fileName switch {
            "ArmorBaseData.user.2" => typeof(Armor),
            "DecorationsBaseData.user.2" => typeof(Decoration),
            "GreatSwordBaseData.user.2" => typeof(GreatSword),
            _ => throw new($"No type found for: {fileName}")
        };
    }
}