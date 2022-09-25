# ObservatoryPlugins
A suite of plugins for Elite ObservatoryCore (https://github.com/Xjph/ObservatoryCore).

For all plugins, just drop the zip/eop into the plugins/ folder in your ObservatoryCore installation.
ObservatoryCore will correctly extract it for you on next startup.

Feel free to open an issue if something doesn't work, or reach out in the ObservatoryCore discord.

## Fleet Commander

### Release notes: 0.0.92.1935-beta
As of Odyssey Update 9 (Dec. 2021), the game is not writing "CarrierJump" events to the journal file
(see https://issues.frontierstore.net/issue-detail/46996), resulting in slightly degraded functionality
when jumping your carrier while not docked on it.

As of Odyssey Update 11 (Mar. 2022), the game is not writing StationName/StationType properties to location
events, further complicating matters. I've managed to work around this for the moment.

## Prospector

### Release notes: 0.22.268.345-beta

Lots of updates and improvements!
* New version numbering.
* During FSS, calls out bodies rings of interest that meet a specified minimum "density".
* When rings are scanned with DSS, calls out hotspots for materials of interest (configurable by setting).
* Added location to the grid view.
* Added options to control which notifications are enabled.
* Misc cleanup.

Give these a try and let me know if you have any issues!
