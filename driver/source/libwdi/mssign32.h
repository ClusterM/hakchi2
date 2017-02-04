/*
 * mssign32.h: MSSign32 interface for code signing
 *
 * Copyright (c) 2011-2013 Pete Batard <pete@akeo.ie>
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
#include <wincrypt.h>

#ifdef __cplusplus
extern "C" {
#endif

/*
 * MSSign32 definitions
 * see http://msdn.microsoft.com/en-us/library/aa380252.aspx#signer_functions
 */

// SIGNER_SUBJECT_INFO
#define SIGNER_SUBJECT_FILE					1
#define SIGNER_SUBJECT_BLOB					2
// SIGNER_CERT
#define SIGNER_CERT_SPC_FILE				1
#define SIGNER_CERT_STORE					2
#define SIGNER_CERT_SPC_CHAIN				3
// SIGNER_SPC_CHAIN_INFO
#define SIGNER_CERT_POLICY_STORE			1
#define SIGNER_CERT_POLICY_CHAIN			2
#define SIGNER_CERT_POLICY_CHAIN_NO_ROOT	8
// SIGNER_SIGNATURE_INFO
#define SIGNER_NO_ATTR						0
#define SIGNER_AUTHCODE_ATTR				1
// SIGNER_PROVIDER_INFO
#define PVK_TYPE_FILE_NAME					1
#define PVK_TYPE_KEYCONTAINER				2
// SignerSignEx
#define SPC_EXC_PE_PAGE_HASHES_FLAG			0x10
#define SPC_INC_PE_IMPORT_ADDR_TABLE_FLAG	0x20
#define SPC_INC_PE_DEBUG_INFO_FLAG			0x40
#define SPC_INC_PE_RESOURCES_FLAG			0x80
#define SPC_INC_PE_PAGE_HASHES_FLAG			0x100
/* 
 * The following is the data Microsoft adds on the 
 * SPC_SP_OPUS_INFO_OBJID and SPC_STATEMENT_TYPE_OBJID OIDs
 */
#define SP_OPUS_INFO_DATA	{ 0x30, 0x00 }
#define STATEMENT_TYPE_DATA	{ 0x30, 0x0c, 0x06, 0x0a, 0x2b, 0x06, 0x01, 0x04, 0x01, 0x82, 0x37, 0x02, 0x01, 0x15}

typedef struct _SIGNER_FILE_INFO {
	DWORD cbSize;
	LPCWSTR pwszFileName;
	HANDLE hFile;
} SIGNER_FILE_INFO, *PSIGNER_FILE_INFO;

typedef struct _SIGNER_BLOB_INFO {
	DWORD cbSize;
	GUID *pGuidSubject;
	DWORD cbBlob;
	BYTE *pbBlob;
	LPCWSTR pwszDisplayName;
} SIGNER_BLOB_INFO, *PSIGNER_BLOB_INFO;

typedef struct _SIGNER_SUBJECT_INFO {
	DWORD cbSize;
	DWORD *pdwIndex;
	DWORD dwSubjectChoice;
	union {
		SIGNER_FILE_INFO *pSignerFileInfo;
		SIGNER_BLOB_INFO *pSignerBlobInfo;
	};
} SIGNER_SUBJECT_INFO, *PSIGNER_SUBJECT_INFO;

typedef struct _SIGNER_CERT_STORE_INFO {  
	DWORD cbSize;  
	PCCERT_CONTEXT pSigningCert;  
	DWORD dwCertPolicy;  
	HCERTSTORE hCertStore;
} SIGNER_CERT_STORE_INFO, *PSIGNER_CERT_STORE_INFO;

typedef struct _SIGNER_SPC_CHAIN_INFO {  
	DWORD cbSize;  
	LPCWSTR pwszSpcFile;  
	DWORD dwCertPolicy;  
	HCERTSTORE hCertStore;
} SIGNER_SPC_CHAIN_INFO, *PSIGNER_SPC_CHAIN_INFO;

typedef struct _SIGNER_CERT {  
	DWORD cbSize;  
	DWORD dwCertChoice;  
	union {    
		LPCWSTR pwszSpcFile;    
		SIGNER_CERT_STORE_INFO *pCertStoreInfo;    
		SIGNER_SPC_CHAIN_INFO *pSpcChainInfo;  
	};
	HWND hwnd;
} SIGNER_CERT, *PSIGNER_CERT;

typedef struct _SIGNER_ATTR_AUTHCODE {  
	DWORD cbSize;
	BOOL fCommercial;
	BOOL fIndividual;
	LPCWSTR pwszName;
	LPCWSTR pwszInfo;
} SIGNER_ATTR_AUTHCODE, *PSIGNER_ATTR_AUTHCODE;

// MinGW32 doesn't know CRYPT_ATTRIBUTES
typedef struct _CRYPT_ATTRIBUTES_ARRAY {
	DWORD cAttr;
	PCRYPT_ATTRIBUTE rgAttr;
} CRYPT_ATTRIBUTES_ARRAY, *PCRYPT_ATTRIBUTES_ARRAY;

typedef struct _SIGNER_SIGNATURE_INFO {  
	DWORD cbSize;
	ALG_ID algidHash;
	DWORD dwAttrChoice;
	union {
		SIGNER_ATTR_AUTHCODE *pAttrAuthcode;
	};
	PCRYPT_ATTRIBUTES_ARRAY psAuthenticated;
	PCRYPT_ATTRIBUTES_ARRAY psUnauthenticated;
} SIGNER_SIGNATURE_INFO, *PSIGNER_SIGNATURE_INFO;

typedef struct _SIGNER_PROVIDER_INFO {  
	DWORD cbSize;  
	LPCWSTR pwszProviderName;  
	DWORD dwProviderType;  
	DWORD dwKeySpec;  
	DWORD dwPvkChoice;  
	union {    
		LPWSTR pwszPvkFileName;    
		LPWSTR pwszKeyContainer;  
	} ;
} SIGNER_PROVIDER_INFO,  *PSIGNER_PROVIDER_INFO;

typedef struct _SIGNER_CONTEXT {  
	DWORD cbSize;  
	DWORD cbBlob;  
	BYTE *pbBlob;
} SIGNER_CONTEXT, *PSIGNER_CONTEXT;

/*
 * typedefs for the function prototypes. Use the something like:
 *   PF_DELC(SignerSignEx);
 * which translates to:
 *  SignerSignEx_t pfSignerSignEx = NULL;
 * in your code, to declare the entrypoint and then use:
 *   PF_INIT(SignerSignEx, mssign32);
 * which translates to:
 *   pfSignerSignEx = (SignerSignEx_t) GetProcAddress(GetDLLHandle("mssign32"), "SignerSignEx");
 * to make it accessible.
 */
typedef HRESULT (WINAPI *SignerFreeSignerContext_t)(
	SIGNER_CONTEXT *pSignerContext
);

typedef HRESULT (WINAPI *SignError_t)(void);

typedef HRESULT (WINAPI *SignerSign_t)(
	SIGNER_SUBJECT_INFO *pSubjectInfo,
	SIGNER_CERT *pSignerCert,
	SIGNER_SIGNATURE_INFO *pSignatureInfo,
	SIGNER_PROVIDER_INFO *pProviderInfo,
	LPCWSTR pwszHttpTimeStamp,
	PCRYPT_ATTRIBUTES_ARRAY psRequest,
	LPVOID pSipData
);

typedef HRESULT (WINAPI *SignerSignEx_t)(
	DWORD dwFlags,
	SIGNER_SUBJECT_INFO *pSubjectInfo,
	SIGNER_CERT *pSignerCert,
	SIGNER_SIGNATURE_INFO *pSignatureInfo,
	SIGNER_PROVIDER_INFO *pProviderInfo,
	LPCWSTR pwszHttpTimeStamp,
	PCRYPT_ATTRIBUTES_ARRAY psRequest,
	LPVOID pSipData,
	SIGNER_CONTEXT **ppSignerContext
);

typedef HRESULT (WINAPI *SignerTimeStamp_t)(
	SIGNER_SUBJECT_INFO *pSubjectInfo,
	LPCWSTR pwszHttpTimeStamp,
	PCRYPT_ATTRIBUTES_ARRAY psRequest,
	LPVOID pSipData
);

typedef HRESULT (WINAPI *SignerTimeStampEx_t)(
	DWORD dwFlags,
	SIGNER_SUBJECT_INFO *pSubjectInfo,
	LPCWSTR pwszHttpTimeStamp,
	PCRYPT_ATTRIBUTES_ARRAY psRequest,
	LPVOID pSipData,
	SIGNER_CONTEXT **ppSignerContext 
);

typedef HRESULT (WINAPI *SignerTimeStampEx2_t)(
	DWORD dwFlags,
	SIGNER_SUBJECT_INFO *pSubjectInfo,
	LPCWSTR pwszHttpTimeStamp,
	ALG_ID dwAlgId,
	PCRYPT_ATTRIBUTES_ARRAY psRequest,
	LPVOID pSipData,
	SIGNER_CONTEXT **ppSignerContext 
);

#ifdef __cplusplus
}
#endif
