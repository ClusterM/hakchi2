/*
 * Library for WinUSB/libusb automated driver installation
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

#include <windows.h>
#include <objbase.h>

#if defined(_MSC_VER)
// disable MSVC warnings that are benign
#pragma warning(disable:4100)  // unreferenced formal parameter
#pragma warning(disable:4127)  // conditional expression is constant
#pragma warning(disable:4201)  // nameless struct/union
#pragma warning(disable:4214)  // bit field types other than int
#pragma warning(disable:4996)  // deprecated API calls
#pragma warning(disable:28159) // more deprecated API calls
#endif

#define MAX_DESC_LENGTH             256
#define MAX_PATH_LENGTH             512
#define MAX_KEY_LENGTH              256
#define STR_BUFFER_SIZE             256
#define MAX_GUID_STRING_LENGTH      40

#define INSTALLER_PIPE_NAME         "\\\\.\\pipe\\libwdi-installer"

#define safe_free(p) do {if (p != NULL) {free((void*)p); p = NULL;}} while(0)
#define safe_min(a, b) min((size_t)(a), (size_t)(b))
#define safe_strcp(dst, dst_max, src, count) do {memcpy(dst, src, safe_min(count, dst_max)); \
	((char*)dst)[safe_min(count, dst_max)-1] = 0;} while(0)
#define safe_strcpy(dst, dst_max, src) safe_strcp(dst, dst_max, src, safe_strlen(src)+1)
#define static_strcpy(dst, src) safe_strcpy(dst, sizeof(dst), src)
#define safe_strncat(dst, dst_max, src, count) strncat(dst, src, safe_min(count, dst_max - safe_strlen(dst) - 1))
#define safe_strcat(dst, dst_max, src) safe_strncat(dst, dst_max, src, safe_strlen(src)+1)
#define static_strcat(dst, src) safe_strcat(dst, sizeof(dst), src)
#define safe_strcmp(str1, str2) strcmp(((str1==NULL)?"<NULL>":str1), ((str2==NULL)?"<NULL>":str2))
#define safe_stricmp(str1, str2) _stricmp(((str1==NULL)?"<NULL>":str1), ((str2==NULL)?"<NULL>":str2))
#define safe_strncmp(str1, str2, count) strncmp(((str1==NULL)?"<NULL>":str1), ((str2==NULL)?"<NULL>":str2), count)
#define safe_closehandle(h) do {if (h != INVALID_HANDLE_VALUE) {CloseHandle(h); h = INVALID_HANDLE_VALUE;}} while(0)
#define safe_sprintf(dst, count, ...) do {_snprintf(dst, count, __VA_ARGS__); (dst)[(count)-1] = 0; } while(0)
#define safe_strlen(str) ((((char*)str)==NULL)?0:strlen(str))
#define static_sprintf(dest, format, ...) safe_sprintf(dest, sizeof(dest), format, __VA_ARGS__)
#define safe_swprintf _snwprintf
#define safe_strdup _strdup
#ifndef ARRAYSIZE
#define ARRAYSIZE(A) (sizeof(A)/sizeof((A)[0]))
#endif
#define IGNORE_RETVAL(expr) do { (void)(expr); } while(0)

#if defined(_MSC_VER)
#define safe_vsnprintf(buf, size, format, arg) _vsnprintf_s(buf, size, _TRUNCATE, format, arg)
#else
#define safe_vsnprintf vsnprintf
#endif

/*
 * For communications between installer <-> libwdi
 */
enum installer_code {
	IC_PRINT_MESSAGE,
	IC_SYSLOG_MESSAGE,
	IC_GET_DEVICE_ID,
	IC_GET_HARDWARE_ID,
	IC_GET_USER_SID,
	IC_SET_TIMEOUT_INFINITE,
	IC_SET_TIMEOUT_DEFAULT,
	IC_SET_STATUS,
	IC_INSTALLER_COMPLETED,
};

/* Helper functions to access DLLs */
static __inline HMODULE GetDLLHandle(char* szDLLName)
{
	HANDLE h = GetModuleHandleA(szDLLName);
	if (h == NULL) {
		// coverity[alloc_fn][var_assign]
		h = LoadLibraryA(szDLLName);
	}
	return h;
}
#define PF_TYPE(api, ret, proc, args) typedef ret (api *proc##_t)args
#define PF_DECL(proc) static proc##_t pf##proc = NULL
#define PF_TYPE_DECL(api, ret, proc, args) PF_TYPE(api, ret, proc, args);  PF_DECL(proc)
#define PF_INIT(proc, dllname) if (pf##proc == NULL) pf##proc = (proc##_t) GetProcAddress(GetDLLHandle(#dllname), #proc)
#define PF_INIT_OR_OUT(proc, dllname) PF_INIT(proc, dllname); if (pf##proc == NULL) { \
	PF_ERR("Unable to locate %s() in %s\n", #proc, #dllname); goto out; }

/*
 * Cfgmgr32.dll interface
 */
typedef DWORD DEVNODE, DEVINST;
typedef DEVNODE *PDEVNODE, *PDEVINST;
typedef DWORD RETURN_TYPE;
typedef RETURN_TYPE CONFIGRET;

#define CR_SUCCESS                        0x00000000
#define CR_NO_SUCH_DEVNODE                0x0000000D
#define CONFIGFLAG_REINSTALL              0x00000020

#define CM_REENUMERATE_NORMAL             0x00000000
#define CM_REENUMERATE_SYNCHRONOUS        0x00000001
#define CM_REENUMERATE_RETRY_INSTALLATION 0x00000002
#define CM_REENUMERATE_ASYNCHRONOUS       0x00000004
#define CM_REENUMERATE_BITS               0x00000007

/*
 * DifXApi.dll interface
 */
#define DRIVER_PACKAGE_REPAIR             0x00000001
#define DRIVER_PACKAGE_FORCE              0x00000004
#define DRIVER_PACKAGE_LEGACY_MODE        0x00000010

#define ERROR_INVALID_CATALOG_DATA        0xE0000304
#ifndef ERROR_DRIVER_STORE_ADD_FAILED
#define ERROR_DRIVER_STORE_ADD_FAILED     0xE0000247
#endif
#ifndef ERROR_NO_AUTHENTICODE_CATALOG
#define ERROR_NO_AUTHENTICODE_CATALOG     0xE000023F
#endif
#ifndef ERROR_IN_WOW64
#define ERROR_IN_WOW64                    0xE0000235
#endif
#ifndef ERROR_AUTHENTICODE_PUBLISHER_NOT_TRUSTED
#define ERROR_AUTHENTICODE_PUBLISHER_NOT_TRUSTED 0xE0000243
#endif
#ifndef ERROR_FILE_HASH_NOT_IN_CATALOG
#define ERROR_FILE_HASH_NOT_IN_CATALOG    0xE000024B
#endif

typedef enum {
	DIFXAPI_SUCCESS,
	DIFXAPI_INFO,
	DIFXAPI_WARNING,
	DIFXAPI_ERROR
} DIFXAPI_LOG;
typedef void (__cdecl* DIFXAPILOGCALLBACK)(DIFXAPI_LOG EventType, DWORD Error, LPCSTR EventDescription, PVOID CallbackContext);

typedef struct {
	LPCSTR  pApplicationId;
	LPCSTR  pDisplayName;
	LPCSTR  pProductName;
	LPCSTR  pMfgName;
} INSTALLERINFO, *PINSTALLERINFO;
typedef const PINSTALLERINFO PCINSTALLERINFO;

/*
 * GPEdit Interface (from MinGW-w64)
 */
typedef enum _GROUP_POLICY_OBJECT_TYPE {
  GPOTypeLocal = 0,GPOTypeRemote,GPOTypeDS
} GROUP_POLICY_OBJECT_TYPE,*PGROUP_POLICY_OBJECT_TYPE;

#define REGISTRY_EXTENSION_GUID { 0x35378EAC,0x683F,0x11D2, {0xA8,0x9A,0x00,0xC0,0x4F,0xBB,0xCF,0xA2} }
#define GPO_OPEN_LOAD_REGISTRY 0x00000001
#define GPO_SECTION_MACHINE 2

#undef INTERFACE
#define INTERFACE IGroupPolicyObject
  DECLARE_INTERFACE_(IGroupPolicyObject,IUnknown) {
    STDMETHOD(QueryInterface) (THIS_ REFIID riid,LPVOID *ppvObj) PURE;
    STDMETHOD_(ULONG,AddRef) (THIS) PURE;
    STDMETHOD_(ULONG,Release) (THIS) PURE;
    STDMETHOD(New) (THIS_ LPOLESTR pszDomainName,LPOLESTR pszDisplayName,DWORD dwFlags) PURE;
    STDMETHOD(OpenDSGPO) (THIS_ LPOLESTR pszPath,DWORD dwFlags) PURE;
    STDMETHOD(OpenLocalMachineGPO) (THIS_ DWORD dwFlags) PURE;
    STDMETHOD(OpenRemoteMachineGPO) (THIS_ LPOLESTR pszComputerName,DWORD dwFlags) PURE;
    STDMETHOD(Save) (THIS_ BOOL bMachine, BOOL bAdd,GUID *pGuidExtension,GUID *pGuid) PURE;
    STDMETHOD(Delete) (THIS) PURE;
    STDMETHOD(GetName) (THIS_ LPOLESTR pszName,int cchMaxLength) PURE;
    STDMETHOD(GetDisplayName) (THIS_ LPOLESTR pszName,int cchMaxLength) PURE;
    STDMETHOD(SetDisplayName) (THIS_ LPOLESTR pszName) PURE;
    STDMETHOD(GetPath) (THIS_ LPOLESTR pszPath,int cchMaxPath) PURE;
    STDMETHOD(GetDSPath) (THIS_ DWORD dwSection,LPOLESTR pszPath,int cchMaxPath) PURE;
    STDMETHOD(GetFileSysPath) (THIS_ DWORD dwSection,LPOLESTR pszPath,int cchMaxPath) PURE;
    STDMETHOD(GetRegistryKey) (THIS_ DWORD dwSection,HKEY *hKey) PURE;
    STDMETHOD(GetOptions) (THIS_ DWORD *dwOptions) PURE;
    STDMETHOD(SetOptions) (THIS_ DWORD dwOptions,DWORD dwMask) PURE;
    STDMETHOD(GetType) (THIS_ GROUP_POLICY_OBJECT_TYPE *gpoType) PURE;
    STDMETHOD(GetMachineName) (THIS_ LPOLESTR pszName,int cchMaxLength) PURE;
    STDMETHOD(GetPropertySheetPages) (THIS_ HPROPSHEETPAGE **hPages,UINT *uPageCount) PURE;
  };
  typedef IGroupPolicyObject *LPGROUPPOLICYOBJECT;
