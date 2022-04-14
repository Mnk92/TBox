@echo off

set msbuild="c:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

"%msbuild%" TBox\wix\install.wixproj /m /t:build /p:Configuration="Release" /p:SolutionDir=%CD%\TBox\

if %ERRORLEVEL%==0 goto script_end
echo Build failed!
pause

:script_end