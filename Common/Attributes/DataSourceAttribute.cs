namespace MHR_Editor.Common.Attributes;

public class DataSourceAttribute : Attribute {
    public readonly DataSourceType dataType;

    public DataSourceAttribute(DataSourceType dataType) {
        this.dataType = dataType;
    }
}

public enum DataSourceType {
    SKILLS
}