# Joost Tweaks

Repository with Clone Hero tweaks made by Joosthoi1

## How to install Chloader tweaks
- You will need Clone Hero **v0.23.2.2**.
- Make sure you install CHLauncher first!
  - Navigate to the `#ch-launcher` channel, and download the latest version of CHLauncher.exe.
  - Run it, and patch your version of Clone Hero with it. You should now have a `Tweaks` folder in your Clone Hero folder.
- Download the tweaks you want from the `Releases` section and place them in that `Tweaks` folder.

## How to install BepInEx tweaks
### BepInEx tweaks will be in a zip file
- You will need Clone Hero **v0.23.2.2**. You can use any version from the website (i.e. Win64, Win32, Mac, or Linux), but your mileage with may vary.
- Install [BepInEx v5.3](https://github.com/BepInEx/BepInEx/releases/tag/v5.3) into your Clone Hero directory.
  - Download the appropriate version and extract **all** of its files into your Clone Hero directory.
  - Please verify that BepInEx has initialised by running the game after extracting, and then checking that there are five folders and a `LogOutput.log` file inside the `BepInEx` folder. One of those folders will be named `plugins`, and you'll need that to run the mods.
- Go to the [Releases page](https://github.com/joosthoi1/Joost-tweaks/releases) and download the latest versions of the mods you want for your version of Clone Hero.
    - Almost all the mods will require `Biendeo CH Lib` to also be installed so please also download that. If you are missing it, the log will inform you. This can be found on [Biendeo's github](https://github.com/Biendeo/My-Clone-Hero-Tweaks/releases)
    - All the downloads are `.zip` files and will need to be extracted to your Clone Hero directory. They should merge with the existing `BepInEx` folder.
    - To ensure that the mods have been extracted properly, check that `LogOutput.log` (or `LogOutput.log.1`, whichever is newer) has a line in this format for each mod: `[Info   :   BepInEx] Loading [Biendeo CH Lib 1.5.0.0]`
