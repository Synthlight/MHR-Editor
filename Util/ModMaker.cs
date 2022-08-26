using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MHR_Editor.Common;
using MHR_Editor.Common.Models;
using MHR_Editor.Models;

namespace MHR_Editor.Util;

public static class ModMaker {
    public static void WriteMods<T>(IEnumerable<T> mods, string inPath, string outPath, string variantBundleName = null, bool copyToFluffy = false) where T : INexusMod {
        foreach (var mod in mods) {
            var folderName = mod.Filename ?? mod.Name.Replace('/', '-');
            var variant    = mod as INexusModVariant;
            var bundleName = variant?.NameAsBundle.Replace('/', '-');

            var modPath = $@"{outPath.Replace('/', '-')}";
            if (variant != null) {
                // In a subfolder because it works with my compress script.
                if (variantBundleName != null) {
                    modPath += $@"\{variantBundleName}";
                } else {
                    modPath += $@"\{bundleName}";
                }
            }
            modPath += $@"\{folderName}";
            if (Directory.Exists(modPath)) Directory.Delete(modPath, true);
            Directory.CreateDirectory(modPath);

            var modInfo = new StringWriter();
            modInfo.WriteLine($"name={mod.Name}");
            modInfo.WriteLine($"version={mod.Version}");
            modInfo.WriteLine($"description={mod.Desc}");
            modInfo.WriteLine("author=LordGregory");
            if (variant != null) {
                modInfo.WriteLine($"NameAsBundle={bundleName}");
            }
            File.WriteAllText(@$"{modPath}\modinfo.ini", modInfo.ToString());

            foreach (var modFile in mod.Files) {
                var sourceFile = @$"{inPath}\{modFile}";
                var outFile    = @$"{modPath}\{modFile}";
                Directory.CreateDirectory(Path.GetDirectoryName(outFile)!);

                var dataFile = ReDataFile.Read(sourceFile);
                var data     = dataFile.rsz.objectData;
                mod.Action.Invoke(data);
                dataFile.Write(outFile);
            }
        }
        CompressTheMod(outPath);

        foreach (var mod in mods) {
            if (copyToFluffy && variantBundleName == null && mod is NexusMod) {
                var folderName = mod.Filename ?? mod.Name.Replace('/', '-');
                File.Copy($@"{outPath.Replace('/', '-')}\{folderName}.rar", $@"{PathHelper.FLUFFY_MODS_PATH}\{folderName}.rar", true);
            }
        }

        if (copyToFluffy && variantBundleName != null) {
            File.Copy($@"{outPath.Replace('/', '-')}\{variantBundleName}.rar", $@"{PathHelper.FLUFFY_MODS_PATH}\{variantBundleName}.rar", true);
        }
    }

    private static void CompressTheMod(string outDir) {
        var p = new Process();
        var info = new ProcessStartInfo {
            FileName               = "wsl.exe",
            Arguments              = PathHelper.RAR_SCRIPT,
            RedirectStandardInput  = true,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
            WorkingDirectory       = outDir.Replace('/', '-')
        };
        p.StartInfo          =  info;
        p.OutputDataReceived += (_, e) => Debug.WriteLine(e.Data);
        p.ErrorDataReceived  += (_, e) => Debug.WriteLine(e.Data);
        p.Start();
        p.BeginOutputReadLine();
        p.BeginErrorReadLine();
        p.WaitForExit();
    }
}