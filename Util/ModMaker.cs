using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;

namespace RE_Editor.Util;

public static class ModMaker {
    public static void WriteMods<T>(IEnumerable<T> mods, string inPath, string outPath, string variantBundleName = null, bool copyToFluffy = false) where T : INexusMod {
        foreach (var mod in mods) {
            var folderName = mod.Filename ?? mod.Name.Replace('/', '-').Replace(':', '-');
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
            if (mod.Image != null) {
                var imageFileName = Path.GetFileName(mod.Image);
                modInfo.WriteLine($"screenshot={imageFileName}");
                File.Copy(mod.Image!, @$"{modPath}\{imageFileName}", true);
            }
            File.WriteAllText(@$"{modPath}\modinfo.ini", modInfo.ToString());

            foreach (var modFile in mod.Files) {
                var sourceFile = @$"{inPath}\{modFile}";
                var outFile    = @$"{modPath}\{modFile}";
                Directory.CreateDirectory(Path.GetDirectoryName(outFile)!);

                var dataFile = ReDataFile.Read(sourceFile);
                var data     = dataFile.rsz.objectData;
                mod.Action.Invoke(data);
                dataFile.Write(outFile, forGp: mod.ForGp);
            }
        }
        CompressTheMod(outPath.Replace('/', '-'));

        foreach (var mod in mods) {
            if (copyToFluffy && variantBundleName == null && mod is NexusMod) {
                var folderName = mod.Filename ?? mod.Name.Replace('/', '-');
                File.Copy($@"{outPath.Replace('/', '-')}\{folderName}.zip", $@"{PathHelper.FLUFFY_MODS_PATH}\{folderName}.zip", true);
            }
        }

        if (copyToFluffy && variantBundleName != null) {
            File.Copy($@"{outPath.Replace('/', '-')}\{variantBundleName}.zip", $@"{PathHelper.FLUFFY_MODS_PATH}\{variantBundleName}.zip", true);
        }
    }

    private static void CompressTheMod(string outDir) {
        // Enumerate the directories in the path and make archives for each.
        foreach (var dir in Directory.EnumerateDirectories(outDir, "*", SearchOption.TopDirectoryOnly)) {
            DoZip(dir);
        }
    }

    private static void DoZip(string dir) {
        var parentDir = Directory.GetParent(dir);
        var outFile   = $"{dir}.zip";
        var zipFile   = new FileInfo(outFile);
        if (zipFile.Exists) zipFile.Delete();

        using var zip = ZipFile.Create(outFile);
        zip.BeginUpdate();

        foreach (var file in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)) {
            var relativePath = file.Replace($@"{parentDir}\", "");
            zip.Add(file, relativePath);
        }

        zip.CommitUpdate();
    }
}