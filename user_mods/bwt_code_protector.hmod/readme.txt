=== BWT Code Protector script by Bewildered Thetan ===

This mod installs a script in te directory /etc/bwt_code_protector which you can use in your scripts to add code protection.

The hmod has no purpose standalone, so you need to create a script (or a hmod) to use this hmod.
In essence, it's a support mod.
It lets you protect functionality (for instance: loading a game via a special wrapper script where you use this mod) with a code.
The code is stored in a file named 'code' in a directory you prefer.
Via informative images on screen you will be guided through the process of setting a code initially, or entering a code if it has been
set already.
When using this mod in a script and calling it, if no code has been set initially, you will need to enter a code first.
The script will ask you to confirm this code by entering it twice, so you are sure you initialized the correct code (pretty useful if
you use a long code). Do not press buttons to fast, or they will not be registered correctly. You will figure it out.
Note that button presses, as well as releases are registered as code characters and you can use all buttons (controller 1 only).
Directional buttons are not supported and the START button confirms the code.
So, you can do this for instance: hold button A, press B, press X, release button A
The code you enter has no limit, so you can make it as long and complex as you want.

DISCLAIMER:
Use this mod at your own risk. It has been thoroughly tested on an SNES Mini European edition, but that doesn't mean
it couldn't have overlooked side-effects. If you brick your console, don't complain about it. Chances are high that
people can help you out, but don't necessarily count on that.
Do NOT install this mod if you do not agree with this disclaimer!!

USAGE:
To use the scripting this hmod provides you need to do the following in your scripts:

- Set the variable 'bwt_code_protector_code_dir' to point to a location where the access code will be stored
  This way you can use multiple codes for various functionalities
  If you don't set this variable, the directory of the hmod is used. This way you can use the same code for
  all functionality you wish to protect.
- source the script file '/etc/bwt_code_protector/bwt_code_protector' in your script file
- call the function 'bwt_code_protector'
  This function will stop the user-interface (/bin/uistop) so it can display images.
  It will NOT start the user-interface when it's finished, because in some scenarios you don't want that
  (when protecting a factory reset for instance), so just issue a /bin/uistart yourself if you need to.  
- The function will set the variable 'bwt_code_protector_return_code' to the following values:
  IMAGE_NOT_FOUND -> the code protector uses background images, if they are not there, you can't use the mod
  CODE_SET -> the code was not set and is set now
  CODE_CORRECT -> user inputted correct code
  CODE_INCORRECT -> user inputted incorrect code

EXAMPLE:  
Poweroff the system in case an incorrect code is entered:

source /etc/bwt_code_protector/bwt_code_protector
bwt_code_protector
if [ "$bwt_code_protector_return_code" != "CODE_INCORRECT" ]
then
    /bin/uistart
else
    poweroff -f
fi
 
IMPORTANT: this mod displays images in a format that only works when the SNES is loaded up already. So do NOT use this
scripting in an initialization script (scripts in /etc/init.d or /etc/preinit.d)
  
Good luck and have fun with this mod.
_________________________________
.November 2017 - Bewildered Thetan