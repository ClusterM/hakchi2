!include "LogicLib.nsh"
!include "Sections.nsh"
!getdllversion "../bin/Release/hakchi.exe" cever_

; The name of the installer
Name "Hakchi2 CE ${cever_1}.${cever_2}.${cever_3}"

; The icon of the installer
Icon "..\icon_app.ico"

; The file to write
OutFile "..\bin\hakchi2-ce-${cever_1}.${cever_2}.${cever_3}-installer.exe"

; The default installation directory
Var defaultInstDir

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

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; Sections

SectionGroup /e "Hakchi2 CE ${cever_1}.${cever_2}.${cever_3} (required)"
  
  Section "Release Build" section_release
    SetOutPath $INSTDIR
    File /r "..\bin\Release\*"
  SectionEnd

  Section /o "Debug Build" section_debug
    SetOutPath $INSTDIR
    File /r "..\bin\Debug\*"
  SectionEnd
SectionGroupEnd

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
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "Publisher" "Team Shinkansen"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "URLInfoAbout" "https://github.com/TeamShinkansen/hakchi2"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "HelpLink" "https://github.com/TeamShinkansen/hakchi2"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "URLUpdateInfo" "https://github.com/TeamShinkansen/hakchi2/releases"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "DisplayIcon" '"$INSTDIR\hakchi.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  AccessControl::GrantOnFile "$INSTDIR\" "(BU)" "GenericRead + GenericWrite"
SectionEnd

Section "Start Menu Shortcuts" section_startmenu
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE"
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Hakchi2 CE.lnk" "$INSTDIR\hakchi.exe" "/nonportable" "$INSTDIR\hakchi.exe" 0
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
SectionEnd

Section /o "Desktop Shortcut" section_desktop
  SetShellVarContext all
  CreateShortcut "$DESKTOP\Hakchi2 CE.lnk" "$INSTDIR\hakchi.exe" "/nonportable" "$INSTDIR\hakchi.exe" 0
SectionEnd

Section "Uninstall"
  SetShellVarContext all

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE"

  ; Remove files and directories used
  Delete "$DESKTOP\Hakchi2 CE.lnk"
  RMDir /r "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE"
  RMDir "$SMPROGRAMS\Team Shinkansen"
  
  IfFileExists "$INSTDIR\hakchi.pdb" uninstall_debug
  
  uninstall_release:
  !include "release-uninstall.nsh"
  Goto uninstall_done
  
  uninstall_debug:
  !include "debug-uninstall.nsh"
  
  uninstall_done:
  Delete "$INSTDIR\nonportable.flag"
  Delete "$INSTDIR\uninstall.exe"
  RMDir "$INSTDIR"
SectionEnd
;--------------------------------

Function .onInit
  StrCpy $defaultInstDir "$PROGRAMFILES\Team Shinkansen\Hakchi2 CE"
  StrCpy $InstDir $defaultInstDir
  StrCpy $1 ${section_release} ; Group 1 - Option 1 is selected by default
  StrCpy $2 ${section_debug} ; Group 1 - Option 1 is selected by default

FunctionEnd

Function .onSelChange
  !insertmacro StartRadioButtons $1
    !insertmacro RadioButton ${section_release}
    !insertmacro RadioButton ${section_debug}
  !insertmacro EndRadioButtons

  ${If} ${SectionIsSelected} ${section_portable}
    StrCpy $InstDir "$EXEDIR\hakchi2-ce-${cever_1}.${cever_2}.${cever_3}"
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