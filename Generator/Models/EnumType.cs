namespace MHR_Editor.Generator.Models {
    public class EnumType {
        public readonly string  name;
        public readonly string  type;
        public          int     useCount = 0;
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