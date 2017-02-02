# hakchi2

NES Mini pimp tool

This application can add more games to your Nintendo Classic Mini or Famicom Mini. All you need is to connect it to PC via microUSB cable. No soldering, no disassembling.

Features:
* Allows to change any game settings including command line arguments
* Fills all game data automatically using included database
* Automatically checks for supported games
* Allows to automatically search for box art using Google Images
* Game Genie support
* Can automatically patch problem games, patches for some popular games included in the package
* Allows to upload hundreds of games at once
* Allows to exit to menu using button combination instead if reset button
* Allows to enable autofire
* Allows to simulate start button on second controller for Famicom Mini
* Allows to disable epilepsy protection
* Allows to disable menu music

## So are you hacked NES Mini?
No! It was my russian сomrade madmonkey. He created original “hakchi” tool. It was not very user friendly so I decided to create tool which is simple for everyone, not only Linux users. I named it “hakchi2” because I don’t like to coming up with names. So first version was 2.0 :)

## How to use it?
Basically you need just to unpack it somewhere (installation not required), run it, press “Add more games”, select some ROMs and press “Synchronize”. Application will guide you.

## How it’s working?
You don’t need to worry about it. But if you really want to know it’s using FEL mode. FEL is a low-level subroutine contained in the BootROM on Allwinner devices. It is used for initial programming and recovery of devices using USB. So we can upload some code into RAM and execute it. In this way we can read Linux kernel (yes, NES Mini runs on Linux), write kernel or execute kernel from memory without writing it to flash. So we can dump kernel image of NES Mini, unpack it, add some games and script which will copy them to flash, repack, upload and execute. But games directory is on read only partition. So we need also to create and flash custom kernel with special script that creates sandbox folder on writable partition and mounts it over original games folder. So your original files are safe. You can’t delete or harm original files in any way. For kernel patching my application just executing other applications, that’s why there is “tools” folder.
