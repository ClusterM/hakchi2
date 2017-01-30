#!/bin/sh
# Changes the version number for the library and non Zadig samples
# !!!THIS SCRIPT IS FOR INTERNAL DEVELOPER USE ONLY!!!

type -P sed &>/dev/null || { echo "sed command not found. Aborting." >&2; exit 1; }
type -P git &>/dev/null || { echo "git command not found. Aborting." >&2; exit 1; }

if [ ! -n "$1" ]; then
  echo "you must provide a version number (eg. 1.0.2)"
  exit 1
else
  MAJOR=`echo $1 | sed "s/\(.*\)[.].*[.].*/\1/"`
  MINOR=`echo $1 | sed "s/.*[.]\(.*\)[.].*/\1/"`
  MICRO=`echo $1 | sed "s/.*[.].*[.]\(.*\)/\1/"`
fi
case $MAJOR in *[!0-9]*) 
  echo "$MAJOR is not a number"
  exit 1
esac
case $MINOR in *[!0-9]*) 
  echo "$MINOR is not a number"
  exit 1
esac
case $MICRO in *[!0-9]*) 
  echo "$MICRO is not a number"
  exit 1
esac
echo "changing version to $MAJOR.$MINOR.$MICRO"
sed -i -e "s/^AC_INIT(\[\([^ ]*\)\], \[[^ ]*\]\(.*\)/AC_INIT([\1], [$MAJOR.$MINOR.$MICRO]\2/" configure.ac

cat > cmd.sed <<\_EOF
s/^[ \t]*FILEVERSION[ \t]*.*,.*,.*,\(.*\)/ FILEVERSION @@MAJOR@@,@@MINOR@@,@@MICRO@@,\1/
s/^[ \t]*PRODUCTVERSION[ \t]*.*,.*,.*,\(.*\)/ PRODUCTVERSION @@MAJOR@@,@@MINOR@@,@@MICRO@@,\1/
s/^\([ \t]*\)VALUE[ \t]*"FileVersion",[ \t]*".*\..*\..*\.\(.*\)"/\1VALUE "FileVersion", "@@MAJOR@@.@@MINOR@@.@@MICRO@@.\2"/
s/^\([ \t]*\)VALUE[ \t]*"ProductVersion",[ \t]*".*\..*\..*\.\(.*\)"/\1VALUE "ProductVersion", "@@MAJOR@@.@@MINOR@@.@@MICRO@@.\2"/
_EOF

# First run sed to substitute our variable in the sed command file
sed -i -e "s/@@MAJOR@@/$MAJOR/g" -e "s/@@MINOR@@/$MINOR/g" -e "s/@@MICRO@@/$MICRO/g" cmd.sed
# Run sed to update the .rc version
sed -i -f cmd.sed libwdi/libwdi.rc
sed -i 's/$/\r/' libwdi/libwdi.rc
sed -i -f cmd.sed examples/zadic.rc
sed -i 's/$/\r/' examples/zadic.rc
sed -i -f cmd.sed examples/wdi-simple.rc
sed -i 's/$/\r/' examples/wdi-simple.rc
rm cmd.sed
