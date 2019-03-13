@ECHO OFF
TITLE Avslutar Skissaplikaton

ECHO Stänger av Skissprogram...
TASKKILL /IM JohnBauerPictureViewer.exe
timeout /t 5

ECHO Stänger av LED-paneler...
REM Kör aux timeline "DMX off"
tst10.exe /r:watchpax-DMX_off.txt
timeout /t 5

ECHO Stänger av Watchpax...
REM Skicka powerDown
tst10.exe /r:watchpax-powerDown.txt
timeout /t 2

ECHO Stänger av Projektor 1...
pjlink.exe -p 192.168.0.10:4352 power off
timeout /t 3

ECHO Stänger av Projektor 2...
pjlink.exe -p 192.168.0.11:4352 power off
timeout /t 3

EXIT