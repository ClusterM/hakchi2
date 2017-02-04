/*
 * Library for USB automated driver installation
 * Copyright (c) 2010-2013 Pete Batard <pete@akeo.ie>
 * Parts of the code from libusb by Daniel Drake, Johannes Erdfelt et al.
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

/* Memory leaks detection - define _CRTDBG_MAP_ALLOC as preprocessor macro */
#ifdef _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#endif

#include <windows.h>
#include <windowsx.h>
#include <stdlib.h>
#include <stdio.h>
#include <shlobj.h>
#include <process.h>
#include <stdint.h>
#include <wingdi.h>
#include <config.h>

#include "installer.h"
#include "libwdi.h"
#include "logging.h"
#include "resource.h"

// WM_APP is not sent on focus, unlike WM_USER
enum stdlg_user_message_type {
	UM_PROGRESS_START = WM_APP,
	UM_PROGRESS_STOP,
};

// Messages that appear in our progress bar as time passes
static const char* progress_message[] = {
	"Installation can take some time...",
	"The installation process can take up to 5 minutes...",
	"You may also be asked to reboot for KMDF upgrades.",
	"If so, please watch for additional popup windows.",	// 1 min
	"The reason driver installation may take time...",
	"...is because a System Restore point is created.",
	"Microsoft offers no means of checking progress...",
	"...so we can't say how long it'll take...",			// 2 mins
	"Please continue to be patient...",
	"There's a 5 minutes timeout enventually...",
	"...so if there's a problem, the process will abort.",
	"I've really seen an installation take 5 minutes...",	// 3 mins
	"...on a Vista 64 machine with a very large disk.",
	"So how was your day...",
	"...before it got ruined by this endless installation?",
	"Seriously, what is taking this process so long?!",		// 4 mins
	"Aborting in 45 seconds...",
	"Aborting in 30 seconds...",
	"Aborting in 15 seconds...",
};

#ifndef PBS_MARQUEE
#define PBS_MARQUEE 0x08
#endif
#ifndef PBM_SETMARQUEE
#define PBM_SETMARQUEE (WM_USER+10)
#endif

/*
 * Globals
 */
static uintptr_t progress_thid = -1L;
static HWND hProgress = INVALID_HANDLE_VALUE;
static HWND hProgressBar = INVALID_HANDLE_VALUE;
static HWND hProgressText = INVALID_HANDLE_VALUE;
static HINSTANCE app_instance = NULL;
static int (*progress_function)(void*);
static void* progress_arglist;
static HANDLE progress_mutex = INVALID_HANDLE_VALUE;

// Work around for GDI calls on DDK (would require end user apps linking with Gdi32 othwerwise)
static HFONT (WINAPI *pCreateFontA)(int, int, int, int, int, DWORD, DWORD, DWORD, DWORD, DWORD, DWORD, DWORD, DWORD, LPCSTR) = NULL;
static HGDIOBJ (WINAPI *pGetStockObject)(int) = NULL;
static int (WINAPI *pSetBkMode)(HDC, int) = NULL;

#define IS_CREATEFONT_AVAILABLE (pCreateFontA != NULL)
#define IS_BACKGROUND_AVAILABLE ((pGetStockObject != NULL) && (pSetBkMode != NULL))

#define INIT_GDI32 do {	\
	pCreateFontA = (HFONT (WINAPI *)(int, int, int, int, int, DWORD, DWORD, DWORD, DWORD, DWORD, DWORD, DWORD, DWORD, LPCSTR))	\
		GetProcAddress(GetDLLHandle("gdi32.dll"), "CreateFontA"); \
	pGetStockObject = (HGDIOBJ (WINAPI *)(int))	\
		GetProcAddress(GetDLLHandle("gdi32.dll"), "GetStockObject"); \
	pSetBkMode = (int (WINAPI *)(HDC, int))	\
		GetProcAddress(GetDLLHandle("gdi32.dll"), "SetBkMode"); \
	} while(0)

extern char *windows_error_str(uint32_t retval);


/*
 * Detect if a Windows Security prompt is active, by enumerating the
 * whole Windows tree and looking for a security popup
 */
static BOOL CALLBACK security_prompt_callback(HWND hWnd, LPARAM lParam)
{
	char str_buf[STR_BUFFER_SIZE];
	HWND *hFound = (HWND*)lParam;
	const char* security_string = "Windows Security";

	// The security prompt has the popup window style
	if (GetWindowLong(hWnd, GWL_STYLE) & WS_POPUPWINDOW) {
		str_buf[0] = 0;
		GetWindowTextA(hWnd, str_buf, STR_BUFFER_SIZE);
		str_buf[STR_BUFFER_SIZE-1] = 0;
		if (safe_strcmp(str_buf, security_string) == 0) {
			*hFound = hWnd;
		}
	}
	return TRUE;
}

HWND find_security_prompt(void) {
	HWND hSecurityPrompt = NULL;
	EnumChildWindows(GetDesktopWindow(), security_prompt_callback, (LPARAM)&hSecurityPrompt);
	return hSecurityPrompt;
}

/*
 * Thread executed by the run_with_progress_bar() function
 */
static void __cdecl progress_thread(void* param)
{
	int r;

	// Call the user provided function
	r = (*progress_function)(progress_arglist);
	progress_thid = -1L;
	PostMessage(hProgress, UM_PROGRESS_STOP, (WPARAM)r, 0);
	_endthread();
}

/*
 * Center a dialog with regards to the main application Window
 */
static void center_dialog(HWND dialog)
{
	HWND hParent;
	POINT Point;
	RECT DialogRect;
	RECT ParentRect;
	int nWidth;
	int nHeight;

	hParent = GetParent(dialog);
	if (hParent == NULL) return;

	// Get the size of the dialog box.
	GetWindowRect(dialog, &DialogRect);
	GetClientRect(hParent, &ParentRect);

	// Calculate the height and width of the current dialog
	nWidth = DialogRect.right - DialogRect.left;
	nHeight = DialogRect.bottom - DialogRect.top;

	// Find the center point and convert to screen coordinates.
	Point.x = (ParentRect.right - ParentRect.left) / 2;
	Point.y = (ParentRect.bottom - ParentRect.top) / 2;
	ClientToScreen(hParent, &Point);

	// Calculate the new x, y starting point.
	Point.x -= nWidth / 2;
	Point.y -= nHeight / 2 + 35;

	// Move the window.
	MoveWindow(dialog, Point.x, Point.y, nWidth, nHeight, FALSE);
}

/*
 * Dialog sub-elements
 */
static void init_children(HWND hDlg) {

	HFONT hFont;
	// Progress Bar
	hProgressBar = CreateWindowExA(WS_EX_NOPARENTNOTIFY, PROGRESS_CLASSA,
		NULL,
		WS_CHILDWINDOW | WS_VISIBLE | PBS_MARQUEE,
		10,35,250,12,
		hDlg,
		NULL,
		app_instance,
		NULL);
	if (hProgressBar == NULL) {
		wdi_err("Unable to create progress bar: %s", windows_error_str(0));
	}

	// Start progress animation
	PostMessage(hProgressBar, PBM_SETMARQUEE, TRUE, 0);

	// Progress Text
	hProgressText = CreateWindowExA(WS_EX_NOPARENTNOTIFY|WS_EX_TRANSPARENT, WC_STATICA,
		"Installing Driver...",
		WS_CHILDWINDOW | WS_VISIBLE | WS_GROUP,
		12,12,250,16,
		hDlg,
		NULL,
		app_instance,
		NULL);
	if (hProgressBar == NULL) {
		wdi_err("Unable to create progress text: %s", windows_error_str(0));
	}

	// Set the font to MS Dialog default
	INIT_GDI32;
	if (IS_CREATEFONT_AVAILABLE) {
		hFont = pCreateFontA(-11, 0, 0, 0, FW_DONTCARE, 0, 0, 0, ANSI_CHARSET, OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS,
			DEFAULT_QUALITY, DEFAULT_PITCH, "MS Shell Dlg 2");
		SendMessage(hProgressText, WM_SETFONT, (WPARAM)hFont, (LPARAM)TRUE);
	}
}

/*
 * Callback for the run_with_progress_bar() function
 */
static LRESULT CALLBACK progress_callback(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	LRESULT loc;
	HANDLE handle;
	static int installation_time = 0;	// active installation time, in secs
	const int msg_max = ARRAYSIZE(progress_message);
	static int msg_index = 0;
	int i;
	// coordinates that we want to disable (=> no resize)
	static LRESULT disabled[9] = { HTLEFT, HTRIGHT, HTTOP, HTBOTTOM, HTSIZE,
		HTTOPLEFT, HTTOPRIGHT, HTBOTTOMLEFT, HTBOTTOMRIGHT };

	switch (message) {

	case WM_CREATE:
		// Reset static variables
		installation_time = 0;
		msg_index = 0;
		hProgress = hDlg;

		// Start modal (disable parent Window)
		EnableWindow(GetParent(hDlg), FALSE);

		init_children(hProgress);
		center_dialog(hProgress);

		// Send a WM_TIMER message every second
		SetTimer(hProgress, 1, 1000, NULL);

		PostMessage(hProgress, UM_PROGRESS_START, 0, 0);

		return (INT_PTR)TRUE;

	case WM_NCHITTEST:
		// Check coordinates to prevent resize actions
		loc = DefWindowProc(hDlg, message, wParam, lParam);
		for(i = 0; i < 9; i++) {
			if (loc == disabled[i]) {
				return (INT_PTR)TRUE;
			}
		}
		return (INT_PTR)FALSE;

	case UM_PROGRESS_START:
		if (progress_thid != -1L) {
			wdi_err("program assertion failed - another operation is in progress");
		} else {
			// Using a thread prevents application freezout on security warning
			progress_thid = _beginthread(progress_thread, 0, NULL);
			if (progress_thid != -1L) {
				return (INT_PTR)TRUE;
			}
			wdi_err("unable to create progress_thread");
		}
		// Fall through and return an error
		wParam = (WPARAM)WDI_ERROR_RESOURCE;

	case UM_PROGRESS_STOP:
		// If you don't re-enable the parent Window before leaving
		// all kind of bad things happen (other Windows get activated, etc.)
		EnableWindow(GetParent(hDlg), TRUE);
		PostQuitMessage((int)wParam);
		DestroyWindow(hProgress);
		return (INT_PTR)TRUE;

	case WM_TIMER:
		if (find_security_prompt() == NULL) {
			installation_time++;	// Only increment outside of security prompts
			if ( (msg_index < msg_max) && (installation_time > 15*(msg_index+1)) ) {
				// Change the progress blurb
				SetWindowTextA(hProgressText, progress_message[msg_index]);
				// Force a full redraw fot the transparent text background
				ShowWindow(hProgressText, SW_HIDE);
				UpdateWindow(hProgressText);
				ShowWindow(hProgressText, SW_SHOW);
				UpdateWindow(hProgressText);
				msg_index++;
			} else if ( (installation_time > 300) && (progress_thid != -1L) ) {
				// Wait 300 (loose) seconds and kill the thread
				// 300 secs is the timeout for driver installation on Vista
				wdi_err("progress timeout expired - KILLING THREAD!");
				handle = OpenThread(THREAD_TERMINATE, FALSE, (DWORD)progress_thid);
				if (handle != NULL) {
					TerminateThread(handle, -1);
					CloseHandle(handle);
				}
				PostQuitMessage(WDI_ERROR_TIMEOUT);
				DestroyWindow(hProgress);
				return (INT_PTR)FALSE;
			}
		}
		return (INT_PTR)TRUE;

	case WM_CLOSE:		// prevent closure using Alt-F4
		return (INT_PTR)TRUE;

	case WM_DESTROY:	// close application
		hProgress = INVALID_HANDLE_VALUE;
		return (INT_PTR)FALSE;

	case WM_CTLCOLORSTATIC:
		pSetBkMode((HDC)wParam, TRANSPARENT);
		return (INT_PTR)pGetStockObject(NULL_BRUSH);
	}
	return DefWindowProc(hDlg, message, wParam, lParam);
}

/*
 * Call a blocking function (returning an int) as a modal thread with a progress bar
 */
int run_with_progress_bar(HWND hWnd, int(*function)(void*), void* arglist) {
	HWND hDlg;
	MSG msg;
	WNDCLASSEX wc;
	BOOL r;

	if ( (function == NULL) || (hWnd == NULL) ) {
		return WDI_ERROR_INVALID_PARAM;
	}

	app_instance = (HINSTANCE)GetWindowLongPtr(hWnd, GWLP_HINSTANCE);

	// protect access to the thread variables and prevent 2 progress
	// dialogs from executing at the same time
	progress_mutex = CreateMutex(NULL, TRUE, NULL);
	if ((progress_mutex == NULL) || (GetLastError() == ERROR_ALREADY_EXISTS)) {
		wdi_err("could not obtain progress dialog mutex - is another dialog active?");
		if (progress_mutex != NULL)
			CloseHandle(progress_mutex);
		progress_mutex = INVALID_HANDLE_VALUE;
		return WDI_ERROR_BUSY;
	}
	progress_function = function;
	progress_arglist = arglist;

	// Since our lib can be static, we can't use resources
	// => create the whole dialog manually.

	// First we create  Window class if it doesn't already exist
	if (!GetClassInfoEx(app_instance, TEXT("wdi_progress_class"), &wc)) {
		wc.cbSize = sizeof(wc);
		wc.style = CS_DBLCLKS | CS_SAVEBITS;
		wc.lpfnWndProc = progress_callback;
		wc.cbClsExtra = 0;
		wc.cbWndExtra = 0;
		wc.hInstance = app_instance;
		wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
		wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION);
		wc.hCursor = LoadCursor(NULL, IDC_ARROW);
		wc.lpszClassName = TEXT("wdi_progress_class");
		wc.lpszMenuName  = NULL;
		wc.hbrBackground = GetSysColorBrush(COLOR_3DFACE);

		if (!RegisterClassEx(&wc)) {
			wdi_err("can't register class %s", windows_error_str(0));
			safe_closehandle(progress_mutex);
			return WDI_ERROR_RESOURCE;
		}
	}

	// Then we create the dialog base
	hDlg = CreateWindowEx(WS_EX_WINDOWEDGE | WS_EX_CONTROLPARENT,
		TEXT("wdi_progress_class"), TEXT("Installing Driver..."),
		WS_CLIPCHILDREN | WS_CLIPSIBLINGS | WS_CAPTION | WS_POPUP | WS_VISIBLE | WS_THICKFRAME,
		100, 100, 287, 102, hWnd, NULL, app_instance, NULL);
	if (hDlg == NULL) {
		wdi_err("Unable to create progress dialog: %s", windows_error_str(0));
		safe_closehandle(progress_mutex);
		return WDI_ERROR_RESOURCE;
	}

	// Finally we Display the dialog...
	ShowWindow(hDlg, SW_SHOWNORMAL);
	UpdateWindow(hDlg);

	// ...and handle the message processing loop
	while( (r = GetMessage(&msg, NULL, 0, 0)) != 0) {
		if (r == -1) {
			wdi_err("GetMessage error");
		} else {
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	safe_closehandle(progress_mutex);

	return (int)msg.wParam;
}
