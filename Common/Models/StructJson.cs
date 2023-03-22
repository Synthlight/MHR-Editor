using Newtonsoft.Json;

namespace RE_Editor.Common.Models;

public class StructJson {
    public string?      crc;
    public List<Field>? fields;
    public string?      name;

    public override string? ToString() {
        return name ?? base.ToString();
    }

    public class Field {
        public int     align;
        public bool    array;
        public string? name;
        public bool    native;
        [JsonProperty("original_type")]
        public string? originalType;
        public int     size;
        public string? type;

        public override string? ToString() {
            return name ?? base.ToString();
        }

        public static class Type {
            public const string S8  = "S8";
            public const string U8  = "U8";
            public const string S16 = "S16";
            public const string U16 = "U16";
            public const string S32 = "S32";
            public const string U32 = "U32";
            public const string S64 = "S64";
            public const string U64 = "U64";
        }
    }
}