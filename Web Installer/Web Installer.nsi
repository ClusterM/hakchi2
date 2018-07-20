!include "LogicLib.nsh"
!include "Sections.nsh"

; The name of the installer
Name "Hakchi2 CE"

; The icon of the installer
Icon "..\icon_app.ico"

; The file to write
OutFile "Hakchi2 CE Web Installer.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\Team Shinkansen\Hakchi2 CE"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE" "InstallLocation"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

; Show details
ShowInstDetails show

; Plugins
!addplugindir .\Plugins

; Installer compression
SetCompressor /FINAL /SOLID lzma

;--------------------------------

Var DownloadURL

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install

SectionGroup /e "Hakchi2 CE (required"
  Section "Release Build" section_release
  SectionEnd
  Section /o "Debug Build" section_debug
  SectionEnd
SectionGroupEnd

Section
  SetShellVarContext all
  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ; Create the installation directory.
  CreateDirectory "$INSTDIR"
  
  ; Download update.xml
  inetc::get $DownloadURL "update.xml"
  Pop $0
  StrCmp $0 "OK" ParseXML InstallError

  ; Parse the xml
  ParseXML:
  nsisXML::create
  nsisXML::load "update.xml"
  Delete "update.xml"
  nsisXML::select '/item/url'
  IntCmp $2 0 InstallError
  nsisXML::getText

  ; Download the release package
  inetc::get "$3" "hakchi2-ce.zip"
  Pop $0
  StrCmp $0 "OK" ExtractZip InstallError

  ExtractZip:
  ZipDLL::extractall "hakchi2-ce.zip" "$INSTDIR"
  Delete "hakchi2-ce.zip"

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

  Goto InstallEnd

  InstallError:
    Delete "update.xml"
    Delete "hakchi2-ce.zip"
    RMDir "$INSTDIR"
    Abort

  InstallEnd:
  AccessControl::GrantOnFile "$INSTDIR\" "(BU)" "GenericRead + GenericWrite"
  
SectionEnd

Section "Start Menu Shortcuts"
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE"
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Hakchi2 CE.lnk" "$INSTDIR\hakchi.exe" "/nonportable" "$INSTDIR\hakchi.exe" 0
  CreateShortcut "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
SectionEnd

Section "Desktop Shortcut"
  SetShellVarContext all
  CreateShortcut "$DESKTOP\Hakchi2 CE.lnk" "$INSTDIR\hakchi.exe" "/nonportable" "$INSTDIR\hakchi.exe" 0
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  SetShellVarContext all

  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Hakchi2 CE"

  ; Remove files and directories used
  Delete "$DESKTOP\Hakchi2 CE.lnk"
  RMDir /r "$SMPROGRAMS\Team Shinkansen\Hakchi2 CE"
  RMDir "$SMPROGRAMS\Team Shinkansen"
  RMDir /r "$INSTDIR"

SectionEnd

Function .onInit
  StrCpy $DownloadURL "https://teamshinkansen.github.io/xml/updates/update-release.xml"
  StrCpy $1 ${section_release}
  StrCpy $2 ${section_debug}

FunctionEnd

Function .onSelChange
  !insertmacro StartRadioButtons $1
    !insertmacro RadioButton ${section_release}
    !insertmacro RadioButton ${section_debug}
  !insertmacro EndRadioButtons
  
  ${If} ${SectionIsSelected} ${section_release}
    StrCpy $DownloadURL "https://teamshinkansen.github.io/xml/updates/update-release.xml"
  ${EndIf}
  ${If} ${SectionIsSelected} ${section_debug}
    StrCpy $DownloadURL "https://teamshinkansen.github.io/xml/updates/update-debug.xml"
  ${EndIf}

FunctionEnd
