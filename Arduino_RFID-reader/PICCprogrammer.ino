
/*******************************************************
  This program is used to write data to PICC's using a
  RC522 RFID-reader. The data is accepted through serial
  communication and is then written into the selected
  data block on the PICC.

  Author: Richard Lowenrud
*******************************************************/

#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN 7      //Slave select pin
#define RST_PIN 8     //Reset pin

MFRC522 mfrc522(SS_PIN, RST_PIN);
MFRC522::MIFARE_Key key;

//Variables used to program PICC
int block = 1;
int result = 0;
bool rfInUse = false;
byte blockcontent[16] = {"A"};
byte readbackblock[18];
byte RFCfgReg = 0x26 << 1;

void setup() {

    //Initialize serial communication
    Serial.begin(9600);
    while (!Serial);

    //Initialize reader
    SPI.begin();
    InitializeRC522();

    //Create key used for programming the card
    for (byte i = 0; i < 6; i++){
        //keyByte is defined in the "MIFARE_Key" 'struct' definition in the .h file of the library
        key.keyByte[i] = 0xFF;
    }
    Serial.println("Looking for new card");
}

void loop() {

    //Look for new card
    if ( ! mfrc522.PICC_IsNewCardPresent() ) {
        return;
    }
    //Can the UID be read
    if ( ! mfrc522.PICC_ReadCardSerial() ) {
        return;
    }

    //Wait for data
    Serial.println("Card found, waiting for data to be written");
    while(Serial.available() == 0){};
    Serial.readBytes(blockcontent, 16);
    result = writeBlock(block, blockcontent);
    if(result != 0){
        Serial.println("Failed writing, restarting RFID reader");
        delay(500);
        InitializeRC522();
        return;       //Error when writing, handled within function
    }
    Serial.println("Data was written successfully");
    result = readBlock(block, readbackblock);
    if(result != 0){
        return;       //Error when reading, handled within function
    }
    Serial.print("Reading block: ");
    for (int j=0 ; j<16 ; j++){ Serial.write (readbackblock[j]); }
    Serial.println("");
    Serial.println("Reading done");
    InitializeRC522();
    delay(5000);
    Serial.println("Ready for new card");
}

//Has to be done between each write
void InitializeRC522(){
    mfrc522.PCD_Init();
    mfrc522.PCD_WriteRegister(RFCfgReg, (0x07 << 4));
    mfrc522.PCD_AntennaOn();
}

int writeBlock(int blockNumber, byte arrayAddress[]){
  
    //Makes sure that we only write into data blocks
    int largestModulo4Number=blockNumber/4*4;
    int trailerBlock=largestModulo4Number+3;
    if (blockNumber > 2 && (blockNumber+1)%4 == 0){ return 2; } //block number is a trailer block
    
    //Authenticate block for writing
    byte status = mfrc522.PCD_Authenticate(MFRC522::PICC_CMD_MF_AUTH_KEY_A, trailerBlock, &key, &(mfrc522.uid));
    if (status != MFRC522::STATUS_OK) { 
        Serial.print("PCD_Authenticate() failed: ");
        Serial.println(mfrc522.GetStatusCodeName(status));
        return 3; 
    }

    //Write to block
    status = mfrc522.MIFARE_Write(blockNumber, arrayAddress, 16);
    if (status != MFRC522::STATUS_OK) { 
        Serial.print("MIFARE_Write() failed: ");
        Serial.println(mfrc522.GetStatusCodeName(status));
        return 4;
    }
    return 0;
}

int readBlock(int blockNumber, byte arrayAddress[]){
  
    //Determine trailer block for the sector
    int largestModulo4Number=blockNumber/4*4;
    int trailerBlock=largestModulo4Number+3;

    //Authenticate block for reading
    byte status = mfrc522.PCD_Authenticate(MFRC522::PICC_CMD_MF_AUTH_KEY_A, trailerBlock, &key, &(mfrc522.uid));
    if (status != MFRC522::STATUS_OK) { 
        Serial.print("PCD_Authenticate() failed: ");
        Serial.println(mfrc522.GetStatusCodeName(status));
        return 3;
    }

    //Read from block
    byte buffersize = 18;
    status = mfrc522.MIFARE_Read(blockNumber, arrayAddress, &buffersize);
    if (status != MFRC522::STATUS_OK) {
        Serial.print("MIFARE_Write() failed: ");
        Serial.println(mfrc522.GetStatusCodeName(status));
        return 4;
    }
    return 0;
}



























