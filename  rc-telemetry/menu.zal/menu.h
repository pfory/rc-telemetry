#ifndef MENU_H_
#define MENU_H_

#include "..\utils.h"


#define MENUCOUNT 13  //index 0...

#define keyUPPort PINB	//UP key ^
#define keyUPPin 2	
#define keyUPPull PORTB
#define keyDNPort PINB	//DOWN key v
#define keyDNPin 3	
#define keyDNPull PORTB
#define keyNPort PINB	//Next key ->
#define keyNPin 4
#define keyNPull PORTB
#define keyPPort PINB	//Previous key <-
#define keyPPin 5
#define keyPPull PORTB
#define citacOpakMez 5

void setVar(int*, uint8_t, int, int);
int *menuFce[MENUCOUNT];
int menuRozsah[MENUCOUNT][2];
void (*pf[MENUCOUNT])(int*, uint8_t, int, int);
void zpracuj(void);
void menuInit(void);
int getMenuIndex(void);

extern int menuIndex;
extern uint8_t ulozData;
extern uint8_t flagBeep;

#endif //MENU_H_
