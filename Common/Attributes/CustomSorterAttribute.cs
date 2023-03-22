namespace RE_Editor.Common.Attributes;

public class CustomSorterAttribute : Attribute {
    public readonly Type customSorterType;

    public CustomSorterAttribute(Type customSorterType) {
        this.customSorterType = customSorterType;
    }
}