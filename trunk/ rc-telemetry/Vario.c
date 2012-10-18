//#include <stdio.h>
#include <avr/io.h>
//#include <string.h>	
#include <avr/interrupt.h>
#define F_OSC 7372800

//#define UART
#define LCD

#ifdef UART
#define BAUD 9600  //baut rate bps
#define UBRR 47 //(F_OSC/(16*BAUD))-1
#endif

#include "lcd.h"

uint8_t citac=0;
uint16_t tlak, tlakMin=0xFFFF, tlakMax=0;

void adc_start_conversion(void);



//*****************************************************************************
//
//  ADC single conversion routine 
//
//*****************************************************************************
void adc_start_conversion() {
	//set ADC channel
	ADMUX=(ADMUX&0xF0)|0;
	//Start conversionio with Interupt after conversion
	ADCSR|=1<<ADSC;
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
	lcd_text("Var v0.1");

	//nastaveni timeru
	OCR1A=7200;        //compare register 3600
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x0D;       	//start timer1, prescaler 1024,CTC
	//crystal 7 372 800 (F_OSC) / 3600 / 1024 = 2 times per second

	//nastaveni AD prevodniku
	//select reference voltage
	ADMUX|=1<<REFS1|1<<REFS0;
    ADCSR=1<<ADEN|1<<ADIE|0<<ADFR|1<<ADIF|1<<ADPS2|1<<ADPS1|1<<ADPS0;

}


ISR(TIMER1_COMPA_vect) {
	if (citac%2==0)
		adc_start_conversion();
	else
		line_2E();
		gotoxy(2,1);
		lcd_i(tlak);
		lcd_c(' ');
		lcd_i(tlakMin);
		lcd_c(' ');
		lcd_i(tlakMax);
		gotoxy(1,11);
		double v=2.56/1024*tlak*2;
		v=(v+0.4845)/0.0459*10;
		lcd_i(v);
		lcd_text("hPa");
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
	adc_value += (ADCH<<8);

	tlak=adc_value;
	if (tlak<tlakMin) tlakMin=tlak;
	if (tlak>tlakMax) tlakMax=tlak;
}


int main(void) {

	init();

	MCUCR|=1<<SM0;
	sei();
	while(1)//neverending loop
	{
		asm volatile("sleep");  //spi! - jde do pøednastaveného sleep módu
	}
	return 0;
}

