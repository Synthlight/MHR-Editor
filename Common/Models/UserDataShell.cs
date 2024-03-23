using JetBrains.Annotations;
using RE_Editor.Common.Attributes;

namespace RE_Editor.Common.Models;

public class UserDataShell(uint hash) : RszObject {
    public readonly uint hash = hash;

    [SortOrder(500)]
    public string Value {
        get => rsz.userDataInfo[userDataRef].str;
        [UsedImplicitly] set => rsz.userDataInfo[userDataRef].str = value;
    }

    public UserDataShell Copy() {
        return new(hash) {
            Value = Value
        };
    }

    public override string ToString() {
        return Value;
    }
}