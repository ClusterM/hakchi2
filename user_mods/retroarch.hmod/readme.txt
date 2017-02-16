=== RetroArch ===

This module adds RetroArch emulator to your NES Mini.

It will automatically detect unsupported NES games and run them instead of default emulator. Save states will work as usual but CRT filter will not work.

Also it can run games for other consoles. This pack already contains cores:
- emux_sms
- fceumm
- gambatte_libretro
- genesis_plus_gx
- nestopia
- snes9x2010

Available executables and arguments:
- retroarch-clover <core> <rom> <clover_args>
  runs retroarch with specified core,
  designed for executing from clover shell, 
	so it parses all clover arguments (saves, aspect ration, etc.)
- /bin/gb <rom> <clover_args>
  runs "gambatte" core
- /bin/md <rom> <clover_args>
  runs "genesis_plus_gx" core
- /bin/nes <rom> <clover_args>
  runs "fceumm" core
- /bin/sms <rom> <clover_args>
  runs "emux_sms" core
- /bin/snes <rom> <clover_args>
  runs "snes9x2010" core

NES Mini port by madmonkey
NES Mini shell integration by Cluster
(c) 2017
