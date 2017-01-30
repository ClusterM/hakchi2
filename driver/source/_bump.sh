#!/bin/sh
# This script bumps the version and updates the rc files and git tree accordingly
# !!!THIS SCRIPT IS FOR INTERNAL DEVELOPER USE ONLY!!!

type -P sed &>/dev/null || { echo "sed command not found. Aborting." >&2; exit 1; }
type -P git &>/dev/null || { echo "git command not found. Aborting." >&2; exit 1; }

if [ ! -n "$1" ]; then
  TAG=$(git describe --tags --abbrev=0 2>/dev/null)
  if [ ! -n "$TAG" ]; then
    echo Unable to read tag - aborting.
    exit 1
  fi
else
  TAG=$1
fi
if [ ! ${TAG:0:1} = 'w' ]; then
  echo Tag "$TAG" does not start with 'w' - aborting
  exit 1
fi
TAGVER=${TAG:1}
case $TAGVER in *[!0-9]*) 
  echo "$TAGVER is not a number - aborting"
  exit 1
esac
TAGVER=`expr $TAGVER + 1`
echo Bumping version to w$TAGVER

cat > cmd.sed <<\_EOF
s/^[ \t]*FILEVERSION[ \t]*\(.*\),\(.*\),\(.*\),.*/ FILEVERSION \1,\2,\3,@@TAGVER@@/
s/^[ \t]*PRODUCTVERSION[ \t]*\(.*\),\(.*\),\(.*\),.*/ PRODUCTVERSION \1,\2,\3,@@TAGVER@@/
s/^\([ \t]*\)VALUE[ \t]*"FileVersion",[ \t]*"\(.*\)\..*"/\1VALUE "FileVersion", "\2.@@TAGVER@@"/
s/^\([ \t]*\)VALUE[ \t]*"ProductVersion",[ \t]*"\(.*\)\..*"/\1VALUE "ProductVersion", "\2.@@TAGVER@@"/
s/^\(.*\)adig v\(.*\)\.\(.*\)"\(.*\)/\1adig v\2.@@TAGVER@@"\4/
s/^zadig_version=\(.*\)\..*/zadig_version=\1.@@TAGVER@@/
s/^\(.*\)"Version \(.*\) (Build \(.*\))"\(.*\)/\1"Version \2 (Build @@TAGVER@@)"\4/
_EOF

# First run sed to substitute our variable in the sed command file
sed -e "s/@@TAGVER@@/$TAGVER/g" cmd.sed > cmd.sed~
mv cmd.sed~ cmd.sed

# Run sed to update the .rc files minor version
sed -f cmd.sed libwdi/libwdi.rc > libwdi/libwdi.rc~
mv libwdi/libwdi.rc~ libwdi/libwdi.rc
sed -f cmd.sed examples/zadic.rc > examples/zadic.rc~
mv examples/zadic.rc~ examples/zadic.rc
sed -f cmd.sed examples/zadig.rc > examples/zadig.rc~
mv examples/zadig.rc~ examples/zadig.rc
sed -f cmd.sed examples/zadig.h > examples/zadig.h~
mv examples/zadig.h~ examples/zadig.h
sed -f cmd.sed examples/wdi-simple.rc > examples/wdi-simple.rc~
mv examples/wdi-simple.rc~ examples/wdi-simple.rc
sed -f cmd.sed _bm.sh > _bm.sh~
mv _bm.sh~ _bm.sh

rm cmd.sed

# Update VID data while we're at it
cd libwdi
. vid_data.sh
cd ..

git commit -a -m "[internal] bumped internal version" -e
git tag "w$TAGVER"