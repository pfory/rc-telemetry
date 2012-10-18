#include "GPS.h"
//#include <string.h>
//#include <stdlib.h>
//
////#define fakeData
//
//static uint16_t cas, datum;
//static int8_t   latS; 
//static int16_t  latDM, lonS, lonDM;
//static uint16_t vyska, kurz;
//static uint8_t  latM, lonM, rychlost, satInView;
//static uint16_t hdop, pdop, vdop;
//static uint8_t  fix;
//static uint8_t  latSChange, latMChange, latDMChange, lonSChange, lonMChange, lonDMChange, vyskaChange, kurzChange, rychlostChange, 
                //satInViewChange, fixChange, casChange, datumChange;
                ////hdopChange, pdopChange, vdopChange,
//
////-----------------------------------------------------------------------------
////
////  Reset all GPS variable to zero
////
////-----------------------------------------------------------------------------
//void resetGPSData()
//{
	//cas=datum=latS=latDM=lonS=lonDM=vyska=kurz=latM=lonM=rychlost=satInView=hdop=pdop=vdop=fix=0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Calculate checksum for GPS sentence
////  Returns:  data checksum
////            0 - if sentense is longer than 83 Bytes
////
////-----------------------------------------------------------------------------
//uint16_t crc(char *b) {
	//uint16_t ks=0;
	//for (uint8_t i=1; i<84; i++) {
		//if (b[i]==42) 
			//return ks;
		//ks^=b[i];
	//}
	//return 0;
//}
//

//const inline uint8_t getLatSChange(void)     { return latSChange;      }
//const inline uint8_t getLatMChange(void)     { return latMChange;      }
//const inline uint8_t getLatDMChange(void)    { return latDMChange;     }
//const inline uint8_t getLonSChange(void)     { return lonSChange;      }
//const inline uint8_t getLonMChange(void)     { return lonMChange;      }
//const inline uint8_t getLonDMChange(void)    { return lonDMChange;     }
//const inline uint8_t getVyskaChange(void)    { return vyskaChange;     }
//const inline uint8_t getKurzChange(void)     { return kurzChange;      }
//const inline uint8_t getRychlostChange(void) { return rychlostChange;  }
//
//const inline uint8_t getSatInViewChange(void) { return satInViewChange;}
////const inline uint8_t getHdopChange(void)     { return hdopChange ;     }
////const inline uint8_t getPdopChange(void)     { return pdopChange;      }
////const inline uint8_t getVdopChange(void)     { return vdopChange;      }
//const inline uint8_t getFixChange(void)      { return fixChange;       }
//const inline uint8_t getCasChange(void)      { return casChange;       }
//const inline uint8_t getDatumChange(void)    { return datumChange;     }
//

////-----------------------------------------------------------------------------
////
////  Return velocity
////
////-----------------------------------------------------------------------------
//const uint8_t getV() {
	//return rychlost;
//}
//
////-----------------------------------------------------------------------------
////
////  Return satelites in view
////
////-----------------------------------------------------------------------------
//const uint8_t getSIV() {
	//return satInView;
//}
//
////-----------------------------------------------------------------------------
////
////  Return latitude min
////
////-----------------------------------------------------------------------------
//const uint8_t getLatM() {
	//return latM;
//}
//
////-----------------------------------------------------------------------------
////
////  Return longitude min
////
////-----------------------------------------------------------------------------
//const uint8_t getLonM() {
	//return lonM;
//}
//
////-----------------------------------------------------------------------------
////
////  Return latitude sec
////
////-----------------------------------------------------------------------------
//const int8_t getLatS() {
	//return latS;
//}
//
////-----------------------------------------------------------------------------
////
////  Return longitude sec
////
////-----------------------------------------------------------------------------
//const int16_t getLonS() {
	//return lonS;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Return latitude sec dec
////
////-----------------------------------------------------------------------------
//const int16_t getLatDM() {
	//return latDM;
//}
//
////-----------------------------------------------------------------------------
////
////  Return longitude sec dec
////
////-----------------------------------------------------------------------------
//const int16_t getLonDM() {
	//return lonDM;
//}
//
////-----------------------------------------------------------------------------
////
////  Return high
////
////-----------------------------------------------------------------------------
//const uint16_t getH() {
	//return vyska;
//}
//
////-----------------------------------------------------------------------------
////
////  Return azimuth
////
////-----------------------------------------------------------------------------
//const uint16_t getK() {
	//return kurz;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Return DOP
////
////-----------------------------------------------------------------------------
///*const uint16_t getDOP(uint8_t typ) {
	//if (typ=='H') return hdop;
	//if (typ=='P') return pdop;
	//if (typ=='V') return vdop;
	//return 0;
//}
//*/
////-----------------------------------------------------------------------------
////
////  Return 3D fix - values include: 1 = no fix
////                                  2 = 2D fix
////                                  3 = 3D fix
////
////-----------------------------------------------------------------------------
//const uint8_t getFix() {
	//return fix;
//}
//
//
///*char getStatus(char *b, uint8_t car) {
	//char pb[1+1];
	//if (getSubstring(b, pb, car, 2)==0) return 'V'; //void
//
	//return pb[0];
//}
//*/
//
////-----------------------------------------------------------------------------
////
////  Return date
////
////-----------------------------------------------------------------------------
//const uint16_t getDate() {
	//return datum;
//}
//
////-----------------------------------------------------------------------------
////
////  Return time
////
////-----------------------------------------------------------------------------
//const uint16_t getTime() {
	//return cas;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Set and pack date from GPS
////  Parameters: *b - pointer to buffer
////              car - number of char ","
////  Return:     pack date
////              0 - error
////
////-----------------------------------------------------------------------------
//void setDate(char *b, uint8_t car)
//{
	//#ifdef fakeData
	//return 99;
	//#else
//
  //uint16_t datumOld=datum;
  //datumChange = 1;
//
	//char pb[6+1];
	//char pb1[2+1];
	//memset (pb1,'\0',3);
	//datum=0;
	//if (getSubstring(b, pb, car, 7)==0) return;
//
	//uint16_t rok;
	//uint8_t mes;
	//uint8_t den;
//
	//strncpy(pb1,pb,2);
	//den=atoi(pb1);
	//strncpy(pb1,pb+2,2);
	//mes=atoi(pb1);
	//strncpy(pb1,pb+4,2);
	//rok=atoi(pb1);
	//rok+=2000;
	//datum=packDate(rok, mes, den);
//
  //#endif
//
  //if (datumOld==datum) datumChange=0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Set and pack time from GPS
////  Parameters: *b - pointer to buffer
////              car - number of char ","
////  Return:     pack date
////              0 - error
////
////-----------------------------------------------------------------------------
//void setTime(char *b, uint8_t car) 
//{
	//#ifdef fakeData
	//return 99;
  //#else
  	//
  //uint16_t casOld = cas;
  //casChange = 1;	
//
  //char pb[6+1];
	//char pb1[2+1];
	//memset (pb1,'\0',3);
	//cas=0;
	//if (getSubstring(b, pb, car, 7)==0) return;
//
	//uint8_t hod;
	//uint8_t min;
	//uint8_t sec;
//
	//strncpy(pb1,pb,2);
	//hod=atoi(pb1);
	//strncpy(pb1,pb+2,2);
	//min=atoi(pb1);
	//strncpy(pb1,pb+4,2);
	//sec=atoi(pb1);
	//cas=packTime(hod, min, sec);
 	//#endif
//
  //if (casOld==cas) casChange=0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Set latitude (degree, min and sec) from GPS
////  Parameters: *b - pointer to buffer
////              car - number of char ","
////  Return:     void
////
////-----------------------------------------------------------------------------
///*void setLat(char *b, uint8_t car) 
//{
	//#ifdef fakeData
	//latS = 49;
	//latM = 43;
	//latDM = 999;
	//return;
  //#else
//
  //int8_t latSOld    = latS;
  //uint8_t latMOld   = latM;
  //int16_t latDMOld  = latDM;
  //latSChange  = 1;
  //latMChange  = 1;
  //latDMChange = 1;
//
	//char pb[9+1];
	//latDM=0;
	//if (getSubstring(b, pb, car+1, 10)==0) return;
//
	//int8_t sign=1;
	//if (pb[0]=='S') sign=-1;
//
	//if (getSubstring(b, pb, car, 0)==0) return;
//
	//char pb1[3+1];
	//pb1[0]=pb[0];
	//pb1[1]=pb[1];
	//pb1[2]='\0';
//
	//latS=atoi(pb1)*sign;
//
	//pb1[0]=pb[2];
	//pb1[1]=pb[3];
	//pb1[2]='\0';
//
	//latM=atoi(pb1);
//
	//pb1[0]=pb[5];
	//pb1[1]=pb[6];
	//pb1[2]=pb[7];
	//pb1[3]='\0';
//
	//latDM=atoi(pb1);
	//#endif
//
  //if (latSOld==latS)  latSChange  = 0;
  //if (latMOld==latM)  latMChange  = 0;
  //if (latDMOld==latS) latDMChange = 0;
//
//}
//*/
//
////-----------------------------------------------------------------------------
////
////  Set longitude from GPS
////
////-----------------------------------------------------------------------------
//void setLon(char *b, uint8_t car) 
//{
	//#ifdef fakeDataA
	//lonS = 13;
	//lonM = 22;
	//lonDM = 999;
	//return;
  //#else
//
  //int16_t lonSOld   = lonS;
  //uint8_t lonMOld   = lonM;
  //int16_t lonDMOld  = lonDM;
  //lonSChange  = 1;
  //lonMChange  = 1;
  //lonDMChange = 1;
	//
	//char pb[10+1];
	//lonDM=0;
	//if (getSubstring(b, pb, car+1, 10)==0) return;
//
	//int8_t sign=1;
	//if (pb[0]=='W') sign=-1;
//
	//if (getSubstring(b, pb, car, 0)==0) return;
//
	//char pb1[4];
	//pb1[0]=pb[0];
	//pb1[1]=pb[1];
	//pb1[2]=pb[2];
	//pb1[3]='\0';
//
	//lonS=atoi(pb1)*sign;
//
	//pb1[0]=pb[3];
	//pb1[1]=pb[4];
	//pb1[2]='\0';
//
	//lonM=atoi(pb1);
//
	//pb1[0]=pb[6];
	//pb1[1]=pb[7];
	//pb1[2]=pb[8];
	//pb1[3]='\0';
//
	//lonDM=atoi(pb1);
	//#endif
//
  //if (lonSOld==lonS)  lonSChange  = 0;
  //if (lonMOld==lonM)  lonMChange  = 0;
  //if (lonDMOld==lonS) lonDMChange = 0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Set altitude from GPS
////
////-----------------------------------------------------------------------------
//void setAlt(char *b, uint8_t car) 
//{
	//#ifdef fakeData
  //vyska = 999;
  //return;
  //#else
  //
  //uint16_t vyskaOld = vyska;
//
	//char pb[7+1];
	//vyska=0;
  //vyskaChange=1;
	//
	//if (getSubstring(b, pb, car, 8)==0) return;
//
	////zaokrouhleni, nahradim tecku koncovym znakem
	//char *pozS=strstr(pb,".");
	//strncpy (pozS,"\0",1);
//
	//vyska=atoi(pb);
	//#endif
//
  //if (vyska==vyskaOld) vyskaChange=0;
//}
//
////-----------------------------------------------------------------------------
////
////  Set speed from GPS
////
////-----------------------------------------------------------------------------
//void setSpeed(char *b, uint8_t car) 
//{
	//#ifdef fakeData
	//rychlost = 159;
	//return;
  //#else
//
  //uint8_t rychlostOld=rychlost;
  	//
	//char pb[6+1];
	//char pb1[3+1];
	//uint32_t u=0;
//
  //rychlost=0;
  //rychlostChange=1;
	//if (getSubstring(b, pb, car, 7)==0) return;
//
	////abych se vyhnul necelociselnym operacim
	//char *pozS=foundChar(pb,'.',1);
	//memset (pb1,'\0',4);
	//if (pozS==0)
		//pozS=foundChar(pb,'\0',1);
	//memset (pb1,'\0',4);
	//strncpy(pb1,pb,pozS-pb);
	//u=atol(pb1)*KOEFUZEL; //1852
//
	//pozS=foundChar(pb,'.',1);
	//if (pozS!=0) { 
		//memset (pb1,'\0',4);
		//strncpy(pb1,pozS+1,1);
		//u+=atol(pb1)*KOEFUZEL/10;
	//}
//
	//rychlost=u/1000;
	//#endif
//
  //if (rychlost==rychlostOld) rychlostChange=0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Set sats in view from GPS
////
////-----------------------------------------------------------------------------
//void setSatInView(char *b, uint8_t car)
//{
  //#ifdef fakeData
	//satInView = 10;
	//return;
  //#else 
  	//
  //uint8_t satInViewOld = satInView;
//
	//char pb[2+1];
	//satInView=0;
  //satInViewChange=1;
//
	//if (getSubstring(b, pb, car, 3)==0) return;
//
	//satInView=atoi(pb);
  //#endif
  //
  //if (satInView==satInViewOld) satInViewChange=0;
//}
//
//
//
////-----------------------------------------------------------------------------
////
////  Set azimuth from GPS
////
////-----------------------------------------------------------------------------
//void setAzimuth(char *b, uint8_t car) 
//{
	//#ifdef fakeData
	//kurz = 359;
	//return;
  //#else
  	//
  //uint16_t kurzOld = kurz;
	//
  //char pb[5+1];
	//kurz=0;
  //kurzChange=1;
//
	//if (getSubstring(b, pb, car, 6)==0) return;
//
	//kurz=atoi(pb);
	//#endif
  //
  //if (kurz==kurzOld) kurzChange=0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Set DOP
////
////-----------------------------------------------------------------------------
////1.1
////typ H(orizontal) - HDOP, P(recision) - PDOP, V(ertical) - VDOP
///*void setDOP(char *b, uint8_t car, char typ)
 //{
	//#ifdef fakeData
	 //hdop = 99;
	 //pdop = 99;
	 //vdop = 99;
	 //return;
	 //#else
//
  //uint16_t dopOld;
	//if (typ=='H') dopOld=hdop;
	//if (typ=='P') dopOld=pdop;
	//if (typ=='V') dopOld=vdop;
  //
  //dopOld = hdop;
 //
  //char pb[3+1];
	//char pb1[1+1];
	//uint16_t u=0;
//
	//if (getSubstring(b, pb, car, 4)!=0) {
//
		////abych se vyhnul necelociselnym operacim
		//char *pozS=foundChar(pb,'.',1);
		//if (pozS==0) 
			//pozS=foundChar(pb,'\0',1);
		//memset (pb1,'\0',2);
		//strncpy(pb1,pb,pozS-pb);
		//u=atoi(pb1)*10;
//
		//pozS=foundChar(pb,'.',1);
		//if (pozS!=0) { 
			//memset (pb1,'\0',2);
			//strncpy(pb1,pozS+1,1);
			//u+=atoi(pb1);
		//}
	//}
	//
  //if (u!=dopOld)
  //{
	  //if (typ=='H') hdop=u;
	  //if (typ=='P') pdop=u;
	  //if (typ=='V') vdop=u;
  //}
	//#endif
//}
////1.1
//*/
//
////-----------------------------------------------------------------------------
////
////  Set fix
////  3D fix - values include: 1 = no fix
////                           2 = 2D fix
////                           3 = 3D fix
////
////-----------------------------------------------------------------------------
////1.2
//void setFix(char *b, uint8_t car) 
//{
	//#ifdef fakeData
	//fix = 3;
	//return;
	//#else 
//
  //uint8_t fixOld = fix;
	//
	//char pb[1+1];
	//fix=0;
  //fixChange=1;
//
	//if (getSubstring(b, pb, car, 2)==0) return;
//
	//fix=atoi(pb);
  //#endif
//
  //if (fix==fixOld) fixChange=0;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Pack time
////
////-----------------------------------------------------------------------------
//uint16_t packTime(uint8_t hod, uint8_t min, uint8_t sec) {
	//uint16_t cas;
	//cas=hod<<6;
	//cas|=min;
	//cas=cas<<5;
	//cas|=sec/2;
	//return cas;
//}
//
//
////-----------------------------------------------------------------------------
////
////  Pack date
////
////-----------------------------------------------------------------------------
//uint16_t packDate(uint16_t rok, uint8_t mes, uint8_t den) {
	//uint16_t datum;
	//datum=(rok-1980)<<4;
	//datum|=mes;
	//datum=datum<<5;
	//datum|=den;
	//return datum;
//}
//
//
//
////-----------------------------------------------------------------------------
////
////  Return substring from NMEA sentence
////
////-----------------------------------------------------------------------------
//uint8_t getSubstring(char *b, char *pb, uint8_t car, uint8_t delka) {
//
	////memset (pb,'\0',delka);
	//
	////while (pch != NULL && i<car)
  ////{
    ////pch = strtok (NULL, ",");
	  ////if (i==car-1)
	    ////pozS=pch;
	  ////if (i==car)
	    ////pozE=(size_t*)pch;
	  ////i++;
  ////}
  //
  ////strncpy(pb,pozS,pozE-pozS);
  ////pb = substring(b, pozS, pozE);
  ////return 1;
//
	//memset (pb,'\0',delka);
//
	//char *pozS=foundChar(b,',',car)+1;
	//char *pozK=foundChar(b,',',car+1);
	//if (pozK==0)
		//pozK=foundChar(b,'*',1);
//
	//if (pozK-pozS==0) return 0;
	//strncpy(pb,pozS,pozK-pozS);
	//return 1;
//}
//
//
////-----------------------------------------------------------------------------
////
////  
////
////-----------------------------------------------------------------------------
////hleda n-ty vyskyt z v b a vraci ukazatel na jeho pozici v b
//char * foundChar(char *b, char z, uint8_t n)
//{
	//char *poz=strchr(b, z);
	//uint8_t c=1;
//
	//if (n==1) return poz;
//
	//while (poz!=NULL) {
		//poz=strchr(poz+1, z);
		//if (c==n-1)
			//return poz;
		//c++;
	//}
	//return 0;
//}
//
//
////char* substring(const char* str, size_t begin, size_t len)
////{
  ////if (str == 0 || strlen(str) == 0 || strlen(str) < begin || strlen(str) < (begin+len))
    ////return 0;
////
  ////return strncpy(str + begin, len);
////}
//
////void sendGPSData(char * buffer) {
  ////uint8_t carka=0, i=0;
  ////uint8_t typSentence=0;
  ////char buff[6];
  ////uint8_t poziceBuff=0;
 ////
 ////while (buffer[i++]!="\0") {
    ////if (buffer[i]==2) {
      ////if buffer[]=="G") { //GGA sentence
        ////typSentence=1;
      ////else if buffer[]=="R") { //RMC sentence
        ////typSentence=2;
      ////else if buffer[]=="V") { //VTG sentence     
    ////}
    ////if (i>=4) {
      ////if (buffer[i]==",")
        ////carka++;
      ////if (carka==1) {
        ////buff[poziceBuf++]=
      ////}
    ////}
    ////
    			/////*setLat(buffer,2);
    			////if (getLatSChange()==1)       sendData8(0xA0,getLatS());   //latS
    			////if (getLatMChange()==1)       sendData8(0xA1,getLatM());   //latM
    			////if (getLatDMChange()==1)      sendData16(0xA2,getLatDM());  //LatDM
    			////setLon(buffer,4);
    			////if (getLonSChange()==1)       sendData8 (0xA3,getLonS());   //LonS
    			////if (getLonSChange()==1)       sendData8 (0xA4,getLonM());   //LonM
    			////if (getLonDMChange()==1)      sendData16(0xA5,getLonDM());  //LonDM
    			////setAlt(buffer,9);
    			////if (getVyskaChange()==1)      sendData16(0xA6,getH());      //altitude
    			////setSatInView(buffer,7);
    			////if (getSatInViewChange()==1)  sendData8 (0xA9,getSIV());    //sat in view
    			//////setTime(buffer,1);//1.1
    			//////setDOP(buffer,8,'H');//1.1
  			////}
////
  			////if (strstr(buffer, "RMC")>0) //RMC sentence
  			////{
    			//////if (getStatus(buffer,2)=='A') { //1.2
    			////setDate(buffer,9);
    			////setTime(buffer,1);
    			////if (getDatumChange()==1)    sendData24(0xAA,getDate());   //GPS date
    			////if (getCasChange()==1)      sendData24(0xAB,getTime());   //GPS time
    			//////setLat(buffer,3);
    			//////setLon(buffer,5);
    			////setAzimuth(buffer,8);
    			////if (getKurzChange()==1)     sendData16(0xA8,getK());      //azimuth
    			////setSpeed(buffer,7);
    			////if (getRychlostChange()==1) sendData16(0xA7,getV());      //speed
  			//////}*/
			////}
////
  ////if (strstr(buffer, "GSA")>0) {
  /////*			setFix(buffer,2);
  			////setDOP(buffer,15,'P');
  			////setDOP(buffer,16, 'H');
  			////setDOP(buffer,17, 'V');
  			////if (getHdopChange()==1)       sendData16(0xAC,getDOP('H')); //HDOP
  			////if (getVdopChange()==1)       sendData16(0xAD,getDOP('V')); //VDOP
  			////if (getPdopChange()==1)       sendData16(0xAE,getDOP('P')); //PDOP
////
  			////if (getFixChange()==1)        sendData8 (0xAF,getFix());    //Fix
  ////*/			
  ////}
////}