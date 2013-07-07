@echo off

set corflags="c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\corflags.exe"
if exist "%WinDir%\Microsoft.NET\Framework\" set FRAMEWORK=Framework
if exist "%WinDir%\Microsoft.NET\Framework64\" set FRAMEWORK=Framework64

copy /Y "%CD%\bin\Release\TBox.exe" "%CD%\bin\Release\TBox32.exe" 
copy /Y "%CD%\bin\Release\TBox.exe.config" "%CD%\bin\Release\TBox32.exe.config" 
%corflags% /32bit+ "%CD%\bin\Release\TBox32.exe"

copy /Y "%CD%\bin\Release\Tools\ConsoleUnitTestsRunner.exe" "%CD%\bin\Release\Tools\ConsoleUnitTestsRunner32.exe" 
%corflags% /32bit+ "%CD%\bin\Release\Tools\ConsoleUnitTestsRunner32.exe"


if %ERRORLEVEL%==0 goto script_end
echo Build failed!
pause

:script_end