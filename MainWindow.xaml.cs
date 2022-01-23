using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MHR_Editor.Models;
using MHR_Editor.Models.Structs;

namespace MHR_Editor;

public partial class MainWindow {
    private const string TITLE = "MHW Editor";

    private readonly ReDataFile                   file;
    private readonly ObservableCollection<object> items;

    public MainWindow() {
        InitializeComponent();

        Title = TITLE;

        Width  = SystemParameters.MaximizedPrimaryScreenWidth * 0.8;
        Height = SystemParameters.MaximizedPrimaryScreenHeight * 0.5;

        file = ReDataFile.Read(@"V:\MHR\re_chunk_000\natives\stm\data\Define\Player\Armor\ArmorBaseData.user.2");

        items = new(file.rsz.objectData.OfType<GreatSword>());
        //items = new(file.rsz.objectData.OfType<Armor>());
        //items = new(file.rsz.objectData.OfType<Decoration>());

        dg_items.ItemsSource = items;

        SetupKeybind(new KeyGesture(Key.D, ModifierKeys.Control), (_, _) => Stuff());
        SetupKeybind(new KeyGesture(Key.S, ModifierKeys.Control), (_, _) => Save());
    }

    private void Stuff() {
        foreach (var item in items) {
            switch (item) {
                case Armor armor:
                    armor.DecorationsNum1 = armor.DecorationsNum2 = 0;
                    armor.DecorationsNum3 = 3;
                    if (armor.Skill1Level > 0) armor.Skill1Level = 10;
                    if (armor.Skill2Level > 0) armor.Skill2Level = 10;
                    if (armor.Skill3Level > 0) armor.Skill3Level = 10;
                    if (armor.Skill4Level > 0) armor.Skill4Level = 10;
                    if (armor.Skill5Level > 0) armor.Skill5Level = 10;
                    break;
                case Decoration decoration:
                    if (decoration.Skill1Level > 0) decoration.Skill1Level = 10;
                    break;
                case GreatSword greatSword:
                    greatSword.DecorationsNum1 = greatSword.DecorationsNum2 = 0;
                    greatSword.DecorationsNum3 = 3;
                    greatSword.SharpnessVal1   = 10;
                    greatSword.SharpnessVal2   = 10;
                    greatSword.SharpnessVal3   = 10;
                    greatSword.SharpnessVal4   = 10;
                    greatSword.SharpnessVal5   = 10;
                    greatSword.SharpnessVal6   = 350;
                    greatSword.SharpnessVal7   = 0;
                    break;
            }
        }
    }

    private void Save() {
    }

    private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) {
        Type sourceClassType = ((dynamic) e.PropertyDescriptor).ComponentType;
        var  propertyInfo    = sourceClassType.GetProperties().FirstOrDefault(info => info.Name == e.PropertyName);
        var  displayName     = ((DisplayNameAttribute) propertyInfo?.GetCustomAttribute(typeof(DisplayNameAttribute), true))?.DisplayName;
        var  showAsHex       = (ShowAsHexAttribute) propertyInfo?.GetCustomAttribute(typeof(ShowAsHexAttribute), true) != null;

        if (displayName != null) e.Column.Header = displayName;

        if (showAsHex) {
            (e.Column as DataGridTextColumn).Binding.StringFormat = "0x{0:X}";
        }
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