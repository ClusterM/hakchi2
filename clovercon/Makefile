CURRENT = $(shell uname -r)
HMOD = ../mods/hmods/clovercon.hmod
TARGET  = clovercon
TARGET_PATH = mod/lib/modules/3.4.112/extra
TARGET  = clovercon
OBJS    = clovercon.o
KDIR    = ~/nesmini-linux
PWD = $(shell pwd)
DEST = .

obj-m      := $(TARGET).o

all:
	make -C $(KDIR) SUBDIRS=$(PWD) ARCH=arm CROSS_COMPILE=arm-linux-gnueabihf-
	modinfo $(TARGET).ko
	arm-linux-gnueabihf-strip --strip-unneeded $(TARGET).ko
	cp -f $(TARGET).ko $(TARGET_PATH)/$(TARGET).ko
	cd mod && tar -czvf ../$(HMOD) *

$(TARGET).o: $(OBJS)
	$(LD) $(LD_RFLAG) ARCH=arm CROSS_COMPILE=arm-linux-gnueabihf- -r -o $@ $(OBJS)

clean:
	$(MAKE) -C $(KDIR) SUBDIRS=$(PWD) clean
	rm -f $(TARGET_PATH)/$(TARGET).ko $(HMOD)

.PHONY: modules clean

-include $(KDIR)/Rules.make
