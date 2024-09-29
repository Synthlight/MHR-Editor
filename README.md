RE-Editor
---

RE data file editor.

Supported games:

Short | Long
--- | ---
DD2 | Dragon Age 2
DRDR | Dead Rising Deluxe Remaster
MHR | Monster Hunter Rise
RE2 | Resident Evil 2
RE3 | Resident Evil 3
RE4 | Resident Evil 4
RE8 | Resident Evil Village

(Almost everything in the code and prog arguments use the short name; case sensitive.)<br>
To build for a game, switch to that configuration: `{Game} - Debug/Release`

Everything else is on the [wiki](https://github.com/Synthlight/MHR-Editor/wiki). (Though not much.)


Building
---

Game text is pre-extracted and committed to the repo since it's small enough.<br>
Structs/enums however *must* be generated before the editor will compile. See [here](Generator/README.md) for more.