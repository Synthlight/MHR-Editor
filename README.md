RE-Editor
---

RE data file editor.

Supported games:
- `DD2`
- `MHR`
- `RE2`
- `RE3`
- `RE4`

To build for a game, switch to that configuration: `{Game} - Debug/Release`

Everything else is on the [wiki](https://github.com/Synthlight/MHR-Editor/wiki). (Though not much.)


Building
---

Game text is pre-extracted and committed to the repo since it's small enough.<br>
Structs/enums however *must* be generated before the editor will compile. See [here](Generator/README.md) for more.