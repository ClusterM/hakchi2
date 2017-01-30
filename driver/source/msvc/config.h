/* config.h.  Manual config for MSVC.  */

#ifndef _MSC_VER
#warn "msvc/config.h shouldn't be included for your development environment."
#error "Please make sure the msvc/ directory is removed from your build path."
#endif

#if defined(_PREFAST_)
/* Disable "Banned API Usage:" errors when using WDK's OACR/Prefast */
#pragma warning(disable:28719)
#endif
#if defined(_MSC_VER)
// Disable some VS2012 Code Analysis warnings
#pragma warning(disable:6258)		// We'll use TerminateThread() regardless
#pragma warning(disable:6387)
#endif

/*
 * Embed WinUSB driver files from the following DDK location
 * NB: You must also make sure the WDF_VER, COINSTALLER_DIR and X64_DIR
 * match your WinUSB redist directrories
 */
#ifndef DDK_DIR
#define DDK_DIR "C:/Program Files (x86)/Windows Kits/10"
#endif

/* DDK WDF coinstaller version */
#define WDF_VER 1011

/* CoInstaller subdirectory for WinUSB redist files ("winusb" or "wdf") */
#define COINSTALLER_DIR "wdf"

/* 64bit subdirectory for WinUSB redist files ("x64" or "amd64") */
#define X64_DIR "x64"

/* embed libusb0 driver files from the following location */
//#ifndef LIBUSB0_DIR
//#define LIBUSB0_DIR "D:/libusb-win32"
//#endif

/* embed libusbK driver files from the following location */
//#ifndef LIBUSBK_DIR
//#define LIBUSBK_DIR "D:/libusbK/bin"
//#endif

/* embed user defined driver files from the following location */
#ifndef USER_DIR
// #define USER_DIR "C:/signed-driver"
#endif

/* 32 bit support */
#define OPT_M32

/* 64 bit support */
#define OPT_M64

/* embed IA64 driver files */
//#define OPT_IA64

/* Debug message logging */
//#define ENABLE_DEBUG_LOGGING

/* Debug message logging (toggable) */
#define INCLUDE_DEBUG_LOGGING

/* Message logging */
#define ENABLE_LOGGING 1
