call "C:\Program Files (x86)\Microsoft Visual Studio\2022\Community\Common7\Tools\VsDevCmd.bat"
MSBuild /nologo /t:Clean /p:Configuration=Debug RE-Editor.sln
MSBuild /nologo /t:Clean /p:Configuration=Release RE-Editor.sln
MSBuild /nologo /t:Build /p:Configuration=Debug RE-Editor.sln
MSBuild /nologo /t:Build /p:Configuration=Release RE-Editor.sln
pause