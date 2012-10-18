// i2c.h
// Funkce pro sbìrnici I2C - master mód

// Zápis a ètení pamìti eeprom 24c01 (24c02) po sbìrnici i2c.
// Zapojení je na obr i2c.jpg

// Ètyøbitový kód souèástky pro 24c01 je 1010. (zjistíme v datasheet)
// Tøíbitová dresa souèástky je nastavena zapojením pinù A0,A1,A2.
// My jsme všechny uzemnili, takže adresa = 000.

// Adresa èipu pro zápis:
// kód souèástky   adresa souèástky  zápis=0
//     1010             000             0    = 10100000 = 0xa0

// Adresa èipu pro ètení:
// kód souèástky   adresa souèástky  ètení=1
//     1010             000             1    = 10100001 = 0xa1


void          i2c_start     (void);              //start
void          i2c_wbyte     (unsigned char byt); //zapiš bajt
unsigned char i2c_rbyte_ack (void);              //èti bajt s ACK
unsigned char i2c_rbyte_nack(void);              //èti bajt bez ACK
void          i2c_stop      (void);              //stop


//upravit podle zapojení:
asm(".equ DDRi , 0x11"); //ddrd    (podle použitého portu)
asm(".equ PINi , 0x10"); //pind    (podle použitého portu)
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

unsigned char adr_w = 0xa0;    //adresa èipu pro zápis
unsigned char adr_r = 0xa1;    //adresa èipu pro ètení

//zapiš hodnotu "val" na adresu "adr":
void write_24c01(unsigned char adr,unsigned char val)
{
    i2c_start();
	i2c_wbyte(adr_w);
    i2c_wbyte(adr);          //adresa buòky v eeprom
    i2c_wbyte(val);          //hodnota k zápisu
    i2c_stop();
    delay_ms(10);
}

//èti hodnotu na adrese "adr":
char read_24c01(unsigned char adr)
{
unsigned char value;
    i2c_start();
	i2c_wbyte(adr_w);
    i2c_wbyte(adr);
    i2c_start();
    i2c_wbyte(adr_r);
    value = i2c_rbyte_nack();  //ètení
    i2c_stop();
    return value;
}
//end




