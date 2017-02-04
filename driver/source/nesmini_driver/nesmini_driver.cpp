
/*
* Zadic: Automated Driver Installer for USB devices (Console version)
* Copyright (c) 2010-2016 Pete Batard <pete@akeo.ie>
* Copyright (c) 2015 PhracturedBlue <6xtc2h7upj@snkmail.com>
* Copyright (c) 2010 Joseph Marshall <jmarshall@gcdataconcepts.com>
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
* NES Mini driver mod by Alexey 'Cluser' Avdyukhin
* clusterrr@clusterrr.com
*/
/*
* WARNING: if any part of the resulting executable name contains "setup" or "instal(l)"
* it will require UAC elevation on Vista and later, and, when run from an MSYS shell,
* will produce a "sh: Bad file number" message.
* See the paragraph on Automatic Elevation at http://helpware.net/VistaCompat.htm
*/

#include <stdio.h>
#include <string.h>
#include "libwdi.h"

#define FLUSHER while(getchar() != 0x0A)
#define INF_NAME "nesmini.inf"

int __cdecl main(int argc, char *argv[])
{
	int c;
	struct wdi_device_info *device, *list;
	char* path = "usb_driver";
	static struct wdi_options_create_list cl_options = { 0 };
	static struct wdi_options_prepare_driver pd_options = { 0 };

	static int prompt_flag = 1;
	static unsigned char iface = 0;
	static int vid = 0x1F3A;
	static unsigned short pid = 0xEFE8;
	static int verbose_flag = 3;
	static char *desc = NULL;
	static int use_supplied_inf_flag = 0;
	int r = WDI_ERROR_OTHER, option_index = 0;

	cl_options.trim_whitespaces = TRUE;
	cl_options.list_all = 1;
	pd_options.driver_type = WDI_WINUSB;
	pd_options.device_name = "NES Mini";
	pd_options.vendor_name = "Nintendo";
	pd_options.device_guid = "{FF176DC4-7BEE-43BE-9BF2-F9299CB990BD}";

	r = wdi_create_list(&list, &cl_options);
	switch (r) {
	case WDI_SUCCESS:
		break;
	case WDI_ERROR_NO_DEVICE:
		printf("No driverless USB devices were found.\n");
		return 0;
	default:
		printf("wdi_create_list: error %s\n", wdi_strerror(r));
		return 0;
	}

	boolean found = FALSE;
	for (device = list; device != NULL; device = device->next) {
		if (vid == 0 || pid == 0)  printf("Found USB device: \"%s\" (%04X:%04X)\n", device->desc, device->vid, device->pid);
		wdi_set_log_level(verbose_flag);
		// If vid and pid have not been initialized
		// prompt user to install driver for each device
		if (vid == 0 || pid == 0) {
			printf("Do you want to install a driver for this device (y/n)?\n");
			c = (char)getchar();
			FLUSHER;
			if ((c != 'y') && (c != 'Y')) {
				continue;
			}
			// Otherwise a specific vid and pid have been given
			// so drivers will install automatically
		}
		else {
			// Is VID and PID a match for our device
			if ((device->vid != vid) || (device->pid != pid)
				|| (device->mi != iface)) {
				continue;
			}
			else {
				printf("Found USB device: \"%s\" (%04X:%04X)\n", device->desc, device->vid, device->pid);
				found = TRUE;
			}
		}
		// Does the user want to use a supplied .inf
		if (use_supplied_inf_flag == 0) {
			if (wdi_prepare_driver(device, path, INF_NAME, &pd_options) == WDI_SUCCESS) {
				printf("Installing wdi driver with <%s> at <%s>\n", INF_NAME, path);
				wdi_install_driver(device, path, INF_NAME, NULL);
			}
		}
		else {
			printf("Installing wdi driver with <%s> at <%s>\n", INF_NAME, path);
			wdi_install_driver(device, path, INF_NAME, NULL);
		}
	}
	wdi_destroy_list(list);
	if (vid != 0 && pid != 0 && !found)
		printf("%s not found, sorry\n", pd_options.device_name);

	// This is needed when ran in UAC mode
	if (prompt_flag) {
		printf("\nPress Enter to exit this driver installer\n");
		FLUSHER;
	}
	return 0;
}
