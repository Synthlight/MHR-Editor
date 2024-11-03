using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using RE_Editor.Common;
using RE_Editor.Common.Models;
using RE_Editor.Models.Enums;
using RE_Editor.Models.Structs;

namespace RE_Editor.Mods;

[UsedImplicitly]
public class DumpArmor : IMod {
    [UsedImplicitly]
    public static void Make() {
        /*
         * Armor paths:
         * `natives/STM/Art/Model/Character/ch02/008/001/2/ch02_008_0012` (Chatacabra male chest.)
         * `natives/STM/Art/Model/Character/{male/female somehow}/{armor series mod id (pad 3)}/{variant (pad 3)}/{armor part type +1}/{male/female somehow}_{armor series mod id (pad 3)}_{variant (pad 3)}{armor part type +1}`
         */

        var armorData       = ReDataFile.Read($@"{PathHelper.CHUNK_PATH}{PathHelper.ARMOR_DATA_PATH}").rsz.GetEntryObject<App_user_data_ArmorData>().Values.Cast<App_user_data_ArmorData_cData>().ToList();
        var armorSeriesData = ReDataFile.Read($@"{PathHelper.CHUNK_PATH}{PathHelper.ARMOR_SERIES_DATA_PATH}").rsz.GetEntryObject<App_user_data_ArmorSeriesData>().Values.Cast<App_user_data_ArmorSeriesData_cData>().ToList();

        var list = new List<ArmorModelData>();
        foreach (var armor in armorData) {
            if (armor.Series[0].Value == 1) continue;

            var modelData = new ArmorModelData {
                name   = armor.Name_,
                series = armorSeriesData.First(series => series.Series[0].Value == armor.Series[0].Value),
                part   = (App_ArmorDef_ARMOR_PARTS) armor.PartsType[0].Value,
            };
            list.Add(modelData);
        }

        var writer = new StreamWriter(File.Open($@"{PathHelper.MODS_PATH}\..\Armor Models.csv", FileMode.Create, FileAccess.Write, FileShare.Read));
        writer.WriteLine("Name,Gender,Path");
        foreach (var data in list) {
            writer.Write(data.name);
            writer.Write(',');
            writer.Write("Male");
            writer.Write(',');
            WritePart(writer, data, Gender.Male);

            writer.Write(data.name);
            writer.Write(',');
            writer.Write("Female");
            writer.Write(',');
            WritePart(writer, data, Gender.Female);
        }
        writer.Close();
    }

    private static void WritePart(StreamWriter writer, ArmorModelData armor, Gender gender) {
        var genderChar = gender == Gender.Male ? "ch02" : "ch03";
        var modelId    = armor.series.ModId;
        var subId      = gender == Gender.Male ? armor.series.ModSubMaleId : armor.series.ModSubFemaleId;
        var part       = (int) armor.part + 1;
        writer.WriteLine($"natives/STM/Art/Model/Character/{genderChar}/{modelId:000}/{subId:000}/{part}/{genderChar}_{modelId:000}_{subId:000}{part}");
    }

    private struct ArmorModelData {
        public string                              name;
        public App_user_data_ArmorSeriesData_cData series;
        public App_ArmorDef_ARMOR_PARTS            part;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private enum Gender {
        Male,
        Female
    }
}