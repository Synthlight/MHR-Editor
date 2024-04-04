using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class SuperHarpoon : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name        = "Super Harpoon";
        const string description = "Fuck that damned fish!";
        const string version     = "1.0";

        var mod = new NexusMod {
            Name    = name,
            Version = version,
            Desc    = description,
            Files   = [PathHelper.HARPOON_DATA_PATH],
            Action  = FixDamage
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true);
    }

    public static void FixDamage(List<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case Chainsaw_HarpoonShellInfoUserData shellData:
                    shellData.AttackInfo[0].DamageRate[0].BaseValue = 100;
                    break;
            }
        }
    }
}