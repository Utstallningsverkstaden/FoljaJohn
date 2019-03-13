@ECHO OFF
TITLE Startar Skissaplikaton

ECHO Startar projektor 1...
pjlink.exe -p 192.168.0.10:4352 power on
timeout /t 3

ECHO Startar projektor 2...
pjlink.exe -p 192.168.0.11:4352 power on
timeout /t 60

ECHO Startar Watchout...
REM Skicka wake-on-lan
WolCmd.exe 002098036439 255.255.255.255 255.255.255.255 4343
timeout /t 20

ECHO Startar Skissprogram...
START C:\Users\Kiosk\Desktop\JohnBauerPictureViewer.lnk

EXIT