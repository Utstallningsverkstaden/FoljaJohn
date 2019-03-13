@ECHO OFF
TITLE Startar Watchpax

ECHO Startar Watchout...
REM Skicka wake-on-lan
WolCmd.exe 002098036439 255.255.255.255 255.255.255.255 4343
timeout /t 5

EXIT