﻿using System.Diagnostics.CodeAnalysis;

namespace RE_Editor.Generator;

public partial class GenerateFiles {
    public  const string ROOT_STRUCT_NAMESPACE = "app";
    private const string ASSETS_DIR            = $@"{BASE_PROJ_PATH}\Data\DD2\Assets";

    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression")]
    private static readonly List<string> WHITELIST = [
        "App_WeaponCatalogData",
    ];
}