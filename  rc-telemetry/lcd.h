#ifndef _LCD_H_
#define _LCD_H_

#include "delay.h"
#include <avr/eeprom.h>
#include <avr/pgmspace.h>
#include <stdlib.h>	
#include <string.h>
#include "utils.h"


//PORT pro data
#define PORT PORTC 
#define DDR  DDRC

//EN a RS
#define DDREN DDRC
#define PORTRSEN PORTC
#define EN   0 
#define RS   1 

#define dataMaska 0x3C //00111100 DATA7-4
#define bit4 2		//pin na kterem jsou DATA4


// Funkce pro displej:
void init_lcd (void);                       //inicializace
void lcd_instr(unsigned char ins);          //napiš instrukci
void lcd_c    (unsigned char dat);          //napiš znak;
void lcd_byt  (unsigned char byt);          //napiš bajt)
void lcd_text (char* text); 		        //napiš string, umístìný v RAM
//void lcd_ftext(const char* text); 			//napiš string, umístìný ve FLASH
void lcd_i(int16_t);
void vlastniZnaky(void);
void gotoxy(uint8_t lin, uint8_t col);
void lcd_cislo(int16_t i, char znak, uint8_t zarovnani, uint8_t delka, uint8_t r, uint8_t s);
void len(void);
void write(char);
void lcd_ftext(const char* text);
void lcd_etext(const char* text);
void smazDisp(uint8_t r, uint8_t s, uint8_t k);

// Makra pro displej:
#define cls()		  	  lcd_instr(0x01);delay_ms(3)		 //vymaž displej
#define cursor_off()      lcd_instr(0x0c)				 //vypni kurzor
#define cursor_on()       lcd_instr(0x0e)				 //zapni kurzor
#define cursor_blink()    lcd_instr(0x0f)				 //kurzor bliká
#define line_1()	      lcd_instr(0x80)				 //jdi na øádek 1
#define line_2() 	      lcd_instr(0xc0)				 //jdi na øádek 2
#define shift_lcd_left()  lcd_instr(0x18)				 //posuò text doleva
#define shift_lcd_right() lcd_instr(0x1c)				 //posuò text doprava
#define line_1E()	      lcd_instr(0x80);lcd_text("                ");lcd_instr(0x80)
#define line_2E()	      lcd_instr(0xc0);lcd_text("                ");lcd_instr(0xc0)

#endif //_LCD_H_

