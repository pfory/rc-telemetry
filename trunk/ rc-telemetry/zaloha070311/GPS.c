#include "GPS.h"
#include <string.h>
#include <stdlib.h>


volatile uint16_t cas, datum;
volatile int8_t latS; 
volatile int16_t latDM, lonS, lonDM;
volatile uint16_t vyska, kurz;
volatile uint8_t latM, lonM, rychlost, satInView;


uint16_t crc(char *b) {
	uint16_t ks=0;
	for (uint8_t i=1; i<84; i++) {
		if (b[i]==42) 
			return ks;
		ks^=b[i];
	}
	return 0;
}

uint8_t getV() {
	return rychlost;
}
uint8_t getSIV() {
	return satInView;
}
uint8_t getLatM() {
	return latM;
}
uint8_t getLonM() {
	return lonM;
}

int8_t getLatS() {
	return latS;
}

uint16_t getH() {
	return vyska;
}
uint16_t getK() {
	return kurz;
}

int16_t getLonS() {
	return lonS;
}
int16_t getLatDM() {
	return latDM;
}
int16_t getLonDM() {
	return lonDM;
}


uint16_t packTime(uint8_t hod, uint8_t min, uint8_t sec) {
	uint16_t cas;
	cas=hod<<6;
	cas|=min;
	cas=cas<<5;
	cas|=sec/2;
	return cas;
}

uint16_t packDate(uint16_t rok, uint8_t mes, uint8_t den) {
	uint16_t datum;
	datum=(rok-1980)<<4;
	datum|=mes;
	datum=datum<<5;
	datum|=den;
	return datum;
}

uint16_t getDate() {
	return datum;
}

uint16_t getTime() {
	return cas;
}

void setDatum(char *b, uint8_t car) {
	char pb[3];
	nulujRetezec(pb,3);
	pb[2]='\0';

	uint16_t rok;
	uint8_t mes;
	uint8_t den;

	char *poz=hledejZnak(b,',',car)+1;
	strncpy(pb,poz,2);
	den=atoi(pb);
	strncpy(pb,poz+2,2);
	mes=atoi(pb);
	strncpy(pb,poz+4,2);
	rok=atoi(pb);
	rok+=2000;
	datum=packDate(rok, mes, den);
}

void setCas(char *b, uint8_t car) {
	char pb[3];
	nulujRetezec(pb,3);
	pb[2]='\0';

	uint8_t hod;
	uint8_t min;
	uint8_t sec;

	char *poz=hledejZnak(b,',',car)+1;
	strncpy(pb,poz,2);
	hod=atoi(pb);
	strncpy(pb,poz+2,2);
	min=atoi(pb);
	strncpy(pb,poz+4,2);
	sec=atoi(pb);
	cas=packTime(hod, min, sec);
}

void setLat(char *b, uint8_t car) {
	char pb[9+1];
	nulujRetezec(pb,10);
	int8_t sign=1;
	char pb1[4];
	nulujRetezec(pb1,4);

	char *pozS=hledejZnak(b,',',car+1)+1;
	strncpy(pb,pozS,1);

	if (pb[0]=='S') sign=-1;

	if (getSubstring(b, pb, car)==0) return;

	pb1[0]=pb[0];
	pb1[1]=pb[1];
	pb1[2]='\0';

	latS=atoi(pb1)*sign;

	pb1[0]=pb[2];
	pb1[1]=pb[3];
	pb1[2]='\0';

	latM=atoi(pb1);

	pb1[0]=pb[5];
	pb1[1]=pb[6];
	pb1[2]=pb[7];
	pb1[3]='\0';

	latDM=atoi(pb1);
}


void setLon(char *b, uint8_t car) {
	char pb[9+1];
	nulujRetezec(pb,10);
	int8_t sign=1;
	char pb1[4];

	char *pozS=hledejZnak(b,',',car+1)+1;
	strncpy(pb,pozS,1);

	if (pb[0]=='W') sign=-1;

	if (getSubstring(b, pb, car)==0) return;

	pb1[0]=pb[0];
	pb1[1]=pb[1];
	pb1[2]=pb[2];
	pb1[3]='\0';

	lonS=atoi(pb1)*sign;

	pb1[0]=pb[3];
	pb1[1]=pb[4];
	pb1[2]='\0';

	lonM=atoi(pb1);

	pb1[0]=pb[6];
	pb1[1]=pb[7];
	pb1[2]=pb[8];
	pb1[3]='\0';

	lonDM=atoi(pb1);
}


void setAlt(char *b, uint8_t car) {
	char pb[7+1];
	nulujRetezec(pb,8);
	pb[strlen(pb)]='\0';

	if (getSubstring(b, pb, car)==0) return;

	//zaokrouhleni, nahradim tecku koncovym znakem
	char *pozS=strstr(pb,".");
	strncpy (pozS,"\0",1);

	vyska=atoi(pb);
}

void setSpeed(char *b, uint8_t car) {
	char pb[6+1];
	char pb1[3];
	nulujRetezec(pb,7);
	nulujRetezec(pb1,3);

	if (getSubstring(b, pb, car)==0) return;

	//abych se vyhnul necelociselnym operacim
	char *pozS=hledejZnak(pb,'.',1);
	strncpy(pb1,pb,pozS-pb);
	uint16_t u=atoi(pb1);
	u*=KOEFUZEL;

	nulujRetezec(pb1,3);
	strncpy(pb1,pozS+1,2);
	uint16_t ud=atoi(pb1);
	ud*=KOEFUZEL;
	ud/=100;

	u+=ud;
	rychlost=u/1000;
}


void setSatInView(char *b, uint8_t car) {
	char pb[2+1];
	nulujRetezec(pb,3);
	pb[2]='\0';

	if (getSubstring(b, pb, car)==0) return;

	satInView=atoi(pb);
}



void setKurz(char *b, uint8_t car) {
	char pb[5+1];
	nulujRetezec(pb,6);

	if (getSubstring(b, pb, car)==0) return;

	kurz=atoi(pb);

/*	if (k>337) ZNAKN;
	if (k<23) ZNAKN;
	if ((k>=22)&(k<68)) ZNAKNE;
	if ((k>=68)&(k<113)) ZNAKE;
	if ((k>=113)&(k<158)) ZNAKSE;
	if ((k>=158)&(k<203)) ZNAKS;
	if ((k>=203)&(k<248)) ZNAKSW;
	if ((k>=248)&(k<293)) ZNAKW;
	if ((k>=293)&(k<338)) ZNAKNW;
*/
}


uint8_t getSubstring(char *b, char *pb, uint8_t car) {
	char *pozS=hledejZnak(b,',',car)+1;
	char *pozK=hledejZnak(b,',',car+1);
	if (pozK-pozS==0) return 0;
	strncpy(pb,pozS,pozK-pozS);
	return 1;
}



//hleda n-ty vyskyt z v b a vraci ukazatel na jeho pozici v b
char * hledejZnak(char *b, char z, uint8_t n) {
	char *poz=strchr(b, z);
	uint8_t c=1;

	if (n==1) return poz;

	while (poz!=NULL) {
		poz=strchr(poz+1, z);
		c++;
		if (c==n) return poz;
	}
	return 0;
}


void nulujRetezec(char *r, uint8_t kolik) {
	memset (r,'\0',kolik);
}

		
