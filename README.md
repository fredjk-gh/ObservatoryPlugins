# ObservatoryPlugins
A suite of plugins for Elite ObservatoryCore (https://github.com/Xjph/ObservatoryCore).

Feel free to open an issue if something doesn't work, or reach out in the ObservatoryCore discord.

# Installation

The download is an `.eop` file.

*  If you installed Observatory Core via the installer, just close Observatory Core and double click the `.eop` file. Observatory Core will launch, and unpack the plugin (and any dependencies) for you. Repeat for each plugin to install.
*  If you installed Observatory Core via a `.zip` package, simply copy the `.eop` file(s) into the `.\plugins` sub-folder of your installation location.
*  With the latest beta version of AutoUpdater (requires the test version of Observatory Core 0.3), you can simply install AutoUpdater and use it to install other plugins.

After copying in .eop file(s) or installing any plugin(s), always close and re-launch Observatory Core.

# Plugins

## AutoUpdater

This plugin keeps all plugins listed here (including itself) up-to-date after installation. After a release of an installed plugin is posted to GitHub, it will download the updated versions so it can be used after ObservatoryCore is restarted. You can also install other plugins using the new UI (Beta).

After installation, be sure to look at the plugin's options and adjust as you like.

NOTE: To enable auto-updating any plugin listed as "Beta", below, you will need to enable the "Use beta versions" option.

*  Status: Released + Beta
*  Download latest stable (requires Observatory Core >= 1.0.4): [ObservatoryPluginAutoUpdater-v1.0.1.27202.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v1.0.1.27202/ObservatoryPluginAutoUpdater-v1.0.1.27202.eop) (Sep 26, 2024)
*  Download Beta (requires Observatory Core >= 1.0.4): [ObservatoryPluginAutoUpdater-v1.0.1.27202.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v1.0.1.27202/ObservatoryPluginAutoUpdater-v1.0.1.27202.eop) (Sep 26, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-AutoUpdater)

## Aggregator

The Aggregator plugin is a notification log -- collecting notifications from all other plugins into one place to reduce the number of times you need to switch between plugins.

*  Status: Released + Beta
*  Download latest stable (Requires Observatory Core >= 1.0.4): [ObservatoryAggregator-v1.0.1.27202.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v1.0.1.27202/ObservatoryAggregator-v1.0.1.27202.eop) (Sept 26, 2024)
*  Download Beta (requires Observatory Core >= 1.0.4): [ObservatoryAggregator-v1.0.1.27202.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v1.0.1.27202/ObservatoryAggregator-v1.0.1.27202.eop) (Sept 26, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Aggregator)

## Archivist

The Archivist plugin captures exploration related journals and stores them in a database for later re-use either by re-sending journal entries to other plugins or by allowing you to search/browse the journal entries yourself (pretty printed) and copy them out for other uses.

*  Status: Beta
*  Download Beta (requires Observatory Core >= 0.3.x): [ObservatoryArchivist-v0.24.143.0120-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v0.24.143.0120-beta/ObservatoryArchivist-v0.24.143.0120-beta.eop) (May 21, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Archivist)

## Fleet Commander

A companion for fleet carrier owners. My favourite feature is the jump cooldown timer. Beta includes Spansh carrier routing integration.

*  Status: Released + Beta
*  Download latest stable (Observatory Core <= 0.2.x): [ObservatoryFleetCommander-v0.24.005.1441-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v0.24.005.1441-beta/ObservatoryFleetCommander-v0.24.005.1441-beta.eop) (Jan 5, 2024)
*  Download Beta (requires Observatory Core >= 1.0.4): [ObservatoryFleetCommander-v1.0.0.28804-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v1.0.0.28804-beta/ObservatoryFleetCommander-v1.0.0.28804-beta.eop) (Oct 14, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Fleet-Commander)

## Helm

Currently testing the following features:

- Low fuel warnings and advising of scoopable stars (or lack thereof) along the neutron highway.
- High gravity heads-up when approaching a relatively high gravity planet.
- Tracks your route progress and provides session stats.
- Injects the last plotted route for the current commander at game startup. (Useful when the game forgets the destination after switching commanders.)

Suggestions welcomed!

*  Status: Beta
*  Download Beta (requires Observatory Core >= 0.3.x): [ObservatoryHelm-v0.24.143.0120-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v0.24.143.0120-beta/ObservatoryHelm-v0.24.143.0120-beta.eop) (May 21, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Helm)

## Prospector

Prospector is a miner's must-have tool, assisting you through the entire prospecting and mining workflow.

-  It will call out mineable rings when you FSS a body.
-  When you DSS a ring, it will tell you if any hotspots of interest are found (which you normally have to look navigation panel or point your ship at every spot to see).
-  When you launch a prospector, it will tell you either there's nothing of interest or tell you it's a good rock (based on a configurable threshold for laser mining or if a core of interest is found).
-  It also provides a static display of the content of your cargo hold (limpets vs. paydirt).

*  Status: Released + Beta
*  Download latest stable (Observatory Core <= 0.2.x): [ObservatoryProspectorBasic.0.22.348.0223-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/0.22.348.0241-beta/ObservatoryProspectorBasic.0.22.348.0223-beta.eop) (Dec. 13, 2022)
*  Download Beta (requires Observatory Core >= 0.3.x): [ObservatoryProspectorBasic-v0.24.143.0120-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v0.24.143.0120-beta/ObservatoryProspectorBasic-v0.24.143.0120-beta.eop) (May 21, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Prospector)

## Stat Scanner

This plugin monitors your discoveries to see if your discovery is near, tied with or better than a known record. It can also track personal bests!

*  Status: Released + Beta
*  Download latest stable (Observatory Core <= 0.2.x): [ObservatoryStatScanner-0.23.165.2200-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/0.23.165.2200-beta/ObservatoryStatScanner-0.23.165.2200-beta.eop) (June 14, 2023)
*  Download Beta (requires Observatory Core >= 0.3.x): [ObservatoryStatScanner-v0.24.143.0120-beta.eop](https://github.com/fredjk-gh/ObservatoryPlugins/releases/download/v0.24.143.0120-beta/ObservatoryStatScanner-v0.24.143.0120-beta.eop) (May 21, 2024)
*  [Details / Changelog](https://github.com/fredjk-gh/ObservatoryPlugins/wiki/Plugin:-Stat-Scanner)
