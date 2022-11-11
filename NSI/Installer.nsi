!include "LogicLib.nsh"
!include "Sections.nsh"
!include "FileFunc.nsh"

; Display Version
!system '..\hakchi_gui\bin\Debug\net461\hakchi.exe --versionFormat "!define DisplayVersion {0}" --versionFile version.nsh'
!include ".\version.nsh"
!system 'del version.nsh'

; Create zip files
!system '..\Zipper\bin\Release\Zipper.exe ..\hakchi_gui\bin\Debug\net461 ..\hakchi_gui\bin\hakchi2-ce-${DisplayVersion}-portable.zip'

; The icon of the installer
Icon "..\hakchi_gui\icon_app.ico"

; The file to write
OutFile "..\hakchi_gui\bin\hakchi2-ce-${DisplayVersion}-installer.exe"

; The default installation directory
Var defaultInstDir
Var extractDir
Var mutex
Var launchExe
var launchArgs

; The name of the installer
Name "Hakchi2 CE ${DisplayVersion}"

; Registry key to check for directory (so if you install again, it will
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Team Shinkansen\Hakchi2 CE" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

; Show details
ShowInstDetails show

; Plugins
!addplugindir .\Plugins

; Installer compression
SetCompressor /FINAL /SOLID lzma

;--------------------------------

; Pages

Page components componentsPre
Page directory dirPre
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; Sections
Section "" section_mutex
  ${GetOptions} $CMDLINE "-MUTEX=" $mutex
  ${If} $mutex != ""
    DetailPrint "Waiting for Hakchi2 CE to exit"
    mutexCheck:
    System::Call 'kernel32::OpenMutex(i 0x100000, b 0, t "$mutex") i .R0'
    IntCmp $R0 0 notRunning
      System::Call 'kernel32::CloseHandle(i $R0)'
      Sleep 1000
      Goto mutexCheck
    ${EndIf}
    notRunning:
SectionEnd

Section "Hakchi2 CE ${DisplayVersion} (required)" section_main
  SectionIn RO
  SetOutPath $INSTDIR
  File /r "..\hakchi_gui\bin\Debug\net461\*"
  AccessControl::GrantOnFile "$INSTDIR\" "(BU)" "GenericRead + GenericWrite"
SectionEnd

Section /o "Portable Install" section_portable
SectionEnd

Section "" section_install
  SetOutPath $INSTDIR

  ; Create nonportable.flag
  FileOpen $9 "nonportable.flag" w
  FileClose $9

  ; Write the installation path into the registry
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "InstallLocation" "$INSTDIR"

  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "DisplayName" "Hakchi2 CE"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "DisplayVersion" "${DisplayVersion}"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "Publisher" "Team Shinkansen"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "URLInfoAbout" "https://github.com/TeamShinkansen/hakchi2"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "HelpLink" "https://github.com/TeamShinkansen/hakchi2"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "URLUpdateInfo" "https://github.com/TeamShinkansen/hakchi2/releases"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "DisplayIcon" '"$INSTDIR\hakchi.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
SectionEnd

Section "Start Menu Shortcuts" section_startmenu
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE"
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Hakchi2 CE.lnk" "$INSTDIR\hakchi.exe" "/nonportable" "$INSTDIR\hakchi.exe" 0
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Hakchi2 CE (Debug).lnk" "$INSTDIR\hakchi.exe" "/nonportable /debug" "$INSTDIR\hakchi.exe" 0
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
SectionEnd

Section /o "Desktop Shortcut" section_desktop
  SetShellVarContext all
  CreateShortcut "$DESKTOP\Hakchi2 CE.lnk" "$INSTDIR\hakchi.exe" "/nonportable" "$INSTDIR\hakchi.exe" 0
SectionEnd

Section "" section_launch
  ${GetOptions} $CMDLINE "-LAUNCH=" $launchExe
  ${GetOptions} $CMDLINE "-LAUNCH_ARGS=" $launchArgs

  ${If} $launchExe != ""
    SetAutoClose true
    SetOutPath "$INSTDIR"
    Exec '"$INSTDIR\$launchExe" $launchArgs'
  ${EndIf}
SectionEnd

Section "Uninstall"
  SetShellVarContext all

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE"

  ; Remove files and directories used
  Delete "$DESKTOP\Hakchi2 CE.lnk"
  RMDir /r "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE"
  RMDir "$SMPROGRAMS\Team Shinkansen"

  !include "debug-uninstall.nsh"

  Delete "$INSTDIR\nonportable.flag"
  Delete "$INSTDIR\uninstall.exe"
  RMDir "$INSTDIR"
SectionEnd
;--------------------------------

Function .onInit
  StrCpy $defaultInstDir "$PROGRAMFILES\Team Shinkansen\Hakchi2 CE"
  StrCpy $InstDir $defaultInstDir
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
  SectionSetFlags ${section_main} $0
FunctionEnd

Function .onSelChange
  ${If} ${SectionIsSelected} ${section_portable}
    StrCpy $InstDir "$EXEDIR\hakchi2-ce-${DisplayVersion}"

    !insertmacro UnselectSection ${section_install}
    !insertmacro UnselectSection ${section_startmenu}
    !insertmacro UnselectSection ${section_desktop}
    SectionSetFlags ${section_startmenu} ${SF_RO}
    SectionSetFlags ${section_desktop} ${SF_RO}
  ${Else}
    StrCpy $InstDir $defaultInstDir

    SectionGetFlags ${section_startmenu} $0
    IntOp $0 $0 & ${SF_SELECTED}
    SectionSetFlags ${section_startmenu} $0

    SectionGetFlags ${section_desktop} $0
    IntOp $0 $0 & ${SF_SELECTED}
    SectionSetFlags ${section_desktop} $0

    !insertmacro SelectSection ${section_install}
  ${EndIf}

FunctionEnd

Function componentsPre
  ${GetOptions} $CMDLINE "-EXTRACT=" $extractDir
  ${If} $extractDir != ""
    StrCpy $InstDir "$extractDir"
    SectionSetFlags ${section_portable} ${SF_SELECTED}
    !insertmacro UnselectSection ${section_install}
    !insertmacro UnselectSection ${section_startmenu}
    !insertmacro UnselectSection ${section_desktop}
    SectionSetFlags ${section_startmenu} ${SF_RO}
    SectionSetFlags ${section_desktop} ${SF_RO}
    Abort
  ${EndIf}
FunctionEnd

Function dirPre
  ${If} $extractDir != ""
    Abort
  ${EndIf}
FunctionEnd
