#include <avr/io.h>
#include <avr/interrupt.h>

#include "delay.h"
#include "utils.h"

#include "lcd.h"
#include "UART.h"

#define UBRR 47 //(F_OSC/(16*BAUD))-1

uint8_t znak=65;

int main(void)
{
  init_lcd();

	cls();
	gotoxy(1,1);
  lcd_text("Test UART");

  init_uart(UBRR);

  //u_puts("restart");

  //nastaveni timeru
	OCR1A=0x10;     //compare register 256
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x0D;    //start timer1, prescaler 1024,CTC
  
	//crystal 7 372 800 (F_OSC) / 256 / 1024 = 28 times per second

  sei();

  while (1)
  {
  }
}


ISR(USART_RXC_vect) 
{
}

ISR(TIMER1_COMPA_vect) 
{
    char buff[7];
    memset(buff,'\0',7);
    for (uint8_t i=0; i<6; i++)
    {
      memcpy(buff+i,&znak,1);
      line_2E();
      lcd_c(znak);
      znak++;
      if (znak>=91) 
      {
        znak=65;
      }
    }
    u_puts(buff);
}
