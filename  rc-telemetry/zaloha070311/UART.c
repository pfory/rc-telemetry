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

// UART pošli znak:
void u_putc( char data )
{

while ( !( UCSRA & (1<<UDRE)) )     // Wait for empty transmit buffer
;
UDR = data;                         // Put data into buffer, sends the data
}

/*
//napiš string, umístìný ve FLASH
void u_fputs(const char* text)
{
unsigned char i=0,temp;

	do
	{
	temp =pgm_read_byte(text+i);   //èti bajt z pamìti FLASH
	if(temp==0) break;
	u_putc(temp);
	i++;
	}
	while(temp>0);
}
*/


// UART pošli string:
void u_puts( char *text )
{
unsigned char i=0,temp;

	do
	{
	temp = text[i];
	if(temp==0) break;
	u_putc(temp);
	delay_ms(10);
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
/*void u_puti( const int val )
{
    char buffer[sizeof(int)*8+1];
    
    u_puts( itoa(val, buffer, 10) );

}*/


// UART pøijmi znak:
unsigned char u_getc( void )
{

while ( !(UCSRA & (1<<RXC)) )       // Wait for data to be received
;
return UDR;                         // Get and return received data from buffer
}
