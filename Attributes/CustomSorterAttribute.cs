using System;

namespace MHR_Editor.Attributes;

public class CustomSorterAttribute : Attribute {
    public readonly Type customSorterType;

    public CustomSorterAttribute(Type customSorterType) {
        this.customSorterType = customSorterType;
    }
}