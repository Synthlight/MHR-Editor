using RE_Editor.Common.Attributes;

namespace RE_Editor.Common.Models;

public class UserDataShell : RszObject {
    [SortOrder(500)]
    public string Value {
        get => rsz.userDataInfo[userDataRef].str;
        set => rsz.userDataInfo[userDataRef].str = value;
    }

    public override string ToString() {
        return Value;
    }
}