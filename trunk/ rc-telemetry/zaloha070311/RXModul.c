//#define LCD     
#define UART   

#ifdef UART
#include "uart.h"
#endif

#include <avr/io.h>             
#include <stdlib.h>             
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#include <string.h>
#include "ds18x20.h"
#include "i2c.h"
#include "crc8.h"
#include "onewire.h"
#include "utils.h"
#include <avr/eeprom.h>
#include <avr/wdt.h>          //funkce pro watchdog 

//#include "menu/menu.h"

//#define _debug //pro ladeni, v konecnem buildu vyhodit

#include "GPS.h"


//#define BAUD 4800  //baut rate bps
//#define UBRR 95// F_OSC/(16*BAUD)-1

char buffer[84];

//DALLAS
#define MAXSENSORS 3

uint8_t gSensorIDs[MAXSENSORS][OW_ROMCODE_SIZE];
uint8_t nSensors=0;
#define OW_ONE_BUS

int16_t teplota[MAXSENSORS];  	//adresa 1-5

uint8_t cyklus=0;


uint8_t ch;
uint16_t napeti[6];

#ifdef _debug
uint16_t valid=0;
#endif

uint16_t koef[6] EEMEM = {10400,11100,11300,0,0,13000};

//uint16_t koef[6];
unsigned int ID EEMEM = 0xFFF;

//pina kterem je parovaci tlacitko
#define PAIRPORT 	PORTB
#define PAIRP	 	1 
#define PAIRPIN 	PINB

//port s indikacni LED
#define LEDPORT 	PORTB
#define LEDP 		2 
#define LEDDDR 		DDRB 


void readTemp();
void sendData8(uint8_t, uint8_t);
void sendData16(uint8_t, uint16_t);
void adc_start_conversion();

//konstanty ve FLASH
char PRGMVER[] PROGMEM = "RXModul1.0";

uint8_t citac=0;
uint8_t start=0;



static uint8_t search_sensors(void)
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


void startTemp() {
	DS18X20_start_meas(DS18X20_POWER_PARASITE, NULL);
}

void readTemp() {
	for (uint8_t i=0; i<nSensors; i++ ) {
		if (DS18X20_read_decicelsius( &gSensorIDs[i][0], &teplota[i] ) 
			== DS18X20_OK ) {
		}
	}
}

void sendData16(uint8_t adresa, uint16_t hodnota) {
	u_putc(adresa|0x80); //nastavim nejvyssi byt na 1
	u_putc(hodnota>>7);
	u_putc(hodnota&0x7F); //shodim nejvyssi bit
}


void sendData24(uint8_t adresa, uint16_t hodnota) {
	u_putc(adresa|0x80);
	u_putc(hodnota>>14);
	u_putc(hodnota>>7&0x7F);
	u_putc(hodnota&0x7F); //shodim nejvyssi bit
}


void sendData8(uint8_t adresa, uint8_t hodnota) {
	u_putc(adresa|0x80);
	u_putc(hodnota);
}

void sendTemp() {
	for (uint8_t i=0; i<nSensors; i++ ) {
		sendData16(i+10,teplota[i]); //adresa 10-13
	}
}

void sendVoltage() {
	for (uint8_t i=0; i<6; i++) {
		sendData16(i+1,napeti[i]); //adresa 1-6
	}
//	sendData16(koef[c]);
}


//*****************************************************************************
//
//  ADC single conversion routine 
//
//*****************************************************************************
void adc_start_conversion() {
	//set ADC channel
	ADMUX=(ADMUX&0xF0)|ch;
	//Start conversionio with Interupt after conversion
	ADCSR|=1<<ADSC;
}


void init() {
	//nastaveni timeru
	OCR1A=0x2D0;        //compare register 720
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x0D;       	//start timer1, prescaler 1024,CTC
	//crystal 7 372 800 (F_OSC) / 720 / 1024 = 10 times per second

	//nastaveni UART
	init_uart(47);           // inicializace UART 9600 baud
	#ifdef _debug
	init_lcd();                // inicializace displeje
	//vlastniZnaky();
	cls();
	//lcd_text("RXModul");
	lcd_ftext(PRGMVER);
	#endif
	//u_change_baud(47);
//	u_fputs(PRGMVER);
	u_puts("\n");


	//nastaveni portu na kterem jsou dallas
	ow_set_bus(&PINB,&PORTB,&DDRB,PB0);
	nSensors = search_sensors();
	//u_puti( (int)nSensors );
	//u_puts(" sensors\n");
	//u_change_baud(95);

	//astaveni AD prevodniku
	//select reference voltage
	ADMUX|=(1<<REFS1)|(1<<REFS0);
    ADCSR=(1<<ADEN)|(1<<ADIE)|(0<<ADFR)|(1<<ADIF)|(1<<ADPS2)| (1<<ADPS1)|(1<<ADPS0);



	//pull up na parovaci tlacirko
	setb(PAIRPORT, PAIRP);

	//konfigurace poru s indiklacni ledkou
	setb(LEDDDR, LEDP);  //output
	clrb(LEDPORT, LEDP); //rozsviti

}


////////////////////////////////////////////////////////////////////
ISR(TIMER1_COMPA_vect) {
	//return;
	//if (citac>0) return;

	wdt_reset();              //nulování watchdog

	#ifdef _debug
	gotoxy(2,1);
	lcd_byt(cyklus);
	#endif

	negb(LEDPORT, LEDP);

	if (cyklus>=11) cyklus=0;

	if (cyklus==0) 
		if (nSensors>0)	startTemp(); //start mereni cidel


	if (cyklus==10) {

		//zmena rychlosti UART na 9600bps
		//u_change_baud(47);
		
		//poslu ID zarizeni
		sendData16(0,eeprom_read_word(&ID));

/*		if (bit_is_clear(PAIRPIN,PAIRP)) {
			//poslu kalibracni konstanty
			for (uint8_t i=0; i<6; i++)
				sendData16(i+20,eeprom_read_word(&koef[i]));
		}
*/		
		/*
		stupnì lat 	32 	10100000	0XXXXXXX
		minuty lat 	33	10100001	00XXXXXX
		desetiny	34	10100010	00000XXX  0XXXXXXX
		stupnì lon 	35 	10100011	0XXXXXXX
		minuty lon 	36	10100100	00XXXXXX
		desetiny	37	10100101	00000XXX  0XXXXXXX
		*/			


		sendData8(32,getLatS());
		sendData8(33,getLatM());
		sendData16(34,getLatDM());
		sendData8(35,getLonS());
		sendData8(36,getLonM());
		sendData16(37,getLonDM());
		/*
		výška		38	10100110	0XXXXXXX  0XXXXXXX
		rychlost	39	10100111	000000XX  0XXXXXXX
		kurz		40	10101000	000000XX  0XXXXXXX
		sat in view	41	10101001	0000XXXX
		datum		42	10101010	0000000X  0XXXXXXX 0XXXXXXX
		èas			43	10101011	0000000X  0XXXXXXX 0XXXXXXX
		*/

		sendData16(38,getH());
		sendData16(39,getV());
		sendData16(40,getK());
		sendData8(41,getSIV());
		sendData24(42,getDate());
		sendData24(43,getTime());

		readTemp();
		sendTemp();
		sendVoltage();	

		//zmena rychlosti UART na 4800bps
		//u_change_baud(95);
	}

	if (cyklus%2) {
		if (ch<6) adc_start_conversion();
		else ch=0;
	}
	
	cyklus++;
}

//*****************************************************************************
//
//  ADC conversion complete service routine 
//
//*****************************************************************************
ISR(ADC_vect)
{
	uint16_t adc_value;
	adc_value = ADCL;
	/*shift from low level to high level ADC, from 8bit to 10bit*/
	adc_value += (ADCH<<8);

	//debug
	#ifdef _debug
	adc_value=546;
	#endif
	uint32_t a=(uint32_t)adc_value * (uint32_t)eeprom_read_word(&koef[ch]) / (uint32_t)10000;
	napeti[ch]=(uint16_t)(a);
	ch++;
}


ISR(USART_RXC_vect) {
	uint8_t znak=u_getc();             // pøijmi znak a ulož do "znak"
	if (znak=='$') {
		start=1;
		citac=0;
	}

	if (znak==0x0D && start==1) { //konec radku
		//kontrolni soucet
		uint16_t k=crc(buffer);

		char bks[3];
		bks[0]=buffer[strstr(buffer,"*")-buffer+1];
		bks[1]=buffer[strstr(buffer,"*")-buffer+2];
		bks[2]='\0';
		uint16_t kk=strtol(bks,NULL,16);

		#ifdef _debug
		gotoxy(1,12);
		lcd_i(++valid);
		#endif

		if (k==kk) { //valid data		
			if (strstr(buffer, "GPGGA")>0) {
				//lcd_text("GGA");
				setLat(buffer,2);
				setLon(buffer,4);
				setAlt(buffer,9);
				setSatInView(buffer,7);
			}

			if (strstr(buffer, "GPRMC")>0) {
				setDatum(buffer,9);
				setCas(buffer,1);
				setLat(buffer,3);
				setLon(buffer,5);
				setKurz(buffer,8);
				setSpeed(buffer,7);
			}
		}
	}

	if (start==1) {
		buffer[citac]=znak;
		citac++;
	}

	if (citac>83) {
		citac=0;
		start=0;
	}

}


/////////////////////////////// MAIN //////////////////////////////
int main (void)
{
	init();
	
	sei();
	wdt_enable(6);        //zapni wdt (6 = interval 1 sec)
	for(;;)  {                  // hlavní smyèka
 	}//for

}//main
/////////////////////////////// END MAIN //////////////////////////
