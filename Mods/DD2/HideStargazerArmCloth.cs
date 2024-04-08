using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class HideStargazerArmCloth : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name        = "Hide Stargazer's Garb Arm Cloth";
        const string description = "Hides the arm cloth on the Stargazer's Garb.";
        const string version     = "1.0";
        const string luaFile     = "HideStargazersGarbArmCloth.lua";
        const string luaPath     = $@"{PathHelper.MODS_PATH}\{name}\{luaFile}";

        Dd2HidePartsBase.WriteHideParts(name, version, luaPath, writer => {
            writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.STARGAZERS_GARB} then");
            writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmPartsEnable)} = {(long) App_TopsAmPartFlags.NONE}");
            writer.WriteLine("    end");
        });

        var mod = new NexusMod {
            Version         = version,
            Name            = name,
            Desc            = description,
            Image           = $@"{PathHelper.MODS_PATH}\{name}\Title.png",
            Files           = [],
            AdditionalFiles = new() {{luaPath, $@"reframework\autorun\{luaFile}"}},
            SkipPak         = true,
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true, noPakZip: true);
    }
}