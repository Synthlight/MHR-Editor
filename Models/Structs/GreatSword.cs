using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Models.MHR_Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
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

    private List<int> RampageSkillIdList { get; set; }

    public int RampageSkill1 {
        get => RampageSkillIdList[0];
        set => RampageSkillIdList[0] = value;
    }

    public int RampageSkill2 {
        get => RampageSkillIdList[1];
        set => RampageSkillIdList[1] = value;
    }

    public int RampageSkill3 {
        get => RampageSkillIdList[2];
        set => RampageSkillIdList[2] = value;
    }

    private List<uint> DecorationsNumList { get; set; }

    public uint DecorationsNum1 {
        get => DecorationsNumList[0];
        set => DecorationsNumList[0] = value;
    }

    public uint DecorationsNum2 {
        get => DecorationsNumList[1];
        set => DecorationsNumList[1] = value;
    }

    public uint DecorationsNum3 {
        get => DecorationsNumList[2];
        set => DecorationsNumList[2] = value;
    }

    public PlWeaponElementTypes MainElementVal {
        get => (PlWeaponElementTypes) fieldData.getFieldByName("_MainElementType").data.GetData<int>();
        set => fieldData.getFieldByName("_MainElementType").data = ((int) value).GetBytes();
    }

    public int MainElementType {
        get => fieldData.getFieldByName("_MainElementVal").data.GetData<int>();
        set => fieldData.getFieldByName("_MainElementVal").data = value.GetBytes();
    }

    private List<int> SharpnessValList { get; set; }

    public int SharpnessVal1 {
        get => SharpnessValList[0];
        set => SharpnessValList[0] = value;
    }

    public int SharpnessVal2 {
        get => SharpnessValList[1];
        set => SharpnessValList[1] = value;
    }

    public int SharpnessVal3 {
        get => SharpnessValList[2];
        set => SharpnessValList[2] = value;
    }

    public int SharpnessVal4 {
        get => SharpnessValList[3];
        set => SharpnessValList[3] = value;
    }

    public int SharpnessVal5 {
        get => SharpnessValList[4];
        set => SharpnessValList[4] = value;
    }

    public int SharpnessVal6 {
        get => SharpnessValList[5];
        set => SharpnessValList[5] = value;
    }

    public int SharpnessVal7 {
        get => SharpnessValList[6];
        set => SharpnessValList[6] = value;
    }

    private List<int> TakumiValList { get; set; }

    public int TakumiVal1 {
        get => TakumiValList[0];
        set => TakumiValList[0] = value;
    }

    public int TakumiVal2 {
        get => TakumiValList[1];
        set => TakumiValList[1] = value;
    }

    public int TakumiVal3 {
        get => TakumiValList[2];
        set => TakumiValList[2] = value;
    }

    public int TakumiVal4 {
        get => TakumiValList[3];
        set => TakumiValList[3] = value;
    }

    protected override void Init() {
        base.Init();

        DecorationsNumList = fieldData.getFieldByName("_SlotNumList").GetDataAsList<uint>();
        RampageSkillIdList = fieldData.getFieldByName("_HyakuryuSkillIdList").GetDataAsList<int>();
        SharpnessValList   = fieldData.getFieldByName("_SharpnessValList").GetDataAsList<int>();
        TakumiValList      = fieldData.getFieldByName("_TakumiValList").GetDataAsList<int>();
    }

    protected override void PreWrite() {
        base.PreWrite();

        fieldData.getFieldByName("_SlotNumList").SetDataFromList(DecorationsNumList);
        fieldData.getFieldByName("_HyakuryuSkillIdList").SetDataFromList(RampageSkillIdList);
        fieldData.getFieldByName("_SharpnessValList").SetDataFromList(SharpnessValList);
        fieldData.getFieldByName("_TakumiValList").SetDataFromList(TakumiValList);
    }
}