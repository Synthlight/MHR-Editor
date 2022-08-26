using MHR_Editor.Common;

namespace MHR_Editor.Generated.Models;

public interface IGem {
    public string Name   { get; }
    public uint   SortId { get; set; }
    public uint   Level  { get; }
    uint          GetFirstSkillId();
    string        GetFirstSkillName(Global.LangIndex lang);
}