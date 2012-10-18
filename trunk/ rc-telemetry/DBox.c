#include <stdio.h>
#include <avr/io.h>
#include <string.h>	
#include <avr/interrupt.h>
#include "menu.h"
#include <avr/eeprom.h>
#include <avr/wdt.h>          //funkce pro watchdog 

#include "delay.h"
#include "utils.h"


#define BAUD 9600  //baut rate bps
#define UBRR 47 //(F_OSC/(16*BAUD))-1

#include "lcd.h"
#include "UART.h"

static uint8_t citac=0;
static char UARTbuffB[3];
//uint8_t delkaUARTbuffU=40;
static char UARTbuffU[15];

//char UARTbuffU[]={0x80, 0x7f, 0x7f, 0xa0, 0x31, 0xa1, 0x2b, 0xa2, 0x6, 0x49, 0xa1, 0xa3, 0xd, 0xa4, 0x17, 0xa5, 0x5, 0x48, 0xa6, 0x2, 0x6a, 0x5, 0xa7, 0x0, 0x0, 0xa8, 0x0, 0x0, 0xa9, 0x0, 0xaa, 0x0, 0x0, 0x7b, 0x15, 0xab, 0x1, 0x6d, 0x1b, 0x8a, 0x1, 0x62, 0x81, 0x1b, 0x3, 0x2c, 0x82, 0x4, 0xe, 0x83, 0x4, 0x4c, 0x84, 0x4, 0x83, 0x62, 0x85, 0x5, 0x8, 0x86, 0x0, 0xf8, 0xe6, 0x82, 0x86, 0x0, 0x80, 0xff, 0x7f, 0xa0, 0x31, 0xa1, 0x20};
//char UARTbuffU[]={0x80, 0x7f, 0x7f, 0xa0, 0x31, 0xa1};

//delky dat
static const char delky[][2]={{0x80,2},{0xA0,1},{0xA1,1},{0xA2,2},{0xA3,1},{0xA4,1},
	{0xA5,2},{0xA6,2},{0xA7,2},{0xA8,2},{0xA9,1},{0xAA,3},{0xAB,3},
	{0x8A,2},{0x8B,2},{0x8C,2},{0x8D,2},{0x81,2},{0x82,2},{0x83,2},
	{0x84,2},{0x85,2},{0x86,2}};

static const uint8_t delkaUARTbuffU=15;
static uint8_t pozice=0;
static uint8_t pocetZnaku;
static uint8_t poziceBufferB=0;
static uint8_t poziceBufferU=0;
static uint8_t priznakPrenosu=0;

static int16_t priznakResetMax=0;

//uint8_t bm=0;

static uint8_t priznak7D=0;
static uint8_t delkaBloku=0;

//double const k=3.3/0xFF;
static const uint8_t pocetClanku=4;
static uint8_t tt=0; //telemetry type
//static uint16_t pocetBloku=0;
//static uint16_t readBytes=0;

static int8_t teplota[4];
static uint16_t napeti[6];
static uint16_t ID;
static uint8_t AD1=0, AD2=0, RSSI=0; //basic telemetry data
static uint8_t koef[6];

static int vyskaLetiste=0; //vyska startu
static char modelName[11];

//alarmy
static int alarmNapetiClanek;
static int alarmTypNapetiClanek;
static int alarmNapetiBaterie;
static int alarmTypNapetiBaterie;

//EEPROM variables
unsigned char      dumy     	EEMEM  = 0xff;      // nepoužívat adr 0

static uint16_t eAlarmNapetiClanek EEMEM=37; //3,7V minimalni napeti
static uint8_t eAlarmTypNapetiClanek EEMEM=0;
static uint16_t eAlarmNapetiBaterie  EEMEM=111; //11,1V minimalni pro 3cl
static uint8_t eAlarmTypNapetiBaterie EEMEM=0;
static uint16_t eVyskaLetiste EEMEM=0;
static unsigned char eModelName[10] EEMEM="TEST      ";

static char eSWVer[] EEMEM="Dbox v1.329";

#define eText(c,t) char eText##c[] EEMEM=t

eText(1,"Napeti");
eText(2,"baterie");
eText(3,"Zadna data");
eText(4,"clanku");
eText(5,"Typ");
eText(6,"alarmu");
eText(7,"h");
eText(8,"sat");
eText(9,"b");
eText(10,"p");
eText(11,"AD");
eText(12,"RSSI");
eText(13,"Max");
eText(14,"vys"); //vyska
eText(15,"rych"); //rychlost
eText(16,"km");
eText(17,"m"); //metru
eText(18,"vyska");
eText(19,"akt"); //aktualni
eText(20,"n"); //nad
eText(21,"bat"); //baterie
eText(22,"cl"); //clanek
eText(23,"SET"); //nastaveni vysky letiste
eText(24,"letiste"); //nastaveni vysky letiste
eText(25,"Jmeno"); 
eText(26,"modelu");
eText(27,"zn");


void (*pfMenu[MENUCOUNT])(void);


#define maxBeepMaska 4
static uint16_t beepMaska[maxBeepMaska];

uint8_t flagBeep=0;
static uint8_t beepCitac=0;

void beep(uint8_t);
void beepInit(void);

//port na kterem je pripojen beeper
#define PORTBEEP PORTD
#define PINBEEP 6
#define DDRBEEP DDRD

//port na kterem je pripojena LED
#define PORTLED PORTD
#define PINLED 7
#define DDRLED DDRD


//deklarace bitového pole
typedef struct { uint8_t den : 5;
                 uint8_t mes : 4;
                 uint8_t rok : 7;
               } DATUM; //2Byte

DATUM datum;

typedef struct { uint8_t sec : 5;
                 uint8_t min : 6;
                 uint8_t hod : 5;
               } CAS; //2Byte

CAS cas;

typedef struct { uint8_t latS 			: 8;
                 uint8_t latM 			: 6;
                 uint16_t latDM			: 10;
				 uint16_t lonS 			: 9;
                 uint8_t lonM 			: 6;
                 uint16_t lonDM			: 10;
				 uint8_t satInView		: 4;
				 uint16_t rychlost		: 9;
				 uint16_t maxRychlost	: 9;
				 int16_t vyska			: 14;
				 int16_t maxVyska		: 14;
				 uint16_t kurz			: 9;
               } GPS; //108bite=13Byte+4bite 

int a;
static GPS gps;

static uint8_t predchoziZnak=0;

#define ZNAKSTUPEN 	lcd_c(0x00)

//vetrna ruzice
#define ZNAKN		lcd_c(0x01)
#define ZNAKNE 		lcd_c(0x02)
#define ZNAKE		lcd_c(126)
#define ZNAKSE 		lcd_c(0x03)
#define ZNAKS  		lcd_c(0x04)
#define ZNAKSW 		lcd_c(0x05)
#define ZNAKW		lcd_c(127)
#define ZNAKNW 		lcd_c(0x06)

#define MEZERA 		lcd_c(' ')
#define TECKA 		lcd_c('.')

#define NORTH 		lcd_c('N');
#define SOUTH 		lcd_c('S');
#define WEST 		lcd_c('W');
#define EAST 		lcd_c('E');
#define NULA 		lcd_c('0');
#define LOMENO 		lcd_c('/');
#define ALARM		lcd_c('A');
#define VOLT		lcd_c('V');
#define DVOJTECKA	lcd_c(':');


#define FOR(x) for (uint8_t i=0; i<x; i++) {

//function prototypes
static void unpackDate(uint16_t);
static void unpackTime(uint16_t);
inline static void zobrazCas(uint8_t, uint8_t);
static void zobrazBT(uint8_t);
static void zobrazBT0(void);
static void zobrazBT1(void);
static void zobrazGPS(void);
static void zobrazTemp(void);
static void zobrazNapetiClanku(void);
static void zobrazDatumCas(void);
inline static void zobrazGPSLatLon(void);
static void zobrazMaxima(void);
static void zpracujFrontu(void);
//static void posliHEX(char);
static void nuluj(void);
static void showNapetiClanek(uint8_t, uint8_t, uint8_t);
static void showNapetiBaterie(uint8_t, uint8_t);
static void setAlarmNapetiClanku(void);
static void setAlarmNapetiBaterie(void);
static void readFromEEPROM(void);
static void nulujVysku(void);
static void setModelName(void);

//*****************************************************************************
//
//  init AVR
//
//*****************************************************************************

void init(void)
{
	
	init_lcd();

	cls();
	gotoxy(1,1);
	lcd_etext(eSWVer);
	vlastniZnaky();

	readFromEEPROM();

	init_uart(UBRR);
	u_puts(eSWVer);

	TCCR1B|=1<<CS11;//|1<<WGM13; //prescaler 8, PWM Phase and frequency correct


	//nastaveni timeru
	OCR1A=0x2D0;       //compare register 720
	TIMSK|=0x10;		//enable interrupt COMP1A
	TCCR1B=0x0D;       	//start timer1, prescaler 1024,CTC
	//crystal 7 372 800 (F_OSC) / 720/ 1024 = 10 times per second

	setb(DDRLED,PINLED); //LED

	MCUCR|=1<<ISC01;
	GICR|=1<<INT0;
	setb(PORTD,2);
	setb(PIND,2);

	menuInit();

	//nastaveni funkci
	menuVar[7]=&priznakResetMax;
	menuVar[8]=&vyskaLetiste;
	menuVar[9]=&alarmNapetiClanek;
	menuVar[10]=&alarmTypNapetiClanek;
	menuVar[11]=&alarmNapetiBaterie;
	menuVar[12]=&alarmTypNapetiBaterie;

	//nastaveni rozsahu
	menuRozsah[7][0]=1;
	menuRozsah[8][0]=vyskaLetiste;
	menuRozsah[9][0]=30;
	menuRozsah[9][1]=42;
	menuRozsah[10][0]=0;
	menuRozsah[10][1]=maxBeepMaska;
	menuRozsah[11][0]=30;
	menuRozsah[11][1]=252; //6 CLANEK
	menuRozsah[12][0]=0;
	menuRozsah[12][1]=maxBeepMaska;

	//modelName
	FOR(10)
		menuRozsah[i+13][0]=32;
		menuRozsah[i+13][1]=122;
		pf[i+13]=&setChar;
		menuVar[i+13]=(int *)&modelName[i];
		//modelName[i]=32;
	}

	pf[1]=&foo;
	pf[2]=&foo;
	pf[3]=&foo;
	pf[4]=&foo;
	pf[5]=&foo;
	pf[6]=&foo;
	pf[7]=&resetVar;
	pf[8]=&resetVar;
	pf[9]=&setVar;
	pf[10]=&setVar;
	pf[11]=&setVar;
	pf[12]=&setVar;

	beepInit();

	pfMenu[0]=&zobrazDatumCas;
	pfMenu[1]=&zobrazGPSLatLon;
	pfMenu[2]=&zobrazGPS;
	pfMenu[3]=&zobrazTemp;
	pfMenu[4]=&zobrazNapetiClanku;
	pfMenu[5]=zobrazBT0;
	pfMenu[6]=zobrazBT1;
	pfMenu[7]=&zobrazMaxima;
	pfMenu[8]=&nulujVysku;
	pfMenu[9]=&setAlarmNapetiClanku;
	pfMenu[10]=&setAlarmNapetiClanku;
	pfMenu[11]=&setAlarmNapetiBaterie;
	pfMenu[12]=&setAlarmNapetiBaterie;
	FOR(10)
		pfMenu[i+13]=&setModelName;
	}
}


void readFromEEPROM() {
	alarmNapetiClanek=eeprom_read_word(&eAlarmNapetiClanek);
	alarmTypNapetiClanek=eeprom_read_byte(&eAlarmTypNapetiClanek);
	alarmNapetiBaterie=eeprom_read_word(&eAlarmNapetiBaterie);
	alarmTypNapetiBaterie=eeprom_read_byte(&eAlarmTypNapetiBaterie);
	vyskaLetiste=eeprom_read_word(&eVyskaLetiste);
	FOR(10)
		modelName[i]=eeprom_read_byte(&eModelName[i]);
	}
	modelName[10]='\0';
}

void ulozDataEEPROM() {
	
	uint8_t zmena=0;
	if (alarmNapetiClanek!=eeprom_read_word(&eAlarmNapetiClanek)) {
		eeprom_write_word(&eAlarmNapetiClanek,alarmNapetiClanek);
		zmena=1;
	}
	if (alarmTypNapetiClanek!=eeprom_read_byte(&eAlarmTypNapetiClanek)) {
		eeprom_write_byte(&eAlarmTypNapetiClanek,alarmTypNapetiClanek);
		zmena=1;
	}

	if (alarmNapetiBaterie!=eeprom_read_word(&eAlarmNapetiBaterie)) {
		eeprom_write_word(&eAlarmNapetiBaterie,alarmNapetiBaterie);
		zmena=1;
	}
	if (alarmTypNapetiBaterie!=eeprom_read_byte(&eAlarmTypNapetiBaterie)) {
		eeprom_write_byte(&eAlarmTypNapetiBaterie,alarmTypNapetiBaterie);
		zmena=1;
	}

	if (vyskaLetiste!=eeprom_read_word(&eVyskaLetiste)) {
		eeprom_write_word(&eVyskaLetiste,vyskaLetiste);
		zmena=1;
	}

	FOR(10)
		if (modelName[i]!=eeprom_read_byte(&eModelName[i])) {
			eeprom_write_byte(&eModelName[i],modelName[i]);
			zmena=1;
		}
	}


	if (zmena==1) {
		negb(PORTLED,PINLED);
		delay_ms(50);
		negb(PORTLED,PINLED);
	}
}

/*
ISR(TIMER1_CAPT_vect) {
	line_2E();
	gotoxy(2,1);
	lcd_text("CAPT");
	lcd_i(ICR1L);
}
*/

/*void zobrazF() {
	u_puts("Vysilam");
	for (uint8_t i=0; i<delkaUARTbuffU; i++) {
		posliHEX(UARTbuffU[i]);
	}
}
*/


//menuIndex=0
//1234567890123456
//----------------
//DBox v0.2
//22.12.2010 10:00
//----------------
inline void zobrazDatumCas() {
	line_1E();
	lcd_etext(eSWVer);
	line_2E();
	if (priznakPrenosu>0)
		zobrazCas(2,1);
	else
		lcd_etext(eText3);

	lcd_cislo(ID,' ',1,3,1,13);
}

//menuIndex=1
//1234567890123456
//----------------
//N 49°43.830 1250
//E013°23.708  124 
//----------------
void zobrazGPSLatLon() {
	line_1E();
	if (gps.latS<90) {
		NORTH;
		MEZERA;
		lcd_i(gps.latS);
	}
	else {
		SOUTH;
		MEZERA;
		lcd_i(256-gps.latS);
	}

	ZNAKSTUPEN;
	lcd_i(gps.latM);
	TECKA;
	lcd_i(gps.latDM);

	line_2E();
	uint16_t l=gps.lonS;
	if (l<180) {
		EAST;
	}
	else {
		WEST;
		l=359-l;
	}
	if (l<100) NULA;
	lcd_i(l);
	ZNAKSTUPEN;
	lcd_i(gps.lonM);
	TECKA;
	lcd_i(gps.lonDM);

	lcd_cislo(gps.vyska - vyskaLetiste,' ',1,4,1,13);
	lcd_cislo(gps.rychlost,' ',1,3,2,14);
}

//menuIndex=2
//1234567890123456
//----------------
//124km/h    1250m
//^325°      10sat
//----------------
inline void zobrazGPS() {
	line_1E();
	lcd_cislo(gps.rychlost,' ',1,3,1,1);
	lcd_etext(eText16);
	LOMENO;
	lcd_etext(eText7);
	lcd_cislo(gps.vyska - vyskaLetiste,' ',1,4,1,12);
	lcd_c('m');

	line_2E();
	lcd_cislo(gps.kurz,' ',1,3,2,2);
	ZNAKSTUPEN;
	lcd_cislo(gps.satInView,' ',1,2,2,12);
	lcd_etext(eText8);
}

//menuIndex=3
//1234567890123456
//----------------
//+125 -10 +15°C
//b:20.8V   p:5.1V
//----------------
void zobrazTemp() {
//+125 -10 +15  °C
//b:20.1V   p:5.1V
	/*teplota[0]=125;
	teplota[1]=-10;
	teplota[2]=15;	*/

	line_1E();

	FOR(3);
		uint8_t t=teplota[i];
		char znak='+';
		if (teplota[i]<0) {
			t=teplota[i]*-1;
			znak='-';
		}
		if (i>0) lcd_c(' ');
		lcd_c(znak);
		lcd_i(t);
	}	
	ZNAKSTUPEN;
	lcd_c('C');

	line_2E();
	lcd_etext(eText9);
	showNapetiBaterie(2,3);

	gotoxy(2,11);
	lcd_etext(eText10);
	DVOJTECKA;
	showNapetiClanek(napeti[5], 2,13);
	VOLT;
}


//menuIndex=4
void zobrazNapetiClanku() {
//1234567890123456
//----------------
//4.1 4.1 4.2 4.2V
//4.2 5.1    20.8V
//----------------
	line_1E();
	line_2E();
	showNapetiClanek(napeti[0],1,1);
	showNapetiClanek(napeti[1],1,5);
	showNapetiClanek(napeti[2],1,9);
	showNapetiClanek(napeti[3],1,13);
	VOLT;
	showNapetiClanek(napeti[4],2,1);
	showNapetiClanek(napeti[5],2,5);
	
	showNapetiBaterie(2,12);
}


void showNapetiBaterie(uint8_t r, uint8_t s) {
	uint16_t soucet=0;
	FOR(5)
		soucet+=napeti[i];
	}

	lcd_cislo(soucet/10,' ',1,2,r,s);
	lcd_c('.');
	lcd_i(soucet%10);
	VOLT;
}

void showNapetiClanek(uint8_t n, uint8_t r, uint8_t s) {
	gotoxy(r,s);
	lcd_i(n/10);
	lcd_c('.');
	lcd_i(n%10);
}

//menuIndex=5
//menu=5
//1234567890123456
//----------------
//AD1   AD2   RSSI 
//255   255    255
//----------------

//menuIndex=6
//1234567890123456
//----------------
//AD1   AD2   RSSI 
//12.4V 5.1V   255
//----------------
inline void zobrazBT0() {zobrazBT(0);}
inline void zobrazBT1() {zobrazBT(1);}

void zobrazBT(uint8_t typ) {
	line_1E();
	lcd_etext(eText11);
	lcd_i(1);
	gotoxy(1,7);
	lcd_etext(eText11);
	lcd_i(2);
	gotoxy(1,13);
	lcd_etext(eText12);
	
	line_2E();

	if (typ==0) {
		lcd_cislo(AD1,' ',1,3,2,1);
		lcd_cislo(AD2,' ',1,3,2,7);
	}
	else {
		uint32_t v = 51562*AD1;
		uint8_t cv=v/1000000;
		uint8_t dv=(v-cv*1000000)/100000;
		lcd_i(cv);
		lcd_c('.');
		lcd_i(dv);
		VOLT;

		gotoxy(2,7);
		v = 51562*AD2;
		cv=v/1000000;
		dv=(v-cv*1000000)/100000;
		lcd_i(cv);
		lcd_c('.');
		lcd_i(dv);
		VOLT;
	}

	lcd_cislo(RSSI,' ',1,3,2,14);

}

//menuIndex=7
//1234567890123456
//----------------
//Max vys :10234m
//Max rych:325km/h
//----------------
inline void zobrazMaxima() {
	line_1E();
	lcd_etext(eText13);
	MEZERA;
	lcd_etext(eText14);
	MEZERA;
	DVOJTECKA;
	lcd_cislo(gps.maxVyska,' ',1,5,1,10);
	lcd_etext(eText17);

	line_2E();
	lcd_etext(eText13);
	MEZERA;
	lcd_etext(eText15);
	DVOJTECKA
	lcd_cislo(gps.maxRychlost,' ',1,3,2,10);
	lcd_etext(eText16);
	LOMENO;
	lcd_etext(eText7);
}

//menuIndex=8
//1234567890123456
//----------------
///vyska letiste
//SET 345m n.m.
//----------------
void nulujVysku() {
	line_1E();
	lcd_etext(eText18);
	MEZERA;
	lcd_etext(eText24);


	line_2E();
	lcd_etext(eText23);
	MEZERA;
	lcd_cislo(gps.vyska,' ',0,0,2,5);
	lcd_etext(eText17);
	MEZERA;
	lcd_etext(eText20);
	TECKA;
	lcd_etext(eText17);
	TECKA;
}

//menuIndex=9 a 10
inline void setAlarmNapetiClanku() {
//Napeti jednoho z clanku
//1234567890123456
//----------------
//Napeti clanek
//<3.7 V A:1
//----------------
//----------------
//Typ alarmu cl.
//<3.7 V A:1
//----------------

	line_1E();
	if (menuIndex==9) {
		lcd_etext(eText1);
		MEZERA;
		lcd_etext(eText4);
	}
	else
	{
		lcd_etext(eText5);
		MEZERA;
		lcd_etext(eText6);
		MEZERA;
		lcd_etext(eText22);
		TECKA;
	}
	line_2E();
	lcd_c('<');
	if (alarmNapetiClanek<10) lcd_i(0);
	else lcd_i(alarmNapetiClanek/10);
	lcd_c('.');
	lcd_i(alarmNapetiClanek%10);
	VOLT;
	gotoxy(2,8);
	ALARM;
	DVOJTECKA;
	lcd_i(alarmTypNapetiClanek);
	if (alarmTypNapetiClanek>0 && menuIndex==10)
		flagBeep=alarmTypNapetiClanek;

	if (menuIndex==9) 
		gotoxy(2,5);
	else
		gotoxy(2,10);

}

//menuIndex=11 a 12
//Napeti baterie
//1234567890123456
//----------------
//Napeti baterie
//<10.7 V A:1
//----------------
//Typ alarmu bat.
//<10.7 V A:1
//----------------
inline void setAlarmNapetiBaterie() {
	line_1E();
	if (menuIndex==11) 
	{
		lcd_etext(eText1);
		MEZERA;
		lcd_etext(eText2);
	}
	else {
		lcd_etext(eText5);
		MEZERA;
		lcd_etext(eText6);
		MEZERA;
		lcd_etext(eText21);
		TECKA;
	}
	line_2E();
	lcd_c('<');
	if (alarmNapetiBaterie<10) lcd_i(0);
	else lcd_i(alarmNapetiBaterie/10);
	lcd_c('.');
	lcd_i(alarmNapetiBaterie%10);
	VOLT;
	gotoxy(2,8);
	ALARM;
	DVOJTECKA;
	lcd_i(alarmTypNapetiBaterie);
	if (alarmTypNapetiBaterie>0 && menuIndex==12)
		flagBeep=alarmTypNapetiBaterie;

	if (menuIndex==11) 
		gotoxy(2,5);
	else
		gotoxy(2,10);

}


////////////////////////////
inline void setModelName() {
	line_1E();
	lcd_etext(eText25); //Jmeno
	MEZERA;
	lcd_etext(eText26); //modelu

	line_2E();
	FOR(10)
			lcd_c(modelName[i]);
	}
	gotoxy(2,11);
	lcd_c('(');
	lcd_i(10);
	lcd_etext(eText27);
	lcd_c(')');

	gotoxy(2,menuIndex-12);
}



inline void zobrazCas(uint8_t r, uint8_t s) {
	gotoxy(r,s);
	if (datum.den<10) lcd_i(0);
	lcd_i(datum.den);
	lcd_c('.');
	if (datum.mes<10) lcd_i(0);
	lcd_i(datum.mes);
	lcd_c('.');
	lcd_i(datum.rok+1980);
	lcd_c(' ');
	if (cas.hod<10) lcd_i(0);
	lcd_i(cas.hod);
	lcd_c(':');
	if (cas.min<10) lcd_i(0);
	lcd_i(cas.min);
	/*lcd_c(':');
	if (cas.sec<10) lcd_i(0);
	lcd_i(cas.sec);*/
}


/*
ISR(INT0_vect) {
	lcd_text("INT");
	u_puts("INT");
	char b[4];
	for (uint8_t i=0; i<255; i++) {
		itoa (UARTbuffU[i],b,16);

		u_puts(b);
		u_putc(' ');
	}
}
*/

uint8_t readBuffer() {
	if (poziceBufferU==0) return 0;
    uint8_t b = UARTbuffU[0];
	FOR(delkaUARTbuffU)
    //for (uint8_t i=0; i<delkaUARTbuffU; i++) {
		UARTbuffU[i] = UARTbuffU[i+1];
    }
    if (poziceBufferU>0) poziceBufferU--;

	//if (poziceBufferU>bm) bm=poziceBufferU;
	return b;
}

/*
void posliHEX(char z) {
	char buffer [5];
	itoa (z,buffer,16);
	u_putc(' ');
	u_puts(buffer);
}
*/
void setGPS(uint8_t *b) {
	switch (b[0]) {
		case 0xA0:
			gps.latS = b[1];
			break;
		case 0xA1:
			gps.latM = b[1];
			break;
		case 0xA2:
			gps.latDM = (b[1] << 7) | b[2];
			break;
		case 0xA3:
			gps.lonS = b[1];
			break;
		case 0xA4:
			gps.lonM = b[1];
			break;
		case 0xA5:
			gps.lonDM = (b[1] << 7) | b[2];
			break;
		case 0xA6:
			gps.vyska = (b[1] << 7) | b[2];
			menuRozsah[8][0]=gps.vyska;
			gps.vyska - vyskaLetiste > gps.maxVyska ? gps.maxVyska = gps.vyska - vyskaLetiste: gps.maxVyska;
			break;
		case 0xA7:
    		gps.rychlost = (b[1] << 7) | b[2]; 
			if (gps.rychlost>gps.maxRychlost)
				gps.maxRychlost=gps.rychlost;
			break;
		case 0xA8:
    		gps.kurz = (b[1] << 7) | b[2];
			break;
		case 0xA9:
    		gps.satInView = b[1];
			break;
		case 0xAA:
    		//datumK = (b[1] << 14) | (b[2] << 7) | b[3];
			//unpackDate(datumK, &den, &mes, &rok);
			unpackDate((b[1] << 14) | (b[2] << 7) | b[3]);
			break;
    	case 0xAB:
	    	//casK = (b[1] << 14) | (b[2] << 7) | b[3];
			//unpackTime(casK, &hod, &min, &sec);
			unpackTime((b[1] << 14) | (b[2] << 7) | b[3]);
			break;
    }
}


inline void unpackTime(uint16_t c) {
	uint16_t p=0;
	p=c;
	cas.hod=(p&=0xF800)>>11;
	p=c;
	cas.min=(p&=0x7E0)>>5;
	p=c;
	cas.sec=(p&=0x1F)*2;
}


/*void unpackTime(uint16_t cas, uint8_t* h, uint8_t *m, uint8_t *s) {
	uint16_t p=0;
	p=cas;
	*h=(p&=0xF800)>>11;
	p=cas;
	*m=(p&=0x7E0)>>5;
	p=cas;
	*s=(p&=0x1F)*2;
}*/

inline void unpackDate(uint16_t d) {
	uint16_t p=0;
	p=d;
	datum.rok=((p&=0xFE00)>>9);
	p=d;
	datum.mes=(p&=0x1E0)>>5;
	p=d;
	datum.den=p&=0x1F;
}

/*void unpackDate(uint16_t datum, uint8_t* d, uint8_t *m, uint16_t *r) {
	uint16_t p=0;
	p=datum;
	*r=((p&=0xFE00)>>9)+1980;
	p=datum;
	*m=(p&=0x1E0)>>5;
	p=datum;
	*d=p&=0x1F;
}*/


void setKoef(uint8_t *b) {
	koef[b[0] - 0x94] = (b[1] << 7) | b[2];
}

void setTeplota(uint8_t *b) {
	teplota[b[0] - 0x8A] = (((b[1] << 7) | b[2])) / 10;
}

void setID(uint8_t *b) {
	ID = (b[1] << 7) | b[2];
}

void setNapeti(uint8_t *b) {
	uint16_t n=(b[1] << 7) | b[2];
	n = 41 * n / 1000;
    napeti[b[0] - 0x81] = n;
}


inline void zpracujFrontu() {
	//zpracovani fronty


	char b=0;
	uint8_t byt[4];


	while (poziceBufferU>5) {
		//b=readBuffer();
		
		if ((b=readBuffer())>0) {
			if ((b&0x80)==0x80) { //horni byt
				uint8_t a=b;
                byt[0]=a;
                if (a==0x80) { //ID
	                byt[1]=readBuffer();
                    byt[2]=readBuffer();
                    setID(byt);
                }
				if (a>=0x81 && a<=0x86) {//napeti
                    byt[1] = readBuffer();
                    byt[2] = readBuffer();
                    setNapeti(byt);
                }
				if (a>=0x94 && a<=0x99) {//koeficienty pro prepocet napeti
                    byt[1] = readBuffer();
                    byt[2] = readBuffer();
                    setKoef(byt);
                }
				if (a>=0x8A && a<=0x8D) {//teplota
                    byt[1] = readBuffer();
                    byt[2] = readBuffer();
                    setTeplota(byt);
                }
				if (a>=0xA0 && a<=0xAB) { //GPS
	                //32,33,35,36,41 (A0,A1,A3,A4,A9)   1 byte
                    byt[1] = readBuffer();
		            //34,37,38,39,40,42,43 (A2,A5,A6,A7,A8,AA,AB   2 byte
                    if (a==0xA2 || a==0xA5 || a==0xA6 || a==0xA7 
						|| a==0xA8 || a==0xAA || a==0xAB) {
                        byt[2] = readBuffer();
                    }
	                //42,43 (AA,AB)             3byte
                    if (a==0xAA || a==0xAB) {
	                    byt[3] = readBuffer();
                    }
                    setGPS(byt);
				}
			}
		}
	}
}


uint8_t testPriznak7D(uint8_t* znak) {
	if (*znak==0x7D) {
		priznak7D=1;
		return 1;
	}

	if (priznak7D==1) {
		//predchozi znak byl 7D, ten jsem zahodil a tento znak xoruji
		*znak^=0x20;
		priznak7D=0;
	}

	return 0;
}


void nuluj() {
	gps.latS=0;
	gps.latM=0;
	gps.lonS=0;
	gps.lonM=0;
	gps.latDM=0;
	gps.lonDM=0;
	gps.satInView=0;
	gps.rychlost=0;
	gps.vyska=0;
	gps.kurz=0;
	AD1=0;
	AD2=0;
	RSSI=0;
	cas.hod=0;
	cas.min=0;
	cas.sec=0;
	datum.den=1;
	datum.mes=1;
	datum.rok=31; //31+1980=2011
	FOR(4)
	//for (uint8_t i=0; i<4; i++) 
		teplota[i]=0;
	}
	FOR(6)
	//for (uint8_t i=0; i<6; i++) 
		napeti[i]=0;
	}
}



void beep(uint8_t maska) {
	if ((beepMaska[maska]&1<<beepCitac%16)>0) 
		setb(PORTBEEP,PINBEEP);
	else 
		clrb(PORTBEEP,PINBEEP);
	beepCitac++;
	if (beepCitac==16 || (beepCitac==2 && maska==0)) {
		beepCitac=0;
		flagBeep=255;
	}
}

inline void beepInit() {
	beepMaska[0]=0b0000100000000001;
	beepMaska[1]=0b0000000000001111;
	beepMaska[2]=0b0000000001010011;
	beepMaska[3]=0b0000000101010011;
	setb(DDRBEEP,PINBEEP);
}


//		INT
//------------------------------------------------------------------------
ISR(TIMER1_COMPA_vect) {
	sei();
	TCCR1B=0x0; //zastaveni citace

	wdt_reset();              //nulování watchdog

	if (flagBeep<255)
		beep(flagBeep);

	if (priznakPrenosu==0) {
		nuluj();
		setb(PORTLED,PINLED);
	}
	else
		negb(PORTLED,PINLED);

	if (ulozData==0) {
		ulozDataEEPROM();
		ulozData=0xff;
	}


	zpracuj();

	if (priznakResetMax==1) {
		gps.maxVyska=0;
		gps.maxRychlost=0;
		priznakResetMax=0;
	}	


	pfMenu[menuIndex]();

	cursor_off();
	if (menuIndex>=9 && menuIndex<=22)
		cursor_on();


	/*if (menuIndex==0) zobrazDatumCas();
	if (menuIndex==1) zobrazGPSLatLon();
	if (menuIndex==2) zobrazGPS();
	if (menuIndex==3) zobrazTemp();
	if (menuIndex==4) zobrazNapetiClanku();
	if (menuIndex==5) zobrazBT(0);
	if (menuIndex==6) zobrazBT(1);
	if (menuIndex==7) zobrazMaxima();
	if (menuIndex==8) nulujVysku();
	if (menuIndex==9 || menuIndex==10) setAlarmNapetiClanku();
	if (menuIndex==11 || menuIndex==12) setAlarmNapetiBaterie();
	if (menuIndex>=13 && menuIndex<=22) setModelName();
	*/

	citac++;
	if (priznakPrenosu>0)
		priznakPrenosu--;
	if (ulozData>0 && ulozData<0xff)
		ulozData--;
	TCCR1B=0x0D; //obnova citace
}


ISR(USART_RXC_vect) {

	unsigned char znak=u_getc();

	priznakPrenosu=10;

	if (znak==0xFE && predchoziZnak==0x7E) { //priznak zacatku bloku zakladni telemetrie
		tt=1;
		pocetZnaku=0;
		poziceBufferB=0;
	}


	if (znak==0xFD && predchoziZnak==0x7E) { //priznak zacatku bloku user telemetrie
		tt=2;
		pocetZnaku=0;
		pozice=0;
		delkaBloku=0;
	}

	if (tt==1) { 	//base telemetry
		if (pocetZnaku<3) {
			if (testPriznak7D(&znak)==0) {
				UARTbuffB[poziceBufferB++]=znak;
				pocetZnaku++;
			}
		}
		if (znak==0x7E || pocetZnaku==3) {
			//reading=0;
			AD1=UARTbuffB[0];
			AD2=UARTbuffB[1];
			RSSI=UARTbuffB[2];
		}
	}
	if (tt==2) { 	//user telemetry
		if (pozice==1) delkaBloku=znak;

		if (znak==0x7E || (delkaBloku==pocetZnaku && delkaBloku>0)) {
			zpracujFrontu();
			pozice=0;
			tt=0;
		}

		//pozice 2 se preskakuje
		if (pozice>2) {
			if (testPriznak7D(&znak)==0) {
				UARTbuffU[poziceBufferU++]=znak;
				pocetZnaku++;
			}
		}
	}
	pozice++;
	predchoziZnak=znak;
}


int main(void) {
	init();
	sei();

	wdt_enable(6);        //zapni wdt (6 = interval 1 sec)

	while(1)//neverending loop
	{
	}
	return 0;
}