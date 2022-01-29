namespace MHR_Editor.Common;

public static class Global {
    public static bool   showIdBeforeName = true;
    public static string locale           = "eng";

    public static readonly string[] FILE_TYPES = {
        "*.user.2"
    };

    public static readonly Dictionary<string, string> LANGUAGE_NAME_LOOKUP = new() {
        {"ara", "العربية"},
        {"chS", "简体中文"},
        {"chT", "繁體中文"},
        {"eng", "English"},
        {"fre", "Français"},
        {"ger", "Deutsch"},
        {"ita", "Italiano"},
        {"jpn", "日本語"},
        {"kor", "한국어"},
        {"pol", "Polski"},
        {"ptB", "Português do Brasil"},
        {"rus", "Русский"},
        {"spa", "Español"}
    };

    public static readonly List<string> LANGUAGES = new(LANGUAGE_NAME_LOOKUP.Keys);

    public static readonly Dictionary<string, Dictionary<string, string>> TRANSLATION_MAP = new() {
        {
            "eng", new() {
                {"Hyakuryu", "Rampage"},
                {"Takumi", "Handicraft"}
            }
        }
    };
}