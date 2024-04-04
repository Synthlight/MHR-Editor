using System.Runtime.Serialization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RE_Editor.Common.Attributes;

namespace RE_Editor.Common.Models;

[UsedImplicitly]
public class StructJson {
    [UsedImplicitly] public string?      crc;
    [UsedImplicitly] public List<Field>? fields;
    [UsedImplicitly] public string?      name;
    [UsedImplicitly] public string?      parent;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [JsonIgnore] public Dictionary<string, Field> fieldNameMap;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [OnDeserialized]
    [UsedImplicitly]
    public void Init(StreamingContext context) {
        fieldNameMap = [];
        if (fields == null) return;
        foreach (var field in fields) {
            if (field.name == null) continue;
            fieldNameMap[field.name] = field;
        }
    }

    public override string? ToString() {
        return name ?? base.ToString();
    }

    public class Field {
        [UsedImplicitly] public int     align;
        [UsedImplicitly] public bool    array;
        [UsedImplicitly] public string? name;
        [UsedImplicitly] public bool    native;
        [JsonProperty("original_type")]
        [UsedImplicitly] public string? originalType;
        [UsedImplicitly] public int     size;
        [UsedImplicitly] public string? type;

        // The two should be updated together, and are done so in `StructType.UpdateUsingCounts`.
        [JsonIgnore] public int             overrideCount; // Used by children to mark they need to override a parent field.
        [JsonIgnore] public int             virtualCount; // Used by children to mark their parent's fields as overwritten.
        [JsonIgnore] public DataSourceType? buttonType; // Used by children to mark their parent's fields as overwritten.

        public override string? ToString() {
            return name ?? base.ToString();
        }
    }
}