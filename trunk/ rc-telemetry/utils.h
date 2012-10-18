// Zmìna bitu portu, IO registru nebo promìnné:
#define setb(port,pin)    port |= _BV(pin)    //nastav bit
#define clrb(port,pin)    port &= ~(_BV(pin)) //nuluj bit
#define negb(port,pin)    port ^= _BV(pin)    //neguj bit
