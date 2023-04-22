RE-Editor
---

RE data file editor.

The main branch is devoid of game specific implementations and will not compile by itself. Checkout one of the game specific branches to build:<br>
This is also a list of supported games.
- `mhr`
- `re4`

Everything's in the [wiki](https://github.com/Synthlight/MHR-Editor/wiki).


Building
---

Game text is pre-extracted and committed to the repo since it's small enough.<br>
Structs/enums however *must* be generated before the editor will compile. See [here](Generator/README.md) for more.