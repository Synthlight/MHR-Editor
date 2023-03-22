namespace RE_Editor.Generator.Models {
    public class EnumType {
        public readonly string  name;
        public          string  type;
        public          int     useCount;
        private         string? contents;
        public string? Contents {
            get => contents;
            set =>
                contents = value?.Replace("        ", "    ")
                                .Replace("    }", "}");
        }

        public EnumType(string name, string type) {
            this.name = name;
            this.type = type;
        }
    }
}