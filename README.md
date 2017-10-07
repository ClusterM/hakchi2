# hakchi2

This is a GUI for hakchi by madmonkey.

This application can add more games (game ROMs) to your NES/SNES Classic Mini or Famicom Mini. All you need is to connect it to a Windows PC via Micro-USB cable. No soldering or disassembling required.

https://github.com/ClusterM/hakchi2

### Features
* Change any game settings (including command-line arguments)
* Fill all game data automatically using included database
* Automatically check for supported games
* Search for box art using Google Images
* Use [Game Genie](https://en.wikipedia.org/wiki/Game_Genie) codes for NES games; includes Game Genie database
* Automatically patch problem games (patches for many popular games included)
* Upload hundreds of games at once
* Return to the HOME menu with a button combination instead of the Reset button
* Enable autofire A/B
* Simulate the start button on the second controller (for Famicom Mini)
* Disable seizure protection
* Allows to install user-mods to add more features (even support for SNES/N64/Genesis/etc., music replacement, themes, etc.)

## So you were the first to hack the NES Classic Mini?
No! It was my Russian сomrade, madmonkey, who first published a successful hack of the the NES Classic Mini. He created the original “hakchi” tool. However, it was not very user-friendly, so I decided to create a tool which is simple to use by anyone--not only Linux users. I named it “hakchi2” because I don’t like to come up with names. So my first version was a 2.0 release :)

## How do I use the tool?
Basically you just need to unpack it somewhere on your harddrive (installation is not required), run it, press “Add more games”, select some game ROMs and press “Synchronize”. The application will guide you through this process.

## How does the tool actually work?
You don’t need to worry about it. But if you really want to know, it’s using FEL mode. FEL is a low-level subroutine contained in the BootROM on Allwinner devices. It is used for initial programming and recovery of devices using USB. So we can upload some code into RAM and execute it. In this way we can read the Linux kernel (yes the NES Classic Mini and Famicom Mini runs an Linux operating-system), write kernel or execute kernel from memory without writing it to flash. So we can dump the kernel image of the NES Mini, unpack it, add some games and run a script which will copy them back to flash, repack, upload and execute. However, the games directory is on a read-only partition. Therefore we also need to create and flash a custom kernel with a special script that creates a sandbox folder on a writable partition and mounts it over the original games folder. This means that your original files are safe: you cannot delete or harm the original files in any way, even if you wanted. For kernel patching my application just executes other applications, which is why there is a “tools” folder.

## What if I have another question?
Many more commonly asked questions are answered in the FAQ:
https://github.com/ClusterM/hakchi2/wiki/FAQ

// Alexey 'Cluster' Avdyukhin ("ClusterM" on GitHub)
