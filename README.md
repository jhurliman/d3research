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
game assets and updates. Then close the launcher.
2. Clone the mooege repo and switch to the '8101' branch.
3. Compile mooege with MSVS or Mono.
4. Copy all of the Diablo III client MPQ files and folders into 
src/Mooege/bin/Debug/Assets/MPQ/ and run the server. Then launch the client 
with "-w -launch -auroraaddress 127.0.0.1:1345".
5. Login with username "test@" and any password.
6. Attach your favorite debugger or run d3sandbox.

NOTE: The server emulator is in an early state right now. Many interactions do
not work, monsters will not attack you and die with a single hit, some 
collisions are not functional, etc. It should work fine for most debugging, 
but do not trust the data coming from the server emulator as canonical.

## Working ##

* Enumerate scene entities
* Parse entity information
* Actor classification
* Identify special entities: waypoints, stash
* Enumerate attributes for each actor
* Enumerate scenes
* Walk to location
* Attack enemy/object
* Use different attacks/abilities
* Open chests/doors
* Determine what attacks/items are mapped to hotkeys
* Inventory enumeration
* Create an API for AI
* Enumerate navcells in scenes and build a navigation map

## TODO ##

* AI helpers for starting a task, measuring progress, testing for completion, and abandoning on success/failure
* A*-assisted navigation
* Implement class AI
* Sell inventory items to NPCs
* EndScene mid-function hook
