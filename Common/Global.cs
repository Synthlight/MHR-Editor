using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Common;

public static class Global {
    public static bool      showIdBeforeName = true;
    public static LangIndex locale           = LangIndex.eng;

    public static readonly string[] FILE_TYPES = {
        "*.user.2"
    };

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LangIndex {
        jpn,
        eng,
        fre,
        ita,
        ger,
        spa,
        rus,
        pol,
        ptB = 10,
        kor,
        chT,
        chS,
        ara = 21,
    }

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public static readonly Dictionary<LangIndex, string> LANGUAGE_NAME_LOOKUP = new() {
        {LangIndex.ara, "العربية"},
        {LangIndex.chS, "简体中文"},
        {LangIndex.chT, "繁體中文"},
        {LangIndex.eng, "English"},
        {LangIndex.fre, "Français"},
        {LangIndex.ger, "Deutsch"},
        {LangIndex.ita, "Italiano"},
        {LangIndex.jpn, "日本語"},
        {LangIndex.kor, "한국어"},
        {LangIndex.pol, "Polski"},
        {LangIndex.ptB, "Português do Brasil"},
        {LangIndex.rus, "Русский"},
        {LangIndex.spa, "Español"}
    };

    public static readonly List<LangIndex> LANGUAGES = Enum.GetValues(typeof(LangIndex)).Cast<LangIndex>().ToList();

    public static readonly Dictionary<LangIndex, Dictionary<string, string>> TRANSLATION_MAP = new() {
        {
            LangIndex.eng, new() {
                {"Hyakuryu", "Rampage"},
                {"Takumi", "Handicraft"},
                {"Hagitori", "Carve"}
            }
        }
    };
}