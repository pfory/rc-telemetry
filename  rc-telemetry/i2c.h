// i2c.h
// Funkce pro sb�rnici I2C - master m�d

// Z�pis a �ten� pam�ti eeprom 24c01 (24c02) po sb�rnici i2c.
// Zapojen� je na obr i2c.jpg

// �ty�bitov� k�d sou��stky pro 24c01 je 1010. (zjist�me v datasheet)
// T��bitov� dresa sou��stky je nastavena zapojen�m pin� A0,A1,A2.
// My jsme v�echny uzemnili, tak�e adresa = 000.

// Adresa �ipu pro z�pis:
// k�d sou��stky   adresa sou��stky  z�pis=0
//     1010             000             0    = 10100000 = 0xa0

// Adresa �ipu pro �ten�:
// k�d sou��stky   adresa sou��stky  �ten�=1
//     1010             000             1    = 10100001 = 0xa1


void          i2c_start     (void);              //start
void          i2c_wbyte     (unsigned char byt); //zapi� bajt
unsigned char i2c_rbyte_ack (void);              //�ti bajt s ACK
unsigned char i2c_rbyte_nack(void);              //�ti bajt bez ACK
void          i2c_stop      (void);              //stop


//upravit podle zapojen�:
asm(".equ DDRi , 0x11"); //ddrd    (podle pou�it�ho portu)
asm(".equ PINi , 0x10"); //pind    (podle pou�it�ho portu)
asm(".equ CLK , 2");     //i2c clock = portd.2
asm(".equ DTA , 3");     //i2c data  = portd.3

//;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

void i2c_start(void)
{
  asm volatile("sbi 	DDRi , CLK");
  asm volatile("cbi 	DDRi, DTA");
  asm volatile("rcall 	waitus5");
  asm volatile("cbi 	DDRi, CLK");
  asm volatile("rcall   waitus5");
  asm volatile("sbi 	DDRi, DTA");
}


//;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

void i2c_wbyte(unsigned char byt)
{
  asm volatile("push 	r6");
  asm volatile("push 	r17");
  asm volatile("mov 	r17,r24");
  asm volatile("sec");
  asm volatile("rol 	r17");
  asm volatile("rjmp 	Avr016c");
asm volatile("Avr016b:");
  asm volatile("lsl 	R17");
asm volatile("Avr016c:");
  asm volatile("Breq 	Avr017a");
  asm volatile("sbi 	DDRi, CLK");
  asm volatile("brsh 	Avr0172");
  asm volatile("nop");
  asm volatile("cbi 	DDRi, DTA");
  asm volatile("rjmp 	Avr0174");
asm volatile("Avr0172:");
  asm volatile("Sbi 	DDRi , DTA");
  asm volatile("rjmp 	Avr0174");
asm volatile("Avr0174:");
  asm volatile("Rcall 	waitus5");
  asm volatile("cbi 	DDRi, CLK");
asm volatile("Avr0176:");
  asm volatile("Sbis 	PINi , CLK");
  asm volatile("rjmp 	Avr0176");
  asm volatile("rcall 	waitus5");
  asm volatile("rjmp 	Avr016b");
asm volatile("Avr017a:");
  asm volatile("Sbi 	DDRi , CLK");
  asm volatile("cbi 	DDRi, DTA");
  asm volatile("rcall   waitus5");
  asm volatile("cbi 	DDRi, CLK");
asm volatile("Avr017e:");
  asm volatile("Sbis 	PINi , CLK");
  asm volatile("rjmp 	Avr017e");
  asm volatile("clt");
  asm volatile("sbic 	PINi, DTA");
  asm volatile("set");
  asm volatile("bld 	r6, 2");
  asm volatile("rcall   waitus5");
  asm volatile("pop 	r17");
  asm volatile("pop 	r6");
}

//;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

unsigned char i2c_rbyte_ack(void)
{
register unsigned char eeval asm("r4");

  asm ("push r17");
  asm volatile("clc");
  asm volatile("rcall	read_byte");
  asm volatile("mov 	r4,r17");
  asm volatile("pop 	r17");
  return eeval;
}

//;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

unsigned char i2c_rbyte_nack(void)
{
register unsigned char eeval asm("r4");

  asm ("push r17");
  asm volatile("sec");
  asm volatile("rcall 	read_byte");
  asm volatile("mov 	r4,r17");
  asm volatile("pop 	r17");
  return eeval;
}

asm("read_byte:");
  asm("ldi 		r17, 1");
asm("Avr0196:");
  asm("Sbi 		DDRi , CLK");
  asm("cbi 		DDRi, DTA");
  asm("rcall 	waitus5");
  asm("cbi 		DDRi, CLK");
asm("Avr019a:");
  asm("Sbis 	PINi , CLK");
  asm("rjmp 	Avr019a");
  asm("rcall 	waitus5");
  asm("clc");
  asm("sbic 	PINi, DTA");
  asm("sec");
  asm("rol 		r17");
  asm("brsh 	Avr0196");
  asm("sbi 		DDRi, CLK");
  asm("brsh 	Avr01a7");
  asm("cbi 		DDRi, DTA");
  asm("rjmp 	Avr01a8");
asm("Avr01a7:");
  asm("Sbi 		DDRi , DTA");
asm("Avr01a8:");
  asm("Rcall 	waitus5");
  asm("cbi 		DDRi, CLK");
asm("Avr01aa:");
  asm("Sbis 	PINi , CLK");
  asm("rjmp 	Avr01aa");
  asm("rcall 	waitus5");
  asm("ret");

//;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
void i2c_stop(void)
{
  asm volatile("sbi 	DDRi, CLK");
  asm volatile("sbi 	DDRi, DTA");
  asm volatile("rcall 	waitus5");
  asm volatile("cbi 	DDRi, CLK");
//asm volatile("Avr0221:");
  asm volatile("sbis 	PINi, CLK");
  //asm volatile("rjmp 	Avr0221");
  asm volatile("rcall 	waitus5");
  asm volatile("cbi 	DDRi, DTA");
  asm volatile("rcall 	waitus5");
}

asm("waitus5:");
  asm("push 	r17");
  asm("ldi 		r17,30");
asm("waitus51:");
  asm("dec 		r17");
  asm("brne 	waitus51");
  asm("pop 		r17");
  asm("ret");

//end



void write_24c01(unsigned char adr,unsigned char val);
char read_24c01 (unsigned char adr);

unsigned char adr_w = 0xa0;    //adresa �ipu pro z�pis
unsigned char adr_r = 0xa1;    //adresa �ipu pro �ten�

//zapi� hodnotu "val" na adresu "adr":
void write_24c01(unsigned char adr,unsigned char val)
{
    i2c_start();
	i2c_wbyte(adr_w);
    i2c_wbyte(adr);          //adresa bu�ky v eeprom
    i2c_wbyte(val);          //hodnota k z�pisu
    i2c_stop();
    delay_ms(10);
}

//�ti hodnotu na adrese "adr":
char read_24c01(unsigned char adr)
{
unsigned char value;
    i2c_start();
	i2c_wbyte(adr_w);
    i2c_wbyte(adr);
    i2c_start();
    i2c_wbyte(adr_r);
    value = i2c_rbyte_nack();  //�ten�
    i2c_stop();
    return value;
}
//end




