@echo off
set cmd=%3
set cmd=%cmd:"=%
%1 %2 %cmd%

if %ERRORLEVEL%==0 exit
echo Build failed!
pause