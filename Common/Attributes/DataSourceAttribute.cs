namespace RE_Editor.Common.Attributes;

public class DataSourceAttribute : Attribute {
    public readonly DataSourceType dataType;

    public DataSourceAttribute(DataSourceType dataType) {
        this.dataType = dataType;
    }
}

public enum DataSourceType {
    DANGO_SKILLS,
    ITEMS,
    RAMPAGE_SKILLS,
    SKILLS,
    SWITCH_SKILLS
}