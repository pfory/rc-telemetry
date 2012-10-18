
// i2c.h
// Funkce pro sbìrnici I2C - master mód

void          i2c_start     (void);              //start
void          i2c_wbyte     (unsigned char byt); //zapiš bajt
unsigned char i2c_rbyte_ack (void);              //èti bajt s ACK
unsigned char i2c_rbyte_nack(void);              //èti bajt bez ACK
void          i2c_stop      (void);              //stop


//upravit podle zapojení:
asm(".equ DDRi , 0x31"); //ddrd    (podle použitého portu)
asm(".equ PINi , 0x30"); //pind    (podle použitého portu)
asm(".equ CLK , 5");     //i2c clock = portd.5
asm(".equ DTA , 6");     //i2c data  = portd.6

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
  asm volatile("sbi 	DDRi, CLK\n\t"
  				"sbi 	DDRi, DTA\n\t"
  				"rcall 	waitus5\n\t"
  				"cbi 	DDRi, CLK\n\t"
				"Avr0221:\n\t"
  				"sbis 	PINi, CLK\n\t"
  				"rjmp 	Avr0221\n\t"

  				//"rcall 	waitus5\n\t\"
  				"cbi 	DDRi, DTA\n\t"
				"rcall 	waitus5");
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
