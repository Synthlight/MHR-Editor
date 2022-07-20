using MHR_Editor.Common;
using MHR_Editor.Models;
using MHR_Editor.Util;

namespace MHR_Editor.Mods;

public static class NoRequirements {
    public static void Make() {
        const string bundleName = "No Crafting Requirements";
        const string outPath    = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var baseMod = new NexusModVariant {
            Version      = "1.6",
            NameAsBundle = bundleName,
            Desc         = "Removes the item requirements when crafting."
        };

        var mods = new[] {
            baseMod
                .SetName("Weapons (Forge/Upgrade/Layer)")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Product) // Forge
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Process)) // Upgrade
                                    .Append(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Change))) // Layer
                .SetAction(CheatMod.NoCost),
            baseMod
                .SetName("Weapons (Layered Only)")
                .SetFiles(PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Change))
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
                .SetName("Armor (Layered)")
                .SetFiles(new[] {PathHelper.LAYERED_ARMOR_RECIPE_PATH})
                .SetAction(CheatMod.NoCost),
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
                .SetAction(CheatMod.NoCost)
        };

        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath);
    }
}