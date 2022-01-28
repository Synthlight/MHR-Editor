using System.Windows;
using MHR_Editor.Data;

namespace MHR_Editor;

public partial class App {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        DataInit.Init();
    }
}