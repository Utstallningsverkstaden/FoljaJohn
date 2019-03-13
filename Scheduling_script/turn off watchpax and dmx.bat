@ECHO OFF
REM TITLE Avslutar Skissaplikaton

REM ECHO Stänger av Skissprogram...
REM TASKKILL /IM JohnBauerPictureViewer.exe
REM timeout /t 5

ECHO Stänger av LED-paneler...
REM Kör aux timeline "DMX off"
tst10.exe /r:watchpax-DMX_off.txt
timeout /t 5

ECHO Stänger av Watchpax...
REM Skicka powerDown
tst10.exe /r:watchpax-powerDown.txt
timeout /t 2

REM ECHO Stänger av Projektor 1...
REM pjlink.exe -p 192.168.0.10:4352 power off
REM timeout /t 3

REM ECHO Stänger av Projektor 2...
REM pjlink.exe -p 192.168.0.11:4352 power off
REM timeout /t 3

EXIT