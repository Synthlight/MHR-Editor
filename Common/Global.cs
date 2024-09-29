using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;

#if MHR
[assembly: XmlnsDefinition("MHR", "RE_Editor.Common")]
#endif

namespace RE_Editor.Common;

public static class Global {
    public static bool      showIdBeforeName = true;
    public static LangIndex locale           = LangIndex.eng;

    public static readonly string[] FILE_TYPES = [
        $"*.user.{USER_VERSION}"
    ];

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

#if MHR
    public static readonly List<string> WEAPON_TYPES = new() {"Bow", "ChargeAxe", "DualBlades", "GreatSword", "GunLance", "Hammer", "HeavyBowgun", "Horn", "InsectGlaive", "Lance", "LightBowgun", "LongSword", "ShortSword", "SlashAxe"};
#elif RE4
    public static readonly List<string> VARIANTS = ["CH", "MC", "AO"];
    public static readonly List<string> FOLDERS  = ["_Chainsaw", "_Mercenaries", "_AnotherOrder"];
    public static          string       variant  = "";
#endif

#if DD2
    public const string MSG_VERSION = "22";
#elif DRDR
    public const string MSG_VERSION = "23";
#elif MHR
    public const string MSG_VERSION = "539100710";
#elif RE2
    public const string MSG_VERSION = "50397457";
#elif RE3
    public const string MSG_VERSION = "67305745";
#elif RE4
    public const string MSG_VERSION = "22";
#elif RE8
    public const string MSG_VERSION = "33685777";
#endif

#if DD2
    public const string USER_VERSION = "2";
#elif DRDR
    public const string USER_VERSION = "3";
#elif MHR
    public const string USER_VERSION = "2";
#elif RE2
    public const string USER_VERSION = "2";
#elif RE3
    public const string USER_VERSION = "2";
#elif RE4
    public const string USER_VERSION = "2";
#elif RE8
    public const string USER_VERSION = "2";
#endif
}