AC_INIT([libwdi], [1.2.5], [libwdi-devel@lists.sourceforge.net], [libwdi], [http://libwdi.akeo.ie])
AM_INIT_AUTOMAKE([-Wno-portability 1.11 foreign])
AC_CONFIG_SRCDIR([libwdi/libwdi.c])
AC_CONFIG_MACRO_DIR([m4])
AM_CONFIG_HEADER([config.h])
# Enable silent build rules by default (Automake v1.11 or later).
# Disable by either passing --disable-silent-rules to configure or passing V=1 to make
m4_ifdef([AM_SILENT_RULES], [AM_SILENT_RULES([yes])],
         [AC_SUBST([AM_DEFAULT_VERBOSITY], [1])])

AC_PREREQ([2.50])
AC_PROG_CC
AC_PROG_LIBTOOL
LT_LANG([Windows Resource])
AC_C_INLINE
AM_PROG_CC_C_O
AC_DEFINE([_GNU_SOURCE], [], [Use GNU extensions])

AC_ARG_WITH([ddkdir],
	[AS_HELP_STRING([--with-ddkdir], [embed WinUSB driver files from the following DDK location])],
	[DDK_DIR=$withval],
	[DDK_DIR=""])
if test "x$DDK_DIR" != "x"; then
	AC_DEFINE_UNQUOTED([DDK_DIR], ["${DDK_DIR}"], [embed WinUSB driver files from the following DDK location])
fi

AC_ARG_WITH([wdfver],
	[AS_HELP_STRING([--with-wdfver], [WDF/KDMF version to use - either 1011 or 1009. Use 1009 if you need XP compatibility])],
	[WDF_VER=$withval],
	[WDF_VER=1011])
AC_DEFINE_UNQUOTED([WDF_VER], [${WDF_VER}], [DDK WDF/KMDF coinstaller version])

AC_ARG_WITH([libusb0],
	[AS_HELP_STRING([--with-libusb0], [embed libusb0 driver files from the following location])],
	[LIBUSB0_DIR=$withval],
	[LIBUSB0_DIR=""])
if test "x$LIBUSB0_DIR" != "x"; then
	AC_DEFINE_UNQUOTED([LIBUSB0_DIR], ["${LIBUSB0_DIR}"], [embed libusb0 driver files from the following location])
fi

AC_ARG_WITH([libusbk],
	[AS_HELP_STRING([--with-libusbk], [embed libusbK driver files from the following location])],
	[LIBUSBK_DIR=$withval],
	[LIBUSBK_DIR=""])
if test "x$LIBUSBK_DIR" != "x"; then
	AC_DEFINE_UNQUOTED([LIBUSBK_DIR], ["${LIBUSBK_DIR}"], [embed libusbK driver files from the following location])
fi

AC_ARG_WITH([userdir],
	[AS_HELP_STRING([--with-userdir], [embed user defined driver files from the following location])],
	[USER_DIR=$withval],
	[USER_DIR=""])
if test "x$USER_DIR" != "x"; then
	AC_DEFINE_UNQUOTED([USER_DIR], ["${USER_DIR}"], [embed user defined driver files from the following location])
fi

if test "x$USER_DIR" == "x" -a "x$DDK_DIR" == "x" -a "x$LIBUSB0_DIR" == "x" -a "x$LIBUSBK_DIR" == "x"; then
	AC_MSG_ERROR([One of --with-ddkdir, --with-libusb0, --with-libusbk or --with-userdir options MUST be provided.])
fi

AC_MSG_CHECKING([development environment])
case $host in
*-mingw*)
	AC_MSG_RESULT([MinGW])
	create_import_lib="yes"
	LIBCONFIG_LIBADD=""
	LIBCONFIG_CFLAGS=""
	AM_CFLAGS="-Wshadow"
	AM_LDFLAGS="-Wl,--add-stdcall-alias"
	;;
*)
	AC_MSG_ERROR([unsupported development environment])
esac

AM_CFLAGS="${AM_CFLAGS} -DWINVER=0x500 -D_WIN32_IE=0x500"
AM_LDFLAGS="${AM_LDFLAGS} -no-undefined -avoid-version"
AM_CONDITIONAL([CREATE_IMPORT_LIB], [test "x$create_import_lib" = "xyes"])

# cross compiling support
if test "x$CC_FOR_BUILD" == "x"; then
	if test "x$cross_compiling" == "xyes"; then
		AC_CHECK_PROGS([CC_FOR_BUILD], [gcc], [cc])
	else
		CC_FOR_BUILD="$CC"
	fi
fi
AC_SUBST([CC_FOR_BUILD])

# detect if the host's CC_FOR_BUILD actually produces executables that run
# (this is required for the embedder)
saved_CC="${CC}"
saved_CFLAGS="${CFLAGS}"
saved_LDFLAGS="${LDFLAGS}"
# we need to disable cross compiling  detection for this test
saved_cross_compiling=$cross_compiling
cross_compiling=no
CC="${CC_FOR_BUILD}"
CFLAGS=""
LDFLAGS=""
AC_MSG_CHECKING([whether the build compiler can produce executables that run on this host])
AC_RUN_IFELSE([AC_LANG_PROGRAM([])],
	[AC_MSG_RESULT([yes])
	 cc_for_build_ok="yes"],
	[AC_MSG_RESULT([no])
	 cc_for_build_ok="no"])
CC="${saved_CC}"
CFLAGS="${saved_CFLAGS}"
LDFLAGS="${saved_LDFLAGS}"
cross_compiling=$saved_cross_compiling
if test "x$cc_for_build_ok" != "xyes"; then
	AC_ERROR([The build compiler cannot produce executables for this
	platform. You might have to define the CC_FOR_BUILD environment variable.
	For instance, on 32 bit Windows, if using a multilib MinGW-w64 that defaults
	to 64 bit executables, you would need: export CC_FOR_BUILD="gcc -m32"])
fi

# 32 bit support
saved_CFLAGS="${CFLAGS}"
CFLAGS="${CFLAGS} -m32"
AC_MSG_CHECKING([whether the compiler can produce 32 bit binaries])
AC_TRY_COMPILE([], [;],
	[compiler_has_m32=yes],
	[compiler_has_m32=no])
if test "x$compiler_has_m32" == "xyes"; then
	AC_MSG_RESULT([yes])
else
	AC_MSG_RESULT([no])
fi
CFLAGS="${saved_CFLAGS}"

# 64 bit support
saved_CFLAGS="${CFLAGS}"
CFLAGS="${CFLAGS} -m64"
AC_MSG_CHECKING([whether the compiler can produce 64 bit binaries])
AC_TRY_COMPILE([], [;],
	[compiler_has_m64=yes],
	[compiler_has_m64=no])
if test "x$compiler_has_m64" == "xyes"; then
	AC_MSG_RESULT([yes])
else
	AC_MSG_RESULT([no])
fi
CFLAGS="${saved_CFLAGS}"

if test "x$compiler_has_m64" != "xyes" -a "x$compiler_has_m32" != "xyes"; then
	AC_MSG_ERROR([neither -m32 nor -m64 is supported by your compiler])
fi

# 32 and 64 bit selection
AC_ARG_ENABLE([32bit], [AS_HELP_STRING([--enable-32bit],
	[build 32 bit compatible library (default y)])],
	[enable_32bit=$enableval],
	[enable_32bit='yes'])
AC_ARG_ENABLE([64bit], [AS_HELP_STRING([--enable-64bit],
	[build 64 bit compatible library (default y)])],
	[enable_64bit='$enableval'],
	[enable_64bit='yes'])
AC_ARG_ENABLE([ia64], [AS_HELP_STRING([--enable-ia64],
	[embed IA64/Itanium driver files (default n)])],
	[enable_ia64='$enableval'],
	[enable_ia64='no'])
if test "x$enable_64bit" == "xno" -a "x$enable_32bit" == "xno"; then
	AC_MSG_ERROR([you cannot disable both 32 and 64 bit support])
fi

# OK, let's make some sense of all this 32/64 bit mess...
if test "x$enable_64bit" != "xno" -a "x$compiler_has_m64" == "xno"; then
	if test "x$enable_32bit" == "xno"; then
		AC_MSG_ERROR([compiler cannot produce 64 bit binaries, and 32 bit support is disabled])
	fi
	AC_MSG_WARN([compiler cannot produce 64 bit binaries - disabling 64 bit support])
	enable_64bit="no"
fi

if test "x$enable_32bit" != "xno" -a "x$compiler_has_m32" == "xno"; then
	if test "x$enable_64bit" == "xno"; then
		AC_MSG_ERROR([compiler cannot produce 32 bit binaries, and 64 bit support is disabled])
	fi
	AC_MSG_WARN([compiler cannot produce 32 bit binaries - disabling 32 bit support])
	enable_32bit="no"
fi

# define the automake and config.h variables
if test "x$enable_32bit" != "xno"; then
	AC_DEFINE([OPT_M32], [], [32 bit support])
	AC_SUBST([OPT_M32])
fi
AM_CONDITIONAL([OPT_M32], [test "x$enable_32bit" != "xno"])
if test "x$enable_64bit" != "xno"; then
	AC_DEFINE([OPT_M64], [], [64 bit support])
	AC_SUBST([OPT_M64])
fi
AM_CONDITIONAL([OPT_M64], [test "x$enable_64bit" != "xno"])
if test "x$enable_ia64" != "xno"; then
	AC_DEFINE([OPT_IA64], [], [embed IA64 driver files])
	AC_SUBST([OPT_IA64])
fi

if test "x$enable_64bit" != "xno"; then
	# the default is to produce 32 bit, when both 64 and 32 bit support are available
	if test "x$enable_32bit" != "xno"; then
		AC_MSG_NOTICE([will produce 32 bit library and samples, compatible with 64 bit platforms])
		AC_MSG_NOTICE([if you want to produce 64 bit only library and samples, use --disable-32bit])
		ARCH_CFLAGS="-m32"
		ARCH_RCFLAGS="--target=pe-i386"

	else
		AC_MSG_WARN([will produce a 64 bit library that is INCOMPATIBLE with 32 bit platforms])
		ARCH_CFLAGS="-m64 -D_WIN64"
		ARCH_RCFLAGS=""
	fi
else
	AC_MSG_WARN([will produce a 32 bit library that is INCOMPATIBLE with 64 bit platforms])
	ARCH_CFLAGS=""
	ARCH_RCFLAGS=""

fi
AC_SUBST([ARCH_CFLAGS])
AC_SUBST([ARCH_RCFLAGS])

# AC_CHECK_FILES only works when not cross compiling
if test "$cross_compiling" != "yes"
then
	if test "x$DDK_DIR" != "x"; then
		# check that the required WinUSB DDK files are available
		AC_CHECK_FILES([$DDK_DIR/redist/winusb/], [COINSTALLER_DIR="winusb"], [COINSTALLER_DIR="wdf"])
		AC_DEFINE_UNQUOTED([COINSTALLER_DIR], ["${COINSTALLER_DIR}"], [CoInstaller subdirectory for WinUSB redist files ("winusb" or "wdf")])
		AC_SUBST([COINSTALLER_DIR])
		if test "x$enable_64bit" != "xno"; then
		AC_CHECK_FILES([$DDK_DIR/redist/wdf/x64/], [X64_DIR="x64"], [X64_DIR="amd64"])
		AC_DEFINE_UNQUOTED([X64_DIR], ["${X64_DIR}"], [64bit subdirectory for WinUSB redist files ("x64" or "amd64")])
		AC_SUBST([X64_DIR])
		AC_CHECK_FILES([$DDK_DIR/redist/wdf/${X64_DIR}/WdfCoInstaller0${WDF_VER}.dll $DDK_DIR/redist/${COINSTALLER_DIR}/${X64_DIR}/winusbcoinstaller2.dll], [],
			[AC_MSG_ERROR([the WinUSB x64 driver files could not be detected (--with-ddkdir)])])
		fi
		if test "x$enable_32bit" != "xno"; then
		AC_CHECK_FILES([$DDK_DIR/redist/wdf/x86/WdfCoInstaller0${WDF_VER}.dll $DDK_DIR/redist/${COINSTALLER_DIR}/x86/winusbcoinstaller2.dll], [],
			[AC_MSG_ERROR([the WinUSB x86 driver files could not be detected (--with-ddkdir)])])
		fi
		if test "x$enable_ia64" != "xno"; then
		AC_CHECK_FILES([$DDK_DIR/redist/wdf/ia64/WdfCoInstaller0${WDF_VER}.dll $DDK_DIR/redist/${COINSTALLER_DIR}/ia64/winusbcoinstaller2.dll], [],
			[AC_MSG_ERROR([the WinUSB ia64 driver files could not be detected (--with-ddkdir)])])
		fi
		case "$DDK_DIR" in
			[[\\/]]*|?:[[\\/]]*) ;;
			*) AC_DEFINE_UNQUOTED([DDK_DIR], "../$DDK_DIR");;
		esac
	fi

	if test "x$LIBUSB0_DIR" != "x"; then
		# check that the required libusb0 driver files are available
		AC_CHECK_FILES([$LIBUSB0_DIR/bin/x86/libusb0.sys $LIBUSB0_DIR/bin/x86/libusb0_x86.dll],,
			[AC_MSG_ERROR([the libusb0 x86 driver files could not be detected (--with-libusb0)])])
		if test "x$enable_64bit" != "xno"; then
		AC_CHECK_FILES([$LIBUSB0_DIR/bin/amd64/libusb0.sys $LIBUSB0_DIR/bin/amd64/libusb0.dll],,
			[AC_MSG_ERROR([the libusb0 amd64 driver files could not be detected (--with-libusb0)])])
		fi
		if test "x$enable_ia64" != "xno"; then
		AC_CHECK_FILES([$LIBUSB0_DIR/bin/ia64/libusb0.sys $LIBUSB0_DIR/bin/ia64/libusb0.dll],,
			[AC_MSG_ERROR([the libusb0 ia64 driver files could not be detected (--with-libusb0)])])
		fi
		case "$LIBUSB0_DIR" in
			[[\\/]]*|?:[[\\/]]*) ;;
			*) AC_DEFINE_UNQUOTED([LIBUSB0_DIR], "../$LIBUSB0_DIR");;
		esac
	fi

	if test "x$LIBUSBK_DIR" != "x"; then
		# check that the required libusbK driver files are available
		AC_CHECK_FILES([$LIBUSBK_DIR/sys/x86/libusbK.sys $LIBUSBK_DIR/dll/x86/libusbK.dll $LIBUSBK_DIR/dll/x86/libusb0.dll],,
			[AC_MSG_ERROR([the libusbK x86 driver files could not be detected (--with-libusbk)])])
		if test "x$enable_64bit" != "xno"; then
		AC_CHECK_FILES([$LIBUSBK_DIR/sys/amd64/libusbK.sys $LIBUSBK_DIR/dll/amd64/libusbK.dll $LIBUSBK_DIR/dll/amd64/libusb0.dll],,
			[AC_MSG_ERROR([the libusbK amd64 driver files could not be detected (--with-libusbk)])])
		fi
		if test "x$enable_ia64" != "xno"; then
		AC_CHECK_FILES([$LIBUSBK_DIR/sys/ia64/libusbK.sys $LIBUSBK_DIR/dll/ia64/libusbK.dll $LIBUSBK_DIR/dll/ia64/libusb0.dll],,
			[AC_MSG_ERROR([the libusbK ia64 driver files could not be detected (--with-libusbk)])])
		fi
		case "$LIBUSBK_DIR" in
			[[\\/]]*|?:[[\\/]]*) ;;
			*) AC_DEFINE_UNQUOTED([LIBUSBK_DIR], "../$LIBUSBK_DIR");;
		esac
	fi

	if test "x$USER_DIR" != "x"; then
		AC_CHECK_FILE([$USER_DIR],,[AC_MSG_ERROR([the custom driver directory could not be detected (--with-userdir)])])
		case "$USER_DIR" in
			[[\\/]]*|?:[[\\/]]*) ;;
			*) AC_DEFINE_UNQUOTED([USER_DIR], "../$USER_DIR");;
		esac
	fi
fi

# Message logging
AC_ARG_ENABLE([log], [AS_HELP_STRING([--enable-log], [enable logging (default y)])],
	[log_enabled=$enableval],
	[log_enabled='yes'])
if test "x$log_enabled" != "xno"; then
	AC_DEFINE([ENABLE_LOGGING], [1], [Message logging])
fi

AC_ARG_ENABLE([debug-log], [AS_HELP_STRING([--enable-debug-log],
	[force debug logging always (default n)])],
	[debug_log_enabled=$enableval],
	[debug_log_enabled='no'])

AC_ARG_ENABLE([toggable-debug], [AS_HELP_STRING([--enable-toggable-debug],
	[enable switchable debug logging (default n)])],
	[toggable_debug=$enableval],
	[toggable_debug='no'])

if test "x$debug_log_enabled" != "xno"; then
	AC_DEFINE([ENABLE_DEBUG_LOGGING], [1], [Debug message logging (forced)])
else
	if test "x$toggable_debug" != "xno"; then
		AC_DEFINE([INCLUDE_DEBUG_LOGGING], [1], [Debug message logging (toggable)])
	fi
fi

# --enable-debug : check whether they want to have debug symbols:
AC_ARG_ENABLE(debug, AS_HELP_STRING([--enable-debug], [include debug symbols for gdb (default y)]),
	[debug_enabled=$enableval],
	[debug_enabled='yes'])
if test "x$debug_enabled" = "xyes" ; then
  CFLAGS="-g -O2"
else
  CFLAGS="-Os"
fi

# Examples build
AC_ARG_ENABLE([examples-build], [AS_HELP_STRING([--enable-examples-build],
	[build example applications (default n)])],
	[build_examples=$enableval],
	[build_examples='no'])
AM_CONDITIONAL([BUILD_EXAMPLES], [test "x$build_examples" != "xno"])

# check for -Wno-pointer-sign compiler support (GCC >= 4)
saved_CFLAGS="${CFLAGS}"
CFLAGS="$CFLAGS -Wno-pointer-sign"
AC_COMPILE_IFELSE([AC_LANG_PROGRAM([])],
	[nopointersign_cflags="-Wno-pointer-sign"], [nopointersign_cflags=""])
CFLAGS="${saved_CFLAGS}"

AM_CFLAGS="$AM_CFLAGS -std=gnu99 -Wall -Wundef -Wunused -Wstrict-prototypes -Werror-implicit-function-declaration $nopointersign_cflags -Wshadow"

AC_SUBST([VISIBILITY_CFLAGS])
AC_SUBST([AM_CFLAGS])
AC_SUBST([AM_LDFLAGS])
AC_SUBST([LIBCONFIG_CFLAGS])
AC_SUBST([LIBCONFIG_LIBADD])

AC_CONFIG_FILES([
Makefile
libwdi/Makefile
libwdi/libwdi.pc
examples/Makefile
])
AC_OUTPUT
