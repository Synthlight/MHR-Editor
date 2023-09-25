using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Constants;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class SuperPunisher : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string bundleName  = "Super Blacktail & Punisher";
        const string description = "So a minimalist run is easy.";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mods = new NexusMod {
            Name    = bundleName,
            Version = version,
            Desc    = description,
            Files = new[] {
                PathHelper.WEAPON_UPGRADE_DATA_PATH,
                PathHelper.WEAPON_UPGRADE_MC_DATA_PATH,
                PathHelper.WEAPON_UPGRADE_AO_DATA_PATH
            },
            Action = FixDamage
        };

        ModMaker.WriteMods(new List<NexusMod> {mods}, PathHelper.CHUNK_PATH, outPath, copyToFluffy: true);
    }

    public static void FixDamage(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Chainsaw_WeaponDetailCustomUserdata_WeaponDetailStage data:
                    if (data.WeaponID != WeaponConstants_CH.PUNISHER
                        && data.WeaponID != WeaponConstants_AO.BLACKTAIL_AC
                        && data.WeaponID != WeaponConstants_AO.PUNISHER_MC) continue;
                    foreach (var damageRate in data.WeaponDetailCustom[0].CommonCustoms[0].AttackUp[0].DamageRates) {
                        damageRate.BaseValue = 10000;
                    }
                    break;
            }
        }
    }
}