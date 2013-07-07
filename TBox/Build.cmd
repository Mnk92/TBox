@echo off

set corflags="c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\corflags.exe"
if exist "%WinDir%\Microsoft.NET\Framework\" set FRAMEWORK=Framework
if exist "%WinDir%\Microsoft.NET\Framework64\" set FRAMEWORK=Framework64

"%WinDir%\Microsoft.NET\%FRAMEWORK%\v4.0.30319\MSBuild.exe" "%CD%\TBox.sln" /m /t:rebuild /p:Configuration="Release" /p:Platform="Any Cpu"
copy /Y "%CD%\bin\Release\TBox.exe" "%CD%\bin\Release\TBox32.exe" 
copy /Y "%CD%\bin\Release\TBox.exe.config" "%CD%\bin\Release\TBox32.exe.config" 
%corflags% /32bit+ "%CD%\bin\Release\TBox32.exe"

if %ERRORLEVEL%==0 goto script_end
echo Build failed!
pause

:script_end