import re
from xml.etree import ElementTree

doc = ElementTree.parse("RE-Editor/RE-Editor.csproj").getroot()
versions = {}

for node in doc.findall(".//PropertyGroup[@Condition]"):
    name = node.find(".//AssemblyName").text
    version = node.find(".//AssemblyVersion").text
    versions[name] = version

# Read the lines from README.md.
with open("README.md", "r") as file:
    lines = file.readlines()

# Update the linked releases in the table based on the current version in `RE-Editor.csproj`.
for i in range(len(lines)):
    match = re.compile(r"^([^|]+\s\|\s[^|]+\s\|)\s\[.+?tag/([^-]+-Editor).+?$").match(lines[i])
    if match is not None:
        lineStart = match.group(1)
        progType  = match.group(2)
        for name in versions:
            version = versions[name]
            if name == progType:
                lines[i] = f"{lineStart} [v{version}](https://github.com/Synthlight/MHR-Editor/releases/tag/{name}_v{version})\n"

# Write the updated lines back to README.md.
with open('README.md', 'w') as file:
    file.writelines(lines)