### Prerequisites
- Python 3.8.x. Anything below 3.9. See [REFramework's doc on these tools](https://github.com/praydog/REFramework/blob/master/reversing/rsz/readme.md) for why.

## Arguments
| Name | Desc |
| --- | --- |
| \<None\> | Nothing will happen. Not even an error. |
| `all` | Run both steps. |
| `part1` | This is based around REF's `emulation-dumper.py`. This runs that with the required args and paths and such setup from `PathHelper`. The output is also a subfolder here and is used by part 2. |
| `part2` | This is based around REF's `non-native-dumper.py`. We run that with extra args to include parent struct info in the result. This is pretty much the only reason we have this tool at all; just for parent info in the dump. |