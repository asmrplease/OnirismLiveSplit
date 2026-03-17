Uses https://github.com/TheTedder/LiveSplitInterop.

# Installation
- Current releases don't include BepInEx, go install that first. 
- Install OnirismLiveSplit directory into your Onirism/BepInEx/plugins directory. 

# Configuration
- Includes configuration options for when to perform splits, and when to pause the timer. Implementation of these options may change, which will affect splits. 
- Please edit your Onirism/BepInEx/config/OnirismLiveSplit.cfg file before starting the game to adjust your settings.

## Autosplit Configuration
- By default, all auto splits are disabled. 
- There are several types of event that can be split on:
	- Scene Load
		- aka level load
		- Setting this value to "Always" will result in dying causing a split.
		- Setting this value to "Unique" will perform a split only the first time you start a scene.
	- Sector Activators
		- Sector Activators are objects that turn on part of a Scene when touched. 
		- Setting this value to "Always" will result in backtracking causing addtional splits.
		- Setting this value to "Unique" will result in a split only the first time you enter each activator, with repeated sectors not performing splits.
	- Julie Phonecalls
		- Split when Julie calls you.
	- Cutscenes
		- Split when a cutscene starts.
	- Game Events
		- Splits any time an event is added to your save file.
		- Julie phonecalls and cutscenes also add game events, so you will get duplicate splits if you have those enabled at the same time as this.
	- Item Collection (WIP)
		- Splits when you collect an item
- There are three values for each autosplit setting:
	- Never
		- This type of event will never activate the autosplit.
	- Unique
		- Only unique occurences of this event will activate the autosplit.
		- This is generally the recommended way of enabling the autosplit.
	- Always
		- All occurrances of this event will autosplit.
		- This will cause repeat splits when respawning or backtracking.
- Recommendations:
	- Unique Scene Load is a good starting point.
	- Enabling Unique Cutscenes, Game Events, and/or Julie Calls will provide more granular splits.
	- Sector Activators are probably the most detailed, but weren't meant to be player-facing, and may not result in consistent splits.
	- You probably shouldn't use Item Collection splits right now.
## Autopause Configuration
- By default, only pausing on scene loading is enabled. This helps mitigate variances between different PC hardware.  
- Pausing while cutscenes are playing can be enabled. IDK if this would be considered legal for leaderboards, but might be nice if you want to enjoy cutscenes while running.
