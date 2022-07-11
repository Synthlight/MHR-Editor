using MHR_Editor.Common;
using MHR_Editor.Common.Models;

namespace MHR_Editor.Generator.Models {
    public class StructType {
        public readonly string     name;
        public readonly string     hash;
        public readonly StructJson structInfo;
        public          int        useCount;

        public StructType(string name, string hash, StructJson structInfo) {
            this.name       = name;
            this.hash       = hash;
            this.structInfo = structInfo;
        }

        public void UpdateUsingCounts() {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields!) {
                if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
                var typeName = field.originalType?.ToConvertedTypeName();
                if (typeName == null) continue;
                if (Program.STRUCT_TYPES.ContainsKey(typeName)) {
                    Program.STRUCT_TYPES[typeName].useCount++;
                    Program.STRUCT_TYPES[typeName].UpdateUsingCounts();
                }
                if (Program.ENUM_TYPES.ContainsKey(typeName)) {
                    Program.ENUM_TYPES[typeName].useCount++;
                }
            }
        }
    }
}