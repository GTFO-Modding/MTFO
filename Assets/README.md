# Mod the FUCK out

A bepinex plugin that saves / loads the internal configuration files (hereafter refered to as "Data Blocks") as raw json files.

MTFO also adds it's own custom configuration files, these should be placed under

```
GTFO/
└──BepInEx/
    └──plugins/
        └──RundownName/
            ├──GameData_*.json
            └──custom/
                └──CustomFiles
```

The custom folder is **not** created by default and must be manually created by the developer.

Documentation on MTFO and the Custom folder can be found on the wiki [here](https://gtfo-modding.gitbook.io/wiki/).

## Installing

### Easy

1. Download [R2ModMan](https://gtfo.thunderstore.io/package/ebkr/r2modman/)
2. Search for MTFO and install it with dependencies
3. Launch modded, you're done!

---

### Manual

Download and install [BepInEx](https://gtfo.thunderstore.io/package/BepInEx/BepInExPack_GTFO/).

If you don't know how to find your GTFO folder

Steam library -> Right click GTFO -> Properties -> View local files

![The first step](https://raw.githubusercontent.com/GTFO-Modding/MTFO/main/images/1.png)

![The second step](https://raw.githubusercontent.com/GTFO-Modding/MTFO/main/images/2.png)

![The third step](https://raw.githubusercontent.com/GTFO-Modding/MTFO/main/images/3.png)

After following the BepInEx installation instructions and **launching the game at least once** place MTFO into your plugins folder.

```
GTFO/
└──BepInEx/
    └──plugins/
          └──MTFO.dll
```

Launch the game once more, your file structure should now look similar to this

```
GTFO/
├──BepInEx/
|    ├──config/
|    |     ├──_gameDataLookup
|    |     ├──BepInEx.cfg
|    |     └──MTFO.cfg
|    └──plugins/
|          ├──MTFO.dll
|          └──GameData_XXXXX
├──mono/
├──doorstop_config.ini
├──GTFO.exe
└──winhttp.dll
```

## Installing Rundowns

# Easy

1. Download [R2ModMan](https://gtfo.thunderstore.io/package/ebkr/r2modman/)
2. Create a [new profile](https://github.com/ebkr/r2modmanPlus/wiki/Profiles)
3. Find and install the rundown you'd like to play and install it with dependencies
4. Launch modded

You're done!

---

# Manual

1. Download the rundown you'd like to play
2. Place the rundown under GTFO/BepInEx/Plugins
   - Make sure to check if the rundown you're installing has extra install steps! These will usually be in a "README.txt" or "INSTALLATION.txt" file.

```
GTFO/
└──BepInEx/
     └──plugins/
           └──RundownName/
```

3. Download and install any dependencies the rundown may have.
4. Launch the game!



### ! Warning !

If you have more than one rundown inside the plugin folder at the same time, whichever rundown is discovered first will be loaded.
- Make sure to uninstall any plugins not related to the rundown you have installed, otherwise there might be issues or crashes!

# Patch notes
-4.4.4
    * Fixed issue with DB's not dumping
    * Updated wiki link finally
-4.4.3
    * Updated to latest bepinex version
    * Updated version to match thunderstore
-4.4.2
    * Added open API for Interop
    * Fixed `GameData_` generating in plugins folder
    * Fixed issues with scan text not appearing
    * Thanks to Flow for these changes!
- 4.4.1
    * Previous patch is thanks to flow aria!
- 4.4.0
    * Removed lookup table / hashing
    * GameData is now dumped to `BepInEx/GameData-Dump/[revision]`
    * Dumping data needs to be turned on in config
    * MTFO will only load included files. For instance, if you do not included the `TextDataBlock` in your rundown dist, MTFO will load the vanilla version and will not auto-generate a dump file. In short: you will no longer need to update files you don't edit when the game updates.
- 4.3.10
    * Re-complied for new bepinex & game version, thanks kas!
- 4.3.9
    * Re-compiled against latest game version
- 4.3.8
    * Fixed custom scan text not working, thanks Flow!
- 4.3.7
    * Added ability to customize scan reveal speed, thanks Flaffy!
- 4.3.6
    * Removed welcome.txt, as it no longer functions and causes issues in r6
    * Merged PR's to disable MM buttons in lobby screen (thanks to chase!)
- 4.3.5
    * Fixed HotReload
- 4.3.4
    * Updated custom scans to work with r6
    * Misc internal fixes

If you're having any issues, send a message in #tech-support on the [GTFO modding discord](https://discord.com/invite/rRMPtv4FAh)!

# Developers


## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/GTFO-Modding/MTFO/blob/main/LICENSE) file for details.
