//*****************************************************************************
//
// File Name	: 'main.c'
// Title		: 
//
//*****************************************************************************
#include <stdio.h>
#include <avr/io.h>
#include <string.h>	
//#include <avr/pgmspace.h>
#include <avr/interrupt.h>
//#include <util/delay.h>
#define F_OSC 7372800
#define BAUD 9600

#define UART


#ifdef UART
#include "uart.h"
#include "uart_addon.h"
#endif


/*#ifdef UART
#define BAUD 9600  //baut rate bps
#define UBRR 47 //(F_OSC/(16*BAUD))-1
#endif
*/

#include "lcd.h"
//#include "dallas.h"
#include "ds18x20.h"
#include "i2c.h"
#include "crc8.h"
#include "onewire.h"

#define MAXSENSORS 3

uint8_t gSensorIDs[MAXSENSORS][OW_ROMCODE_SIZE];
uint8_t nSensors=0;
#define OW_ONE_BUS

#define ZNAKSTUPEN lcd_c(0x03)

//function skeletons
void init_adc(void);
void adc_start_conversion();
void init(void);
void init_timer(void);
void init_dallas(void);
void send(uint8_t,uint16_t);
static uint8_t search_sensors(void);
void sendData16(uint16_t);

char UARTbuff[6];

int16_t teplota[MAXSENSORS];  	//adresa 1-5
int16_t napeti[6];				//adresa 6-11

//current channel
uint8_t ch;

//cyklus
uint8_t cyklus=0;
uint16_t koef[6];


//*****************************************************************************
//
//  ADC module initialization 
//
//*****************************************************************************
void init_adc(void)
{
	//select reference voltage
	ADMUX|=(1<<REFS1)|(1<<REFS0);
    ADCSR=(1<<ADEN)|(1<<ADIE)|(0<<ADFR)|(1<<ADIF)|(1<<ADPS2)| (1<<ADPS1)|(1<<ADPS0);
}

void init_timer(void) {
	OCR1A=0x2D0;        //compare register 720
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x0D;       	//start timer1, prescaler 1024,CTC
	//crystal 7 372 800 (F_OSC) / 720 / 1024 = 10 times per second
}

void init_dallas() {
	//nastaveni portu na kterem jsou dallas
	ow_set_bus(&PINB,&PORTB,&DDRB,PB2);
	nSensors = search_sensors();
	//uart_put_int( (int)nSensors );
	//uart_puts(" sensors\n");
}

//*****************************************************************************
//
//  ADC single conversion routine 
//
//*****************************************************************************
void adc_start_conversion()
{
	//set ADC channel
	ADMUX=(ADMUX&0xF0)|ch;
	//Start conversionio with Interupt after conversion
	ADCSR|=1<<ADSC;
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

void showTemp() {
	char s[10];
	line_2E();
	gotoxy(2,1);
	for (uint8_t i=0; i<nSensors; i++ ) {
		DS18X20_format_from_decicelsius(teplota[i], s, 10 );
		uart_puts(s);
		uart_puts("°C");

		lcd_text(s);
		ZNAKSTUPEN;
		lcd_c(' ');
	}
}


void sendTemp() {
	for (uint8_t i=0; i<nSensors; i++ ) {
		send(i+1,teplota[i]); //adresa 1-5
	}
}


static uint8_t search_sensors(void)
{
	uint8_t i;
	uint8_t id[OW_ROMCODE_SIZE];
	uint8_t diff, nSensors;
	
	#ifdef UART
	//uart_puts("\nScanning Bus for DS18X20\n");
	#endif

	ow_reset();

	nSensors = 0;
	
	diff = OW_SEARCH_FIRST;
	while ( diff != OW_LAST_DEVICE && nSensors < MAXSENSORS ) {
		DS18X20_find_sensor( &diff, &id[0] );
		
		if( diff == OW_PRESENCE_ERR ) {
			#ifdef UART
			//uart_puts( "No Sensor found\n");
			#endif
			break;
		}
		
		if( diff == OW_DATA_ERR ) {
			#ifdef UART
			//uart_puts( "Bus Error\n");
			#endif
			break;
		}
		
		for ( i=0; i < OW_ROMCODE_SIZE; i++ )
			gSensorIDs[nSensors][i] = id[i];
		
		nSensors++;
	}
	
	return nSensors;
}



//*****************************************************************************
//
//  init AVR
//
//*****************************************************************************

void init(void)
{
	//init LCD
	init_lcd();

	cls();
	gotoxy(1,1);
	lcd_text("RCT");

	//init UART
	//init_uart(UBRR);
	#ifdef UART
	uart_init(UART_BAUD_SELECT(BAUD,F_OSC));
	//uart_puts("\nRC Telemetry v0.1\n");
	#endif

	//Init ADC
	init_adc();
	//init DALLAS
	init_dallas();
	//Init timer
	init_timer();


	//koeficienty, budou se nacitat z EEPROMky
	koef[0]=1900;
    koef[1]=4150;
    koef[2]=5545;
    koef[3]=7666;
    koef[4]=6555;
    koef[5]=2785;
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

	napeti[ch]=adc_value;
	ch++;
}


void sendVoltage(uint8_t c) {
	send(c+6,napeti[c]); //adresa 6-11
	sendData16(koef[c]);
}

/*
void getNapetiFromAdc(uint8_t ch, char[] t) {
	napeti[ch]*=2;

    lcd_i(napeti[ch] * k / 10000);
    lcd_c('.');

	uint16_t dc=((napeti[ch] * k) % 10000)/100;
	if (dc<10) lcd_i(0);
    lcd_i(dc);  
    lcd_c('V');  
}
*/

void send(uint8_t adresa, uint16_t hodnota) {
	uint8_t hb, lb;
	hb=(adresa<<3)|(hodnota>>7);
	lb=(hodnota&0xFF)|(1<<7);
	uart_putc(hb);
	uart_putc(lb);
}

void sendData16(uint16_t hodnota) {
	//koeficient pro prepocet
	uart_putc(hodnota>>8);
	uart_putc(hodnota&0xFF);
}

//----- >>preruseni od timeru 1<< ----------------------------------------------------------
ISR(TIMER1_COMPA_vect) {
	
	gotoxy(2,1);
	lcd_i(cyklus);

	if (cyklus==0) {
		if (nSensors>0)
			startTemp(); //start mereni cidel

		ch=0;
	}

	if (ch<6)
		adc_start_conversion();

	if (cyklus%4) {
		sendVoltage(0);	
		sendVoltage(1);	
		sendVoltage(2);	
		sendVoltage(3);	
		sendVoltage(4);	
		sendVoltage(5);	
	}

	cyklus++;
	if (cyklus==10) {
		if (nSensors>0) {
			readTemp(); //zobrazeni teplot
			//showTemp();
			sendTemp();
		}

		cyklus=0;
	}
}

//*****************************************************************************
//
//  main programm
//
//*****************************************************************************
int main(void)
{
	init();
	//delay_ms(1000);

	//sleep mode enabled
	//MCUCR|=0x80;
	sei();
	while(1)//loop demos
	{
		//asm volatile("sleep");  //spi! - jde do pøednastaveného sleep módu
	}
	return 0;
}
