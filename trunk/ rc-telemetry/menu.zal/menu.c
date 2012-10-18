#include <avr/io.h>
#include "menu.h"

uint8_t citacOpak=0;
uint8_t lastKey=0;
int menuIndex=0;
uint8_t ulozData;


void setVar(int *uk, uint8_t op, int Lval, int Hval) {
	ulozData=10;
	if (op==1) {
		*uk=*uk+1;
		if (*uk>Hval) *uk=Lval;
	}
	if (op==0) {
		*uk=*uk-1;
		if (*uk<Lval) *uk=Hval;
	}
}

int getMenuIndex() {
	return menuIndex;
}


int *menuFce[MENUCOUNT];
int menuRozsah[MENUCOUNT][2];

void (*pf[MENUCOUNT])(int *uk, uint8_t op, int Lval, int Hval);


void menuInit() {
	menuIndex=0;
	setb(keyUPPull,keyUPPin); //pullup
	setb(keyDNPull,keyDNPin); //pullup
	setb(keyNPull,keyNPin); //pullup
	setb(keyPPull,keyPPin); //pullup
	
	pf[0]=&setVar;
}

void zpracuj() {
	uint8_t key=0;

	if (bit_is_clear(keyUPPort,keyUPPin)) {
		key=8;
		if (lastKey==8) {
			citacOpak++;
			if (citacOpak<citacOpakMez) return; 
		}
		pf[0](menuFce[menuIndex], 1, menuRozsah[menuIndex][0], menuRozsah[menuIndex][1]);
	}
	else if (bit_is_clear(keyDNPort,keyDNPin)) {
		key=4;
		if (lastKey==4) {
			citacOpak++;
			if (citacOpak<citacOpakMez) return; 
		}
		pf[0](menuFce[menuIndex], 0, menuRozsah[menuIndex][0], menuRozsah[menuIndex][1]);
	}
	else if (bit_is_clear(keyNPort,keyNPin)) {
		key=2;
		if (lastKey==2) {
			citacOpak++;
			if (citacOpak<citacOpakMez) return; 
		}
		pf[0](&menuIndex, 1, 0, MENUCOUNT-1);
	}
	else if (bit_is_clear(keyPPort,keyPPin)) {
		key=1;
		if (lastKey==1) {
			citacOpak++;
			if (citacOpak<citacOpakMez) return; 
		}
		pf[0](&menuIndex, 0, 0, MENUCOUNT-1);
	}

	if (key==0) {
		lastKey=0;
		citacOpak=0;
	}
	else {
		flagBeep=0;
		lastKey=key;
	}
}



