@echo off

if exist "%WinDir%\Microsoft.NET\Framework\" set FRAMEWORK=Framework
if exist "%WinDir%\Microsoft.NET\Framework64\" set FRAMEWORK=Framework64

"%WinDir%\Microsoft.NET\%FRAMEWORK%\v4.0.30319\MSBuild.exe" wix\install.wixproj /m /t:build /p:Configuration="Release" /p:SolutionDir=%CD%\

if %ERRORLEVEL%==0 goto script_end
echo Build failed!
pause

:script_end