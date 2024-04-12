﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using RE_Editor.Common;
using RE_Editor.Common.Models;

namespace RE_Editor.Models;

public interface INexusMod {
    public string                      Name            { get; set; }
    public string                      Desc            { get; set; }
    public string                      Version         { get; set; }
    public string?                     Image           { get; set; }
    public IEnumerable<string>         Files           { get; set; }
    public Action<List<RszObject>>?    Action          { get; set; }
    public bool                        ForGp           { get; set; }
    public string?                     NameAsBundle    { get; set; }
    public bool                        SkipPak         { get; set; }
    public Dictionary<string, string>? AdditionalFiles { get; set; }
}

public struct NexusMod : INexusMod {
    public string                      Name            { get; set; }
    public string                      Desc            { get; set; }
    public string                      Version         { get; set; }
    public string?                     Image           { get; set; }
    public IEnumerable<string>         Files           { get; set; }
    public Action<List<RszObject>>?    Action          { get; set; }
    public bool                        ForGp           { get; set; }
    public string?                     NameAsBundle    { get; set; }
    public bool                        SkipPak         { get; set; }
    public Dictionary<string, string>? AdditionalFiles { get; set; }

    [Pure]
    public readonly override string ToString() {
        return $"{NameAsBundle} :: {Name}";
    }
}

public static class NexusModExtensions {
    public static T SetName<T>(this T nexusMod, string name) where T : INexusMod {
        nexusMod.Name = name;
        return nexusMod;
    }

    public static T SetDesc<T>(this T nexusMod, string desc) where T : INexusMod {
        nexusMod.Desc = desc;
        return nexusMod;
    }

    public static T SetVersion<T>(this T nexusMod, string version) where T : INexusMod {
        nexusMod.Version = version;
        return nexusMod;
    }

    public static T SetImage<T>(this T nexusMod, string image) where T : INexusMod {
        nexusMod.Image = image;
        return nexusMod;
    }

    public static T SetFiles<T>(this T nexusMod, IEnumerable<string> files) where T : INexusMod {
        nexusMod.Files = files;
        return nexusMod;
    }

    public static T SetAction<T>(this T nexusMod, Action<List<RszObject>> action) where T : INexusMod {
        nexusMod.Action = action;
        return nexusMod;
    }

    public static T SetForGp<T>(this T nexusMod, bool forGp) where T : INexusMod {
        nexusMod.ForGp = forGp;
        return nexusMod;
    }

    public static T SetNameAsBundle<T>(this T nexusMod, string nameAsBundle) where T : INexusMod {
        nexusMod.NameAsBundle = nameAsBundle;
        return nexusMod;
    }

    public static T SetSkipPak<T>(this T nexusMod, bool skipPak) where T : INexusMod {
        nexusMod.SkipPak = skipPak;
        return nexusMod;
    }

    public static T SetAdditionalFiles<T>(this T nexusMod, Dictionary<string, string> additionalFiles) where T : INexusMod {
        nexusMod.AdditionalFiles = additionalFiles;
        return nexusMod;
    }

    public static IEnumerable<Type> GetAllModTypes() {
        var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                    where type.Is(typeof(IMod))
                    select type;
        return types;
    }
}