
/*******************************************************
  This program is used to read data from a PICC's using 
  a RC522 RFID-reader and output it as keyboard presses. 
  Data is read from the selected block and the 16 bytes 
  contained in the block is stored in an array.

  Author: Richard Lowenrud
*******************************************************/

#include <SPI.h>
#include <MFRC522.h>
#include <MFRC522Extended.h>
#include <Keyboard.h>


#define SS_PIN 7              //Slave select pin
#define RST_PIN 8             //Reset pin

#define LED1_PIN 5            //Led active while waiting
#define LED2_PIN 6            //Led active when tag present 

int readDelay = 250;          //Delay in ms before trying to read data
int block = 1;                //Which block in the tag to be read
byte readbackblock[18];       //Array to hold read data
byte RFCfgReg = 0x26 << 1;    //Register for antenna gain

bool keyboard = true;        //True to output keyboard presses, otherwise serial
bool rfInUse = false;         //If card is active or not
bool readNewCard = false;     //If card has been read

MFRC522 mfrc522(SS_PIN, RST_PIN);
MFRC522::MIFARE_Key key;

void setup() {

    //Led setup
    pinMode(LED1_PIN, OUTPUT);
    pinMode(LED2_PIN, OUTPUT);
    pinMode(LED_BUILTIN, OUTPUT);
    
    //Init communication
    if(keyboard){
        Keyboard.begin();
    }else{
        Serial.begin(9600);
        while (!Serial);
    }

    //Initialize reader
    SPI.begin();
    InitializeRC522();

    //Create key used for programming the card
    createKey();
    delay(500);
    switchLeds(false);
}

void loop() {
  
    bool rfCheck = (mfrc522.PICC_IsNewCardPresent() || mfrc522.PICC_IsNewCardPresent() || mfrc522.PICC_IsNewCardPresent()); 
    if(rfCheck != rfInUse){
        if(rfInUse != (mfrc522.PICC_IsNewCardPresent() || mfrc522.PICC_IsNewCardPresent() || mfrc522.PICC_IsNewCardPresent())){ //Double check one more time.
            rfInUse = rfCheck;
            if(rfInUse){
                if(mfrc522.PICC_ReadCardSerial()){
                    byte readStatus = readBlock(block, readbackblock);
                    InitializeRC522();
                    if(readStatus == 0){
                        readNewCard = true;
                        switchLeds(true);
                        if(keyboard){
                            Keyboard.write(readbackblock[0]);
                        }else{
                            Serial.write(readbackblock[0]);
                        }
                    }
                }
            }else if(readNewCard){
                switchLeds(false);
                readNewCard = false;
                if(keyboard){
                    Keyboard.write(readbackblock[1]);
                }else{
                    Serial.write(readbackblock[1]);
                }
            }
        }
    }
}

//Has to be done between each write
void InitializeRC522(){
    mfrc522.PCD_Init();
    mfrc522.PCD_WriteRegister(RFCfgReg, (0x07 << 4));
    mfrc522.PCD_AntennaOn();
}

//Inaktiverad kod för två lysdioder /Oscar Engberg
//void switchLeds(bool state){
//    if(state){
//        digitalWrite(LED1_PIN, LOW);
//        digitalWrite(LED2_PIN, HIGH);
//        digitalWrite(LED_BUILTIN, LOW);
//    }else{
//        digitalWrite(LED1_PIN, HIGH);
//        digitalWrite(LED2_PIN, LOW);
//        digitalWrite(LED_BUILTIN, HIGH);
//    }
//}

//Start moddad kod för en lysdiod /Oscar Engberg
void switchLeds(bool state) {
  if (state) {
    digitalWrite(LED1_PIN, HIGH);
    digitalWrite(LED_BUILTIN, LOW);
  } else {
    analogWrite(LED1_PIN, 40);
    digitalWrite(LED_BUILTIN, HIGH);

  }
}
//Slut på moddad kod /Oscar Engberg



int readBlock(int blockNumber, byte arrayAddress[]){
  
    //Determine trailer block for the sector
    int largestModulo4Number=blockNumber/4*4;
    int trailerBlock=largestModulo4Number+3;
    byte status;
    //delay(readDelay);
    
    //Authenticate block for reading
    status = mfrc522.PCD_Authenticate(MFRC522::PICC_CMD_MF_AUTH_KEY_A, trailerBlock, &key, &(mfrc522.uid));
    if (status != MFRC522::STATUS_OK) { 
        Serial.print("PCD_Authenticate() failed: ");
        Serial.println(mfrc522.GetStatusCodeName(status));
        //mfrc522.PCD_StopCrypto1();
        return 3;
    }

    //Read from block
    byte buffersize = 18;
    status = mfrc522.MIFARE_Read(blockNumber, arrayAddress, &buffersize);
    if (status != MFRC522::STATUS_OK) {
        Serial.print("MIFARE_Read() failed: ");
        Serial.println(mfrc522.GetStatusCodeName(status));
        //mfrc522.PCD_StopCrypto1();
        return 4;
    }
    //mfrc522.PCD_StopCrypto1();
    return 0;
}

int retryReading(){
    
    InitializeRC522();
    createKey();
    delay(1000);
    if(mfrc522.PICC_IsNewCardPresent()){
        byte status = readBlock(block, readbackblock);
        if(status == 0){
            return status;
        }else{
            return retryReading();
        }
    }
    return 2;
}

void createKey(){
    for (byte i = 0; i < 6; i++){
        //keyByte is defined in the "MIFARE_Key" 'struct' definition in the .h file of the library
        key.keyByte[i] = 0xFF;
    }
}
