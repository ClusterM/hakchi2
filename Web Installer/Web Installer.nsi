; The name of the installer
Name "Hakchi2 CE"

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

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Hakchi2 CE (required)"
  SetShellVarContext all
  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR

  ; Create the installation directory.
  CreateDirectory "$INSTDIR"
  
  ; Download update.xml
  inetc::get "https://teamshinkansen.github.io/xml/updates/update.xml" "update.xml"
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
  inetc::get "$3" "hakchi2-ce-release.zip"
  Pop $0
  StrCmp $0 "OK" ExtractZip InstallError

  ExtractZip:
  ZipDLL::extractall "hakchi2-ce-release.zip" "$INSTDIR"
  Delete "hakchi2-ce-release.zip"

  ; Create a debuglog.txt file writable to all users
  FileOpen $9 "debuglog.txt" w
  FileClose $9
  AccessControl::GrantOnFile "$INSTDIR\debuglog.txt" "(BU)" "GenericRead + GenericWrite"

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
    Delete "hakchi2-ce-release.zip"
    RMDir "$INSTDIR"
    Abort

  InstallEnd:
  
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
