PREFIX := /usr
INSTALLDIR := $(PREFIX)/share/libretro/info

all:
	@echo "Nothing to make for libretro-core-info. Available commands:\n"
	@echo "  make install       Installs files to $(DESTDIR)$(INSTALLDIR)"
	@echo "  make update        Updates the local info files"
	@echo "  make test-install  Runs through a test install\n"

install:
	mkdir -p $(DESTDIR)$(INSTALLDIR)
	cp -a *.info $(DESTDIR)$(INSTALLDIR)

update:
	rm -rf libretro-super *.info
	git clone --depth=1 https://github.com/libretro/libretro-super.git
	cp -f libretro-super/dist/info/*.info .

test-install: all
	DESTDIR=/tmp/build $(MAKE) install
