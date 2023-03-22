namespace RE_Editor.Common.Attributes;

public class SortOrderAttribute : Attribute {
    public readonly int sortOrder;

    public SortOrderAttribute(int sortOrder) {
        this.sortOrder = sortOrder;
    }
}