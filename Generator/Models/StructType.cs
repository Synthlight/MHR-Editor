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

        public void UpdateUsingCounts(GenerateFiles generator) {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields!) {
                if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
                var typeName = field.originalType?.ToConvertedTypeName();
                if (typeName == null) continue;
                if (generator.structTypes.ContainsKey(typeName)) {
                    generator.structTypes[typeName].useCount++;
                    generator.structTypes[typeName].UpdateUsingCounts(generator);
                }
                if (generator.enumTypes.ContainsKey(typeName)) {
                    generator.enumTypes[typeName].useCount++;
                }
            }
        }
    }
}