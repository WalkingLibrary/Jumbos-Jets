# Jumbos Jets: Close Air Support for RimWorld

Build, crew, and send the Warthog into action. Jumbos Jets brings heavy close air support to RimWorld, from researching jet propulsion and manufacturing aircraft parts to keeping your plane fueled and on station. Draw a strafing line across an existing map, spend steel to unleash the run, and use a laser designator to guide its accuracy.

## What's inside

Take command of the Warthog, a purpose-built attack jet. Develop its specialized engines and components, produce dedicated jet fuel, then launch aerial sorties over maps in your world. Strafing runs strike along a line you choose, while a pawn using the laser designator can help focus the attack.

## Required mods

- [Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
- [Vehicle Framework](https://steamcommunity.com/sharedfiles/filedetails/?id=3014915404)
- [Vanilla Vehicles Expanded](https://steamcommunity.com/sharedfiles/filedetails/?id=3014906877)

Load these mods before Jumbos Jets.

## Thanks and credits

Jumbos Jets is made by Tha Mary Man. A big thank-you to the creators of Harmony, Vehicle Framework, and Vanilla Vehicles Expanded; their work provides the foundation that lets this mod bring aircraft to RimWorld.

## Art and code note

All artwork in Jumbos Jets was created by a human. AI helped write some of the code, which was reviewed and tested by a human in-game.
## Planned future features

Future ideas include more aircraft, enemy air support, anti-air defenses, additional munitions, and aircraft crash or ejection events. These are possibilities for later updates, not features included in the current release.

## A note about the mod

Jumbos Jets is a work in progress, and its balance, systems, and compatibility may still have rough edges. Bug reports must include the full `Player.log` from the session where the issue occurred, steps to reproduce it, and your game and mod versions. Reports without session logs will not be reviewed.

**Find your log after the problem occurs, before relaunching RimWorld** (the file is replaced each time the game starts):

- **Windows:** Press `Win+R`, paste `%localappdata%low\Ludeon Studios\RimWorld by Ludeon Studios`, and press Enter. Attach `Player.log` from that folder.
- **macOS:** In Finder, press `Cmd+Shift+G`, paste `~/Library/Logs/Ludeon Studios/RimWorld by Ludeon Studios/`, and press Enter. Attach `Player.log` from that folder.

## IMAGES


## Gameplay

## AirCraft

## Research

## Components

## Items



1. Mod overview / hero graphic
2. The Warthog aircraft
3. Jet propulsion research and aircraft parts
4. Jet fuel production
5. Aerial strafing runs
6. Laser designation and guided accuracy
7. Dependencies, compatibility notes, credits, and FAQ

The individual briefs are in [`Features`](Features) and [`Items`](Items). Each includes Steam-ready draft copy and a separate image brief. Use the image brief as a starting point for your graphics; the copy can be shortened or revised to fit the final page layout.

Finished feature cards and individual item cards are in [`Graphics`](Graphics). The feature set covers [Aerial Sorties](Graphics/AerialSorties.png), [Laser Guidance](Graphics/LaserGuidance.png), [Jet Propulsion](Graphics/JetPropulsion.png), and [Jet Fuel Production](Graphics/JetFuel.png). Individual cards cover the [Warthog](Graphics/Item-Warthog.png), [Laser Designator](Graphics/Item-LaserDesignator.png), [Jet Engine](Graphics/Item-JetEngine.png), [Jet Intake](Graphics/Item-JetIntake.png), [Jet Exhaust](Graphics/Item-JetExhaust.png), [Jet Fuel Tank](Graphics/Item-JetFuelTank.png), [Jet Parts](Graphics/Item-JetParts.png), and [Jet Fuel](Graphics/Item-JetFuel.png). Their source scene panels and the rebuild script are kept alongside them.

## Release facts to confirm before publishing

- This project targets RimWorld 1.6 and lists Harmony, Vehicle Framework, and Vanilla Vehicles Expanded as dependencies in `About.xml`.
- The Warthog sortie definition currently specifies a 30-cell maximum line, 20 impacts, 53 damage per impact, one steel per impact, 7-cell unguided scatter, and 2.5-cell guided scatter. The guidance area is currently a 10-cell radius.
- The laser designator currently has 120-cell range, 25 charges, and requires the operator to remain stationary with line of sight.
- The Warthog definition currently requires two jet engines, two intakes, two exhausts, fifteen jet-parts items, two jet-fuel tanks, three car wheels, 500 steel, four industrial components, and two advanced components.
- Confirm the exact recipe bench and final balancing in a clean RimWorld launch before describing those details as tested. Some older README notes and aircraft descriptions are stale.
- Enemy aircraft, anti-air weapons, crash events, bombs, and additional plane models appear in the idea list; they are not presented here as included features.

## Page assembly notes

Keep each section focused on one player-facing benefit. Pair it with one clear graphic or in-game screenshot. Good candidates are a Warthog feature plate, a production-chain graphic, a marked strafing line with its blast footprint, and a laser-designation comparison. The screenshot or infographic should show actual current behavior; avoid depicting planned systems as if they were already in the mod.

## Requirements and FAQ copy draft

**Required mods:** Harmony, Vehicle Framework, and Vanilla Vehicles Expanded.

**Who made Jumbos Jets?** Tha Mary Man.

**How do I unlock the Warthog and its parts?** Research Jet Propulsion in the Vehicles tree. The project branches from Aerial Vehicles and requires a techprint.

**How do I make jet fuel?** Refine three chemfuel into one jet fuel at a biofuel refinery after researching Biofuel Refining.

**Can the Warthog attack a map it has not visited?** A sortie targets an existing map associated with the aircraft's current world location. The attack does not generate a new map.

**Does the laser designator deal damage?** No. It marks a target and improves the accuracy of a Warthog run that passes close to the designation.

**Can I move a pawn while it is aiming the designator?** No. A move order stops the designation and lets the pawn move.

**What other aircraft and air-defense systems are included?** The current release content described in this draft includes the Warthog only. Additional aircraft, anti-air weapons, and enemy air support are future ideas, not included features.

