using MHR_Editor.Data;

namespace Tests;

[TestClass]
public class Setup {
    [AssemblyInitialize]
    public static void Init(TestContext _) {
        DataInit.Init();
    }
}