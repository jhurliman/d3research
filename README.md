## Intro ##

Get a feel for how Diablo III plays by watching a few minutes of this 
playthrough of the beta: http://www.youtube.com/watch?v=FpgTFLJ_tZY

To start hacking, you need a few things:

* Diablo III client
 - Windows: http://us.media.battle.net.edgesuite.net/downloads/d3-installers/4de82d80-ddeb-4e61-80ae-b4e8817f54b0/Diablo-III-Beta-enUS-Setup.exe
 - OSX: http://us.media.battle.net.edgesuite.net/downloads/d3-installers/4de82d80-ddeb-4e61-80ae-b4e8817f54b0/Diablo-III-Beta-enUS-Setup.zip
* Diablo III server emulator
 - https://github.com/mooege/mooege

### Instructions ###

1. Download, install, and run the launcher which will download about 4GB of 
game assets and updates. Then close the app.
2. Clone the repo and switch to the '8101' branch.
3. Compile mooege with MSVS or Mono.
4. Copy all of the client MPQ files and folders into 
src/Mooege/bin/Debug/Assets/MPQ/ and run the server. Then launch the client 
with "-w -launch -auroraaddress 127.0.0.1:1345".
5. Login with username "test@" and any password.
6. Attach your favorite debugger or run d3sandbox.

## Working ##

* Enumerate RActors
* Parse basic RActor information (GUID, model name, position/direction)

## TODO ##

* Enumerate the attributes/stats for each actor
![Stat enumeration example](http://i51.tinypic.com/jz6teu.png)
* Classify each RActor (may need to inspect SNO files for each actor)
* Find hitpoints for enemy actors
* Fetch player attributes (hitpoints, experience, etc)
* Enumerate scenes
* Enumerate navcells in scenes and build a navigation map
![Navcell enumeration example](http://dl.dropbox.com/u/6736045/navcell-list.jpg)
![Navcell display example](http://dl.dropbox.com/u/4381027/bssknuul.jpg)
* Walk to location
* Attack enemy/object, interact with NPC, open chest/door, etc
* Use different attacks/abilities
