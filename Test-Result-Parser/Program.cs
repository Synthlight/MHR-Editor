using System.Text.RegularExpressions;

namespace RE_Editor.ID_Parser;

public static class Program {
    private const           string TEST_RESULTS_PATH = @"..\..\..\TestResults";
    private static readonly Regex  GOOD_REGEX        = new(@"TestWriteUserFile.+(natives[a-zA-Z0-9_\\.]+)");

    public static void Main() {
        foreach (var file in Directory.EnumerateFiles(TEST_RESULTS_PATH, "*.html", SearchOption.TopDirectoryOnly)) {
            var paths = from line in File.ReadAllLines(file)
                        where line.Contains('✔')
                        let match = GOOD_REGEX.Match(line)
                        where match.Success
                        orderby line.ToLower()
                        select match.Groups[1].Value;
            File.WriteAllLines(file.Replace(".html", ".txt"), paths);
        }
    }
}