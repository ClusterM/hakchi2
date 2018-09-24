; The file to write
OutFile "telnet-install-helper.exe"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

; Show details
ShowInstDetails show

SilentInstall silent

Section "Telnet"
  SectionIn RO
  Exec "dism.exe /online /Enable-Feature /FeatureName:TelnetClient"
SectionEnd
