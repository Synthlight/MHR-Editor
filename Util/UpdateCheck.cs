using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RE_Editor.Common;
using RE_Editor.Windows;

// Separate because `using System.Windows.Forms;` in an xaml class causes ReSharper to imagine annoying problems.
namespace RE_Editor.Util;

public static class UpdateCheck {
    [CanBeNull] public static NotifyIcon notifyIcon;

    public static async void Run(MainWindow mainWindow) {
        await Task.Run(() => {
            try {
                var json           = GetHttpText(PathHelper.JSON_VERSION_CHECK_URL);
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                var newestVersion  = JsonConvert.DeserializeObject<VersionCheck>(json)?.latest;

                if (currentVersion != newestVersion) {
                    mainWindow.Dispatcher?.InvokeAsync(() => {
                        notifyIcon = new NotifyIcon {
                            Icon    = SystemIcons.Application,
                            Visible = false,
                            Text = "MHR Editor\r\n" +
                                   "Update Available.\r\n" +
                                   "Click to go to the mod page."
                        };
                        //notifyIcon.BalloonTipClosed += (s, e) => notifyIcon.Visible = false;
                        notifyIcon.MouseClick += (sender, args) => { Process.Start(PathHelper.NEXUS_URL); };

                        notifyIcon.Visible = true;
                        notifyIcon.ShowBalloonTip(10000, "Update Available", "A newer version has been detected.\r\n" +
                                                                             $"Your Version: {currentVersion}\r\n" +
                                                                             $"Newer Version: {newestVersion}", ToolTipIcon.Info);
                    });
                }
            } catch (Exception e) {
                Console.Error.Write(e);
            }
        });
    }

    private static string GetHttpText(string url) {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
#pragma warning disable CS0618
        var request = (HttpWebRequest) WebRequest.Create(url);
#pragma warning restore CS0618
#pragma warning restore SYSLIB0014 // Type or member is obsolete
        request.Method = "GET";

        using var response = (HttpWebResponse) request.GetResponse();
        using var reader   = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException());
        return reader.ReadToEnd();
    }
}

public class VersionCheck {
    [UsedImplicitly]
    public string latest;
}