=== SNES Custom Filter BWT - Controller handled filters by Bewildered Thetan ===

This module allows you to tweak video filters on SNES Mini or Super Famicom Mini (yes, only SNES, not NES/Famicom) by using
your controller in port 1.

DISCLAIMER:
Use this mod at your own risk. It has been thoroughly tested on an SNES Mini European edition, but that doesn't mean
it couldn't have overlooked side-effects. If you brick your console, don't complain about it. Chances are high that
people can help you out, but don't necessarily count on that.
Do NOT install this mod if you do not agree with this disclaimer!!

Usage:

While loading a game (pressing START) just hold the specific buttons (AFTER pressing START) and release them when the game is loaded (you see some game activity)

The A and B buttons handle the post-process graphics filter in the following way:

No A and B pressed -> no post-process graphics filter
Only B pressed  -> OpenGL
Only A pressed  -> Scanlines
A and B pressed -> CRT

The X and Y buttons handle the magnification filter in the following way:

No X and Y pressed -> Nearest
Only Y pressed     -> Linear
Only X pressed     -> HorizontalLinear
X and Y pressed    -> AntiAliasedNearest (looks like this mode has no effect and is essentially the Nearest mode. Maybe in specific games?)

The LB (left bumper) button handles pixel perfect mode (square pixels) in the following way:

LB pressed -> pixel perfect mode is enabled
LB not pressed -> pixel perfect mode is disabled

Holding the RB (right bumper) button handles loading the game with the settings from the SNES Menu (so you can also use the menu settings without removing this mod!)
Just load a game by holding RB and none of the above settings will be applied, but the default SNES Classic behaviour will be applied. 

Filters are only in effect during gameplay (so, when launching a game you determine the settings for that session only)

For your information, the SNES Classic menu settings translated to the following settings:
CRT Filter    -> post-process filter Scanlines, magnification filter Linear
4:3           -> post-process filter OpenGL, magnification filter AntiAliasedNearest
Pixel-perfect -> post-process filter OpenGL, NO magnification filter and pixel-perfect mode enabled

With these three possibilities, you could not use the CRT Filter settings combined with pixel-perfect mode, but with this mod you can. With this mod you can make any
combination you like, so have fun with it!

ADVANCED Functionality (saving filters per game)
------------------------------------------------
You can save/load the filter settings per game (even the filter setting from the SNES Classic menu) by using the UP and DOWN buttons while loading a game.
When holding the UP button (upload), the selected filter will be selected and saved for the loaded game only!
When holding the DOWN button (download), the game will be loaded with the filter settings saved for that specific game.

Example scenario:

1. Select pixel perfect mode in the SNES Classic menu and load a game by pressing START and then hold the RB and UP buttons -> pixel perfect mode is saved
   for the selected game
2. Change the filter settings in the SNES Classic menu
3. Load the same game by pressing START and then hold the DOWN button and the saved settings are applied (so menu settings are ignored!)
4. Load a different game by pressing START and then hold the RB button -> setting from the SNES Classic menu is used again
5. Load a different game by pressing START and then hold the LB,A,X and UP buttons -> game is loaded in pixel perfect mode with Scanlines and HorizontalLinear filtering
   This settings is also saved for this specific game!
6. Load the same game by pressing START and then hold the DOWN button -> game is loaded again using the saved settings (pixel perfect mode with Scanlines and HorizontalLinear filtering)

TIP: The settings also work when loading a suspend point/save game. You have to be careful which buttons you press though, as the X and Y buttons do have
effect in the suspend point/game save loading menu. Just press START to load the suspend point/save game and quickly press and hold the appropriate buttons
you want to use to configure the filter settings. 

WARNING: this mod can conflict with other mods! You can install the snes_custom_filters mod, but this mod will override it.
When uninstalling this mod when the snes_custom_filters mod is also installed will make the snes_custom_filters mod active again, so they do not interfere.

Good luck and have fun with this mod.
_________________________________
.October 2017 - Bewildered Thetan