@echo off
>nul 2>&1 "%SYSTEMROOT%\system32\cacls.exe" "%SYSTEMROOT%\system32\config\system"
if '%errorlevel%' NEQ '0' (
    echo request admin...
    goto UACPrompt
) else ( goto gotAdmin )
:UACPrompt
    echo Set UAC = CreateObject^("Shell.Application"^) > "%temp%\getadmin.vbs"
    set params = %*:"=""
    echo UAC.ShellExecute "cmd.exe", "/c %~s0 %params%", "", "runas", 1 >> "%temp%\getadmin.vbs"

    "%temp%\getadmin.vbs"
    rem del "%temp%\getadmin.vbs"
    exit /B

:gotAdmin
pushd "%CD%"
    CD /D "%~dp0"

nssm.exe install WEBTextLabelingSVC "java.exe" "-Xms4096m -Xmx4096m -jar WebTextLabeling.jar"
nssm.exe set WEBTextLabelingSVC AppDirectory "C:\LABELING_SERVER\backend"
nssm.exe start WEBTextLabelingSVC
