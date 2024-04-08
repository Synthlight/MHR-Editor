using System;
using System.IO;

namespace RE_Editor.Mods;

public readonly struct SwapDbTweak(string name, string desc, string luaFile, Action<StreamWriter> action) {
    public readonly string               name    = name;
    public readonly string               desc    = desc;
    public readonly string               luaFile = luaFile;
    public readonly Action<StreamWriter> action  = action;
}