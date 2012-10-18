#include "BMP085.h"
#include "i2csoft.h"
#include <avr/io.h>          
#include "uart.h"   

//#define verbose
//#define fakeData

static uint16_t BMP085read16(uint8_t address);
static uint8_t BMP085read8(uint8_t address);
static uint8_t BMP085Write(uint8_t address,uint8_t data);
static uint8_t BMP085read(uint8_t address);
static uint16_t BMP085readRawTemperature(void);
static uint32_t BMP085readRawPressure(void);

static int16_t ac1, ac2, ac3, b1, b2, mb, mc, md;
static uint16_t ac4, ac5, ac6;

static uint8_t BMP085read(uint8_t address)
{
  uint8_t res;   //result
  uint8_t data;
  //Start
  SoftI2CStart();
  //SLA+W (for dummy write to set register pointer)
  res=SoftI2CWriteByte(BMP085_I2CADDR_W); //BMP085 address + W
  //Error
  if(!res) return 0;
  //Now send the address of required register
  res=SoftI2CWriteByte(address);
  //Error
  if(!res) return 0;
  //Repeat Start
	SoftI2CStart();
	//SLA + R
	res=SoftI2CWriteByte(BMP085_I2CADDR_R);	//DS1307 Address + R
	//Error
	if(!res)	return 0;
  //Now read the value with NACK
  data=SoftI2CReadByte(0);
  //Error
  if(!res) return 0;
  //STOP
  SoftI2CStop();

  return data;
}

static uint8_t BMP085Write(uint8_t address,uint8_t data)
{
   uint8_t res;   //result
   //Start
   SoftI2CStart();
   //SLA+W
   res=SoftI2CWriteByte(BMP085_I2CADDR_W); //BMP085 address + W
   //Error
   if(!res) return 0;
   //Now send the address of required register
   res=SoftI2CWriteByte(address);
   //Error
   if(!res) return 0;
   //Now write the value
   res=SoftI2CWriteByte(data);
   //Error
   if(!res) return 0;
   //STOP
   SoftI2CStop();

   return 1;
}

void BMP085Init()
{
	#ifdef verbose
	u_puts("Init BMP085");
  #endif
  #ifdef fakeData
	ac1 = 408;
	ac2 = -72;
	ac3 = -14383;
	ac4 = 32741;
	ac5 = 32757;
	ac6 = 23153;
	b1 = 6190;
	b2 = 4;
	mb = -32767;
	mc = -8711;
	md = 2868;
	#else
	ac1 = BMP085read16(BMP085_CAL_AC1);
	ac2 = BMP085read16(BMP085_CAL_AC2);
	ac3 = BMP085read16(BMP085_CAL_AC3);
	ac4 = BMP085read16(BMP085_CAL_AC4);
	ac5 = BMP085read16(BMP085_CAL_AC5);
	ac6 = BMP085read16(BMP085_CAL_AC6);
	b1 = BMP085read16(BMP085_CAL_B1);
	b2 = BMP085read16(BMP085_CAL_B2);
	mb = BMP085read16(BMP085_CAL_MB);
	mc = BMP085read16(BMP085_CAL_MC);
	md = BMP085read16(BMP085_CAL_MD);
	#endif
	
	#ifdef verbose
  u_puts("AC1-AC6 ");
  u_puti(ac1);
  u_putc(' ');

  u_puti(ac2);
  u_putc(' ');

  u_puti(ac3);
  u_putc(' ');

  u_puti(ac4);
  u_putc(' ');

  u_puti(ac5);
  u_putc(' ');

  u_puti(ac6);
  u_putc(' ');
  
  u_puts("B1-B2 ");
  u_puti(b1);
  u_putc(' ');
  u_puti(b2);
  u_putc(' ');

  u_puts("MB-MD ");
  u_puti(mb);
  u_putc(' ');

  u_puti(mc);
  u_putc(' ');

  u_puti(md);
  u_putc(' ');
  #endif

}

uint8_t BMP085read8(uint8_t address)
{
	return BMP085read(address);
}


uint16_t BMP085read16(uint8_t address)
{
	uint16_t data;
	data = BMP085read8(address);
  data <<= 8;	
	data += BMP085read8(address+1);
	return data;
}


//-------------TEMPERATURE------------------------
void BMP085startTemperature(void)
{
  BMP085Write(BMP085_CONTROL, BMP085_READTEMPCMD);
}


int16_t BMP085readTemperature(void)
{
  int32_t ut = BMP085readRawTemperature();
  int32_t x1 = ((int32_t)ut - ac6) * ac5 >> 15;
  int32_t x2 = ((int32_t) mc << 11) / (x1 + md);
  int32_t b5 = x1 + x2;
  int16_t T = (b5 + 8) >> 4;

  #ifdef verbose
  u_puts("T=");
  u_puti(T/10);
  u_putc('.');
  u_puti(T%10);
  u_putc(' ');
  #endif
   
  return T;
}

uint16_t BMP085readRawTemperature(void)
{
  #ifdef fakeData
  return 27898;
  #endif

  //BMP085Write(BMP085_CONTROL, BMP085_READTEMPCMD);
  //_delay_ms(5);
  return BMP085read16(BMP085_TEMPDATA);
}

//----------PRESSURE-----------------------------------
void BMP085startPressure(void)
{
  BMP085Write(BMP085_CONTROL, BMP085_READPRESSURECMD);
}
  
int32_t BMP085readPressure(void)
{
  int32_t ut = BMP085readRawTemperature();
  int32_t up = BMP085readRawPressure();

  #ifdef verbose
  u_puts("UT="); u_puti(ut); u_putc(' ');
  u_puts("UP="); u_puti(up); u_putc(' ');
  #endif

  int32_t x1 = (ut - (int32_t)ac6) * (int32_t)ac5 >> 15;
  int32_t x2 = ((int32_t) mc << 11) / (x1 + (int32_t)md);
  int32_t b5 = x1 + x2;
  int32_t b6 = b5 - 4000;
  x1 = ((int32_t)b2 * (b6 * b6 >> 12)) >> 11;
  x2 = ((int32_t)ac2 * b6) >> 11;
  int32_t x3 = x1 + x2;
  int32_t b3 = ((((int32_t)ac1 * 4 + x3) << OSS) + 2) >> 2;
  x1 = ((int32_t)ac3 * b6) >> 13;
  x2 = ((int32_t)b1 * ((b6 * b6) >> 12)) >> 16;
  x3 = ((x1 + x2) + 2) >> 2;
  uint32_t b4 = ((uint32_t)ac4 * (uint32_t)(x3 + 32768)) >> 15;
  uint32_t b7 = ((uint32_t)up - b3) * (uint32_t)(50000UL >> OSS);
  int32_t p;
  if (b7 < 0x80000000)
  p = (b7 * 2) / b4;
  else
  p = (b7 / b4) * 2;
  x1 = (p >> 8) * (p >> 8);
  x1 = (x1 * 3038) >> 16;
  x2 = (-7357 * p) >> 16;
  
  int32_t pressure = p + ((x1 + x2 + (int32_t)3791) >> 4);

  #ifdef verbose
  u_puts("p=");
  u_puti(pressure);
  u_puts(" ");
  #endif
  
  return pressure;
}

 
uint32_t BMP085readRawPressure(void)
{
  #ifdef fakeData
  return 23843;
  #endif

/*  BMP085Write(BMP085_CONTROL, BMP085_READPRESSURECMD);

  if (OSS == BMP085_ULTRALOWPOWER) 
    _delay_ms(5);
  else if (OSS == BMP085_STANDARD) 
    _delay_ms(8);
  else if (OSS == BMP085_HIGHRES) 
    _delay_ms(14);
  else 
    _delay_ms(26);
*/

  uint32_t raw = BMP085read16(BMP085_PRESSUREDATA);
  raw <<= 8;
  raw |= BMP085read8(BMP085_PRESSUREDATA+2);
  raw >>= (8 - OSS);
  return raw;  	
}


