using System;
using System.Windows;
using System.Windows.Input;

namespace RE_Editor.Util;

public static class Utils {
    public static void SetupKeybind(UIElement control, InputGesture gesture, Action command) {
        var changeItemValues = new RoutedCommand();
        var ib               = new InputBinding(changeItemValues, gesture);
        control.InputBindings.Add(ib);
        // Bind handler.
        var cb = new CommandBinding(changeItemValues);
        cb.Executed += (_, _) => command();
        control.CommandBindings.Add(cb);
    }
}