@ECHO OFF
REM TITLE Avslutar Skissaplikaton

REM ECHO St�nger av Skissprogram...
REM TASKKILL /IM JohnBauerPictureViewer.exe
REM timeout /t 5

ECHO St�nger av LED-paneler...
REM K�r aux timeline "DMX off"
tst10.exe /r:watchpax-DMX_off.txt
timeout /t 5

ECHO St�nger av Watchpax...
REM Skicka powerDown
tst10.exe /r:watchpax-powerDown.txt
timeout /t 2

REM ECHO St�nger av Projektor 1...
REM pjlink.exe -p 192.168.0.10:4352 power off
REM timeout /t 3

REM ECHO St�nger av Projektor 2...
REM pjlink.exe -p 192.168.0.11:4352 power off
REM timeout /t 3

EXIT