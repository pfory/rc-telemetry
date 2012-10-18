#include "onewire.h"

#define MAXSENSORS 5

uint8_t gSensorIDs[MAXSENSORS][OW_ROMCODE_SIZE];
uint8_t nSensors=0;

/*uint8_t search_sensors(void) {
	uint8_t i;
	uint8_t id[OW_ROMCODE_SIZE];
	uint8_t diff;
	
	#ifdef UART
	u_puts( "\nScanning Bus for DS18X20\n" );
	#endif
	
	nSensors = 0;
	
	for (diff=OW_SEARCH_FIRST; diff != OW_LAST_DEVICE 
		&& nSensors < MAXSENSORS ;)	{
		DS18X20_find_sensor(&diff, &id[0]);
		
		if( diff==OW_PRESENCE_ERR) {
			#ifdef UART
			u_puts("\nNo Sensor found" );
			#endif
			break;
		}
		
		if( diff==OW_DATA_ERR ) {
			#ifdef UART
			u_puts("\nBus Error" );
			#endif
			break;
		}
		
		for (i=0;i<OW_ROMCODE_SIZE;i++) {
			gSensorIDs[nSensors][i]=id[i];
			#ifdef UART
			u_puti(id[i]);
			u_puts("\n");
			#endif
		}

		nSensors++;
	}
	
	return nSensors;
}*/


