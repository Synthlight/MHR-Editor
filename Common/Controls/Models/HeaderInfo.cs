using RE_Editor.Common.Models;

namespace RE_Editor.Common.Controls.Models;

public class HeaderInfo : OnPropertyChangedBase {
    private readonly string                               originalText;
    private readonly Dictionary<Global.LangIndex, string> translatedText = new();
    public           string                               OriginalText => translatedText.TryGet(Global.locale, originalText);

    public string PropertyName { get; }

    public HeaderInfo(string originalText, string propertyName) {
        this.originalText = originalText;
        PropertyName      = propertyName;

        foreach (var (locale, map) in Global.TRANSLATION_MAP) {
            if (map.Count == 0) continue;
            if (!translatedText.ContainsKey(locale)) translatedText[locale] = originalText;

            foreach (var (find, replace) in map) {
                translatedText[locale] = translatedText[locale].Replace(find, replace);
            }
        }
    }
}