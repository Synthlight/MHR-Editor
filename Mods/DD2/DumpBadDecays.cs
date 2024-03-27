using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RE_Editor.Common;
using RE_Editor.Common.Data;
using RE_Editor.Common.Models;
using RE_Editor.Models.Structs;

namespace RE_Editor.Mods;

[UsedImplicitly]
[SuppressMessage("ReSharper", "SwitchStatementHandlesSomeKnownEnumValuesWithDefault")]
[SuppressMessage("ReSharper", "ConvertSwitchStatementToSwitchExpression")]
[SuppressMessage("ReSharper", "SwitchStatementMissingSomeEnumCasesNoDefault")]
public class DumpBadDecays : IMod {
    [UsedImplicitly]
    public static void Make() {
        var dataFile = ReDataFile.Read(@$"{PathHelper.CHUNK_PATH}\{PathHelper.ITEM_DATA_PATH}");
        var data     = dataFile.rsz.GetEntryObject<App_ItemData>();

        var decayItemsWhichAreBad = new Dictionary<uint, string>();
        var itemsWithBadDecay     = new Dictionary<uint, string>();
        foreach (var itemData in data.Params) {
            var itemName      = DataHelper.ITEM_NAME_LOOKUP[Global.LangIndex.eng].TryGet((uint) itemData.Id, "");
            var decayItemName = DataHelper.ITEM_NAME_LOOKUP[Global.LangIndex.eng].TryGet((uint) itemData.DecayedItemId, "");
            if (decayItemName.StartsWith("Rotten") || decayItemName.StartsWith("Dried")) {
                decayItemsWhichAreBad.TryAdd((uint) itemData.DecayedItemId, decayItemName);
                itemsWithBadDecay.TryAdd((uint) itemData.Id, itemName);
            }
        }
        File.WriteAllText($@"{PathHelper.MODS_PATH}\..\decayItemsWhichAreBad.json", JsonConvert.SerializeObject(decayItemsWhichAreBad, Formatting.Indented));
        File.WriteAllText($@"{PathHelper.MODS_PATH}\..\itemsWithBadDecay.json", JsonConvert.SerializeObject(itemsWithBadDecay, Formatting.Indented));
    }
}