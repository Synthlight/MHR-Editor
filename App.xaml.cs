using System.Windows;
using RE_Editor.Data;

namespace RE_Editor;

public partial class App {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        DataInit.Init();
    }
}