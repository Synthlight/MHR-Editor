using System.Diagnostics;
using RE_Editor.Common;

namespace RE_Editor.Dump_Parser;

public static class Program {
    public const  string BASE_PROJ_PATH = @"..\..\..";
    private const string SCRIPTS_DIR    = $@"{PathHelper.REFRAMEWORK_PATH}\reversing\rsz";
    private const string OUTPUT_DIR     = $@"{BASE_PROJ_PATH}\Dump-Parser\Output\{PathHelper.CONFIG_NAME}";

    public static void Main(string[] args) {
        var mode = ParseArgs(args);
        Directory.CreateDirectory(OUTPUT_DIR);

        if (mode is Mode.PART1 or Mode.ALL) {
            Console.WriteLine("Running part 1...");
            var processStartInfo = new ProcessStartInfo(PathHelper.PYTHON38_PATH) {
                WorkingDirectory = OUTPUT_DIR,
                ArgumentList = {
                    $@"{SCRIPTS_DIR}\emulation-dumper.py",
                    $"--p={PathHelper.EXE_PATH}",
                    $"--il2cpp_path={PathHelper.IL2CPP_DUMP_PATH}",
                }
            };
            Process.Start(processStartInfo)?.WaitForExit();
        }

        if (mode is Mode.PART2 or Mode.ALL) {
            Console.WriteLine("Running part 2...");
            var processStartInfo = new ProcessStartInfo(PathHelper.PYTHON38_PATH) {
                WorkingDirectory = OUTPUT_DIR,
                ArgumentList = {
                    $@"{SCRIPTS_DIR}\non-native-dumper.py",
                    $"--out_postfix={PathHelper.CONFIG_NAME}",
                    $"--il2cpp_path={PathHelper.IL2CPP_DUMP_PATH}",
                    $@"--natives_path={OUTPUT_DIR}\native_layouts_{Path.GetFileName(PathHelper.EXE_PATH)}.json",
                    "--use_typedefs=False",
                    "--use_hashkeys=True",
                    "--include_parents=True",
                }
            };
            Process.Start(processStartInfo)?.WaitForExit();
        }
    }

    private static Mode ParseArgs(string[] args) {
        foreach (var arg in args) {
// Pointless since we throw alter anyway.
#pragma warning disable CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
            return arg.ToLower() switch {
#pragma warning restore CS8509 // The switch expression does not handle all possible values of its input type (it is not exhaustive).
                "all" => Mode.ALL,
                "part1" => Mode.PART1,
                "part2" => Mode.PART2,
            };
        }
        throw new ArgumentOutOfRangeException(nameof(args));
    }

    private enum Mode {
        ALL,
        PART1,
        PART2,
    }
}