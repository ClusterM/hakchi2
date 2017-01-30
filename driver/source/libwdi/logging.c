/*
 * libwdi logging functions
 * Copyright (c) Johannes Erdfelt, Daniel Drake et al.
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

/* Memory leaks detection - define _CRTDBG_MAP_ALLOC as preprocessor macro */
#ifdef _CRTDBG_MAP_ALLOC
#include <stdlib.h>
#include <crtdbg.h>
#endif

#include <windows.h>
#include <config.h>
#include <stdarg.h>
#include <stdio.h>
#include <io.h>
#include <fcntl.h>
#include <stdint.h>

#include "libwdi.h"
#include "logging.h"

static HANDLE logger_rd_handle = INVALID_HANDLE_VALUE;
static HANDLE logger_wr_handle = INVALID_HANDLE_VALUE;
// Handle and Message for the destination Window when registered
static HWND logger_dest = NULL;
static UINT logger_msg = 0;
// Detect spurious log readouts
static unsigned log_messages_pending = 0;
// Keep track of how many bytes are in the pipe
static DWORD log_messages_pipe_size = 0;
// Global debug level
static int global_log_level = WDI_LOG_LEVEL_INFO;

extern char *windows_error_str(uint32_t retval);

static void write_to_pipe(const char* buffer, DWORD size, enum wdi_log_level level)
{
	DWORD written;

	// Earlier checks should ensure that this is never the case, but if it is
	// drop the message
	if (size > LOGGER_PIPE_SIZE)
		return;

	// Do not allow the pipe to outgrow its allocated size, as this freezes the app
	// See http://msdn.microsoft.com/en-us/library/aa365150.aspx
	// If we are about to overflow, we deplete the pipe queue by issuing a bunch of
	// SendMessage, which forces the messages to be processed (as opposed to PostMessage)
	if (((log_messages_pipe_size + size) > LOGGER_PIPE_SIZE) && (log_messages_pending > 0)) {
		while (log_messages_pending > 0)
			SendMessage(logger_dest, logger_msg, (WPARAM)level, 0);
		// Just in case
		log_messages_pipe_size = 0;
	}
	if (WriteFile(logger_wr_handle, buffer, size, &written, NULL))
		log_messages_pipe_size += written;
	log_messages_pending++;
	PostMessage(logger_dest, logger_msg, (WPARAM)level, 0);
}

static void pipe_wdi_log_v(enum wdi_log_level level,
	const char *function, const char *format, va_list args)
{
	char buffer[LOGBUF_SIZE];
	int size1, size2;
	BOOL truncated = FALSE;
	const char* prefix;
	const char* truncation_notice = "TRUNCATION detected for above line - Please "
		"send this log excerpt to the libwdi developers so we can fix it.";

	if (logger_wr_handle == INVALID_HANDLE_VALUE)
		return;

#ifndef ENABLE_DEBUG_LOGGING
	if (level < global_log_level)
		return;
#endif

	switch (level) {
	case WDI_LOG_LEVEL_DEBUG:
		prefix = "debug";
		break;
	case WDI_LOG_LEVEL_INFO:
		prefix = "info";
		break;
	case WDI_LOG_LEVEL_WARNING:
		prefix = "warning";
		break;
	case WDI_LOG_LEVEL_ERROR:
		prefix = "error";
		break;
	default:
		prefix = "unknown";
		break;
	}

	size1 = safe_snprintf(buffer, LOGBUF_SIZE, "libwdi:%s [%s] ", prefix, function);
	size2 = 0;
	if (size1 < 0) {
		buffer[LOGBUF_SIZE-1] = 0;
		size1 = LOGBUF_SIZE-1;
		truncated = TRUE;
	} else {
		size2 = safe_vsnprintf(buffer+size1, LOGBUF_SIZE-size1, format, args);
		if (size2 < 0) {
			buffer[LOGBUF_SIZE-1] = 0;
			size2 = LOGBUF_SIZE-1-size1;
			truncated = TRUE;
		}
	}

	write_to_pipe(buffer, size1+size2+1, level);
	if (truncated)
		write_to_pipe(truncation_notice, strlen(truncation_notice)+1, level);
}

static void console_wdi_log_v(enum wdi_log_level level,
	const char *function, const char *format, va_list args)
{
	FILE *stream;
	const char *prefix;

	stream = stdout;

#ifndef ENABLE_DEBUG_LOGGING
	if (level < global_log_level)
		return;
#endif

	switch (level) {
	case WDI_LOG_LEVEL_DEBUG:
		stream = stderr;
		prefix = "debug";
		break;
	case WDI_LOG_LEVEL_INFO:
		prefix = "info";
		break;
	case WDI_LOG_LEVEL_WARNING:
		stream = stderr;
		prefix = "warning";
		break;
	case WDI_LOG_LEVEL_ERROR:
		stream = stderr;
		prefix = "error";
		break;
	default:
		stream = stderr;
		prefix = "unknown";
		break;
	}

	fprintf(stream, "libwdi:%s [%s] ", prefix, function);

	vfprintf(stream, format, args);

	fprintf(stream, "\n");

}

void wdi_log(enum wdi_log_level level,
	const char *function, const char *format, ...)
{
	va_list args;

	va_start (args, format);
	if (logger_dest != NULL) {
		pipe_wdi_log_v(level, function, format, args);
	} else {
		console_wdi_log_v(level, function, format, args);
	}
	va_end (args);
}

// Create a synchronous pipe for messaging
static int create_logger(DWORD buffsize)
{
	if (buffsize == 0) {
		buffsize = LOGGER_PIPE_SIZE;
	}

	if (logger_wr_handle != INVALID_HANDLE_VALUE) {
		// We (supposedly) don't have logging, so try to reach a stderr
		fprintf(stderr, "trying to recreate logger pipe\n");
		return WDI_ERROR_EXISTS;
	}

	// Read end of the pipe
	logger_rd_handle = CreateNamedPipeA(LOGGER_PIPE_NAME, PIPE_ACCESS_INBOUND,
		PIPE_TYPE_MESSAGE|PIPE_READMODE_MESSAGE, 1, buffsize, buffsize, 0, NULL);
	if (logger_rd_handle == INVALID_HANDLE_VALUE) {
		fprintf(stderr, "could not create logger pipe for reading: %s\n", windows_error_str(0));
		return WDI_ERROR_RESOURCE;
	}

	// Write end of the pipe
	logger_wr_handle = CreateFileA(LOGGER_PIPE_NAME, GENERIC_WRITE, 0, NULL, OPEN_EXISTING,
		FILE_ATTRIBUTE_NORMAL, NULL);
	if (logger_wr_handle == INVALID_HANDLE_VALUE) {
		fprintf(stderr, "could not create logger pipe for writing: %s\n", windows_error_str(0));
		CloseHandle(logger_rd_handle);
		logger_rd_handle = INVALID_HANDLE_VALUE;
		return WDI_ERROR_RESOURCE;
	}

	log_messages_pending = 0;

	return WDI_SUCCESS;
}

// Destroy the logging pipe
static void destroy_logger(void)
{
	if (logger_wr_handle != INVALID_HANDLE_VALUE) {
		CloseHandle(logger_wr_handle);
		logger_wr_handle = INVALID_HANDLE_VALUE;
	}
	if (logger_rd_handle != INVALID_HANDLE_VALUE) {
		CloseHandle(logger_rd_handle);
		logger_rd_handle = INVALID_HANDLE_VALUE;
	}
}

/*
 * Register a Window as destination for logging message
 * This Window will be notified with a message event and should call
 * wdi_read_logger() to retrieve the message data
 */
int LIBWDI_API wdi_register_logger(HWND hWnd, UINT message, DWORD buffsize)
{
	int r;

	MUTEX_START;

	if (logger_dest != NULL) {
		MUTEX_RETURN(WDI_ERROR_EXISTS);
	}

	r = create_logger(buffsize);
	if (r == WDI_SUCCESS) {
		logger_dest = hWnd;
		logger_msg = message;
	}

	MUTEX_RETURN(r);
}

/*
 * Unregister a Window as destination for logging message
 */
int LIBWDI_API wdi_unregister_logger(HWND hWnd)
{
	MUTEX_START;

	if (logger_dest == NULL) {
		MUTEX_RETURN(WDI_SUCCESS);
	}

	if (logger_dest != hWnd) {
		MUTEX_RETURN(WDI_ERROR_INVALID_PARAM);
	}

	destroy_logger();
	logger_dest = NULL;
	logger_msg = 0;

	MUTEX_RETURN(WDI_SUCCESS);
}

/*
 * Read a log message
 */
int LIBWDI_API wdi_read_logger(char* buffer, DWORD buffer_size, DWORD* message_size)
{
	int size;
	DWORD r;

	MUTEX_START;

	if ( (logger_rd_handle == INVALID_HANDLE_VALUE) && (create_logger(0) != WDI_SUCCESS) ) {
		*message_size = 0;
		MUTEX_RETURN(WDI_ERROR_NOT_FOUND);
	}

	if (log_messages_pending == 0) {
		if (log_messages_pipe_size == 0) {
			buffer[0] = 0;
			*message_size = 0;
			MUTEX_RETURN(WDI_SUCCESS);
		}
		size = safe_snprintf(buffer, buffer_size, "ERROR: log buffer is empty");
		if (size < 0) {
			buffer[buffer_size-1] = 0;
			MUTEX_RETURN(buffer_size);
		}
		*message_size = (DWORD)size;
		MUTEX_RETURN(WDI_SUCCESS);
	}
	log_messages_pending--;

	if (ReadFile(logger_rd_handle, (void*)buffer, buffer_size, message_size, NULL)) {
		log_messages_pipe_size -= *message_size;
		MUTEX_RETURN(WDI_SUCCESS);
	}

	log_messages_pipe_size -= *message_size;
	*message_size = 0;
	r = GetLastError();
	if ((r == ERROR_INSUFFICIENT_BUFFER) || (r == ERROR_MORE_DATA)) {
		MUTEX_RETURN(WDI_ERROR_OVERFLOW);
	}
	MUTEX_RETURN(WDI_ERROR_IO);
}

/*
 * Set the global log level. Only works if ENABLE_DEBUG_LOGGING is not set
 */
int LIBWDI_API wdi_set_log_level(int level)
{
#if defined(ENABLE_DEBUG_LOGGING)
	return WDI_ERROR_NOT_SUPPORTED;
#endif
	global_log_level = level;
	return WDI_SUCCESS;
}
