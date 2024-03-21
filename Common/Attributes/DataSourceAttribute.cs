namespace RE_Editor.Common.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DataSourceAttribute(DataSourceType dataType) : Attribute {
    public readonly DataSourceType dataType = dataType;
}