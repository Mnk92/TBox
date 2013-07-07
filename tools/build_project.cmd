cls
@echo off

set t=%2
call :unquote t %t%

:loop
for /f "tokens=1* delims=*" %%a in ("%t%") do (
   call :build %1 "%%a" %3
   set t=%%b
   )  
if defined t goto :loop

goto :EOF

:unquote
  set %1=%~2
  goto :EOF
  
:build  
  %1 %2 /m /t:rebuild /p:Configuration=%3   
  echo %ERRORLEVEL%
  if %ERRORLEVEL% NEQ 0 goto :showerror
 goto :EOF

:showerror
  echo Build failed!
  pause
goto :EOF