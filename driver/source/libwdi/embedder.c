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

/*
 * This tool is meant to be call as a pre-built event when compiling the
 * installer library, to produce a .h that embeds all the necessary driver
 * binary resources.
 *
 * This is required work around the many limitations of resource files, as
 * well as the impossibility to force the MS linker to embed resources always
 * with a static library (unless the library is split into .res + .lib)
 */

#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <inttypes.h>
#include <time.h>
#include <sys/types.h>
#include <sys/stat.h>
#if defined(_WIN32)
#include <windows.h>
#else
#include <string.h>
#include <dirent.h>
#include <unistd.h>
#endif

#include <config.h>
#include "embedder.h"
#include "embedder_files.h"

#define safe_free(p) do {if (p != NULL) {free(p); p = NULL;}} while(0)
#define perr(...) fprintf(stderr, "embedder : error: " __VA_ARGS__)

const int nb_embeddables_fixed = sizeof(embeddable_fixed)/sizeof(struct emb);
int nb_embeddables;
struct emb* embeddable = embeddable_fixed;

#ifndef MAX_PATH
#ifdef PATH_MAX
#define MAX_PATH PATH_MAX
#else
#define MAX_PATH 260
#endif
#endif

#if defined(USER_DIR)
char initial_dir[MAX_PATH];
#endif

#if defined(_WIN32)
#define NATIVE_STAT				_stat
#define NATIVE_STRDUP			_strdup
#define NATIVE_UNLINK			_unlink
#define NATIVE_SEPARATOR		'\\'
#define NON_NATIVE_SEPARATOR	'/'
#else
#define NATIVE_STAT				stat
#define NATIVE_STRDUP			strdup
#define NATIVE_UNLINK			unlink
#define NATIVE_SEPARATOR		'/'
#define NON_NATIVE_SEPARATOR	'\\'
#endif

void dump_buffer_hex(FILE* fd, unsigned char *buffer, size_t size)
{
	size_t i;

	// Make sure we output something even if the original file is empty
	if (size == 0) {
		fprintf(fd, "0x00");
	}

	for (i=0; i<size; i++) {
		if (!(i%0x10))
			fprintf(fd, "\n\t");
		fprintf(fd, "0x%02X,", buffer[i]);
	}
	fprintf(fd, "\n");
}

void handle_separators(char* path)
{
	size_t i;
	if (path == NULL) return;
	for (i=0; i<strlen(path); i++) {
		if (path[i] == NON_NATIVE_SEPARATOR) {
			path[i] = NATIVE_SEPARATOR;
		}
	}
}

// These 2 functions split a path into basename and dirname
// You must call basename_free to release the resources once done
// Note that basename_split also normalizes the path separators
static char* _path_copy = NULL;
int basename_split(char* path, char** dn, char** bn)
{
	char* basename_pos;
	if ((path == NULL) || (dn == NULL) || (bn == NULL)) return 1;
	safe_free(_path_copy);
	_path_copy = malloc(strlen(path)+1);
	if (_path_copy == NULL) return 1;
	memcpy(_path_copy, path, strlen(path)+1);
	handle_separators(_path_copy);
	basename_pos = strrchr(_path_copy, NATIVE_SEPARATOR);
	if (basename_pos == NULL) {
		*dn = ".";
		*bn = _path_copy;
	} else {
		basename_pos[0] = 0;
		*dn = _path_copy;
		*bn = &basename_pos[1];
	}
	return 0;
}

void basename_free(char* path)
{
	// Ideally, we'd maintain an array associating path with the alloc'd copy
	safe_free(_path_copy);
}

// returns 0 on success, non zero on error
int get_full_path(char* src, char* dst, size_t dst_size)
{
#if defined(_WIN32)
	DWORD r;
	char* src_copy = NULL;
#else
	char *dn, *bn;
#endif
	if ((src == NULL) || (dst == NULL) || (dst_size == 0)) {
		return 1;
	}
#if defined(_WIN32)
	if ((src_copy = malloc(strlen(src) + 1)) == NULL) return 1;
	memcpy(src_copy, src, strlen(src) + 1);
	handle_separators(src_copy);
	r = GetFullPathNameA(src_copy, (DWORD)dst_size, dst, NULL);
	safe_free(src_copy);
	if ((r != 0) || (r <= dst_size)) {
		return 0;
	}
#else
	if ( (basename_split(src, &dn, &bn) == 0)
	  && (realpath(dn, dst) != NULL)
	  && (strlen(dst) + strlen(bn) + 2 < dst_size) ) {
		strcat(dst, "/");
		strcat(dst, bn);
		basename_free(src);
		return 0;
	}
	basename_free(src);
#endif
	perr("Could not get full path for '%s'.\n", src);
	return 1;
}

#if defined(USER_DIR)
// Modified from http://www.zemris.fer.hr/predmeti/os1/misc/Unix2Win.htm
void scan_dir(char *dirname, int countfiles)
{
	char dir[MAX_PATH+1];
	char subdir[MAX_PATH+1];
#if defined(_WIN32)
	char entry[MAX_PATH];
	wchar_t wdir[MAX_PATH+1];
	HANDLE hList;
	WIN32_FIND_DATAW FileData;
#else
	char* entry;
	int r;
	DIR *dp;
	char cwd[MAX_PATH];
	struct dirent* dir_entry;
	struct stat stat_info;
#endif

	// Get the proper directory path
	if ( (strlen(initial_dir) + strlen(dirname) + 4) > sizeof(dir) ) {
		perr("Path overflow.\n");
		return;
	}
	sprintf(dir, "%s%c%s", initial_dir, NATIVE_SEPARATOR, dirname);

	// Get the first file
#if defined(_WIN32)
	strcat(dir, "\\*");
	MultiByteToWideChar(CP_UTF8, 0, dir, -1, wdir, MAX_PATH);
	hList = FindFirstFileW(wdir, &FileData);
	if (hList == INVALID_HANDLE_VALUE) return;
#else
	dp = opendir(dir);
	if (dp == NULL) return;
	dir_entry = readdir(dp);
	if (dir_entry == NULL) return;
#endif

	// Traverse through the directory structure
	do {
		// Check if the object is a directory or not
#if defined(_WIN32)
		WideCharToMultiByte(CP_UTF8, 0, FileData.cFileName, -1, entry, MAX_PATH, NULL, NULL);
		if (FileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
#else
		entry = dir_entry->d_name;
		getcwd(cwd, sizeof(cwd));
		chdir(dir);
		r = NATIVE_STAT(entry, &stat_info);
		chdir(cwd);
		if (r != 0) {
			continue;
		}
		if (S_ISDIR(stat_info.st_mode)) {
#endif
			if ( (strcmp(entry, ".") != 0)
			  && (strcmp(entry, "..") != 0)) {
				// Get the full path for sub directory
				if ( (strlen(dirname) + strlen(entry) + 2) > sizeof(subdir) ) {
					perr("Path overflow.\n");
					return;
				}
				sprintf(subdir, "%s%c%s", dirname, NATIVE_SEPARATOR, entry);
				scan_dir(subdir, countfiles);
			}
		} else {
			if (!countfiles) {
				if ( (embeddable[nb_embeddables].file_name =
					  malloc(strlen(initial_dir) + strlen(dirname) +
					  strlen(entry) + 2) ) == NULL) {
					return;
				}
				if ( (embeddable[nb_embeddables].extraction_subdir =
					  malloc(strlen(dirname)) ) == NULL) {
					return;
				}
				sprintf(embeddable[nb_embeddables].file_name,
					"%s%s%c%s", initial_dir, dirname, NATIVE_SEPARATOR, entry);
				if (dirname[0] == NATIVE_SEPARATOR) {
					sprintf(embeddable[nb_embeddables].extraction_subdir,
						"%s", dirname+1);
				} else {
					safe_free(embeddable[nb_embeddables].extraction_subdir);
					embeddable[nb_embeddables].extraction_subdir = NATIVE_STRDUP(".");
				}
			}
			nb_embeddables++;
		}
	}
#if defined(_WIN32)
	while ( FindNextFileW(hList, &FileData) || (GetLastError() != ERROR_NO_MORE_FILES) );
	FindClose(hList);
#else
	while ((dir_entry = readdir(dp)) != NULL);
	closedir(dp);
#endif
}

void add_user_files(void) {
	int i;

	get_full_path(USER_DIR, initial_dir, sizeof(initial_dir));
	// Dry run to count additional files
	scan_dir("", -1);
	if (nb_embeddables == nb_embeddables_fixed) {
		perr("No user embeddable files found.\nNote that the USER_DIR path must be provided in Windows format\n" \
			"(eg: 'C:\\signed-driver'), if compiling from a Windows platform.\n");
		return;
	}

	// Extend the array to add the user files
	embeddable = calloc(nb_embeddables, sizeof(struct emb));
	if (embeddable == NULL) {
		perr("Could not include user embeddable files.\n");
		return;
	}
	// Copy the fixed part of our table into our new array
	for (i=0; i<nb_embeddables_fixed; i++) {
		embeddable[i].reuse_last = 0;
		embeddable[i].file_name = embeddable_fixed[i].file_name;
		embeddable[i].extraction_subdir = embeddable_fixed[i].extraction_subdir;
	}
	nb_embeddables = nb_embeddables_fixed;

	// Fill in the array
	scan_dir("", 0);
}
#endif

int
#ifdef DDKBUILD
__cdecl
#endif
main (int argc, char *argv[])
{
	int ret = 1, i, j, rebuild;
	size_t size;
	char* file_name = NULL;
	char* junk;
	size_t* file_size = NULL;
	int64_t* file_time = NULL;
	FILE *fd, *header_fd;
	time_t header_time;
	struct NATIVE_STAT stbuf;
	struct tm* ltm;
	char internal_name[] = "file_###";
	unsigned char* buffer = NULL;
	unsigned char last;
	char fullpath[MAX_PATH];
#if defined(_WIN32)
	wchar_t wfullpath[MAX_PATH];
#endif

	// Disable stdout bufferring
	setvbuf(stdout, NULL, _IONBF, 0);

	if (argc != 2) {
		perr("You must supply a header name.\n");
		return 1;
	}

	nb_embeddables = nb_embeddables_fixed;
#if defined(USER_DIR)
	add_user_files();
#endif
	// Check if any of the embedded files have changed
	rebuild = 0;
	if (NATIVE_STAT(argv[1], &stbuf) == 0) {
		header_time = stbuf.st_mtime;	// make sure to use modification time!
		for (i=0; i<nb_embeddables; i++) {
			if (embeddable[i].reuse_last) continue;
			if (get_full_path(embeddable[i].file_name, fullpath, MAX_PATH)) {
				perr("Unable to get full path for '%s'.\n", embeddable[i].file_name);
				goto out1;
			}
			if (NATIVE_STAT(fullpath, &stbuf) != 0) {
				printf("unable to stat '%s' - assuming rebuild needed\n", fullpath);
				rebuild = 1;
				break;
			}
			if (stbuf.st_mtime > header_time) {
				printf("detected change for '%s'\n", fullpath);
				rebuild = 1;
				break;
			}
		}
		if (!rebuild) {
			printf("  resources haven't changed - skipping step\n");
			ret = 0; goto out1;
		}
	}

	size = sizeof(size_t)*nb_embeddables;
	file_size = malloc(size);
	if (file_size == NULL) goto out1;
	size = sizeof(int64_t)*nb_embeddables;
	file_time = malloc(size);
	if (file_time == NULL) goto out1;

	header_fd = fopen(argv[1], "w");
	if (header_fd == NULL) {
		perr("Could not create file '%s'.\n", argv[1]);
		goto out1;
	}
	fprintf(header_fd, "#pragma once\n");

	for (i=0; i<nb_embeddables; i++) {
		if (embeddable[i].reuse_last) {
			continue;
		}
		if (get_full_path(embeddable[i].file_name, fullpath, MAX_PATH)) {
			perr("Could not get full path for '%s'.\n", embeddable[i].file_name);
			goto out2;
		}
#if defined(_WIN32)
		MultiByteToWideChar(CP_UTF8, 0, fullpath, -1, wfullpath, MAX_PATH);
		wprintf(L"  EMBED  %s ", wfullpath);
		fd = _wfopen(wfullpath, L"rb");
#else
		printf("  EMBED  %s ", fullpath);
		fd = fopen(fullpath, "rb");
#endif
		if (fd == NULL) {
			perr("Could not open file '%s'.\n", fullpath);
			goto out2;
		}

		// Read the creation date
		memset(&stbuf, 0, sizeof(stbuf));
		if ( (NATIVE_STAT(fullpath, &stbuf) == 0) && ((ltm = localtime(&stbuf.st_ctime)) != NULL) ) {
			printf("(%04d.%02d.%02d)\n", ltm->tm_year+1900, ltm->tm_mon+1, ltm->tm_mday);
		} else {
			printf("\n");
		}
		file_time[i] = (int64_t)stbuf.st_ctime;
		file_size[i] = (size_t)stbuf.st_size;

		buffer = (unsigned char*) malloc(file_size[i]);
		if (buffer == NULL) {
			perr("Could not allocate buffer.\n");
			goto out3;
		}

		if (fread(buffer, 1, file_size[i], fd) != file_size[i]) {
			perr("Could not read file '%s'.\n", fullpath);
			goto out4;
		}
		fclose(fd);

		sprintf(internal_name, "file_%03X", (unsigned char)i);
		fprintf(header_fd, "const unsigned char %s[] = {", internal_name);
		dump_buffer_hex(header_fd, buffer, file_size[i]);
		fprintf(header_fd, "};\n\n");
		safe_free(buffer);
	}
	fprintf(header_fd, "struct res {\n" \
		"\tchar* subdir;\n" \
		"\tchar* name;\n" \
		"\tsize_t size;\n" \
		"\tint64_t creation_time;\n" \
		"\tconst unsigned char* data;\n" \
		"};\n\n");

	fprintf(header_fd, "const struct res resource[] = {\n");
	for (last=0,i=0; i<nb_embeddables; i++) {
		if (!embeddable[i].reuse_last) {
			last = (unsigned char)i;
		}
		sprintf(internal_name, "file_%03X", last);
		fprintf(header_fd, "\t{ \"");
		// Backslashes need to be escaped
		for (j=0; j<(int)strlen(embeddable[i].extraction_subdir); j++) {
			if ( (embeddable[i].extraction_subdir[j] == NATIVE_SEPARATOR)
			  || (embeddable[i].extraction_subdir[j] == NON_NATIVE_SEPARATOR) ) {
				fputc('\\', header_fd);
				fputc('\\', header_fd);
			} else {
				fputc(embeddable[i].extraction_subdir[j], header_fd);
			}
		}
		basename_split(embeddable[i].file_name, &junk, &file_name);
		fprintf(header_fd, "\", \"%s\", %d, INT64_C(%"PRId64"), %s },\n",
			file_name, (int)file_size[last], file_time[last], internal_name);
		basename_free(embeddable[i].file_name);
	}
	fprintf(header_fd, "};\n");
	fprintf(header_fd, "\nconst int nb_resources = sizeof(resource)/sizeof(resource[0]);\n");

	fclose(header_fd);
	ret = 0; goto out1;

out4:
	safe_free(buffer);
out3:
	fclose(fd);
out2:
	fclose(header_fd);
	// Must delete a failed file so that Make can relaunch its build
	// coverity[tainted_string]
	NATIVE_UNLINK(argv[1]);
out1:
#if defined(USER_DIR)
	for (i=nb_embeddables_fixed; i<nb_embeddables; i++) {
		safe_free(embeddable[i].file_name);
		safe_free(embeddable[i].extraction_subdir);
	}
	if (embeddable != embeddable_fixed) {
		safe_free(embeddable);
	}
#endif
	safe_free(file_size);
	safe_free(file_time);
	return ret;
}
