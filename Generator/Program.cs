namespace RE_Editor.Generator;

public static class Program {
    public static void Main(string[] args) {
        new GenerateFiles().Go(args);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}