using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class NoRequirements : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName        = "No Crafting Requirements";
        const string variantBundleName = $"{bundleName} (Fluffy Selective Install)";
        const string outPath           = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = "1.14",
            NameAsBundle = bundleName,
            Desc         = "Removes the item requirements when crafting."
        };

        var mods = new[] {
            baseMod
                .SetName("Weapons (Forge/Upgrade/Layer)")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Product) // Forge
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Process)) // Upgrade
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Change))
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.OverwearProduct))) // Layer
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Weapons (Forge/Upgrade/Layer, Ignore Unlock Flags)")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Product) // Forge
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Process)) // Upgrade
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Change))
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.OverwearProduct))) // Layer
                .SetAction(list => {
                    CheatMod.NoCost(list);
                    CheatMod.NoUnlockFlag(list);
                }),
            baseMod
                .SetName("Weapons (Layered Only)")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Change)
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.OverwearProduct)))
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Weapons (Layered Only, Ignore Unlock Flags)")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Change)
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.OverwearProduct)))
                .SetAction(list => {
                    CheatMod.NoCost(list);
                    CheatMod.NoUnlockFlag(list);
                }),
            baseMod
                .SetName("Rampage Skills")
                .SetFiles(new[] {PathHelper.RAMPAGE_SKILL_RECIPE_PATH})
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Decorations")
                .SetFiles(new[] {PathHelper.DECORATION_RECIPE_PATH, PathHelper.RAMPAGE_DECORATION_RECIPE_PATH})
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Decorations (Ignore Unlock Flags)")
                .SetFiles(new[] {PathHelper.DECORATION_RECIPE_PATH, PathHelper.RAMPAGE_DECORATION_RECIPE_PATH})
                .SetAction(list => {
                    CheatMod.NoCost(list);
                    CheatMod.NoUnlockFlag(list);
                }),
            baseMod
                .SetName("Armor (Normal)")
                .SetFiles(new[] {PathHelper.ARMOR_RECIPE_PATH})
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Armor (Normal, Ignore Unlock Flags)")
                .SetFiles(new[] {PathHelper.ARMOR_RECIPE_PATH})
                .SetAction(list => {
                    CheatMod.NoCost(list);
                    CheatMod.NoUnlockFlag(list);
                }),
            baseMod
                .SetName("Armor (Layered)")
                .SetFiles(new[] {PathHelper.LAYERED_ARMOR_RECIPE_PATH})
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Armor (Layered, Ignore Unlock Flags)")
                .SetFiles(new[] {PathHelper.LAYERED_ARMOR_RECIPE_PATH})
                .SetAction(list => {
                    CheatMod.NoCost(list);
                    CheatMod.NoUnlockFlag(list);
                }),
            baseMod
                .SetName("Cat/Dog (Armor/Weapons)")
                .SetFiles(new[] {
                    PathHelper.CAT_ARMOR_RECIPE_PATH,
                    PathHelper.CAT_WEAPON_RECIPE_PATH,
                    PathHelper.DOG_ARMOR_RECIPE_PATH,
                    PathHelper.DOG_WEAPON_RECIPE_PATH
                })
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Cat/Dog (Layered Armor)")
                .SetFiles(new[] {PathHelper.CAT_DOG_LAYERED_ARMOR_RECIPE_PATH})
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Augment/Qurio Craft")
                .SetFiles(new[] {
                    PathHelper.AUGMENT_ARMOR_ENABLE_BASE_PATH,
                    PathHelper.AUGMENT_WEAPON_ENABLE_BASE_PATH,
                    PathHelper.AUGMENT_ARMOR_MATERIAL_BASE_PATH,
                    PathHelper.AUGMENT_WEAPON_MATERIAL_BASE_PATH
                })
                .SetAction(CheatMod.NoCost)
        };

        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, variantBundleName, true);

        var gpMods = new List<NexusModVariant>();
        foreach (var mod in mods) {
            var newMod = mod;
            newMod.NameAsBundle += " (GamePass)";
            newMod.Files = from file in newMod.Files
                           select file.Replace(@"\STM\", @"\MSG\");
            newMod.ForGp = true;
            gpMods.Add(newMod);
        }

        ModMaker.WriteMods(gpMods, PathHelper.CHUNK_PATH, outPath, variantBundleName + " (GamePass)", true);
    }
}