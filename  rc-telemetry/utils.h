// Zm�na bitu portu, IO registru nebo prom�nn�:
#define setb(port,pin)    port |= _BV(pin)    //nastav bit
#define clrb(port,pin)    port &= ~(_BV(pin)) //nuluj bit
#define negb(port,pin)    port ^= _BV(pin)    //neguj bit
