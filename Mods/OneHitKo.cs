using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Generated.Models;
using RE_Editor.Models;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class OneHitKo : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "OHKO";
        const string description = "Changes Defender I weapons to have 50k base damage.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mods = new List<NexusMod> {
            new() {
                Name    = bundleName,
                Version = version,
                Desc    = description,
                Files   = PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base),
                Action  = ChangeDamage
            },
            new() {
                Name    = bundleName + " (GamePass)",
                Version = version,
                Desc    = description,
                Files   = PathHelper.GetAllWeaponFilePaths(PathHelper.WeaponDataType.Base, "MSG"),
                Action  = ChangeDamage,
                ForGp   = true
            }
        };

        ModMaker.WriteMods(mods, PathHelper.CHUNK_PATH, outPath, copyToFluffy: true);
    }

    private static void ChangeDamage(IList<RszObject> rszObjectData) {
        SortedTitles.SortTitles(rszObjectData, Global.LangIndex.eng);

        foreach (var obj in rszObjectData) {
            switch (obj) {
                case IWeapon weapon:
                    switch (weapon.Id) {
                        case BowConstants.DEFENDER_BOW_I:
                        case ChargeAxeConstants.DEFENDER_CHARGE_BLADE_I:
                        case DualBladesConstants.DEFENDER_DUAL_BLADES_I:
                        case GreatSwordConstants.DEFENDER_GREAT_SWORD_I:
                        case GunLanceConstants.DEFENDER_GUNLANCE_I:
                        case HammerConstants.DEFENDER_HAMMER_I:
                        case HeavyBowgunConstants.DEFENDER_HEAVY_BOWGUN_I:
                        case HornConstants.DEFENDER_HORN_I:
                        case InsectGlaiveConstants.DEFENDER_GLAIVE_I:
                        case LightBowgunConstants.DEFENDER_LIGHT_BOWGUN_I:
                        case LongSwordConstants.DEFENDER_LONG_SWORD_I:
                        case ShortSwordConstants.DEFENDER_SWORD_I:
                        case SlashAxeConstants.DEFENDER_SWITCH_AXE_I:
                            weapon.Atk = 50000;
                            break;
                    }
                    break;
            }
        }
    }
}