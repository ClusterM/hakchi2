#!/bin/sh
# Changes the version number for Zadig
# !!!THIS SCRIPT IS FOR INTERNAL DEVELOPER USE ONLY!!!

type -P sed &>/dev/null || { echo "sed command not found. Aborting." >&2; exit 1; }
type -P git &>/dev/null || { echo "git command not found. Aborting." >&2; exit 1; }

if [ ! -n "$1" ]; then
  echo "you must provide a version number (eg. 2.2)"
  exit 1
else
  MAJOR=`echo $1 | sed "s/\(.*\)[.].*/\1/"`
  MINOR=`echo $1 | sed "s/.*[.]\(.*\)/\1/"`
fi
case $MAJOR in *[!0-9]*) 
  echo "$MAJOR is not a number"
  exit 1
esac
case $MINOR in *[!0-9]*) 
  echo "$MINOR is not a number"
  exit 1
esac
echo "changing version to $MAJOR.$MINOR"

cat > cmd.sed <<\_EOF
s/^[ \t]*FILEVERSION[ \t]*.*,.*,\(.*\),\(.*\)/ FILEVERSION @@MAJOR@@,@@MINOR@@,\1,\2/
s/^[ \t]*PRODUCTVERSION[ \t]*.*,.*,\(.*\),\(.*\)/ PRODUCTVERSION @@MAJOR@@,@@MINOR@@,\1,\2/
s/^\([ \t]*\)VALUE[ \t]*"FileVersion",[ \t]*".*\..*\.\(.*\)"/\1VALUE "FileVersion", "@@MAJOR@@.@@MINOR@@.\2"/
s/^\([ \t]*\)VALUE[ \t]*"ProductVersion",[ \t]*".*\..*\.\(.*\)"/\1VALUE "ProductVersion", "@@MAJOR@@.@@MINOR@@.\2"/
s/^\(.*\)"Zadig .*\..*\.\(.*\)"\(.*\)/\1"Zadig @@MAJOR@@.@@MINOR@@.\2"\3/
_EOF

sed -i -e "s/@@MAJOR@@/$MAJOR/g" -e "s/@@MINOR@@/$MINOR/g" cmd.sed
sed -i -f cmd.sed examples/zadig.rc
sed -i 's/$/\r/' examples/zadig.rc
sed -i -f cmd.sed examples/zadig.h
sed -i 's/$/\r/' examples/zadig.h

rm cmd.sed

# Update VID data while we're at it
cd libwdi
./vid_data.sh
cd ..
