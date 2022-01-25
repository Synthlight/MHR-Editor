using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using MHR_Editor.Attributes;
using MHR_Editor.Models.List_Wrappers;

namespace MHR_Editor.Models.Structs;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class Decoration : RszObject {
    public static readonly uint HASH = uint.Parse("e7bd2c0d", NumberStyles.HexNumber);

    [ShowAsHex]
    public uint Id {
        get => fieldData.getFieldByName("_Id").data.GetData<uint>();
        set => fieldData.getFieldByName("_Id").data = value.GetBytes();
    }

    public byte Rarity {
        get => fieldData.getFieldByName("_Rare").data.GetData<byte>();
        set => fieldData.getFieldByName("_Rare").data = value.GetBytes();
    }

    public int DecorationLevel {
        get => fieldData.getFieldByName("_DecorationLv").data.GetData<int>();
        set => fieldData.getFieldByName("_DecorationLv").data = value.GetBytes();
    }

    public int BasePrice {
        get => fieldData.getFieldByName("_BasePrice").data.GetData<int>();
        set => fieldData.getFieldByName("_BasePrice").data = value.GetBytes();
    }

    public ObservableCollection<SkillId<byte>> SkillList { get; set; }

    public ObservableCollection<GenericWrapper<int>> SkillLevelList { get; set; }

    protected override void Init() {
        base.Init();

        SkillList      = new(fieldData.getFieldByName("_SkillIdList").GetDataAsList<SkillId<byte>>());
        SkillLevelList = new(fieldData.getFieldByName("_SkillLvList").GetDataAsList<GenericWrapper<int>>());
    }

    protected override void PreWrite() {
        base.PreWrite();

        fieldData.getFieldByName("_SkillIdList").SetDataFromList(SkillList);
        fieldData.getFieldByName("_SkillLvList").SetDataFromList(SkillLevelList);
    }

    public override string ToString() {
        return Id.ToString();
    }
}