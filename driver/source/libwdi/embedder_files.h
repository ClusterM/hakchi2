/*
 * embedder : converts binary resources into a .h include
 * "If you can think of a better way to get ice, I'd like to hear it."
 * Copyright (c) 2010-2013 Pete Batard <pete@akeo.ie>
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

/*
 * This include defines the driver files that should be embedded in the library.
 * This file is meant to be used by libwdi developers only.
 * If you want to add extra files from a specific directory (eg signed inf and cat)
 * you should either define the macro USER_DIR in msvc/config.h (MS compilers) or
 * use the --with-userdir option when running configure.
 */

struct emb {
	int reuse_last;
	char* file_name;
	char* extraction_subdir;
};

#define _STR(s) #s
#define STR(s) _STR(s)

/*
 * files to embed
 */
struct emb embeddable_fixed[] = {

// WinUSB
#if defined(DDK_DIR)
#	if defined(OPT_M32)
		{ 0, DDK_DIR "\\redist\\wdf\\x86\\WdfCoInstaller0" STR(WDF_VER) ".dll", "x86" },
		{ 0, DDK_DIR "\\redist\\" COINSTALLER_DIR "\\x86\\winusbcoinstaller2.dll", "x86" },
#	endif	// OPT_M32
#	if defined(OPT_M64)
		{ 0, DDK_DIR "\\redist\\wdf\\" X64_DIR "\\WdfCoInstaller0" STR(WDF_VER) ".dll", "amd64" },
		{ 0, DDK_DIR "\\redist\\" COINSTALLER_DIR "\\" X64_DIR "\\winusbcoinstaller2.dll", "amd64" },
#	endif	// OPT_M64
#	if defined(OPT_IA64)
		{ 0, DDK_DIR "\\redist\\wdf\\ia64\\WdfCoInstaller0" STR(WDF_VER) ".dll", "ia64" },
		{ 0, DDK_DIR "\\redist\\" COINSTALLER_DIR "\\ia64\\winusbcoinstaller2.dll", "ia64" },
#	endif	// OPT_IA64
#endif	// DDK_DIR

// libusb0
#if defined(LIBUSB0_DIR)
	{ 0, LIBUSB0_DIR "\\bin\\x86\\libusb0_x86.dll", "x86" },
	{ 0, LIBUSB0_DIR "\\bin\\x86\\install-filter.exe", "x86" },
#	if defined(LIBUSBK_DIR)
#		if defined(OPT_M32)
			{ 1, "libusb0.dll", "x86" },	// reuse
#		endif	// OPT_M32
#		if defined(OPT_M64)
			{ 1, "libusb0_x86.dll", "amd64" },	// reuse
#		endif	// OPT_M64
#	endif	// LIBUSBK_DIR
#	if defined(OPT_M32)
		{ 0, LIBUSB0_DIR "\\bin\\x86\\libusb0.sys", "x86" },
#	endif	// OPT_M32
#	if defined(OPT_M64)
		{ 0, LIBUSB0_DIR "\\bin\\amd64\\libusb0.dll", "amd64" },
		{ 0, LIBUSB0_DIR "\\bin\\amd64\\libusb0.sys", "amd64" },
		{ 0, LIBUSB0_DIR "\\bin\\amd64\\install-filter.exe", "amd64" },
#	endif	// OPT_M64
#	if defined(OPT_IA64)
		{ 0, LIBUSB0_DIR "\\bin\\ia64\\libusb0.dll", "ia64" },
		{ 0, LIBUSB0_DIR "\\bin\\ia64\\libusb0.sys", "ia64" },
		{ 0, LIBUSB0_DIR "\\bin\\ia64\\install-filter.exe", "ia64" },
#	endif	// OPT_IA64
	{ 0, LIBUSB0_DIR "\\installer_license.txt", "license\\libusb0" },
#endif	// LIBUSB0_DIR

// libusbK
#if defined(LIBUSBK_DIR)

#	if	defined(OPT_M32)
#		if !defined(DDK_DIR)
			{ 0, LIBUSBK_DIR "\\sys\\x86\\WdfCoInstaller" STR(WDF_VER) ".dll", "x86" },
#		endif	// DDK_DIR
		{ 0, LIBUSBK_DIR "\\sys\\x86\\libusbK.sys", "x86" },
		{ 0, LIBUSBK_DIR "\\dll\\x86\\libusbK.dll", "x86" },
#		if defined(OPT_M64)
			{ 1, "libusbK_x86.dll", "amd64" },	// reuse
#		endif	// OPT_M64
#		if !defined(LIBUSB0_DIR)
			{ 0, LIBUSBK_DIR "\\dll\\x86\\libusb0.dll", "x86" },
#			if defined(OPT_M64)
				{ 1, "libusb0_x86.dll", "amd64" },	// reuse
#			endif	// OPT_M64
#		endif	// LIBUSB0_DIR
#	endif	// OPT_M32

#	if defined(OPT_M64)
#		if !defined(DDK_DIR)
			{ 0, LIBUSBK_DIR "\\sys\\amd64\\WdfCoInstaller" STR(WDF_VER) ".dll", "amd64" },
#		endif	// DDK_DIR
		{ 0, LIBUSBK_DIR "\\sys\\amd64\\libusbK.sys", "amd64" },
		{ 0, LIBUSBK_DIR "\\dll\\amd64\\libusbK.dll", "amd64" },
#		if !defined(LIBUSB0_DIR)
			{ 0, LIBUSBK_DIR "\\dll\\amd64\\libusb0.dll", "amd64" },
#		endif	// LIBUSB0_DIR
#		if !defined(OPT_M32)
			// The x86/ DLLs will not be used, but they are required for rename to _x86
			{ 0, LIBUSBK_DIR "\\dll\\x86\\libusbK.dll", "x86" },
			{ 1, "libusbK_x86.dll", "amd64" },
#			if !defined(LIBUSB0_DIR)
				{ 0, LIBUSBK_DIR "\\dll\\x86\\libusb0.dll", "x86" },
				{ 1, "libusb0_x86.dll", "amd64" },
#			endif	// LIBUSB0_DIR
#		endif	// OPT_M32
#	endif	// OPT_M64

#	if defined(OPT_IA64)
#		if !defined(DDK_DIR)
			{ 0, LIBUSBK_DIR "\\sys\\ia64\\WdfCoInstaller" STR(WDF_VER) ".dll", "ia64" },
#		endif	// DDK_DIR
		{ 0, LIBUSBK_DIR "\\sys\\ia64\\libusbK.sys", "ia64" },
		{ 0, LIBUSBK_DIR "\\dll\\ia64\\libusbK.dll", "ia64" },
		{ 0, LIBUSBK_DIR "\\dll\\ia64\\libusb0.dll", "ia64" },
#	endif	// OPT_IA64

#endif	// LIBUSBK_DIR

// Common files
#if defined(OPT_M32)
	{ 0, INSTALLER_PATH_32 "\\installer_x86.exe", "." },
#endif
#if defined(OPT_M64)
	{ 0, INSTALLER_PATH_64 "\\installer_x64.exe", "." },
#endif
// inf templates for the tokenizer ("" directory means no extraction)
	{ 0, "winusb.inf.in", "" },
	{ 0, "libusb0.inf.in", "" },
	{ 0, "libusbk.inf.in", "" },
	{ 0, "usbser.inf.in", "" },
// cat file lists for self signing
	{ 0, "winusb.cat.in", "" },
	{ 0, "libusb0.cat.in", "" },
	{ 0, "libusbk.cat.in", "" },
	{ 0, "usbser.cat.in", "" },
};
