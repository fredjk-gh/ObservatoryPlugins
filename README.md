# ObservatoryPlugins
A suite of plugins for Elite ObservatoryCore (https://github.com/Xjph/ObservatoryCore).

For all plugins, just drop the zip/eop into the plugins/ folder in your ObservatoryCore installation.
ObservatoryCore will correctly extract it for you on next startup.

Feel free to open an issue if something doesn't work, or reach out in the ObservatoryCore discord.

## Stat Tracker

### Release notes: 0.23.46.717-beta

Fix number parsing errors in locales which use ',' for decimal separator.

The galactic records is written using US formatting, notably, using a period for decimal separator, not a comma. Attempting to import these values using a locale which uses a comma as decimal separator (ie. de or ru) causes lots of errors to be written to the crash log on startup.

This change forces US locale when parsing the galactic records file.


### Release notes: 0.23.46.610-beta

Added a "First Discoveries only" option which shows only first discovered entries plus visited known record holders.

Also added some error handling better deal with parsing errors (likely due to locale) that I need to better understand. This *should* prevent start-up crashes.

Plus a bunch of refactoring and internal improvements.

### Release notes: 0.23.44.439-beta

This new plugin (as of February 2023) monitors your discoveries to see if your discovery is near, tied with or better than a known record.
Record data is sourced from edastro.com and updated ~weekly. Note that the plugin suggests "potential new record" because there are 
plenty of reasons why it's not guaranteed to be, including rounding and/or data errors, other data that has turned in since the last time
the galactic records were updated -- this does NOT guarantee you an ESDM record badge. :) But I wish you good luck, none-the-less.

The initial release of the plugin watches records for the following variables/attributes and body classes (as applicable):

Planets:

*   Planet Mass
*   Planet Radius
*   Surface Gravity
*   Surface Pressure

Planets and Stars:

*   Surface Temperature
*   Orbital Eccentricity (where applicable)
*   Orbital Period

Stars:

*   Star Mass
*   Star Radius

Still to come:

*   Ring width, inner/outer radius

Future plans include tracking "personal bests" including other system-level stats that aren't tracked on edastro.com such as:

*   Max bodies in system
*   Max direct moons of a planet
*   Max total moons of a planet
*   Odyssey: Max bios for a single body
*   Odyssey: Max bios in a system
*   Max # of bodies per type in a system

Let me know if you any issues either via the tracker or via the Observatory Core discord! I'd like to hear your constructive feedback as well.

## Fleet Commander

### Release notes: 0.22.348.0241-beta

Update to .net 6.0 to support latest version of ObsCore.

As of Odyssey Update 9 (Dec. 2021), the game is not writing "CarrierJump" events to the journal file
(see https://issues.frontierstore.net/issue-detail/46996), resulting in slightly degraded functionality
when jumping your carrier while not docked on it.

As of Odyssey Update 11 (Mar. 2022), the game is not writing StationName/StationType properties to location
events, further complicating matters. I've managed to work around this for the moment.

## Prospector

### Release notes: 0.22.348.0223-beta

Update to .net 6.0 to support latest version of ObsCore.

* Plus a bugfix for an NPE and a minor notification adjustment.

Give these a try and let me know if you have any issues!
