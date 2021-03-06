# VolumeFade

This is a  terminal application that controls the Windows Volume Mixer to fade out the volume of a program to 0 under the specified time. For example a media player like Windows Media Player, Audacious, Spotify. 

NOTICE: if the media player changes the title in the mixer (like when you play a song with spotify it will have the artist and the song instead of `spotify`) you need to use one word from that. I will address this issue in the future.

To control the mixer the [Core Audio](https://github.com/morphx666/CoreAudio) library is used as a git submodule.

##### Usage

`VolumeFade.exe <mixer title> [fadeout time] [decrease rate]`

* `mixer title`: part or the entire name of the program you want to fade out inside the windows mixer (enclose in apostrophes if it contains space characters)
* `fadeout time`: optional, the number of milliseconds to fade out the program. Default is 5000 if ommitted, which is 5 seconds
* `decrease rate`: optional, the volume is decreased based on 1-(t^decrease rate) . Default is 1, if you don't understand what this means leave it on default.

##### How to compile

You can open the project as a Visual Studio 2019 solution. Hopefully no extra configuration is required.

##### Distribution

After compiling the project, go into the `VolumeFade/bin/Debug` or `VolumeFade/bin/Release` folder depending on your configuration, and copy the contents of the folder to the intended destination.

You will need CoreAudio.dll and VolumeFade.exe for it to work
