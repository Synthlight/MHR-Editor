using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Attributes;
using MHR_Editor.Data;
using MHR_Editor.Models.MHR_Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Item : RszObject {
    public static readonly uint HASH = uint.Parse("ee6b61f7", NumberStyles.HexNumber);

    public string Name => DataHelper.ITEM_NAME_LOOKUP.TryGet(Id);

    [ShowAsHex]
    public uint Id {
        get => fieldData.getFieldByName("_Id").data.GetData<uint>();
        set => fieldData.getFieldByName("_Id").data = value.GetBytes();
    }

    public int CarryableFilter {
        get => fieldData.getFieldByName("_CariableFilter").data.GetData<int>();
        set => fieldData.getFieldByName("_CariableFilter").data = value.GetBytes();
    }

    public ItemTypes Type {
        get => (ItemTypes) fieldData.getFieldByName("_Type").data.GetData<int>();
        set => fieldData.getFieldByName("_Type").data = ((int) value).GetBytes();
    }

    public RareTypes Rarity {
        get => (RareTypes) fieldData.getFieldByName("_Rare").data.GetData<byte>();
        set => fieldData.getFieldByName("_Rare").data = ((byte) value).GetBytes();
    }

    public uint PlMaxCount {
        get => fieldData.getFieldByName("_PlMaxCount").data.GetData<uint>();
        set => fieldData.getFieldByName("_PlMaxCount").data = value.GetBytes();
    }

    public uint OtMaxCount {
        get => fieldData.getFieldByName("_OtMaxCount").data.GetData<uint>();
        set => fieldData.getFieldByName("_OtMaxCount").data = value.GetBytes();
    }

    public ushort SortId {
        get => fieldData.getFieldByName("_SortId").data.GetData<ushort>();
        set => fieldData.getFieldByName("_SortId").data = value.GetBytes();
    }

    public bool Supply {
        get => fieldData.getFieldByName("_Supply").data.GetData<bool>();
        set => fieldData.getFieldByName("_Supply").data = value.GetBytes();
    }

    public bool ShowItemWindow {
        get => fieldData.getFieldByName("_ShowItemWindow").data.GetData<bool>();
        set => fieldData.getFieldByName("_ShowItemWindow").data = value.GetBytes();
    }

    public bool ShowActionWindow {
        get => fieldData.getFieldByName("_ShowActionWindow").data.GetData<bool>();
        set => fieldData.getFieldByName("_ShowActionWindow").data = value.GetBytes();
    }

    public bool Infinite {
        get => fieldData.getFieldByName("_Infinite").data.GetData<bool>();
        set => fieldData.getFieldByName("_Infinite").data = value.GetBytes();
    }

    public bool Default {
        get => fieldData.getFieldByName("_Default").data.GetData<bool>();
        set => fieldData.getFieldByName("_Default").data = value.GetBytes();
    }

    public bool IconCanEat {
        get => fieldData.getFieldByName("_IconCanEat").data.GetData<bool>();
        set => fieldData.getFieldByName("_IconCanEat").data = value.GetBytes();
    }

    public IconRank IconItemRank {
        get => (IconRank) fieldData.getFieldByName("_IconItemRank").data.GetData<int>();
        set => fieldData.getFieldByName("_IconItemRank").data = ((int) value).GetBytes();
    }

    public bool EffectRare {
        get => fieldData.getFieldByName("_EffectRare").data.GetData<bool>();
        set => fieldData.getFieldByName("_EffectRare").data = value.GetBytes();
    }

    public int IconChara {
        get => fieldData.getFieldByName("_IconChara").data.GetData<int>();
        set => fieldData.getFieldByName("_IconChara").data = value.GetBytes();
    }

    public int IconColor {
        get => fieldData.getFieldByName("_IconColor").data.GetData<int>();
        set => fieldData.getFieldByName("_IconColor").data = value.GetBytes();
    }

    public SeType SeType {
        get => (SeType) fieldData.getFieldByName("_SeType").data.GetData<int>();
        set => fieldData.getFieldByName("_SeType").data = ((int) value).GetBytes();
    }

    public uint SellPrice {
        get => fieldData.getFieldByName("_SellPrice").data.GetData<uint>();
        set => fieldData.getFieldByName("_SellPrice").data = value.GetBytes();
    }

    public uint BuyPrice {
        get => fieldData.getFieldByName("_BuyPrice").data.GetData<uint>();
        set => fieldData.getFieldByName("_BuyPrice").data = value.GetBytes();
    }

    public ItemActionType ItemActionType {
        get => (ItemActionType) fieldData.getFieldByName("_ItemActionType").data.GetData<int>();
        set => fieldData.getFieldByName("_ItemActionType").data = ((int) value).GetBytes();
    }

    public RankTypes RankType {
        get => (RankTypes) fieldData.getFieldByName("_RankType").data.GetData<int>();
        set => fieldData.getFieldByName("_RankType").data = ((int) value).GetBytes();
    }

    public ItemGroupTypes ItemGroup {
        get => (ItemGroupTypes) fieldData.getFieldByName("_ItemGroup").data.GetData<int>();
        set => fieldData.getFieldByName("_ItemGroup").data = ((int) value).GetBytes();
    }

    public uint CategoryWorth {
        get => fieldData.getFieldByName("_CategoryWorth").data.GetData<uint>();
        set => fieldData.getFieldByName("_CategoryWorth").data = value.GetBytes();
    }

    public int MaterialCategory {
        get => fieldData.getFieldByName("_MaterialCategory").data.GetData<int>();
        set => fieldData.getFieldByName("_MaterialCategory").data = value.GetBytes();
    }

    public uint EvaluationValue {
        get => fieldData.getFieldByName("_EvalutionValue").data.GetData<uint>();
        set => fieldData.getFieldByName("_EvalutionValue").data = value.GetBytes();
    }

    public override string ToString() {
        return Name;
    }
}