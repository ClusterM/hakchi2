all-local: embedded.h libwdi.la
if CREATE_IMPORT_LIB
# Rebuild the import lib from the .def so that MS and MinGW DLLs can be interchanged
	@echo "  CCLD   libwdi.dll.a"
	@$(DLLTOOL) $(DLLTOOLFLAGS) --kill-at --input-def libwdi.def --dllname libwdi.dll --output-lib .libs/libwdi.dll.a
endif

AM_CPPFLAGS = -I$(top_srcdir)

BUILT_SOURCES = embedded.h
noinst_PROGRAMS =
noinst_EXES =
lib_LTLIBRARIES = libwdi.la
LIB_SRC = resource.h logging.h tokenizer.h installer.h libwdi_i.h mssign32.h logging.c tokenizer.c vid_data.c pki.c libwdi_dlg.c libwdi.c
LIB_HDR = libwdi.h

if OPT_M32
noinst_PROGRAMS += installer_x86
noinst_EXES += installer_x86.exe
installer_x86_SOURCES = installer.h installer.c
installer_x86_CFLAGS = -m32 $(AM_CFLAGS)
installer_x86_LDFLAGS = -m32 $(AM_LDFLAGS) -static
installer_x86_LDADD = -lsetupapi -lnewdev -lole32
endif

if OPT_M64
noinst_PROGRAMS += installer_x64
noinst_EXES += installer_x64.exe
installer_x64_SOURCES = installer.h installer.c
installer_x64_CFLAGS = -m64 -D_WIN64 $(AM_CFLAGS)
installer_x64_LDFLAGS = -m64 $(AM_LDFLAGS) -static
installer_x64_LDADD = -lsetupapi -lnewdev -lole32
endif

pkg_v_rc = $(pkg_v_rc_$(V))
pkg_v_rc_ = $(pkg_v_rc_$(AM_DEFAULT_VERBOSITY))
pkg_v_rc_0 = @echo "  RC     $@";
pkg_v_localcc = $(pkg_v_localcc_$(V))
pkg_v_localcc_ = $(pkg_v_localcc_$(AM_DEFAULT_VERBOSITY))
pkg_v_localcc_0 = @echo "  CCLD   $@";

# call host's CC to allow for cross compilation
embedder: embedder.h embedder_files.h embedder.c
	$(pkg_v_localcc)$(CC_FOR_BUILD) -I.. embedder.c -o $@

EXTRA_DIST = $(LIB_SRC)

libwdi_rc.lo: libwdi.rc
	$(pkg_v_rc)$(LIBTOOL) $(AM_V_lt) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --tag=RC --mode=compile $(RC) $(ARCH_RCFLAGS) -i $< -o $@

libwdi_la_CFLAGS = $(ARCH_CFLAGS) $(VISIBILITY_CFLAGS) $(AM_CFLAGS)
libwdi_la_LDLAGS = $(AM_LDFLAGS)
libwdi_la_LIBADD = libwdi_rc.lo -lsetupapi -lole32
libwdi_la_SOURCES = $(LIB_SRC)
libwdi_la_HEADERS = $(LIB_HDR)
libwdi_ladir = $(includedir)

embedded.h: embedder $(noinst_PROGRAMS)
	@./embedder embedded.h

clean-local:
	-rm -rf embedded.h embedder embedder.exe

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = libwdi.pc
