#ifndef GPS_H_
#define GPS_H_

//#include <inttypes.h>
//#include <stdlib.h>
//
//#define KOEFUZEL 1852
//
////void nulujRetezec(char *);
//
////-----------------------------------------------------------------------------
////
////  Interfaces
////
////-----------------------------------------------------------------------------
//void resetGPSData(void);
//
//void setLat(char *, uint8_t);
//void setLon(char *, uint8_t);
//void setDate(char *, uint8_t);
//void setTime(char *, uint8_t);
//void setAlt(char *, uint8_t);
//void setSpeed(char *, uint8_t);
//void setSatInView(char *, uint8_t);
//void setAzimuth(char *, uint8_t);
////void setDOP(char *,uint8_t,char);//1.1
//void setFix(char *b, uint8_t); //1.2
//
//
//const uint8_t getV();
//const uint8_t getSIV();
//const uint16_t getH();
//const uint8_t getLatM();
//const uint8_t getLonM();
//const int8_t getLatS();
//const uint16_t getK();
//const int16_t getLonS();
//const int16_t getLatDM();
//const int16_t getLonDM();
////const uint16_t getDOP(uint8_t typ);
//const uint8_t getFix();
//const uint16_t getDate(void);
//const uint16_t getTime(void);
//
//const uint8_t getLatSChange(void);
//const uint8_t getLatMChange(void);
//const uint8_t getLatDMChange(void);
//const uint8_t getLonSChange(void);
//const uint8_t getLonMChange(void);
//const uint8_t getLonDMChange(void);
//const uint8_t getVyskaChange(void);
//const uint8_t getKurzChange(void);
//const uint8_t getRychlostChange(void);
//
//const uint8_t getSatInViewChange(void);
////const uint8_t getHdopChange(void);
////const uint8_t getPdopChange(void);
////const uint8_t getVdopChange(void);
//const uint8_t getFixChange(void);
//const uint8_t getCasChange(void);
//const uint8_t getDatumChange(void);
//
//
////char getStatus(char *b, uint8_t car); //1.2
//
//
////-----------------------------------------------------------------------------
////
////  Functions
////
////-----------------------------------------------------------------------------
//uint16_t packTime(uint8_t, uint8_t, uint8_t);
//uint16_t packDate(uint16_t, uint8_t, uint8_t);
//
//
//uint8_t getSubstring(char *, char *, uint8_t, uint8_t);
//uint16_t crc(char *);
//
//char * foundChar(char *b, char z, uint8_t n);
//char * substring(const char *str, size_t begin, size_t len);
//

#endif //GPS_H_
