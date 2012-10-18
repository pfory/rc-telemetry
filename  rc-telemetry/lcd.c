#include <avr/io.h>
#include "lcd.h"

inline void gotoxy(uint8_t lin, uint8_t col) {
   lcd_instr(64*(lin+1)+(col-1));
}


// Inicializace LCD ve 4-bitovém módu
void init_lcd(void)
{
	DDR |= dataMaska;
	setb(DDREN,RS);
	setb(DDREN,EN);
	clrb(PORTRSEN,RS);
	clrb(PORTRSEN,EN);
	delay_ms(16);
	PORT |= 0x03<<bit4;
	len();
	delay_ms(5);
	len();
	delay_us(6);
	len();
	clrb(PORT,bit4);
	len();
	lcd_instr(0x28); //inicializace, 4 bitovy rezim, dvouradkovy, font 5x8
	lcd_instr(0x06); //posuv kurzoru vpravo
	lcd_instr(0x0c); //zapne displej
	lcd_instr(0x01); //smaže displej a nastaví kurzor na zaèátek
	delay_ms(3);
	//vlastniZnaky();
}

/*
///smaze oblast displeje na radku r od sloupce s k znaku
void smazDisp(uint8_t r, uint8_t s, uint8_t k) {
	gotoxy(r,s);
	for (uint8_t i=s; i<s+k; i++)
		lcd_c(' ');
}
*/

/*
//napiš bajt (tøi místa bez úvodních nul)
void lcd_byt(unsigned char byt)
{
unsigned char b1,b2=1;

	b1=byt/100;                    //b1=stovky
    if(b1==0) {b2=0; lcd_c(32);}   //byt<100 zobraz mezeru
	else lcd_c(b1+0x30);           //zobraz stovky

    b1=byt%100;                    //zbytek po dìlení stem
	byt=b1;
	b1=b1/10;                      //b1=desítky
    if((b1==0) & (b2==0))          //byt<10 zobraz mezeru
	lcd_c(32);
	else lcd_c(b1+0x30);           //zobraz desítky

	b1=byt%10;                     //zbytek po dìlení deseti
    lcd_c(b1+0x30);                //zobraz jednotky
 }

*/


//54 - i
//    54 	- zarovani 1 (vpravo), znak ' ', delka 6 
// 54 		- zarovani 1 (vpravo), znak ' ', delka 3 
//0054 		- zarovani 1 (vpravo) znak '0', delka 4
//54 		- zarovani 0 (vlevo), znak ' ', delka je ignorovana, znak je ignorovan


void lcd_cislo(int16_t i, char znak, uint8_t zarovnani, uint8_t delka, uint8_t r, uint8_t s) {
	char buffer[6+1];
	itoa(i,buffer,10);
	if (zarovnani==1) {//zarovnani doprava


		volatile char p=strchr(buffer, '\0')-buffer;

		gotoxy(r,s);
		for (uint8_t i=0; i<delka-p; i++) {
			lcd_c(znak);
		}

		for (uint8_t i=0; i<p; i++) {
			lcd_c(buffer[i]);
		}
	}
	else
		lcd_text(buffer);
}

void lcd_i(int16_t i) {
	char buffer[6+1];
	itoa(i,buffer,10);
	lcd_text(buffer);
}


//napiš string, umístìný v RAM
void lcd_text(char* text)
{
unsigned char i=0,temp;

	do
	{
	temp = text[i];
	if(temp==0) break;             //každý string je ukonèený nulou
	lcd_c(temp);
	i++;
	}
	while(temp>0);
}


//napiš string, umístìný v EEPROM
void lcd_etext(const char* text)
{
uint8_t i=0, temp=0;

	do
	{
	temp=eeprom_read_byte((uint8_t *)text+i);   //èti bajt z pamìti EEPROM
	if(temp==0) break;
	lcd_c(temp);
	i++;
	}
	while(temp>0);
}

/*
//napiš string, umístìný ve FLASH
void lcd_ftext(const char* text)
{
unsigned char i=0,temp;

	do
	{
	temp =pgm_read_byte(text+i);   //èti bajt z pamìti FLASH
	if(temp==0) break;
	lcd_c(temp);
	i++;
	}
	while(temp>0);
}
*/

void len(void)                     //impuls Enable do LCD
{
	setb(PORTRSEN,EN);
	delay_us(1);
	clrb(PORTRSEN,EN);
	delay_us(50);
}


void write(char c)
{
	volatile char temp;
	temp=(c&0xf0)>>(4-bit4);
	//temp  = c & 0xf0; //vynuluje spodni 4 bity instrukce
	//temp  = c & maska; //vynuluje spodni 4 bity instrukce
	//PORT &= 0x0f; //nastaví výstupy D4-D7 na 0
	PORT &= ~dataMaska; //nastaví výstupy D4-D7 na 0
	PORT += temp; //nastaví výstupy D4-D7 na horni 4 bity instrukce
	len();
	//temp  = c & 0x0f;
	//temp  = c & maska;
	//temp  = temp << 4;
	//PORT &= 0x0f;
	temp=(c&0x0f)<<bit4;
	PORT &= ~dataMaska;
	PORT += temp;
	len();
}




// Napiš znak na LCD:
void lcd_c(unsigned char dat)
{
setb(PORTRSEN,RS);
write(dat);
}

// Pošli instrukci do LCD:
void lcd_instr(unsigned char ins)
{
clrb(PORTRSEN,RS);
write(ins);
}


//----- >>definice vlastnich znaku<<--------------------------------------------
void vlastniZnaky(void){
/*	unsigned char a[56]={4,14,21,4,4,4,4,0, 	//adresa 0 - znak sipka nahoru
						4,4,4,4,21,14,4,0,  	//adresa 1 - znak sipka dolu
						0,4,4,10,10,17,31,0, 	//adresa 2 - delta	
						14,10,14,0,0,0,0,0,		//adresa 3 - stupen
						1,14,19,21,25,14,16, 	//adresa 4 - prumer
						0,0,0,0,14,0,0,0, 		//adresa 5 - -
						0,0,0,4,14,4,0,0 };		//adresa 6 - +
*/	

	unsigned char a[56]={14,10,14,0,0,0,0,0, 	//adresa 0 - stupen
						 0,4,14,21,4,4,0,0,		//adresa 1 - N
						 0,7,3,5,8,16,0,0,	 	//adresa 2 - NE
						 0,16,8,5,3,7,0,0,		//adresa 3 - SE
						 0,4,4,21,14,4,0,0,	 	//adresa 4 - S
						 0,1,2,20,24,28,0,0,	//adresa 5 - SW
						 0,28,24,20,2,1,0,0}; 	//adresa 6 - NW
	


	uint8_t i;

	for (i=0;i<56;i++) {
		lcd_instr(64+i); 
		lcd_c(a[i]);
	}
}






