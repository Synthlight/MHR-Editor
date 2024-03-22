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
        const string bundleName  = "Super Harpoon";
        const string description = "Fuck that damned fish!";
        const string version     = "1.0";
        const string outPath     = $@"{PathHelper.MODS_PATH}\{bundleName}";

        var mods = new NexusMod {
            Name    = bundleName,
            Version = version,
            Desc    = description,
            Files   = new[] {PathHelper.HARPOON_DATA_PATH},
            Action  = FixDamage
        };

        ModMaker.WriteMods(new List<NexusMod> {mods}, PathHelper.CHUNK_PATH, outPath, copyToFluffy: true);
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