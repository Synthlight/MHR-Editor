using RE_Editor.Data;

namespace RE_Editor.Tests;

[TestClass]
public class Setup {
    [AssemblyInitialize]
    public static void Init(TestContext _) {
        DataInit.Init();
    }
}