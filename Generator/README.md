Enum & Struct Generation
---

This needs to be run before the editor will even compile. This generates all the needed files from parsing the struct & enum dumps.

Arguments
---

| Name | Desc |
| --- | --- |
| useWhitelist | By default the generator creates 50k+ files for all the structs. This makes debugging a nightmare so this option was added.<br>Edit the whitelist in `GenerateFiles.cs` is you need to. |
| dryRun | Writes to throwaway memory streams instead of files. This makes unit testing possible. |