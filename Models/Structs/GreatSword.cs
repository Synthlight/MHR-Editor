using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Attributes;
using MHR_Editor.Models.List_Wrappers;
using MHR_Editor.Models.MHR_Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[MhrStruct]
public class GreatSword : RszObject {
    public static readonly uint HASH = uint.Parse("5ce7e37b", NumberStyles.HexNumber);

    [ShowAsHex]
    public uint Id {
        get => fieldData.getFieldByName("_Id").data.GetData<uint>();
        set => fieldData.getFieldByName("_Id").data = value.GetBytes();
    }

    public uint SortId {
        get => fieldData.getFieldByName("_SortId").data.GetData<uint>();
        set => fieldData.getFieldByName("_SortId").data = value.GetBytes();
    }

    public RareTypes Rarity {
        get => (RareTypes) fieldData.getFieldByName("_RareType").data.GetData<byte>();
        set => fieldData.getFieldByName("_RareType").data = ((byte) value).GetBytes();
    }

    public uint ModelId {
        get => fieldData.getFieldByName("_ModelId").data.GetData<uint>();
        set => fieldData.getFieldByName("_ModelId").data = value.GetBytes();
    }

    public uint BaseValue {
        get => fieldData.getFieldByName("_BaseVal").data.GetData<uint>();
        set => fieldData.getFieldByName("_BaseVal").data = value.GetBytes();
    }

    public uint BuyValue {
        get => fieldData.getFieldByName("_BuyVal").data.GetData<uint>();
        set => fieldData.getFieldByName("_BuyVal").data = value.GetBytes();
    }

    public int Atk {
        get => fieldData.getFieldByName("_Atk").data.GetData<int>();
        set => fieldData.getFieldByName("_Atk").data = value.GetBytes();
    }

    public int CriticalRate {
        get => fieldData.getFieldByName("_CriticalRate").data.GetData<int>();
        set => fieldData.getFieldByName("_CriticalRate").data = value.GetBytes();
    }

    public int DefBonus {
        get => fieldData.getFieldByName("_DefBonus").data.GetData<int>();
        set => fieldData.getFieldByName("_DefBonus").data = value.GetBytes();
    }

    public ObservableCollection<GenericWrapper<int>> RampageSkillIdList { get; set; }

    public ObservableCollection<GenericWrapper<uint>> DecorationsNumList { get; set; }

    public PlWeaponElementTypes MainElementVal {
        get => (PlWeaponElementTypes) fieldData.getFieldByName("_MainElementType").data.GetData<int>();
        set => fieldData.getFieldByName("_MainElementType").data = ((int) value).GetBytes();
    }

    public int MainElementType {
        get => fieldData.getFieldByName("_MainElementVal").data.GetData<int>();
        set => fieldData.getFieldByName("_MainElementVal").data = value.GetBytes();
    }

    public ObservableCollection<GenericWrapper<int>> SharpnessValList { get; set; }

    public ObservableCollection<GenericWrapper<int>> TakumiValList { get; set; }

    protected override void Init() {
        base.Init();

        DecorationsNumList = new(fieldData.getFieldByName("_SlotNumList").GetDataAsList<GenericWrapper<uint>>());
        RampageSkillIdList = new(fieldData.getFieldByName("_HyakuryuSkillIdList").GetDataAsList<GenericWrapper<int>>());
        SharpnessValList   = new(fieldData.getFieldByName("_SharpnessValList").GetDataAsList<GenericWrapper<int>>());
        TakumiValList      = new(fieldData.getFieldByName("_TakumiValList").GetDataAsList<GenericWrapper<int>>());
    }

    protected override void PreWrite() {
        base.PreWrite();

        fieldData.getFieldByName("_SlotNumList").SetDataFromList(DecorationsNumList);
        fieldData.getFieldByName("_HyakuryuSkillIdList").SetDataFromList(RampageSkillIdList);
        fieldData.getFieldByName("_SharpnessValList").SetDataFromList(SharpnessValList);
        fieldData.getFieldByName("_TakumiValList").SetDataFromList(TakumiValList);
    }
}