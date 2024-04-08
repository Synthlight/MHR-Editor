using System.Collections.Generic;
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
public class HidePetticoatSkirts : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string nameAsBundle = "Hide Petticoat Parts";
        const string version      = "1.1.1";

        const string elegantPetticoat    = "Elegant Petticoat";
        const string gauntletedPetticoat = "Gauntleted Petticoat";

        var tweaks = new List<SwapDbTweak> {
            new($"Hide {elegantPetticoat} [Armbands]",
                $"Hides the armbands on the {elegantPetticoat}.",
                $"Hide{elegantPetticoat.Replace(" ", "")}Armbands.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.ELEGANT_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.LeftArmUpper}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.RightArmUpper}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {gauntletedPetticoat} [Elbow Plates]",
                $"Hides the elbow plates on the {gauntletedPetticoat}.",
                $"Hide{gauntletedPetticoat.Replace(" ", "")}ElbowPlates.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.GAUNTLETED_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmPartsEnable)} = {(ulong) App_TopsAmPartFlags.NONE}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {elegantPetticoat} [Gauntlets] (Requires Invis Fixer)",
                $"Hides the gauntlets on the {elegantPetticoat}.",
                $"Hide{elegantPetticoat.Replace(" ", "")}Gauntlets.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.ELEGANT_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmPartsEnable)} = {(ulong) App_TopsAmPartFlags.NONE}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {gauntletedPetticoat} [Gauntlets, All] (Requires Invis Fixer)",
                $"Hides the gauntlets on the {gauntletedPetticoat}.",
                $"Hide{gauntletedPetticoat.Replace(" ", "")}AllGauntlets.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.GAUNTLETED_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = {(ulong) App_TopsAmPartFlags.NONE}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {gauntletedPetticoat} [Gauntlet, Left] (Requires Invis Fixer)",
                $"Hides the gauntlets on the {gauntletedPetticoat}.",
                $"Hide{gauntletedPetticoat.Replace(" ", "")}LeftGauntlets.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.GAUNTLETED_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.LeftArmUpper}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.LeftArmLower}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.LeftHand}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.LeftPauldron}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.LeftAccessory2}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {gauntletedPetticoat} [Gauntlet, Right]",
                $"Hides the gauntlets on the {gauntletedPetticoat}.",
                $"Hide{gauntletedPetticoat.Replace(" ", "")}RightGauntlets.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.GAUNTLETED_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.RightHand}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.RightPauldron}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.RightFastener}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.RightAccessory1}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.AmSubPartsEnable)} ~ {(long) App_TopsAmPartFlags.RightAccessory2}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {elegantPetticoat} [Shoulder Plates]",
                $"Hides the shoulder plates on the {elegantPetticoat}.",
                $"Hide{elegantPetticoat.Replace(" ", "")}ShoulderPlates.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.ELEGANT_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.BdPartsEnable)} = {(ulong) App_TopsBdPartFlags.NONE}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {elegantPetticoat} [Skirt]",
                $"Hides the skirt on the {elegantPetticoat}.",
                $"Hide{elegantPetticoat.Replace(" ", "")}Skirt.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.ELEGANT_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.WbPartsEnable)} = {(ulong) App_TopsWbPartFlags.Torso}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {gauntletedPetticoat} [Skirt]",
                $"Hides the skirt on the {gauntletedPetticoat}.",
                $"Hide{gauntletedPetticoat.Replace(" ", "")}Skirt.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.GAUNTLETED_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.WbPartsEnable)} = {(ulong) App_TopsWbPartFlags.Torso}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {elegantPetticoat} [Waist]",
                $"Hides the waist on the {elegantPetticoat}.",
                $"Hide{elegantPetticoat.Replace(" ", "")}Waist.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.ELEGANT_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.BtPartsEnable)} = entry._{nameof(App_TopsSwapItem.BtPartsEnable)} ~ {(long) App_TopsBtPartFlags.Waist}");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.BtSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.BtSubPartsEnable)} ~ {(long) App_TopsBtPartFlags.Waist}");
                    writer.WriteLine("    end");
                }),
            new($"Hide {gauntletedPetticoat} [Waist]",
                $"Hides the waist on the {gauntletedPetticoat}.",
                $"Hide{gauntletedPetticoat.Replace(" ", "")}Waist.lua",
                writer => {
                    writer.WriteLine($"    if entry._TopsStyle == {(uint) (dynamic) StyleConstants.GAUNTLETED_PETTICOAT} then");
                    writer.WriteLine($"        entry._{nameof(App_TopsSwapItem.BdSubPartsEnable)} = entry._{nameof(App_TopsSwapItem.BdSubPartsEnable)} ~ {(long) App_TopsBdPartFlags.WaistAccessory1}");
                    writer.WriteLine("    end");
                }),
        };

        var mods = new List<NexusMod>();
        foreach (var tweak in tweaks) {
            var luaPath = $@"{PathHelper.MODS_PATH}\{nameAsBundle}\{tweak.luaFile}";
            mods.Add(new() {
                Version         = version,
                Name            = tweak.name,
                Desc            = tweak.desc,
                NameAsBundle    = nameAsBundle,
                Image           = $@"{PathHelper.MODS_PATH}\{nameAsBundle}\Title.png",
                Files           = [],
                AdditionalFiles = new() {{luaPath, $@"reframework\autorun\{tweak.luaFile}"}},
                SkipPak         = true,
            });

            Dd2HidePartsBase.WriteHideParts(tweak.name, version, luaPath, writer => { tweak.action(writer); });
        }

        ModMaker.WriteMods(mods, nameAsBundle, copyLooseToFluffy: true, noPakZip: true);
    }
}