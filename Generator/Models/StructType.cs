using System.Diagnostics.CodeAnalysis;
using RE_Editor.Common;
using RE_Editor.Common.Attributes;
using RE_Editor.Common.Models;

namespace RE_Editor.Generator.Models;

public class StructType(string name, string? parent, string hash, StructJson structInfo) {
    public readonly string     name       = name;
    public readonly string?    parent     = parent;
    public readonly string     hash       = hash;
    public readonly StructJson structInfo = structInfo;
    public          int        useCount;

    public void UpdateUsingCounts(GenerateFiles generator, List<string> history) {
        if (history.Contains(structInfo.name!)) return;
        history.Add(structInfo.name!);

        if (parent != null) {
            generator.structTypes[parent].useCount++;
            generator.structTypes[parent].UpdateUsingCounts(generator, history);
        }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var field in structInfo.fields!) {
            if (string.IsNullOrEmpty(field.name)) continue;

            if (parent != null) {
                // Can't be null as the json includes parent's fields in children.
                if (generator.structTypes[parent].structInfo.fieldNameMap.TryGetValue(field.name, out var parentField)) {
                    parentField.virtualCount++;
                    field.overrideCount++;
                }
            }

            if (string.IsNullOrEmpty(field.originalType)) continue;
            if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
            if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;
            var typeName = field.originalType?.ToConvertedTypeName();
            if (typeName == null) continue;
            if (field.originalType!.GetViaType() != null) continue;
            if (generator.structTypes.ContainsKey(typeName)) {
                generator.structTypes[typeName].useCount++;
                generator.structTypes[typeName].UpdateUsingCounts(generator, history);
            }
            if (generator.enumTypes.TryGetValue(typeName, out var enumType)) {
                enumType.useCount++;
            }
        }
    }

    public void UpdateButtons(GenerateFiles generator, List<string> history) {
        if (history.Contains(structInfo.name!)) return;
        history.Add(structInfo.name!);

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var field in structInfo.fields!) {
            if (string.IsNullOrEmpty(field.name)) continue;
            if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
            if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;

            if (parent != null) {
                generator.structTypes[parent].UpdateButtons(generator, history);
            }
            if (parent != null && generator.structTypes[parent].structInfo.fieldNameMap.TryGetValue(field.name, out var parentField)) {
                field.buttonType = parentField.buttonType;
            } else {
                field.buttonType = GetButtonType(field);
            }
        }
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private DataSourceType? GetButtonType(StructJson.Field field) {
        // This part check the class + field name.
        var fullName = $"{name}.{field.name.ToConvertedFieldName()}";
#pragma warning disable IDE0066
#pragma warning disable CS1522
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (fullName) {
#if DD2
            // Many of these don't seem to be the enum type, probably because the enum doesn't allow zero but the fields do.
            case "App_EnhanceParamBase.ItemId":
            case "App_EnhanceParamBase.NeedItemId0":
            case "App_EnhanceParamBase.NeedItemId1":
            case "App_Gm80_001Param_ItemParam.ItemId":
            case "App_ItemCommonParam.ItemDropId":
            case "App_ItemDataParam.DecayedItemId":
            case "App_ItemDropParam_Table_Item.Id":
            case "App_ItemShopParamBase.ItemId":
                return DataSourceType.ITEMS;
#endif
        }
#pragma warning restore CS1522
#pragma warning restore IDE0066

        // And this check the original type.
        return field.originalType?.Replace("[]", "") switch {
#if DD2
            "app.ItemIDEnum" => DataSourceType.ITEMS,
#elif MHR
            "snow.data.ContentsIdSystem.ItemId" => DataSourceType.ITEMS,
            "snow.data.DataDef.PlEquipSkillId" => DataSourceType.SKILLS,
            "snow.data.DataDef.PlHyakuryuSkillId" => DataSourceType.RAMPAGE_SKILLS,
            "snow.data.DataDef.PlKitchenSkillId" => DataSourceType.DANGO_SKILLS,
            "snow.data.DataDef.PlWeaponActionId" => DataSourceType.SWITCH_SKILLS,
#elif RE2
            "app.ropeway.gamemastering.Item.ID" => DataSourceType.ITEMS,
            "app.ropeway.EquipmentDefine.WeaponType" => DataSourceType.WEAPONS,
#elif RE3
            "offline.EquipmentDefine.WeaponType" => DataSourceType.WEAPONS,
            "offline.gamemastering.Item.ID" => DataSourceType.ITEMS,
#elif RE4
            "chainsaw.ItemID" => DataSourceType.ITEMS,
            "chainsaw.WeaponID" => DataSourceType.WEAPONS,
#endif
            _ => null
        };
    }
}