using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using MHR_Editor.Common;
using MHR_Editor.Common.Data;
using MHR_Editor.Common.Models;
using MHR_Editor.Data;
using MHR_Editor.Models.Enums;
using MHR_Editor.Models.Structs;
using Newtonsoft.Json;

namespace MHR_Editor.Data_Helper_Extractor;

/**
 * Separate from the ID-Parser since it needs to load user.2 files which means including the generated files.
 */
public static class Program {
    public const string BASE_PROJ_PATH  = @"..\..\..";
    public const string PAK_FOLDER_PATH = @"V:\MHR\re_chunk_000";

    public static void Main() {
        // We can't call `DataInit.Init()` directly since that is something we're directly altering here.
        // But we still need the bare minimum here to load the file and have names.
        Assembly.Load(nameof(Common));
        Assembly.Load(nameof(Generated));
        DataInit.InitStructTypeInfo();

        DataHelper.STRUCT_INFO       = JsonConvert.DeserializeObject<Dictionary<uint, StructJson>>(File.ReadAllText($@"{BASE_PROJ_PATH}\Data\Assets\STRUCT_INFO.json"))!;
        DataHelper.SKILL_NAME_LOOKUP = JsonConvert.DeserializeObject<Dictionary<Global.LangIndex, Dictionary<uint, string>>>(File.ReadAllText($@"{BASE_PROJ_PATH}\Data\Assets\SKILL_NAME_LOOKUP.json"))!;

        ExtractSkillEnumToIdLookup();
    }

    /**
     * Creates lang maps to map the skill enum to the skill name.
     * This is needed to do some find/replace in the `PlayerUserDataSkillParameter.user.2` columns.
     */
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    private static void ExtractSkillEnumToIdLookup() {
        var skillEnumToIdLookup = new Dictionary<Global.LangIndex, Dictionary<Snow_data_DataDef_PlEquipSkillId, string>>();
        var skills = ReDataFile.Read($@"{PAK_FOLDER_PATH}\natives\STM\data\Define\Player\Skill\PlEquipSkill\PlEquipSkillBaseData.user.2")
                               .rsz.objectData.OfType<Snow_data_PlEquipSkillBaseUserData_Param>()
                               .ToList();
        foreach (var lang in Enum.GetValues<Global.LangIndex>()) {
            skillEnumToIdLookup[lang] = new();
            foreach (var skill in skills) {
                skillEnumToIdLookup[lang][skill.EnumName] = DataHelper.SKILL_NAME_LOOKUP[lang].TryGet(skill.Id);
            }
        }

        File.WriteAllText($@"{BASE_PROJ_PATH}\Data\Assets\SKILL_ENUM_NAME_LOOKUP.json", JsonConvert.SerializeObject(skillEnumToIdLookup, Formatting.Indented));
    }
}