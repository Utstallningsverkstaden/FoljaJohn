## RFID reader/programmer based on Arduino Micro

Reads/writes RFID tags and outputs as text via USB (HID keyboard emulation).
Outputs one character when RFID tag is placed on reader and another when tag is removed.

**Arduino-settings.txt** - board settings for uploading via Arduino

**PICCprogrammer.ino** - sketch used with Arduino to program RFID-tags. Open Serial monitor and follow instructions.

**PICCreader-One_LED.ino** - sketch used in standalone mode with LED connected to GPIO 5. Output is dim when no tag is present and high when tag is present

**PICCreader-Two_LED.ino** - sketch used in standalone mode with one LED connected to GPIO 5 and another to GPIO 6. Output is LOW on GPIO 5 and HIGH on GPIO 6 when tag is present and HIGH on GPIO 5 and LOW on GPIO 6 when tag is not present.

**PICCreader-connection_diagram.jpg**
