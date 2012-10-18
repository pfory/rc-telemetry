#include "uart.h"

// UART inicializace
void init_uart (uint16_t ubrr)
{

	UBRRH=(unsigned char) (ubrr>>8);
	UBRRL=(unsigned char) ubrr;
	UCSRB=(1<<RXEN|1<<TXEN)|(1<<RXCIE);
	UCSRC=(1<<URSEL)|(0<<USBS)|(3<<UCSZ0);
}

void u_change_baud (uint16_t ubrr)
{

	UBRRH=(unsigned char) (ubrr>>8);
	UBRRL=(unsigned char) ubrr;
}

// UART po�li znak:
void u_putc( char data )
{

while ( !( UCSRA & (1<<UDRE)) )     // Wait for empty transmit buffer
;
UDR = data;                         // Put data into buffer, sends the data
}

/*
//napi� string, um�st�n� ve FLASH
void u_fputs(const char* text)
{
unsigned char i=0,temp;

	do
	{
	temp =pgm_read_Byte(text+i);   //�ti bajt z pam�ti FLASH
	if(temp==0) break;
	u_putc(temp);
	i++;
	}
	while(temp>0);
}
*/


// UART po�li string:
void u_puts( char *text )
{
unsigned char i=0,temp;
	do	{
  	temp = text[i];
	  if(temp==0) break;
    /*while ( !( UCSRA & (1<<UDRE)) );    // Wait for empty transmit buffer
    UDR = temp;                         // Put data into buffer, sends the data
  	//_delay_ms(2);*/
	  u_putc(temp);
	  i++;
	}
	
  while(temp>0);
}


/*************************************************************************
Function: uart_puti()
Purpose:  transmit integer as ASCII to UART
Input:    integer value
Returns:  none
This functions has been added by Martin Thomas <eversmith@heizung-thomas.de>
Don't blame P. Fleury if it doesn't work ;-)
**************************************************************************/
void u_puti( const long val )
{
    char buffer[sizeof(long)*8+1];
    
    u_puts( ltoa(val, buffer, 10) );

}


// UART p�ijmi znak:
unsigned char u_getc( void )
{

while ( !(UCSRA & (1<<RXC)) )       // Wait for data to be received
;
return UDR;                         // Get and return received data from buffer
}
