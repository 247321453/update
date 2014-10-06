@echo off
cd /d "%~dp0"
if exist update_new.exe move /y update_new.exe update.exe
start update.exe
exit