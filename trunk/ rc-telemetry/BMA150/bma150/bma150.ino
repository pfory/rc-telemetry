#include <Wire.h>

//SCL - yellow - PC5 (analog5) - pin 28
//SDA - brown  - PC4 (analog4) - pin 27


void setup()
{
  Wire.begin();        // join i2c bus (address optional for master)
  Serial.begin(9600);  // start serial for output
}

void loop()
{
  Wire.beginTransmission(0x38); // transmit to device #112 (0x70)
                               // the address specified in the datasheet is 224 (0xE0)
                               // but i2c adressing uses the high 7 bits so it's 112
  Wire.write(byte(0x00));      // sets register pointer to the command register (0x00)  
  Wire.endTransmission();      // stop transmitting


  Wire.requestFrom(0x38, 9);    // request 6 bytes from slave device #2

 int chipID = Wire.read();    
  Serial.print("ChipID:");     
  Serial.print(chipID);       
  Serial.print(" ");         

  int ver = Wire.read();    
  Serial.print("ml version:");     
  Serial.print(ver&0xF);       
  Serial.print(" al version:");     
  Serial.print((ver&0xF0)>>4);       
  Serial.print(" ");         

  Serial.print("acc_x:");     
  int adl = Wire.read();
  int adh = Wire.read();

  int acc = (adh << 2) + (adl >> 6); 
  Serial.print(acc);         // print the character
  Serial.print(",");         // print the character

  Serial.print("acc_y:");     
  adl = Wire.read();
  adh = Wire.read();

  acc = (adh << 2) + (adl >> 6); 
  Serial.print(acc);         // print the character
  Serial.print(",");         // print the character

  Serial.print("acc_z:");     
  adl = Wire.read();
  adh = Wire.read();

  acc = (adh << 2) + (adl >> 6); 
  Serial.print(acc);         // print the character

  Serial.print(" Temp:");     
  int temp = Wire.read();
  Serial.print(temp);         // print the character

  Serial.println("");         // print the character

  delay(50);
}  

/*0x70 write
0x71 read

i2c_start(0x70 I2C_WRITE); 
i2c_write(0x02); 
i2c_rep_start(0x70 I2C_READ); 

adl = i2c_readAck(); 
adh = i2c_readAck(); 
accX = (adh << 2) (adl >> 6); 

adl = i2c_readAck(); 
adh = i2c_readAck(); 
accY = (adh << 2) (adl >> 6); 

adl = i2c_readAck(); 
adh = i2c_readNak(); 
accZ = (adh << 2) (adl >> 6); 

i2c_stop(); */
