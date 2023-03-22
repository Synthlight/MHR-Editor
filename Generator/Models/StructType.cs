using System.DirectoryServices.ActiveDirectory;
using RE_Editor.Common;
using RE_Editor.Common.Models;

namespace RE_Editor.Generator.Models {
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

        public void UpdateUsingCounts(GenerateFiles generator, List<string> history) {
            if (history.Contains(structInfo.name!)) return;
            history.Add(structInfo.name!);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var field in structInfo.fields!) {
                if (string.IsNullOrEmpty(field.name) || string.IsNullOrEmpty(field.originalType)) continue;
                if (GenerateFiles.UNSUPPORTED_DATA_TYPES.Contains(field.type!)) continue;
                if (GenerateFiles.UNSUPPORTED_OBJECT_TYPES.Any(s => field.originalType!.Contains(s))) continue;
                var typeName = field.originalType?.ToConvertedTypeName();
                if (typeName == null) continue;
                if (field.originalType!.GetViaType() != null) continue;
                if (generator.structTypes.ContainsKey(typeName)) {
                    generator.structTypes[typeName].useCount++;
                    generator.structTypes[typeName].UpdateUsingCounts(generator, history);
                }
                if (generator.enumTypes.ContainsKey(typeName)) {
                    generator.enumTypes[typeName].useCount++;
                }
            }
        }
    }
}