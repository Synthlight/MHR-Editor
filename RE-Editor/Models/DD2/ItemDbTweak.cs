﻿using System;
using System.Collections.Generic;
using System.IO;
using RE_Editor.Common.Models;

namespace RE_Editor.Models;

public interface IItemDbTweak {
    string                          LuaName { get; set; }
    public List<ItemDbTweak.Change> Changes { get; set; }
}

public struct ItemDbTweak : INexusMod, IItemDbTweak {
    public string                     Name            { get; set; }
    public string                     Desc            { get; set; }
    public string                     Version         { get; set; }
    public string                     Image           { get; set; }
    public IEnumerable<string>        Files           { get; set; }
    public Action<List<RszObject>>    Action          { get; set; }
    public bool                       ForGp           { get; set; }
    public string                     NameAsBundle    { get; set; }
    public bool                       SkipPak         { get; set; }
    public Dictionary<string, string> AdditionalFiles { get; set; }
    public string                     LuaName         { get; set; }
    public List<Change>               Changes         { get; set; }

    public struct Change {
        public Target               Target { get; set; }
        public Action<StreamWriter> Action { get; set; }
    }

    public enum Target {
        ARMOR,
        ITEM,
        WEAPON,
    }
}

public static class ItemDbTweakExtensions {
    public static T SetLuaName<T>(this T nexusMod, string luaName) where T : IItemDbTweak {
        nexusMod.LuaName = luaName;
        return nexusMod;
    }

    public static T SetChanges<T>(this T nexusMod, List<ItemDbTweak.Change> changes) where T : IItemDbTweak {
        nexusMod.Changes = changes;
        return nexusMod;
    }
}