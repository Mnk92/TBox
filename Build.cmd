@echo off

set corflags="c:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\corflags.exe"
set msbuild="c:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe"

%msbuild% "%CD%\TBox.sln" /m /t:rebuild /p:Configuration="Release" /p:Platform="Any Cpu"
copy /Y "%CD%\bin\Release\TBox.exe" "%CD%\bin\Release\TBox32.exe" 
copy /Y "%CD%\bin\Release\TBox.exe.config" "%CD%\bin\Release\TBox32.exe.config" 
%corflags% /32bit+ "%CD%\bin\Release\TBox32.exe"

copy /Y "%CD%\bin\Release\Tools\ConsoleUnitTestsRunner.exe" "%CD%\bin\Release\Tools\ConsoleUnitTestsRunner32.exe" 
%corflags% /32bit+ "%CD%\bin\Release\Tools\ConsoleUnitTestsRunner32.exe"

if %ERRORLEVEL%==0 goto script_end
echo Build failed!
pause

:script_end