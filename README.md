#Introduction
This is a strategy game that  Stephen Fulcher (UMich '12), Jordan Mack (UMich '13)  and Max Wittek (UMich 2012) **created in ten days** for [Sid Meier's Game Design Bootcamp 2012][bootcamp]. Given the limited timeframe we had, we're pretty proud of the results.

#The idea
The original idea was to make a lightweight turn-based strategy game that could be played asynchronously between players - like Words with Friends, or even harkening back to the days of chess-by-email. Eventually we would port it to a mobile platform, where such games thrive. Some gameplay ideas included limiting resource collection present in most other strategy games so that players could finish up a round within twenty turns or so. Influences included Sid Meier's *Civilization* and *Alpha Centauri*, as well as Intelligent Systems' *Advance Wars*.

#Features
- Exciting gameplay!
- Classy soundtrack featuring such hits as "Ave Maria" and "The sound when Windows 95 boots up"!

#Screenshots
An Archer attacks an SSV Normandy from afar, since it has an Attack Range of 3 spaces.
<img width="100%" src="https://github.com/wittekm/SidMeierBootcampFinal/raw/master/Readme/rangedattack.png" />

The glowing lights represent where the currently selected unit, the red Normandy, can move. The box labeled "Archer" means we can attack the enemy's Archer.
<img width="100%" src="https://github.com/wittekm/SidMeierBootcampFinal/raw/master/Readme/normandyvsarcher.png" />

Factories can create new units in exchange for Action Points.
<img width="100%" src="https://github.com/wittekm/SidMeierBootcampFinal/raw/master/Readme/factories.png" />

The orthographic camera moves to a more dramatic angle during a battle.
<img width="100%" src="https://github.com/wittekm/SidMeierBootcampFinal/raw/master/Readme/angle.png" />

#Post-mortem
The asynchronous gameplay was scrapped because our team was unable to focus on networking stuff until a bit too late in the brief development process. We resigned ourselves to hot-seat gameplay, with players taking turns at the same computer. Considering that none of us had ever used Unity or even C# before, we're pretty happy with how fleshed out and indeed playable our game was after just ten days. The code is of questionable quality in some spots, but this is just a gameplay prototype! We all want to continue to pursue this idea in the future, but would probably scrap the current version.

#Code
Most of the scripting things can be found [here][scripting].

#Other
Many of the models used in this prototype are not our own, and the member of our team who scoured 3D modeling sites looking for the models didn't keep track of where they're all from. So, if you find one of your models in this repo and want it deleted - email us!

[bootcamp]: http://www.eecs.umich.edu/meier-bootcamp/leaders.html
[scripting]: https://github.com/wittekm/SidMeierBootcampFinal/tree/master/BootCamp/Assets/Scripts
