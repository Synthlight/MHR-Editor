Enum & Struct Generation
---

This needs to be run before the editor will even compile. This generates all the needed files from parsing the struct & enum dumps.

VS will try to read/open files if you generate with ti open and it can make a mess. It can also crash the generator due to VS holding a handle to a file we want to write to.<br>
For best results, close the solution in VS and run the generator.

Arguments
---

| Name | Desc |
| --- | --- |
| useWhitelist | By default the generator creates 50k+ files for all the structs. This makes debugging a nightmare so this option was added.<br>Edit the whitelist in `GenerateFiles.cs` if you need to. |
| dryRun | Writes to throwaway memory streams instead of files. This makes unit testing possible. |