#include <avr/io.h>
#include <avr/interrupt.h>

#define F_OSC 7372800

#include "lcd.h"
#include "menu.h"

int sec,min,hod,den,mes,rok=0;

void menuInit(void);



ISR(TIMER1_COMPA_vect) {
	zpracuj();

	line_2E();
	gotoxy(2,1);
	lcd_i(hod);
	lcd_c(':');
	lcd_i(min);
	lcd_c(':');
	lcd_i(sec);

//	gotoxy(1,10);
//	lcd_i(menuIndex);
}


int main(void) {

	//GICR|=1<<INT0;	//int0 enable
	//MCUCR|=1<<ISC01; //falling edge on int0

	//inicializace menu
	menuInit();

	//nastaveni funkci
	menuFce[0]=&sec;
	menuFce[1]=&min;
	menuFce[2]=&hod;
	menuFce[3]=&den;
	
	//nastaveni rozsahu
	menuRozsah[0][0]=0;
	menuRozsah[0][1]=59;
	menuRozsah[1][0]=0;
	menuRozsah[1][1]=59;
	menuRozsah[2][0]=0;
	menuRozsah[2][1]=23;

	OCR1A=0x2D0;        //compare register 720
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x0D;       	//start timer1, prescaler 1024,CTC
	//crystal 7 372 800 (F_OSC) / 720 / 1024 = 10 times per second

	init_lcd();
	lcd_text("MENU");

	pf[0]=&setVar;
	
	//pf[0](&min, 1);
	//pf[0](&min, 0);

	sei();
	while (1) {}


}
