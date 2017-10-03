CURRENT = $(shell uname -r)
HMOD = ../mods/hmods/clovercon.hmod
TARGET  = clovercon
TARGET_PATH = mod/lib/modules/3.4.112/extra
TARGET2  = clvcon
TARGET_PATH2 = mod/lib/modules/3.4.113/extra
OBJS    = clovercon.o
KDIR    = ~/nesmini-linux
PWD = $(shell pwd)

obj-m      := $(TARGET).o

all:
	make -C $(KDIR) SUBDIRS=$(PWD) ARCH=arm CROSS_COMPILE=arm-linux-gnueabihf-
	modinfo $(TARGET).ko
	arm-linux-gnueabihf-strip --strip-unneeded $(TARGET).ko
	mkdir -p $(TARGET_PATH)
	mkdir -p $(TARGET_PATH2)
	cp -f $(TARGET).ko $(TARGET_PATH)/$(TARGET).ko
	hexdump -ve '1/1 "%.2X"' $(TARGET).ko | sed "s/332E342E313132/332E342E313133/g" | xxd -r -p > $(TARGET_PATH2)/$(TARGET2).ko
	modinfo $(TARGET_PATH2)/$(TARGET2).ko
	cd mod && tar -czvf ../$(HMOD) *

$(TARGET).o: $(OBJS)
	$(LD) $(LD_RFLAG) ARCH=arm CROSS_COMPILE=arm-linux-gnueabihf- -r -o $@ $(OBJS)

clean:
	$(MAKE) -C $(KDIR) SUBDIRS=$(PWD) clean
	rm -f $(TARGET_PATH)/$(TARGET).ko $(TARGET_PATH2)/$(TARGET2).ko $(HMOD)

.PHONY: modules clean

-include $(KDIR)/Rules.make
