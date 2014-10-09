@echo off
cd /d "%~dp0"
if exist update_new.exe move /y update_new.exe update.exe
start update.exe -d "%~dp0MagicSetEditor2-NI" "https://github.com/247321453/MagicSetEditor2-NI/raw/master/"
exit