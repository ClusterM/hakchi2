#!/bin/sh

# rebuilds the Windows def file by exporting all LIBDWI API calls
create_def()
{
  echo "rebuidling libwdi.def file"
  echo 'LIBRARY "libwdi.dll"' > libwdi/libwdi.def
  echo "EXPORTS" >> libwdi/libwdi.def
  sed -n -e "s/.*LIBWDI_API.*\([[:blank:]]\)\(wdi.*\)(.*/  \2/p" libwdi/libwdi.c libwdi/vid_data.c libwdi/logging.c >> libwdi/libwdi.def
  # We need to manually define a whole set of DLL aliases if we want the MS
  # DLLs to be usable with dynamically linked MinGW executables. This is
  # because it is not possible to avoid the @ decoration from import WINAPI
  # calls in MinGW generated objects, and .def based MS generated DLLs don't
  # have such a decoration => linking to MS DLL will fail without aliases.
  # Currently, the maximum size is 16 and all sizes are multiples of 4
  for i in 4 8 12 16
  do
    sed -n -e "s/.*LIBWDI_API.*\([[:blank:]]\)\(wdi.*\)(.*/  \2@$i = \2/p" libwdi/libwdi.c libwdi/vid_data.c libwdi/logging.c >> libwdi/libwdi.def
  done
  type -P unix2dos &>/dev/null && unix2dos -q libwdi/libwdi.def
}

set -e

./bootstrap.sh
./configure --enable-toggable-debug --enable-examples-build --disable-debug --with-ddkdir="C:/Program Files (x86)/Windows Kits/10" --with-wdfver=1011 --with-libusb0="D:/libusb-win32" --with-libusbk="D:/libusbK/bin" $*
# rebuild .def, if sed is available
type -P sed &>/dev/null && create_def
