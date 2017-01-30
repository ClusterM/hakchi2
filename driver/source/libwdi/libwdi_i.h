/*
 * Library for USB automated driver installation - internal header
 * Copyright (c) 2010-2014 Pete Batard <pete@akeo.ie>
 * Parts of the code from libusb by Daniel Drake, Johannes Erdfelt et al.
 * For more info, please visit http://libwdi.akeo.ie
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA
 */
#pragma once

#include <windows.h>
#include <stdint.h>
#include "libwdi.h"
#include "tokenizer.h"

// Initial timeout delay to wait for the installer to run
#define DEFAULT_TIMEOUT 10000
#define PF_ERR          wdi_err

// These warnings are taken care of in configure for other platforms
#if defined(_MSC_VER)

#define __STR2__(x) #x
#define __STR1__(x) __STR2__(x)
#if defined(_WIN64) && defined(OPT_M32)
// a 64 bit application/library CANNOT be used on 32 bit platforms
#pragma message(__FILE__ "(" __STR1__(__LINE__) ") : warning : library is compiled as 64 bit - disabling 32 bit support")
#undef OPT_M32
#endif

#if !defined(OPT_M32) && !defined(OPT_M64)
#error both 32 and 64 bit support have been disabled - check your config.h
#endif
#if defined(OPT_M64) && !defined(OPT_M32)
#pragma message(__FILE__ "(" __STR1__(__LINE__) ") : warning : this library will be INCOMPATIBLE with 32 bit platforms")
#endif
#if defined(OPT_M32) && !defined(OPT_M64)
#pragma message(__FILE__ "(" __STR1__(__LINE__) ") : warning : this library will be INCOMPATIBLE with 64 bit platforms")
#endif

#endif

// These functions are defined in libwdi_dlg
HWND find_security_prompt(void);
int run_with_progress_bar(HWND hWnd, int(*function)(void*), void* arglist);
// These ones are defined in pki
BOOL AddCertToTrustedPublisher(BYTE* cert_data, DWORD cert_size, BOOL disable_warning, HWND hWnd);
BOOL SelfSignFile(LPCSTR szFileName, LPCSTR szCertSubject);
BOOL CreateCat(LPCSTR szCatPath, LPCSTR szHWID, LPCSTR szSearchDir, LPCSTR* szFileList, DWORD cFileList);

// Structure used for the threaded call to install_driver_internal()
struct install_driver_params {
	struct wdi_device_info* device_info;
	const char* path;
	const char* inf_name;
	struct wdi_options_install_driver* options;
};

// Tokenizer data
enum INF_TAGS
{
	INF_FILENAME,
	CAT_FILENAME,
	DEVICE_DESCRIPTION,
	DEVICE_HARDWARE_ID,
	DEVICE_INTERFACE_GUID,
	DEVICE_MANUFACTURER,
	DRIVER_DATE,
	DRIVER_VERSION,
	USE_DEVICE_INTERFACE_GUID,
	WDF_VERSION,
	KMDF_VERSION,
	LK_COMMA,
	LK_DLL,
	LK_X86_DLL,
	LK_EQ_X86,
	LK_EQ_X64,
};

token_entity_t inf_entities[]=
{
	{"INF_FILENAME",""},
	{"CAT_FILENAME",""},
	{"DEVICE_DESCRIPTION",""},
	{"DEVICE_HARDWARE_ID",""},
	{"DEVICE_INTERFACE_GUID",""},
	{"DEVICE_MANUFACTURER",""},
	{"DRIVER_DATE",""},
	{"DRIVER_VERSION",""},
	{"USE_DEVICE_INTERFACE_GUID",""},
	{"WDF_VERSION",""},
	{"KMDF_VERSION",""},
	{"LK_COMMA",""},
	{"LK_DLL",""},
	{"LK_X86_DLL",""},
	{"LK_EQ_X86",""},
	{"LK_EQ_X64",""},
	{NULL, ""} // DO NOT REMOVE!
};

/*
 * List of Android devices that need to be assigned a specific Device Interface GUID
 * so that they are recognized with Google's debug tools.
 * This list gets updated from https://github.com/gu1dry/android_winusb/ (Cyanogenmod)
 * and http://developer.android.com/sdk/win-usb.html (Google USB driver) as well as
 * http://www.linux-usb.org/usb.ids (for newer Google devices as well as Samsung's)
 * NB: We don't specify an MI, as the assumption is that the MTP driver has already been
 * installed automatically, which will only leave the driverless debug interface to pick
 * a driver for.
 */
const char* android_device_guid = "{f72fe0d4-cbcb-407d-8814-9ed673d0dd6b}";
const struct {uint16_t vid; uint16_t pid;} android_device[] = {
	{0x0451, 0xD022},
	{0x0451, 0xD101},
	{0x0489, 0xC001},
	{0x04E8, 0x685B},	// Samsung Galaxy
	{0x04E8, 0x685C},
	{0x04E8, 0x685D},
	{0x04E8, 0x685E},
	{0x04E8, 0x6860},
	{0x04E8, 0x6863},
	{0x04E8, 0x6864},
	{0x04E8, 0x6865},
	{0x04E8, 0x6866},
	{0x04E8, 0x6868},
	{0x04E8, 0x6877},
	{0x04E8, 0x689e},
	{0x05C6, 0x9018},	// Qualcomm HSUSB Device
	{0x05C6, 0x9025},
	{0x0955, 0x7030},	// NVidia Tegra
	{0x0955, 0x7100},
	{0x0955, 0xB400},	// NVidia Shield
	{0x0955, 0xB401},
	{0x0955, 0xCF05},
	{0x0955, 0xCF06},
	{0x0955, 0xCF07},
	{0x0955, 0xCF08},
	{0x0955, 0xCF09},
	{0x0B05, 0x4D00},	// Asus Transformer
	{0x0B05, 0x4D01},
	{0x0B05, 0x4D02},
	{0x0B05, 0x4D03},
	{0x0B05, 0x4E01},
	{0x0B05, 0x4E03},
	{0x0B05, 0x4E1F},
	{0x0B05, 0x4E3F},
	{0x0BB4, 0x0C01},	// HTC
	{0x0BB4, 0x0C02},
	{0x0BB4, 0x0C03},
	{0x0BB4, 0x0C13},
	{0x0BB4, 0x0C1F},	// Sony Ericsson Xperia X1
	{0x0BB4, 0x0C5F},
	{0x0BB4, 0x0C86},
	{0x0BB4, 0x0C87},
	{0x0BB4, 0x0C8B},
	{0x0BB4, 0x0C8D},
	{0x0BB4, 0x0C91},
	{0x0BB4, 0x0C92},
	{0x0BB4, 0x0C93},
	{0x0BB4, 0x0C94},
	{0x0BB4, 0x0C95},
	{0x0BB4, 0x0C96},
	{0x0BB4, 0x0C97},
	{0x0BB4, 0x0C98},
	{0x0BB4, 0x0C99},
	{0x0BB4, 0x0C9E},
	{0x0BB4, 0x0CA2},
	{0x0BB4, 0x0CA3},
	{0x0BB4, 0x0CA4},
	{0x0BB4, 0x0CA5},
	{0x0BB4, 0x0CAC},
	{0x0BB4, 0x0CAD},
	{0x0BB4, 0x0CAE},
	{0x0BB4, 0x0CBA},
	{0x0BB4, 0x0CED},
	{0x0BB4, 0x0E03},
	{0x0BB4, 0x0F64},
	{0x0BB4, 0x0FF8},
	{0x0BB4, 0x0FF9},
	{0x0BB4, 0x0FFE},
	{0x0BB4, 0x0FFF},
	{0x0FCE, 0x0DDE},	// Sony Ericsson
	{0x0FCE, 0x4E30},
	{0x0FCE, 0x6860},
	{0x0FCE, 0xD001},
	{0x1004, 0x618E},	// LG
	{0x12D1, 0x1501},
	{0x18D1, 0x0D02},
	{0x18D1, 0x2C10},
	{0x18D1, 0x2C11},
	{0x18D1, 0x4D00},	// Project Tango
	{0x18D1, 0x4D02},
	{0x18D1, 0x4D04},
	{0x18D1, 0x4D06},
	{0x18D1, 0x4D07},
	{0x18D1, 0x4E10},	// Project Tango
	{0x18D1, 0x4E11},
	{0x18D1, 0x4E12},
	{0x18D1, 0x4E13},
	{0x18D1, 0x4E20},	// Nexus S
	{0x18D1, 0x4E21},
	{0x18D1, 0x4E22},
	{0x18D1, 0x4E23},
	{0x18D1, 0x4E24},
	{0x18D1, 0x4E30},	// Galaxy Nexus
	{0x18D1, 0x4E40},	// Nexus 7
	{0x18D1, 0x4E41},
	{0x18D1, 0x4E42},
	{0x18D1, 0x4E43},
	{0x18D1, 0x4E44},
	{0x18D1, 0x4EE0},	// Nexus 4/5
	{0x18D1, 0x4EE1},
	{0x18D1, 0x4EE2},
	{0x18D1, 0x4EE3},
	{0x18D1, 0x4EE4},
	{0x18D1, 0x4EE5},
	{0x18D1, 0x4EE6},
	{0x18D1, 0x4EE7},
	{0x18D1, 0x708C},
	{0x18D1, 0x708C},
	{0x18D1, 0x7102},	// Toshiba Thrive tablet
	{0x18D1, 0x9001},
	{0x18D1, 0xB004},
	{0x18D1, 0xD001},	// Nexus 4
	{0x18D1, 0xD002},
	{0x18D1, 0xD109},	// LG G2x
	{0x18D1, 0xD10A},
	{0x19D2, 0x1351},
	{0x19D2, 0x1354},
	{0x2080, 0x0001},	// Nook
	{0x2080, 0x0002},
	{0x2080, 0x0003},
	{0x2080, 0x0004},
	{0x22B8, 0x2D66},	// Motorola
	{0x22B8, 0x41DB},
	{0x22B8, 0x4286},
	{0x22B8, 0x42A4},
	{0x22B8, 0x42DA},
	{0x22B8, 0x4331},
	{0x22B8, 0x70A9},
};


// For the retrieval of the device description on Windows 7
#ifndef DEVPROPKEY_DEFINED
typedef struct {
    GUID  fmtid;
    ULONG pid;
} DEVPROPKEY;
#endif

const DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc = {
	{ 0x540b947e, 0x8b40, 0x45bc, {0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2} }, 4 };

// Check the status of the installer process
static int __inline check_completion(HANDLE process_handle) {
	DWORD exit_code;
	GetExitCodeProcess(process_handle, &exit_code);
	return (exit_code==0)?WDI_SUCCESS:((exit_code==STILL_ACTIVE)?WDI_ERROR_TIMEOUT:WDI_ERROR_OTHER);
}

// Convert a UNIX timestamp to a MS FileTime one
static int64_t __inline unixtime_to_msfiletime(time_t t)
{
	int64_t ret = (int64_t)t;
	ret *= INT64_C(10000000);
	ret += INT64_C(116444736000000000);
	return ret;
}
