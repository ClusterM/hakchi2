CURRENT = $(shell uname -r)
TARGET  = clovercon
OBJS    = clovercon.o
KDIR    = linux-nesmini
#KDIR	= ~/linux
PWD = $(shell pwd)
DEST = .

obj-m      := $(TARGET).o

default:
	make -C $(KDIR) SUBDIRS=$(PWD) ARCH=arm CROSS_COMPILE=arm-linux-gnueabihf-
	modinfo $(TARGET).ko
	arm-linux-gnueabihf-strip --strip-unneeded $(TARGET).ko

$(TARGET).o: $(OBJS)
	$(LD) $(LD_RFLAG) ARCH=arm CROSS_COMPILE=arm-linux-gnueabihf- -r -o $@ $(OBJS)

install:
	su -c "cp -v $(TARGET).ko $(DEST) && /sbin/depmod -a"

clean:
	$(MAKE) -C $(KDIR) SUBDIRS=$(PWD) clean

.PHONY: modules clean

-include $(KDIR)/Rules.make
