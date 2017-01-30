#!/bin/sh
#
# Bumps the nano version according to the number of commits on this branch
#
# To have git run this script on commit, create a "pre-commit" text file in
# .git/hooks/ with the following content:
# #!/bin/sh
# if [ -x ./_pre-commit.sh ]; then
# 	source ./_pre-commit.sh
# fi

type -P sed &>/dev/null || { echo "sed command not found. Aborting." >&2; exit 1; }
type -P git &>/dev/null || { echo "git command not found. Aborting." >&2; exit 1; }

VER=`git log --oneline | wc -l`
# adjust so that we match the github commit count
TAGVER=`expr $VER + 1`
# there may be a better way to prevent improper nano on amend. For now the detection
# of a .amend file in the current directory will do
if [ -f ./.amend ]; then
	TAGVER=`expr $TAGVER - 1`
	rm ./.amend;
fi
echo "setting nano to $TAGVER"

cat > cmd.sed <<\_EOF
s/^[ \t]*FILEVERSION[ \t]*\(.*\),\(.*\),\(.*\),.*/ FILEVERSION \1,\2,\3,@@TAGVER@@/
s/^[ \t]*PRODUCTVERSION[ \t]*\(.*\),\(.*\),\(.*\),.*/ PRODUCTVERSION \1,\2,\3,@@TAGVER@@/
s/^\([ \t]*\)VALUE[ \t]*"FileVersion",[ \t]*"\(.*\)\..*"/\1VALUE "FileVersion", "\2.@@TAGVER@@"/
s/^\([ \t]*\)VALUE[ \t]*"ProductVersion",[ \t]*"\(.*\)\..*"/\1VALUE "ProductVersion", "\2.@@TAGVER@@"/
s/^\(.*\)"Zadig \(.*\)\..*"\(.*\)/\1"Zadig \2.@@TAGVER@@"\3/
_EOF

# First run sed to substitute our variable in the sed command file
sed -i -e "s/@@TAGVER@@/$TAGVER/g" cmd.sed

# Run sed to update the nano version, and add the modified files
sed -i -f cmd.sed libwdi/libwdi.rc
sed -i 's/$/\r/' libwdi/libwdi.rc
sed -i -f cmd.sed examples/zadic.rc
sed -i 's/$/\r/' examples/zadic.rc
sed -i -f cmd.sed examples/zadig.rc
sed -i 's/$/\r/' examples/zadig.rc
sed -i -f cmd.sed examples/zadig.h
sed -i 's/$/\r/' examples/zadig.h
sed -i -f cmd.sed examples/wdi-simple.rc
sed -i 's/$/\r/' examples/wdi-simple.rc
git add libwdi/libwdi.rc examples/zadic.rc examples/zadig.rc examples/zadig.h examples/wdi-simple.rc
rm cmd.sed
