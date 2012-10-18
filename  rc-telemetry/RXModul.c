//#define LCD

//-----------------------------------------------------------------------------
//
// Include files
//
//-----------------------------------------------------------------------------

#define UART
#define watchdog
//#define debug //only for debugging, comment it in final build
//#define verbose

#ifdef UART
#include "uart.h"
#endif


#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#include <string.h>
#include "ds18x20.h"
#include "crc8.h"
#include "onewire.h"
#include "utils.h"
#include <avr/eeprom.h>
#ifdef watchdog
#include <avr/wdt.h>          //for watchdog
#endif
//#include "GPS.h"
#include "BMP085.h"

//-----------------------------------------------------------------------------
//
// Hardware notes
//
//-----------------------------------------------------------------------------

//POLSTAR GPS
//Output message NMEA0283 V2.2 GGA, GSV, GSA, RMC (optional VTG, GLL)
//Baud rate 4800 bps (Optional 9600,19300,38400 bps) - for this project is set to 9600
//Serial port TTL, RS232 - for this project TTL
//Navigation Update Rate once per second

//POLSTAR GPS
//pin signal        pin ATMEGA8
//------------------------------
//1   TTL Tx        2 Rxd PORTD 0
//2   TTL Rx
//3   VCC           7 VCC
//4   GND           8 GND
//5   RS232 Tx
//6   RS232 Rx


//-----------------------------------------------------------------------------
//
// Software notes
//
//-----------------------------------------------------------------------------

//Application protocol for data
// see datadecode.cs



//NMEA protocol
//http://www.abclinuxu.cz/clanky/ruzne/gps-a-komunikacni-protokol-nmea-3-dekodovani-dat


//-----------------------------------------------------------------------------
//
// Local variables
//
//-----------------------------------------------------------------------------
static uint32_t millis=0;


#ifdef debug
volatile int32_t tmp;
volatile uint8_t k;
#endif

//#define BAUD 4800  //baut rate bps
//#define UBRR 95// F_OSC/(16*BAUD)-1

//char buffer[84]="GPRMC,183729,A,3907.356,N,12102.482,W,000.0,360.0,080301,015.5,E*6F";
//char buffer[84]="GPGGA,170139.615,4912.2526,N,01635.0378,E,1,07,1.0,1357.5,M,43.5,M,0.0,0000*7D";
//char buffer[84]="GPGLL,4916.45,N,12311.12,W,225444,A,*1D";
//char buffer[84]="GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48";
#define bufferLength 90
char buffer[bufferLength];
char bufferGGA[bufferLength];
char bufferRMC[bufferLength];
char readyToSendGPSData=0;
uint32_t millisFromLastGPS=0;

//DALLAS
#define MAXSENSORS 4

static uint8_t gSensorIDs[MAXSENSORS][OW_ROMCODE_SIZE];
static uint8_t nSensors=0;
#define OW_ONE_BUS

static int16_t temperature[MAXSENSORS];

static uint16_t napeti[6];
static uint8_t ch;

#ifdef _debug
static uint16_t valid=0;
#endif

//static uint16_t koefEEP[6] EEMEM = {10400,11100,11300,10000,10000,13000};
static uint16_t koefEEP[6] EEMEM = {10244,10000,11351,10000,10000,10000};
static uint16_t koef[6];

//uint16_t koef[6];
static unsigned int ID EEMEM = 0xFFF;
static unsigned int id;

//1.1
//HDOP, VDOP, PDOP - 0.9 = 9 (nasobi se 10) //1.1
//static uint16_t hdop, pdop, vdop; //1.1


//static int16_t citac=0;

//Port with LED
#define LEDPORT 	PORTB
#define LEDP 		  2 
#define LEDDDR 		DDRB 


//BMP085
uint32_t startBMP085 = 0;
uint8_t BMP085MeasDuration=255;  //26ms from start meas to valid meas is minimum
//static uint32_t pressure=0;
void showall(void);
  
//DS18X20
uint32_t startDS18X20 = 0;
uint16_t DS18X20MeasDuration=750;  //750ms from start meas to valid meas is minimum

//ADC
uint32_t startADC = 0;
uint16_t ADCMeasDuration=1;  //1ms from start meas to valid meas is minimum


//-----------------------------------------------------------------------------
//
// Function prototypes
//
//-----------------------------------------------------------------------------

//non public function
static inline uint8_t search_sensors(void);
static void sendData16(uint8_t adresa, uint16_t hodnota);
static void sendData24(uint8_t adresa, uint32_t hodnota);
//static void sendData8(uint8_t adresa, uint8_t hodnota);
static inline void readTemp(uint8_t cidlo);
static void adc_start_conversion(void);
static void init(void);
static uint16_t crc(char *);
static void sendDataGPS(char *buffer, uint8_t prikaz);

//interfaces

//konstanty ve FLASH
char PRGMVER[] PROGMEM = "RXModul1.2";

//UART
static uint8_t counter=0;
static uint8_t start=0;


//-----------------------------------------------------------------------------
//
// searching DALLAS temperature sensor on bus
//
//-----------------------------------------------------------------------------
static inline uint8_t search_sensors(void)
{
	uint8_t i;
	uint8_t id[OW_ROMCODE_SIZE];
	uint8_t diff, nSensors;
	
	ow_reset();

	nSensors = 0;
	
	diff = OW_SEARCH_FIRST;
	while ( diff != OW_LAST_DEVICE && nSensors < MAXSENSORS ) {
		DS18X20_find_sensor( &diff, &id[0] );
		
		if( diff == OW_PRESENCE_ERR ) break;
		
		if( diff == OW_DATA_ERR ) break;
		
		for ( i=0; i < OW_ROMCODE_SIZE; i++ )
			gSensorIDs[nSensors][i] = id[i];
		
		nSensors++;
	}
	
	return nSensors;
}

//-----------------------------------------------------------------------------
//
// Read temperature from DALLAS sensor(s)
//
//-----------------------------------------------------------------------------
static inline void readTemp(uint8_t sensor) 
{
  if (DS18X20_read_decicelsius( &gSensorIDs[sensor][0], &temperature[sensor] ) 
			== DS18X20_OK )
	{
	}
}


//-----------------------------------------------------------------------------
//
// Send 8 bit data
//
//-----------------------------------------------------------------------------
//static void sendData8(uint8_t adresa, uint8_t hodnota) {
	//u_putc(adresa|0x80);
	//u_putc(hodnota);
//}
//
//-----------------------------------------------------------------------------
//
// Send 16 bit data
//
//-----------------------------------------------------------------------------
static void sendData16(uint8_t adresa, uint16_t hodnota) 
{
	u_putc(adresa|0x80);  //set highest bit on 1
	u_putc(hodnota>>7);
	u_putc(hodnota&0x7F); //shodim nejvyssi bit
}

//-----------------------------------------------------------------------------
//
// Send 24 bit data
//
//-----------------------------------------------------------------------------
static void sendData24(uint8_t adresa, uint32_t hodnota) 
{
	u_putc(adresa|0x80);
	u_putc(hodnota>>14);
	u_putc(hodnota>>7&0x7F);
	u_putc(hodnota&0x7F); //shodim nejvyssi bit
}


//*****************************************************************************
//
//  ADC single conversion routine 
//
//*****************************************************************************
static void adc_start_conversion() {
	//set ADC channel
	ADMUX=(ADMUX&0xF0)|ch;
	//Start conversion with Interupt after conversion
	ADCSR|=1<<ADSC;
}


//*****************************************************************************
//
//  Initial section
//
//*****************************************************************************
static void init() 
{
	//set up timer
	OCR1A=0x1CCD;     //compare register 7373
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x09;    //start timer1, prescaler 1,CTC
	//crystal 7 372 800 (F_OSC) / 7373 / 1 =~ 1000 times per second, 1ms

	TIMSK|=0x01;	  //enable interrupt TOIE0
	TCCR0=0x05;    //start timer1, prescaler 1024,CTC
	//crystal 7 372 800 (F_OSC) / 1024 / 256 =~ 28 times per second, 35.5ms

	//set up UART to 9600 baud
	init_uart((F_OSC / 16 / 9600)-1);
	
	if (bit_is_set(MCUCSR,3)) {
  	u_puts("STARTWDT\n");
	}
	else {
	  u_puts("START\n");
	}
	
	//set port with DALLAS temperature sensor(s)
	//ow_set_bus(&PINB,&PORTB,&DDRB,PB0);
	nSensors = search_sensors();
	
	u_puts("DALLAS:");
	u_puti(nSensors);
	u_puts("\n");

	//set up AD converter
	//select reference voltage
	ADMUX|=(1<<REFS1)|(1<<REFS0); //internal 2,56V reference
	ADCSR=(1<<ADEN)|(1<<ADIE)|(0<<ADFR)|(1<<ADIF)|(1<<ADPS2)| (1<<ADPS1)|(1<<ADPS0);

	//set up port with control LED
	setb(LEDDDR, LEDP);  //output
	clrb(LEDPORT, LEDP); //light on

  BMP085Init();
  
  u_puts("BMP085\n");
  
	id=eeprom_read_word(&ID);
	for (uint8_t i=0; i<6; i++) {
	  koef[i] = (uint32_t)eeprom_read_word(&koefEEP[i]);
	}	  
}



//*****************************************************************************
//
//  INTRERRUPTS
//
//*****************************************************************************

//-----------------------------------------------------------------------------
//
//  TIMER0 OVF vector
//  run each 35ms=28times a sec
//
//-----------------------------------------------------------------------------
ISR(TIMER0_OVF_vect)
{
  #ifdef watchdog
	wdt_reset();              //reset watchdog
	#endif
  
  if (readyToSendGPSData) {
      millisFromLastGPS=millis;
      sendData16(0x80,0xFFF); //ID
      
      setb(LEDPORT, LEDP);
      //u_putc('\n');
      //u_puti(++citac);
      //u_putc('.');
      sendDataGPS(bufferGGA, 0xA0);
      //u_putc('\n');
      sendDataGPS(bufferRMC, 0xA2);
      //u_putc('\n');
      readyToSendGPSData=0;
      clrb(LEDPORT, LEDP);
      sendData16(0x96,15384); //current in 10mA

  }  

  if (millisFromLastGPS+400>=millis) {
  
    if (startBMP085==0) {
      //start new meassurement
      u_puts("S:");
      BMP085startTemperature();
      BMP085startPressure();
      startBMP085=millis;
    }


    //time between start and read must be longer than 26ms
    if ((startBMP085+BMP085MeasDuration)<=millis) {
      /*u_puts("E:");
      int32_t press=BMP085readPressure();
      u_puti(press);
      u_putc(',');
      int16_t temp=BMP085readTemperature();
      u_puti(temp);
      u_putc('\n');*/
   	  sendData24(0x94, BMP085readPressure());  //pressure from BMP085
   	  sendData16(0x95, BMP085readTemperature());   //temperature from BMP085
      startBMP085=0;
    }

    //time between start and read must be longer than 750ms
    if (startDS18X20==0) {
      DS18X20_start_meas(DS18X20_POWER_PARASITE, NULL); //start temperature measuring
	    startDS18X20=millis;
    }    

  
    if (startDS18X20+DS18X20MeasDuration<=millis) {
	    for (uint8_t i=0; i<nSensors;i++)
	    {
   	    readTemp(i); 
  	    sendData16(0x8A+i,temperature[i]); //0x8A-0x8D
	    }
	    startDS18X20=0;
    }  


    //time between start and read must be longer than ms
    if (startADC==0) {
      adc_start_conversion();
      startADC=millis;
    }
    
    if (startADC+ADCMeasDuration<=millis) {
      sendData16(0x81+ch,(uint16_t)(napeti[ch] * koef[ch] / 10000));
      ch++;
        if (ch>5) ch=0;
    }
    

  }  

  /*for (uint8_t i=0; i<6; i++) {
	  adc_start_conversion();
    i++
	else ch=0;*/
 	//negb(LEDPORT, LEDP);

	//carriageFlag--;

	//if (carriageFlag==0 || citacTimer1==10)      //no valid data from GPS
  //{
////		resetGPSData();
    //citacTimer1=0;
  //}
//
	//sendData16(0,   id);          //ID
	/*sendData8 (0xA0,getLatS());   //latS
	sendData8 (0xA1,getLatM());   //latM
	sendData16(0xA2,getLatDM());  //LatDM
	sendData8 (0xA3,getLonS());   //LonS
	sendData8 (0xA4,getLonM());   //LonM
	sendData16(0xA5,getLonDM());  //LonDM
 	sendData16(0xA6,getH());      //altitude
 	sendData16(0xA7,getV());      //speed
 	sendData16(0xA8,getK());      //azimuth
 	sendData8 (0xA9,getSIV());    //sat in view
	sendData24(0xAA,getDate());   //GPS date
 	sendData24(0xAB,getTime());   //GPS time
 	sendData16(0xAC,getDOP('H')); //HDOP
 	sendData16(0xAD,getDOP('V')); //VDOP
 	sendData16(0xAE,getDOP('P')); //PDOP
 	sendData8 (0xAF,getFix());    //Fix
*/
  
}

  
//-----------------------------------------------------------------------------
//
//  TIMER COMPA vector
//  run each 1ms
//
//-----------------------------------------------------------------------------
ISR(TIMER1_COMPA_vect)
{
  millis++;
  return;
}  
 



//-----------------------------------------------------------------------------
//
//  ADC conversion complete service routine 
//
//-----------------------------------------------------------------------------
ISR(ADC_vect)
{
	uint16_t adc_value;
	adc_value = ADCL;
	/*shift from low level to high level ADC, from 8bit to 10bit*/
	adc_value = (ADCH<<8);

	//debug
	#ifdef _debug
	adc_value=546;
	#endif
  napeti[ch]=adc_value;
}

//-----------------------------------------------------------------------------
//
//  USART RX interrupt from GPS
//
//-----------------------------------------------------------------------------
ISR(USART_RXC_vect) {
	//carriageFlag=44;	
	uint8_t receivedChar=u_getc();    //receive char

  if (receivedChar=='$') {      //start of NMEA sentence
		start=1;
		counter=0;
		memset(buffer,'\0',bufferLength);
	}

	if (receivedChar==0x0D && start==1) //end of NMEA sentence 0D 0A
	{ 
  		start=0;
		//buffer[counter]='\0';

		//calculate check sum
		uint8_t k=crc(buffer);

		char bks[3];
		bks[0]=buffer[strstr(buffer,"*")-buffer+1];
		bks[1]=buffer[strstr(buffer,"*")-buffer+2];
		bks[2]='\0';
		uint16_t kk=strtol(bks,NULL,16);

		#ifdef verbose
		u_puts(buffer);
		#endif //verbose
	
		//compare calculated and computed checksum
		if (k==kk) 
		{ //checksums are same = valid data		
  	  #ifdef verbose
			u_puts("-CRC:");
			u_puti(k);
			u_puts(" ");
			u_puti(kk);
			u_puts("\n");
			#endif //verbose

  		if (strstr(buffer, "GGA")>0) //GGA sentence
			{
        strcpy(bufferGGA, buffer);
			}

			if (strstr(buffer, "RMC")>0) //RMC sentence
			{
        strcpy(bufferRMC, buffer);
        readyToSendGPSData=1;
			}
		}
		else
		{
			//checksums invalid
  			#ifdef verbose
			u_puts("CRC ERROR\n");
			#endif //verbose
		}
  
	}

	if (start==1) 
	{
		buffer[counter]=receivedChar;
		counter++;
	}

	if (counter>89) 
	{
 	  #ifdef verbose
	  u_puts("Counter>89\n");
	  #endif
		start=0;
	}
  
}


//*****************************************************************************
//
//  MAIN
//
//*****************************************************************************
int main (void)
{
  /*napeti[0]=0xFFFF;
  napeti[0]+=0xFFFF;
  napeti[0]+=0xAB7F;
  citacTimer0=3;
  koef[0]=10244;
  sendData16(0x81,(uint16_t)((napeti[0] / citacTimer0) * koef[0] / 10000));
  */
/*
  //strcpy(buffer,"$GPGGA,154135.935,4943.8467,N,01323.6351,E,1,03,6.8,311.4,M,46.4,M,,0000*5D");
	//volatile uint8_t k=crc(buffer);
	//u_puti(k);
	//#ifdef debug
	////RMC
	//strcpy(buffer,"GPRMC,183729,A,3907.356,N,12102.482,W,000.0,360.0,240311,015.5,E*6F");
	//setDate(buffer,9);
	//setTime(buffer,1);
  //if (setDatum(buffer,9)!=15992)
  //{
    //goto error;
  //}
//
	//if (setTime(buffer,1)!=38062)
  //{
    //goto error;
  //}
	//
  //setLat(buffer,3);
  //if ((getLatS()!=39) || (getLatM()!=07) || getLatDM()!=365)
  //{
    //goto error;
  //}
//
	//setLon(buffer,5);
	//setAzimuth(buffer,8);
	//setSpeed(buffer,7);
//
	//tmp=getDate(); //43949
	//tmp=getTime(); //39984
	//tmp=getLatS(); //39
	//tmp=getLatM(); //7
	//tmp=getLatDM();//356
	//tmp=getLonS(); //-121
	//tmp=getLonM(); //2
	//tmp=getLonDM();//482
//
//
	////GGA
  //strcpy(buffer,"$GPGGA,183730,3907.356,N,12102.482,W,1,05,1.6,646.4,M,-24.1,M,,*75");
	//setLat(buffer,2);
	//setLon(buffer,4);
	//setAlt(buffer,9);
	//setSatInView(buffer,7);
	////setCas(buffer,1);//1.1
	////setDOP(buffer,8,'H');//1.1
//
//	tmp=getLatS();  //49
	//tmp=getLatM();  //12
	//tmp=getLatDM(); //252
	//tmp=getLonS();  //16
	//tmp=getLonM();	//35
	//tmp=getLonDM(); //37
	//tmp=getH(); 	//1357	//vyska
	//tmp=getSIV();	//7
	//tmp=getTime();	//56189
	//tmp=getDOP('H');//10
//
//
	////GSA
	//strcpy(buffer,"GPGSA,A,3,04,05,,09,12,,,24,,,,,2.5,1.3,2.1*39");
	//setFix(buffer,2);
	//setDOP(buffer,15,'P');
	//setDOP(buffer,16, 'H');
	//setDOP(buffer,17, 'V');
//
	//tmp=getFix();		//3
	//tmp=getDOP('P');	//25
	//tmp=getDOP('H');	//13
	//tmp=getDOP('V');	//21
//
	//strcpy(buffer,"GPGLL,4916.45,N,12311.12,W,225444,A,*1D");
	//setLat(buffer,1);
	//setLon(buffer,3);
	//setTime(buffer,5);
//
	//tmp=getLatS();	//49
	//tmp=getLatM();	//16
	//tmp=getLatDM();	//45
	//tmp=getLonS();	//-123
	//tmp=getLonM();	//11
	//tmp=getLonDM();	//12
	//tmp=getTime();	//13442
//
	//strcpy(buffer,"GPVTG,054.7,T,034.4,M,005.5,N,010.2,K*48");
//
	//setSpeed(buffer,5);
	//tmp=getV();	//10
//
	//k=crc(buffer);
//
	//#endif //debug
*/
	init();
	
	sei();                //enable interrupts
	#ifdef watchdog
	wdt_enable(7);        //turn on watchdog (7 = interval 2 sec)
	#endif
	while (1) {}          //loop

	//#ifdef debug
	//error: 
	//#endif
}


/*
ISR(TIMER1_COMPA_vect)
{
	#ifdef watchdog
	wdt_reset();              //reset watchdog
	#endif

	carriageFlag--;

	if (carriageFlag==0)      //no valid data from GPS
		resetGPSData();

	if (cycle==0) 
  {
	  sendData16(0,id); //3 chars
  }

  if (secCounter==0) 
  {
		if (nSensors>0)	
		  DS18X20_start_meas(DS18X20_POWER_PARASITE, NULL); //start temperature measuring
  }

  if (cycle==1)
  {
	  sendData8(0xA0,getLatS()); //latS
    sendData8(0xA1,getLatM()); //latM
  }

  if (cycle==2)
  {
    sendData16(0xA2,getLatDM()); //LatDM
  	sendData8(0xA3,getLonS()); //LonS
  }

  if (cycle==3)
  {
    sendData8(0xA4,getLonM());  //LonM
  	sendData16(0xA5,getLonDM()); //LonDM
  }

  if (cycle==4)
  {
  	sendData16(0xA6,getH());  //latitude
  	sendData16(0xA7,getV());  //speed
  }

  if (cycle==5)
  {
  	sendData16(0xA8,getK());  //azimuth
  	sendData8(0xA9,getSIV()); //sat in view
  }
	
  if (cycle==6)
  {
    sendData16(0xAA,getDate());  //GPS date
  	sendData24(0xAB,getTime());  //GPS time
  }

  if (cycle==7)
  {
  	sendData16(0xAC,getDOP('H'));  //HDOP
  	sendData16(0xAD,getDOP('V'));  //VDOP
  }

  if (cycle==8)
  {
  	sendData16(0xAE,getDOP('P'));  //PDOP
  	sendData8(0xAF,getFix());      //Fix
  }


	if (cycle>=10 && cycle<=15) //voltages
  {
  	sendVoltage(cycle-10); //0x81-0x86
  }


  if (cycle>=16 && cycle<=19) //temperatures
   {
	  if (secCounter == 1 && cycle == 16) 
	    readTemp(cycle-16); //only one read for all temperatures, result in temperature[]
		
    sendData16(cycle-16+0x8A,temperature[cycle-16]); //0x8A-0x8D
   }
  
  if (cycle==20)
  {
	  //readRawPressure();
  	sendData24(0x94,101325);  //pressure
	 	sendData16(0x95,235);     //temperature from BMP085
  }

  if (cycle==21)
  {
	  sendData16(0x96, 15384); //current in 10mA  
  }	  

	if (cycle%2) {
  	negb(LEDPORT, LEDP);
		if (ch<6) adc_start_conversion();
		else ch=0;
	}
	
	cycle++;


  if (cycle==22) 
  {
    cycle=0;
	  secCounter++;
	  if (secCounter == 2)
	    secCounter = 0;
  }		
}
*/


//-----------------------------------------------------------------------------
//
//  Calculate checksum for GPS sentence
//  Returns:  data checksum
//            0 - if sentense is longer than 83 Bytes
//
//-----------------------------------------------------------------------------
uint16_t crc(char *b) {
  uint16_t ks=0;
  for (uint8_t i=1; i<84; i++) {
    if (b[i]==42)
    return ks;
    ks^=b[i];
  }
  return 0;
}

void sendDataGPS(char *buffer, uint8_t prikaz) {
  u_putc(prikaz);
  u_puts(buffer);
  u_putc(prikaz+1);
}
