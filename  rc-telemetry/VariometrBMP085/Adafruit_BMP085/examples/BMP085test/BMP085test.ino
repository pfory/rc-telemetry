#include <Wire.h>
#include <Adafruit_BMP085.h>

/*************************************************** 
  This is an example for the BMP085 Barometric Pressure & Temp Sensor

  Designed specifically to work with the Adafruit BMP085 Breakout 
  ----> https://www.adafruit.com/products/391

  These displays use I2C to communicate, 2 pins are required to  
  interface
  Adafruit invests time and resources providing this open source code, 
  please support Adafruit and open-source hardware by purchasing 
  products from Adafruit!

  Written by Limor Fried/Ladyada for Adafruit Industries.  
  BSD license, all text above must be included in any redistribution
 ****************************************************/

// Connect VCC of the BMP085 sensor to 3.3V (NOT 5.0V!)
// Connect GND to Ground
// Connect SCL to i2c clock - on '168/'328 Arduino Uno/Duemilanove/etc thats Analog 5
// Connect SDA to i2c data - on '168/'328 Arduino Uno/Duemilanove/etc thats Analog 4
// EOC is not used, it signifies an end of conversion
// XCLR is a reset pin, also not used here

Adafruit_BMP085 bmp;
boolean logging = true;
int citac=0;
float alt=0;
  
void setup() {
  Serial.begin(9600);
  bmp.begin(BMP085_HIGHRES);  
}
  
void loop() {
        alt+=bmp.readAltitude
        ();

        citac++;
  
        if (citac>19
        )
        {      
  	  if (logging)
	  {
		//Serial.print(bmp.readTemperature());
                //Serial.print(";");
		//Serial.print(bmp.readPressure());
                //Serial.print(";");
		Serial.println(alt/19
);
                //Serial.print(";");
                
		//Serial.print(bmp.readSeaLevelAltitude(360));
	  }
	  else
	  {
		Serial.print("Temperature = ");
		Serial.print(bmp.readTemperature());
		Serial.println(" *C");
		
		Serial.print("Pressure = ");
		Serial.print(bmp.readPressure());
		Serial.println(" Pa");
		
		// Calculate altitude assuming 'standard' barometric
		// pressure of 1013.25 millibar = 101325 Pascal
		Serial.print("Altitude = ");
		Serial.print(bmp.readAltitude());
		Serial.println(" meters");

	  // you can get a more precise measurement of altitude
	  // if you know the current sea level pressure which will
	  // vary with weather and such. If it is 1015 millibars
	  // that is equal to 101500 Pascals.
		Serial.print("Real altitude = ");
		Serial.print(bmp.readAltitude(100990));
		Serial.println(" meters");

		//Serial.print("Pressure at sea level = ");
		//Serial.print(bmp.readSeaLevelAltitude(340));
		//Serial.println(" Pa");
	  }
          citac=0;
          alt=0;
        }

          
        
        
 //   Serial.println();
    delay(50);
}
