################################################################################
# Automatically-generated file. Do not edit!
################################################################################

SHELL := cmd.exe
RM := rm -rf

USER_OBJS :=

LIBS := 
PROJ := 

O_SRCS := 
C_SRCS := 
S_SRCS := 
S_UPPER_SRCS := 
OBJ_SRCS := 
ASM_SRCS := 
PREPROCESSING_SRCS := 
OBJS := 
OBJS_AS_ARGS := 
C_DEPS := 
C_DEPS_AS_ARGS := 
EXECUTABLES := 
OUTPUT_FILE_PATH :=
OUTPUT_FILE_PATH_AS_ARGS :=
AVR_APP_PATH :=$$$AVR_APP_PATH$$$
QUOTE := "
ADDITIONAL_DEPENDENCIES:=
OUTPUT_FILE_DEP:=

# Every subdirectory with source files must be described here
SUBDIRS := 


# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS +=  \
../BMP085.c \
../crc8.c \
../i2csoft.c \
../ds18x20.c \
../onewire.c \
../RXModul.c \
../UART.c


PREPROCESSING_SRCS += 


ASM_SRCS += 


OBJS +=  \
BMP085.o \
crc8.o \
i2csoft.o \
ds18x20.o \
onewire.o \
RXModul.o \
UART.o


OBJS_AS_ARGS +=  \
BMP085.o \
crc8.o \
i2csoft.o \
ds18x20.o \
onewire.o \
RXModul.o \
UART.o


C_DEPS +=  \
BMP085.d \
crc8.d \
i2csoft.d \
ds18x20.d \
onewire.d \
RXModul.d \
UART.d


C_DEPS_AS_ARGS +=  \
BMP085.d \
crc8.d \
i2csoft.d \
ds18x20.d \
onewire.d \
RXModul.d \
UART.d


OUTPUT_FILE_PATH +=RXModul.elf

OUTPUT_FILE_PATH_AS_ARGS +=RXModul.elf

ADDITIONAL_DEPENDENCIES:=

OUTPUT_FILE_DEP:= ./makedep.mk

# AVR32/GNU C Compiler















./%.o: .././%.c
	@echo Building file: $<
	@echo Invoking: AVR/GNU C Compiler : (WinAVR 20100110) 4.3.3
	$(QUOTE)c:\WinAVR-20100110\bin\avr-gcc.exe$(QUOTE) -funsigned-char -funsigned-bitfields -DF_OSC=7372800UL -DF_CPU=7372800UL  -Os -fpack-struct -fshort-enums -Wall -Werror -c -gdwarf-2 -std=gnu99  -MD -MP -MF "$(@:%.o=%.d)" -MT"$(@:%.o=%.d)" -MT"$(@:%.o=%.o)"  -mmcu=atmega8   -o"$@" "$<" 
	@echo Finished building: $<



# AVR32/GNU Preprocessing Assembler



# AVR32/GNU Assembler




ifneq ($(MAKECMDGOALS),clean)
ifneq ($(strip $(C_DEPS)),)
-include $(C_DEPS)
endif
endif

# Add inputs and outputs from these tool invocations to the build variables 

# All Target
all: $(OUTPUT_FILE_PATH) $(ADDITIONAL_DEPENDENCIES)

$(OUTPUT_FILE_PATH): $(OBJS) $(USER_OBJS) $(OUTPUT_FILE_DEP)
	@echo Building target: $@
	@echo Invoking: AVR/GNU Linker : (WinAVR 20100110) 4.3.3
	$(QUOTE)c:\WinAVR-20100110\bin\avr-gcc.exe$(QUOTE) -o$(OUTPUT_FILE_PATH_AS_ARGS) $(OBJS_AS_ARGS) $(USER_OBJS) $(LIBS) -Wl,-Map="RXModul.map" -Wl,--start-group -Wl,-lm  -Wl,--end-group  -mmcu=atmega8  
	@echo Finished building target: $@
	"c:\WinAVR-20100110\bin\avr-objcopy.exe" -O ihex -R .eeprom -R .fuse -R .lock -R .signature  "RXModul.elf" "RXModul.hex"
	"c:\WinAVR-20100110\bin\avr-objcopy.exe" -j .eeprom  --set-section-flags=.eeprom=alloc,load --change-section-lma .eeprom=0  --no-change-warnings -O ihex "RXModul.elf" "RXModul.eep" || exit 0
	"c:\WinAVR-20100110\bin\avr-objdump.exe" -h -S "RXModul.elf" > "RXModul.lss"
	"c:\WinAVR-20100110\bin\avr-size.exe" "RXModul.elf"
	





# Other Targets
clean:
	-$(RM) $(OBJS_AS_ARGS)$(C_DEPS_AS_ARGS) $(EXECUTABLES) 
	rm -rf "RXModul.elf" "RXModul.a" "RXModul.hex" "RXModul.lss" "RXModul.eep" "RXModul.map"
	