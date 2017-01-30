@echo off
rem default builds static library. 
rem you can pass the following arguments (case insensitive):
rem - "DLL" to build a DLL instead of a static library
rem - "no_samples" to build the library only

if Test%BUILD_ALT_DIR%==Test goto usage
rem /M 2 for multiple cores
set BUILD_CMD=build -bcwgZ -M2
set PWD=%~dp0

rem process commandline parameters
set TARGET=LIBRARY
set BUILD_SAMPLES=YES

:more_args
if "%1" == "" goto no_more_args
rem /I for case insensitive
if /I Test%1==TestDLL set TARGET=DYNLINK
if /I Test%1==Testno_samples set BUILD_SAMPLES=NO
rem - shift the arguments and examine %1 again
shift
goto more_args
:no_more_args

rem Set DDK_DIR (=BASEDIR with escaped backslashes)
set DDK_DIR=%BASEDIR:\=\\%
rem Set target platform type
set ORG_BUILD_ALT_DIR=%BUILD_ALT_DIR%
set ORG_BUILDARCH=%_BUILDARCH%
set ORG_PATH=%PATH%
set ORG_BUILD_DEFAULT_TARGETS=%BUILD_DEFAULT_TARGETS%

set ARCH_DIR=%_BUILDARCH%
if /I Test%_BUILDARCH%==Testx86 set ARCH_DIR=i386

if /I Test%_BUILDARCH%==Testamd64 goto x86_64
echo #define NO_BUILD64> libwdi\build64.h
goto main_start
:x86_64
echo #define BUILD64> libwdi\build64.h

:main_start
cd libwdi
set srcPath=obj%BUILD_ALT_DIR%\%cpudir%

del Makefile.hide >NUL 2>&1
if EXIST Makefile ren Makefile Makefile.hide

set 386=1
set AMD64=
set BUILD_DEFAULT_TARGETS=-386
set _AMD64bit=
set _BUILDARCH=x86
set PATH=%BASEDIR%\bin\x86;%BASEDIR%\bin\x86\x86

copy .msvc\embedder_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\i386\embedder.exe . >NUL 2>&1

copy .msvc\installer_x86_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\i386\installer_x86.exe . >NUL 2>&1

set 386=
set AMD64=1
set BUILD_DEFAULT_TARGETS=-amd64
set _AMD64bit=true
set _BUILDARCH=AMD64
set PATH=%BASEDIR%\bin\x86\amd64;%BASEDIR%\bin\x86

copy .msvc\installer_x64_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\amd64\installer_x64.exe . >NUL 2>&1

if /I Test%ORG_BUILDARCH%==TestAMD64 goto restorePath
set 386=1
set AMD64=
set BUILD_DEFAULT_TARGETS=-386
set _AMD64bit=
set _BUILDARCH=x86

:restorePath
set PATH=%ORG_PATH%

echo.
echo Embedding binary resources
embedder.exe embedded.h

rem DLL or static lib selection (must use concatenation)
echo TARGETTYPE=%TARGET% > target
copy target+.msvc\libwdi_sources sources >NUL 2>&1
del target
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\%ARCH_DIR%\libwdi.lib . >NUL 2>&1
copy obj%BUILD_ALT_DIR%\%ARCH_DIR%\libwdi.dll . >NUL 2>&1

if EXIST Makefile.hide ren Makefile.hide Makefile
cd ..
if Test%BUILD_SAMPLES%==TestNO goto done
cd examples\getopt

del Makefile.hide >NUL 2>&1
if EXIST Makefile ren Makefile Makefile.hide
copy .msvc\getopt_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\%ARCH_DIR%\getopt.lib . >NUL 2>&1

if EXIST Makefile.hide ren Makefile.hide Makefile
cd ..

del Makefile.hide >NUL 2>&1
if EXIST Makefile ren Makefile Makefile.hide
rem Work around MS's VC++ and DDK weird icompatibilities with regards to rc files
echo #include ^<windows.h^> > afxres.h
echo #ifndef IDC_STATIC >> afxres.h
echo #define IDC_STATIC -1 >> afxres.h
echo #endif >> afxres.h
copy .msvc\zadic_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\%ARCH_DIR%\zadic.exe . >NUL 2>&1

copy .msvc\zadig_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
del afxres.h
copy obj%BUILD_ALT_DIR%\%ARCH_DIR%\zadig.exe . >NUL 2>&1

copy .msvc\wdi-simple_sources sources >NUL 2>&1
@echo on
%BUILD_CMD%
@echo off
if errorlevel 1 goto builderror
copy obj%BUILD_ALT_DIR%\%ARCH_DIR%\wdi-simple.exe . >NUL 2>&1

if EXIST Makefile.hide ren Makefile.hide Makefile
cd ..

goto done

:builderror
if EXIST Makefile.hide ren Makefile.hide Makefile
if EXIST afxres.h del afxres.h
echo Build failed
goto done

:usage
echo ddk_build must be run in a Windows Driver Kit build environment
pause
goto done

:done
set BUILD_ALT_DIR=%ORG_BUILD_ALT_DIR%
set _BUILDARCH=%ORG_BUILDARCH%
set PATH=%ORG_PATH%
set BUILD_DEFAULT_TARGETS=%ORG_BUILD_DEFAULT_TARGETS%

if Test%DDK_TARGET_OS%==TestWinXP goto nowarn

echo.
echo.
echo WARNING: You do not seem to use the Windows XP DDK build environment.
echo Be mindful that using the Windows Vista or Windows 7 DDK build environments
echo will result in library and applications that do NOT run on Windows XP.
echo.

:nowarn
cd %PWD%
