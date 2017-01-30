#!/bin/sh
# Create a release archive
# !!!THIS SCRIPT IS FOR INTERNAL DEVELOPER USE ONLY!!!

type -P sed &>/dev/null || { echo "sed command not found. Aborting." >&2; exit 1; }
type -P git &>/dev/null || { echo "git command not found. Aborting." >&2; exit 1; }

VERSION=`sed -n -e "s/^AC_INIT(\[libwdi\], \[\(.*\)\], \[.*\], \[.*\], \[.*/\1/p" configure.ac`
echo "Creating libwdi-$VERSION"

BASEDIR=/e/dailies/libwdi
TARGET_DIR=$BASEDIR/libwdi-$VERSION
mkdir $TARGET_DIR
git clean -fdx

(glibtoolize --version) < /dev/null > /dev/null 2>&1 && LIBTOOLIZE=glibtoolize || LIBTOOLIZE=libtoolize
$LIBTOOLIZE --copy --force || exit 1
sed -e s/\\\\\${\$lt_var+set}/set/g ltmain.sh > ltmain.sh~
mv ltmain.sh~ ltmain.sh
aclocal || exit 1
autoheader || exit 1
autoconf || exit 1
automake -a -c || exit 1

git archive master | tar -x -C $TARGET_DIR
cp -r m4 autom4te.cache $TARGET_DIR
rm -f $TARGET_DIR/*.sh
cp aclocal.m4 compile config.guess config.sub config.h.in configure depcomp INSTALL install-sh ltmain.sh Makefile.in missing $TARGET_DIR
cp libwdi/Makefile.in $TARGET_DIR/libwdi/Makefile.in
cp examples/Makefile.in $TARGET_DIR/examples/Makefile.in
tar -C $BASEDIR -cf $BASEDIR/libwdi-$VERSION.tar libwdi-$VERSION
gzip $BASEDIR/libwdi-$VERSION.tar
