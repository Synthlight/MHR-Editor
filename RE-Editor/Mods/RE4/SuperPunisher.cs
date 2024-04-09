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
        const string name        = "Super Blacktail & Punisher";
        const string description = "So a minimalist run is easy. Blacktail is included for SW/Ada.";
        const string version     = "1.0";

        var mod = new NexusMod {
            Name    = name,
            Version = version,
            Desc    = description,
            Files = new[] {
                PathHelper.WEAPON_UPGRADE_DATA_PATH,
                PathHelper.WEAPON_UPGRADE_MC_DATA_PATH,
                PathHelper.WEAPON_UPGRADE_AO_DATA_PATH,
                @"\natives\STM\_Chainsaw\AppSystem\Shell\Bullet\wp4001\WP4001ShellInfo.user.2", // Punisher
                @"\natives\STM\_Chainsaw\AppSystem\Shell\Bullet\wp4003\WP4003ShellInfo.user.2", // Blacktail
            },
            Action = FixDamage
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true);
    }

    public static void FixDamage(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Chainsaw_WeaponDetailCustomUserdata_WeaponDetailStage data:
                    if (data.WeaponID == WeaponConstants_CH.PUNISHER
                        || data.WeaponID == WeaponConstants_AO.BLACKTAIL_AC
                        || data.WeaponID == 6112 /* Punisher MC (AO). Dunno why it has a separate ID like this. */) {
                        foreach (var damageRate in data.WeaponDetailCustom[0].CommonCustoms[0].AttackUp[0].DamageRates) {
                            damageRate.BaseValue = 10000;
                        }
                    }
                    break;
                case Chainsaw_BulletShellInfoUserData shellData:
                    shellData.AttackInfo[0].DamageRate[0].BaseValue = 10000;
                    break;
            }
        }
    }
}