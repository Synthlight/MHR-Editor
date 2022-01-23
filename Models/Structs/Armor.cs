using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Attributes;
using MHR_Editor.Data;
using MHR_Editor.Models.MHR_Enums;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Armor : RszObject {
    public static readonly uint HASH = uint.Parse("6c3de53e", NumberStyles.HexNumber);

    public string Name => DataHelper.ARMOR_NAME_LOOKUP.TryGet(Id);

    [ShowAsHex]
    public uint Id {
        get => fieldData.getFieldByName("_Id").data.GetData<uint>();
        set => fieldData.getFieldByName("_Id").data = value.GetBytes();
    }

    public bool IsValid {
        get => fieldData.getFieldByName("_IsValid").data.GetData<bool>();
        set => fieldData.getFieldByName("_IsValid").data = value.GetBytes();
    }

    public int Series {
        get => fieldData.getFieldByName("_Series").data.GetData<int>();
        set => fieldData.getFieldByName("_Series").data = value.GetBytes();
    }

    public uint SortId {
        get => fieldData.getFieldByName("_SortId").data.GetData<uint>();
        set => fieldData.getFieldByName("_SortId").data = value.GetBytes();
    }

    public uint ModelId {
        get => fieldData.getFieldByName("_ModelId").data.GetData<uint>();
        set => fieldData.getFieldByName("_ModelId").data = value.GetBytes();
    }

    public RareTypes Rarity {
        get => (RareTypes) fieldData.getFieldByName("_Rare").data.GetData<byte>();
        set => fieldData.getFieldByName("_Rare").data = ((byte) value).GetBytes();
    }

    public uint Value {
        get => fieldData.getFieldByName("_Value").data.GetData<uint>();
        set => fieldData.getFieldByName("_Value").data = value.GetBytes();
    }

    public uint BuyValue {
        get => fieldData.getFieldByName("_BuyValue").data.GetData<uint>();
        set => fieldData.getFieldByName("_BuyValue").data = value.GetBytes();
    }

    public SexualEquipableFlag SexualEquipable {
        get => (SexualEquipableFlag) fieldData.getFieldByName("_SexualEquipable").data.GetData<int>();
        set => fieldData.getFieldByName("_SexualEquipable").data = ((int) value).GetBytes();
    }

    public bool SymbolColor1 {
        get => fieldData.getFieldByName("_SymbolColor1").data.GetData<bool>();
        set => fieldData.getFieldByName("_SymbolColor1").data = value.GetBytes();
    }

    public bool SymbolColor2 {
        get => fieldData.getFieldByName("_SymbolColor2").data.GetData<bool>();
        set => fieldData.getFieldByName("_SymbolColor2").data = value.GetBytes();
    }

    public int DefVal {
        get => fieldData.getFieldByName("_DefVal").data.GetData<int>();
        set => fieldData.getFieldByName("_DefVal").data = value.GetBytes();
    }

    public int FireRegVal {
        get => fieldData.getFieldByName("_FireRegVal").data.GetData<int>();
        set => fieldData.getFieldByName("_FireRegVal").data = value.GetBytes();
    }

    public int WaterRegVal {
        get => fieldData.getFieldByName("_WaterRegVal").data.GetData<int>();
        set => fieldData.getFieldByName("_WaterRegVal").data = value.GetBytes();
    }

    public int IceRegVal {
        get => fieldData.getFieldByName("_IceRegVal").data.GetData<int>();
        set => fieldData.getFieldByName("_IceRegVal").data = value.GetBytes();
    }

    public int ThunderRegVal {
        get => fieldData.getFieldByName("_ThunderRegVal").data.GetData<int>();
        set => fieldData.getFieldByName("_ThunderRegVal").data = value.GetBytes();
    }

    public int DragonRegVal {
        get => fieldData.getFieldByName("_DragonRegVal").data.GetData<int>();
        set => fieldData.getFieldByName("_DragonRegVal").data = value.GetBytes();
    }

    public ArmorBuildupData BuildupTable {
        get => (ArmorBuildupData) fieldData.getFieldByName("_BuildupTable").data.GetData<int>();
        set => fieldData.getFieldByName("_BuildupTable").data = ((int) value).GetBytes();
    }

    public SeriesBufType BuffFormula {
        get => (SeriesBufType) fieldData.getFieldByName("_BuffFormula").data.GetData<int>();
        set => fieldData.getFieldByName("_BuffFormula").data = ((int) value).GetBytes();
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

    private List<byte> SkillList      { get; set; }
    private List<int>  SkillLevelList { get; set; }

    public byte Skill1 {
        get => SkillList[0];
        set => SkillList[0] = value;
    }

    public int Skill1Level {
        get => SkillLevelList[0];
        set => SkillLevelList[0] = value;
    }

    public byte Skill2 {
        get => SkillList[1];
        set => SkillList[1] = value;
    }

    public int Skill2Level {
        get => SkillLevelList[1];
        set => SkillLevelList[1] = value;
    }

    public byte Skill3 {
        get => SkillList[2];
        set => SkillList[2] = value;
    }

    public int Skill3Level {
        get => SkillLevelList[2];
        set => SkillLevelList[2] = value;
    }

    public byte Skill4 {
        get => SkillList[3];
        set => SkillList[3] = value;
    }

    public int Skill4Level {
        get => SkillLevelList[3];
        set => SkillLevelList[3] = value;
    }

    public byte Skill5 {
        get => SkillList[4];
        set => SkillList[4] = value;
    }

    public int Skill5Level {
        get => SkillLevelList[4];
        set => SkillLevelList[4] = value;
    }

    public uint IdAfterExChange {
        get => fieldData.getFieldByName("_IdAfterExChange").data.GetData<uint>();
        set => fieldData.getFieldByName("_IdAfterExChange").data = value.GetBytes();
    }

    protected override void Init() {
        base.Init();

        DecorationsNumList = fieldData.getFieldByName("_DecorationsNumList").GetDataAsList<uint>();
        SkillList          = fieldData.getFieldByName("_SkillList").GetDataAsList<byte>();
        SkillLevelList     = fieldData.getFieldByName("_SkillLvList").GetDataAsList<int>();
    }

    protected override void PreWrite() {
        base.PreWrite();

        fieldData.getFieldByName("_DecorationsNumList").SetDataFromList(DecorationsNumList);
        fieldData.getFieldByName("_SkillList").SetDataFromList(SkillList);
        fieldData.getFieldByName("_SkillLvList").SetDataFromList(SkillLevelList);
    }

    public override string ToString() {
        return Name;
    }
}