// Needed to keep `Assembly.Load(nameof(Generated));` in `DataInit` happy.
// Basically just ensures the namespace exists.
// Some configs might not need this, but having it costs nothing.
// ReSharper disable once UnusedType.Global
// Required to build things when there are no models/enums generated as the namespace won't exist from having the folders only.

// ReSharper disable UnusedType.Global

namespace RE_Editor.Generated {
    public static class Placeholder;
}

namespace RE_Editor.Generated.Enums {
    public static class Placeholder;
}

namespace RE_Editor.Models.Enums {
    public static class Placeholder;
}