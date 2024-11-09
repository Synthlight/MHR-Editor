﻿using System.Collections.Generic;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models;
using RE_Editor.Models.Structs;
using RE_Editor.Util;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class MorePopupCamps : IMod {
    [UsedImplicitly]
    public static void Make() {
        const string name        = "More Popup Camps";
        const string description = "Raises the limit on Popup Camps.";
        const string version     = "1.0.0";

        var mod = new NexusMod {
            Name    = name,
            Version = version,
            Desc    = description,
            Files   = [PathHelper.POPUP_CAMP_PATH],
            Action  = HigherCampLimits
        };

        ModMaker.WriteMods([mod], name, copyLooseToFluffy: true, noPakZip: true);
    }

    public static void HigherCampLimits(IList<RszObject> rszObjectData) {
        foreach (var obj in rszObjectData) {
            switch (obj) {
                case App_user_data_CampManagerSetting_cCampMaxNum campMaxNum:
                    campMaxNum.Core =
                        campMaxNum.Desert =
                            campMaxNum.Forest =
                                campMaxNum.Oil =
                                    campMaxNum.ShowOBT =
                                        campMaxNum.Wall = 50;
                    break;
            }
        }
    }
}