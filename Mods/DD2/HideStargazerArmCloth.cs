using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
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

        var mod = new NexusMod {
            Version         = version,
            Name            = name,
            Desc            = description,
            Image           = $@"{PathHelper.MODS_PATH}\{name}\Title.png",
            Files           = [],
            AdditionalFiles = new() {{$@"{PathHelper.MODS_PATH}\{name}\HideStargazersGarbArmCloth.lua", @"reframework\autorun\HideStargazersGarbArmCloth.lua"}},
            SkipPak         = true,
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true, noPakZip: true);
    }
}