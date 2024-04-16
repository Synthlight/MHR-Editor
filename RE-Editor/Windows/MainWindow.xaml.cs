﻿using System;
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
using Microsoft.Win32;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models;
using RE_Editor.Controls;
using RE_Editor.Models;
using RE_Editor.Util;

namespace RE_Editor.Windows;

public partial class MainWindow {
#if DEBUG
    private const bool ENABLE_CHEAT_BUTTONS = true;
#else
    private const bool ENABLE_CHEAT_BUTTONS = false;
    public const  bool SHOW_RAW_BYTES = false;
#endif

#if DD2
    private const string TITLE = "DD2 Editor";
#elif MHR
    private const string TITLE = "MHR Editor";
#elif RE2
    private const string TITLE = "RE2 Editor";
#elif RE3
    private const string TITLE = "RE3 Editor";
#elif RE4
    private const string TITLE = "RE4 Editor";
#endif

    [CanBeNull] private CancellationTokenSource savedTimer;
    [CanBeNull] private ReDataFile              file;
    public static       string                  targetFile { get; private set; }
    public readonly     string                  filter            = $"RE Data Files|{string.Join(";", Global.FILE_TYPES)}";
    public readonly     List<MethodInfo>        allMakeModMethods = new();

    public Global.LangIndex Locale {
        get => Global.locale;
        set {
            Global.locale = value;
            if (file?.rsz.objectData == null) return;
            foreach (var grid in GetGrids().OfType<AutoDataGrid>()) {
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

        var visibility = File.Exists($@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\enable_cheats") ? Visibility.Visible : Visibility.Collapsed;
        btn_make_mods.Visibility = visibility;
        btn_test.Visibility      = visibility;

        foreach (var modType in NexusModExtensions.GetAllModTypes()) {
            var button = new Button {
                Content = $"Create \"{modType.Name}\" Mod"
            };
            var make = modType.GetMethod("Make", BindingFlags.Static | BindingFlags.Public);
            allMakeModMethods.Add(make);
            button.Click += (_, _) => { make!.Invoke(null, null); };
            panel_mods.Children.Add(button);
        }

#if MHR
        btn_wiki_dump.Visibility = visibility;
        btn_all_cheats.Visibility = visibility;
#endif

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

            var supportedSearchTarget = target.ToLower().Replace('/', '\\');
            var nativesIndex          = supportedSearchTarget.IndexOf(@"natives\stm", StringComparison.Ordinal);
            if (nativesIndex > 0
                && DataHelper.SUPPORTED_FILES.Length > 0
                && !DataHelper.SUPPORTED_FILES.Contains(supportedSearchTarget[nativesIndex..])) {
                var result = MessageBox.Show("This file has not passed write tests. It may or may not even open.\n" +
                                             "This also means you WILL BE UNABLE TO SAVE ANY CHANGES MADE TO IT.\n\nTry opening the file anyway?", "File not supported.", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result is MessageBoxResult.No or MessageBoxResult.Cancel) return;
            }

            targetFile = target;
            Title      = Path.GetFileName(targetFile);

            ClearGrids(main_grid);

            GC.Collect();

#if RE4
                // ReSharper disable StringLiteralTypo
            if (targetFile.ToLower().Contains("_mercenaries")) {
                Global.variant = "MC";
            } else if (targetFile.ToLower().Contains("_anotherorder")) {
                Global.variant = "AO";
            } else {
                Global.variant = "CH";
            }
                // ReSharper restore StringLiteralTypo
#endif

            file = ReDataFile.Read(target);

            var rszObjectData = file?.rsz.objectData;
            if (rszObjectData == null || rszObjectData.Count == 0) throw new("Error loading data; rszObjectData is null/empty.\n\nPlease report the path/name of the file you are trying to load.");
            var rszObjectInfo = file?.rsz.objectEntryPoints;
            if (rszObjectInfo == null || rszObjectInfo.Count == 0) throw new("Error loading data; rszObjectInfo is null/empty.\n\nPlease report the path/name of the file you are trying to load.");

            // Find entry object & type.
            var objectDataIndex     = (int) rszObjectInfo[0] - 1; // 1 based.
            var entryPointRszObject = rszObjectData[objectDataIndex];

            /*
             * Gets the items & typeName to use as the root entry in the dataGrid.
             * For param types, the is the list of params. (A shortcut we make.)
             * For the rest, it's the entry point & type.
             */
            var structInfo = entryPointRszObject.structInfo;
            if (structInfo.fields is {Count: 1} && structInfo.fields[0].array && structInfo.fields[0].type == "Object") {
                var type     = rszObjectData[^2].GetType();
                var items    = rszObjectData.GetGenericItemsOfType(type);
                var dataGrid = MakeDataGrid((dynamic) items, file.rsz);
                Debug.WriteLine($"Loading type: {type.Name}");
                AddMainDataGrid(dataGrid);
            } else {
                var type       = entryPointRszObject.GetType();
                var structGrid = MakeStructGrid((dynamic) Convert.ChangeType(entryPointRszObject, type), file.rsz);
                Debug.WriteLine($"Loading type: {type.Name}");
                AddStructGrid(structGrid);
            }

#if MHR
            btn_sort_gems_by_skill_name.Visibility = target.Contains("DecorationsBaseData.user.2") || target.Contains("HyakuryuDecoBaseData.user.2") ? Visibility.Visible : Visibility.Collapsed;
#endif
        } catch (FileNotSupported) {
            MessageBox.Show("It's using a struct or type the editor doesn't support yet.\n" +
                            "You can comment on the nexus page (if there is one) or on Discord about the file (and give the full path),\n" +
                            "but it may take a while.\n" +
                            "Use RE_RSZ as a good alternative.", "File not supported.", MessageBoxButton.OK, MessageBoxImage.Error);
        } catch (Exception e) when (!Debugger.IsAttached) {
            ShowError(e, "Load Error");
        }
    }

    private static (Type, object) GetItemAndTypeToUseAsRoot(IReadOnlyList<RszObject> rszObjectData, RszObject entryPointRszObject) {
        var structInfo = entryPointRszObject.structInfo;
        if (structInfo.fields is {Count: 1} && structInfo.fields[0].array && structInfo.fields[0].type == "Object") {
            var type  = rszObjectData[0].GetType();
            var items = rszObjectData.GetGenericItemsOfType(type);
            return (type, items);
        } else {
            var type  = entryPointRszObject.GetType();
            var items = new List<RszObject> {entryPointRszObject}.GetGenericItemsOfType(type);
            return (type, items);
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

    public static AutoDataGridGeneric<T> MakeDataGrid<T>(IEnumerable<T> itemSource, RSZ rsz) {
        var dataGrid        = new AutoDataGridGeneric<T>(rsz);
        var observableItems = itemSource as ObservableCollection<T> ?? new(itemSource);
        dataGrid.SetItems(observableItems);
        RowHelper.AddKeybinds(dataGrid, observableItems, dataGrid, rsz);
        return dataGrid;
    }

    public static StructGridGeneric<T> MakeStructGrid<T>(T item, RSZ rsz) where T : RszObject {
        var dataGrid = new StructGridGeneric<T>(rsz);
        dataGrid.SetItem(item);
        return dataGrid;
    }

    public void AddMainDataGrid(AutoDataGrid dataGrid) {
        dataGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        dataGrid.VerticalScrollBarVisibility   = ScrollBarVisibility.Auto;

        AddMainControl(dataGrid);
    }

    public void AddStructGrid(StructGrid structGrid) {
        structGrid.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        structGrid.VerticalScrollBarVisibility   = ScrollBarVisibility.Auto;

        AddMainControl(structGrid);
    }

    private void AddMainControl(UIElement uiElement) {
        main_grid.AddControl(uiElement);

        Grid.SetRow(uiElement, 1);
        Grid.SetColumn(uiElement, 0);
        Grid.SetColumnSpan(uiElement, 3);

        main_grid.UpdateLayout();
    }

    public void ClearGrids(Panel panel) {
        var grids = GetGrids().ToList();

        // Remove them.
        foreach (var grid in grids) {
            switch (grid) {
                case AutoDataGrid dataGrid:
                    dataGrid.SetItems(null);
                    break;
                case StructGrid structGrid:
                    structGrid.SetItem(null);
                    break;
            }
            panel.Children.Remove(grid);
        }

        // Cleanup if needed.
        if (grids.Count > 0) {
            panel.UpdateLayout();
        }
    }

    public IEnumerable<UIElement> GetGrids() {
        foreach (UIElement child in main_grid.Children) {
            switch (child) {
                case AutoDataGrid:
                case StructGrid:
                    yield return child;
                    break;
            }
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
        var scv = (ScrollViewer) sender;
        scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    public static void ShowError(Exception err, string title) {
        var errMsg = "Error occurred. Press Ctrl+C to copy the contents of this window and report to the developer.\r\n\r\n";

        if (!string.IsNullOrEmpty(targetFile)) {
            errMsg += $"Target File: {targetFile}\r\n\r\n";
        }

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