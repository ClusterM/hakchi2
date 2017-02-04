/*
* Library for USB automated driver installation
* Copyright (c) 2010-2016 Pete Batard <pete@akeo.ie>
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

#include <windows.h>
#include <stdint.h>

#pragma once

#define REGKEY_HKCU                 HKEY_CURRENT_USER
#define REGKEY_HKLM                 HKEY_LOCAL_MACHINE

#define WDI_COMPANY_NAME            "Akeo Consulting"
#define WDI_APPLICATION_NAME        "libwdi"

// Windows versions
enum WindowsVersion {
	WINDOWS_UNDEFINED = -1,
	WINDOWS_UNSUPPORTED = 0,
	WINDOWS_XP = 0x51,
	WINDOWS_2003 = 0x52,	// Also XP x64
	WINDOWS_VISTA = 0x60,
	WINDOWS_7 = 0x61,
	WINDOWS_8 = 0x62,
	WINDOWS_8_1_OR_LATER = 0x63,
	WINDOWS_10_PREVIEW1 = 0x64,
	WINDOWS_10 = 0xA0,
	WINDOWS_MAX
};

extern int nWindowsVersion;
extern char WindowsVersionStr[128];

void GetWindowsVersion(void);

/* Read a string registry key value */
static __inline BOOL ReadRegistryStr(HKEY key_root, const char* key_name, char* str, DWORD len)
{
	BOOL r = FALSE;
	size_t i = 0;
	LONG s;
	HKEY hApp = NULL;
	DWORD dwType = -1, dwSize = len;
	LPBYTE dest = (LPBYTE)str;
	// VS Code Analysis complains if we don't break our initialization into chars
	char long_key_name[256] = { 0 };
	memset(dest, 0, len);

	if (key_name == NULL)
		return FALSE;

	for (i = safe_strlen(key_name); i>0; i--) {
		if (key_name[i] == '\\')
			break;
	}

	if (i != 0) {
		strcpy(long_key_name, "SOFTWARE\\");
		safe_strcat(long_key_name, sizeof(long_key_name), key_name);
		long_key_name[sizeof("SOFTWARE\\") + i - 1] = 0;
		i++;
		if (RegOpenKeyExA(key_root, long_key_name, 0, KEY_READ, &hApp) != ERROR_SUCCESS) {
			hApp = NULL;
			goto out;
		}
	} else {
		goto out;
	}

	s = RegQueryValueExA(hApp, &key_name[i], NULL, &dwType, (LPBYTE)dest, &dwSize);
	// No key means default value of 0 or empty string
	if ((s == ERROR_FILE_NOT_FOUND) || ((s == ERROR_SUCCESS) && (dwType == REG_SZ) && (dwSize > 0))) {
		r = TRUE;
	}
out:
	if (hApp != NULL)
		RegCloseKey(hApp);
	return r;
}
