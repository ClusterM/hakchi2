/* LIBUSB-WIN32, Generic Windows USB Library
 * Copyright (c) 2010 Travis Robinson <libusbdotnet@gmail.com>
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

#ifndef _TOKENIZER_H
#define _TOKENIZER_H

#include <windows.h>

typedef struct _token_entity_t
{
	const char* match;
	char replace[1024];
}token_entity_t;

long tokenize_string(const char* src,
						 long src_count,
						 char** dst,
						 const token_entity_t* token_entities,
						 const char* tok_prefix,
						 const char* tok_suffix,
						 int recursive);

long tokenize_resource(LPCSTR resource_name,
					 LPCSTR resource_type,
					 char** dst,
					 const token_entity_t* token_entities,
					 const char* tok_prefix,
					 const char* tok_suffix,
					 int recursive);
#endif
