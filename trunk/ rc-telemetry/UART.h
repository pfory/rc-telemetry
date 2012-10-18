#ifndef _UART_H_
#define _UART_H_

#include <avr/pgmspace.h>
//#include <util\delay.h>
#include <stdlib.h>

#include <avr/io.h>

void init_uart (uint16_t);         //inicializace UART
unsigned char u_getc( void );               //pøijmi znak z UART
void u_putc( char data );                   //napiš znak do UART
void u_puts( char *text );                  //napiš string do UART
void u_puti( const long val );				//napis int do UART
//void u_change_baud (uint16_t ubrr);

#endif //_UART_H_
