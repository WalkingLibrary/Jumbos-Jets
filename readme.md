# Jumbos Jets

A RimWorld 1.6 aircraft mod using Vehicle Framework and Vanilla Vehicles Expanded.

## A-10 sorties

The first sortie implementation is ready for in-game testing.

1. Fuel the Warthog, load steel into its cargo inventory, board a capable pawn in its pilot seat, and clear its runway.
2. Use the normal **Launch** command, select a world tile with an existing local map, and choose **Sortie over [map]**. You can also issue this order to an aircraft already in flight.
3. The aircraft travels to that tile and then circles its neighboring tiles. It stays selectable on the world map and continues consuming its normal flight fuel.
4. Select it on the world map and click **Strafing run**. Once the aircraft is overhead, this opens the sortie map. Click a visible, unroofed starting square, then move the mouse and click an endpoint to set the line's direction and length. Horizontal, vertical, diagonal and intermediate angles work. Right-click or Escape cancels either selection step.
5. The attack line covers up to **15 consecutive grid squares**, including the start, and sends up to **30 impacts** across that line. Impact count scales with the selected line: `floor(30 × selected cells / 15)`, with at least one impact for a valid line. A one-cell line allows 2 impacts, a seven-cell line allows 14, and a full 15-cell run allows all 30. Steel can reduce those numbers further. One shared outline shows the possible blast area around that line, including scatter and explosion radius; there are no overlapping preview rings. Roofed or fogged impact centers are skipped without extending the line. The label displays impact count and steel cost.
6. After a two-second approach at normal game speed, explosions sweep from the start toward the endpoint, six game ticks apart. Unguided impact centers scatter by up to **15 cells** around their aim squares, while the future laser-guided baseline is **2.5 cells**, making unguided attacks much wider and less uniform. Shots still progress forward. Each actual impact consumes **one steel from the jet's inventory**. With no steel, the attack command is disabled. If steel is removed during a run, firing stops when it cannot afford another shot. The aircraft stays airborne, with a ten-second cooldown after the planned pass.
7. Firing audio overlaps between shots; a separate sound begins at the end of the pass and continues afterward. These sounds play at a consistent level while viewing the target map, independent of camera distance. They replace the stock bomb sounds for this attack. Both are placeholders you can replace (see below).
8. Use **Land on this map** to choose a normal landing position and rotation, or use **Launch** to travel or land elsewhere. Canceling target selection leaves the sortie running and consumes no steel. Committing a new flight or landing order cancels the remaining strafe impacts; only shots already fired have consumed steel.

Only maps that already exist can be selected; a sortie never generates a map. If its map disappears, attacks stop and the aircraft keeps circling until redirected. A warning appears at 20% fuel. There is no automatic return or free hovering: fuel exhaustion uses Vehicle Framework's crash behavior.

The current pass uses vanilla explosion damage and can hurt friendly pawns and buildings. The shared outline is the maximum possible blast footprint; each pass hits a randomized portion of it. Flight fuel settings are unchanged, so the current Warthog configuration burns fuel quickly. `maxRunCells`, `steelPerCell`, `scatterRadius`, damage, timing, sound references and the fuel warning threshold are editable in `JumbosJets/1.6/Defs/VehicleDefs/Tier4/Warthog/Warthog_VehiclePawn.xml`. Custom jet flyover animation remains future work.

Lower `scatterRadius` for more accurate runs; **zero** fires precisely along the aimed squares. The current laser-guided accuracy value is `laserGuidedScatterRadius=2.5`. Edit the values in the Warthog XML: XML overrides the C# field defaults when RimWorld loads the vehicle. `CompSortie.GetScatterRadius(Map, IntVec3)` is a virtual hook for future ground-support/laser-guidance integration to return that tighter value per target. Ground laser designation itself is not implemented yet. Both the preview and confirmed run use this hook. Random positions are chosen once on confirmation and saved, so hovering does not reroll shots and loading a save does not change the queued run.

Replace `JumbosJets/Sounds/Sorties/StrafeFire.wav` and `StrafeTail.wav` with your own sounds, then rebuild and restart RimWorld. The first plays per shot; the second plays once after the pass. See [audio replacement notes](JumbosJets/Sounds/Sorties/README.md) for timing and volume settings.

Saved angled runs preserve their selected squares, progress and cooldown. An unfinished centered burst from the older implementation is canceled when loading; the aircraft's sortie and cooldown remain intact. Already-playing audio is not restored on load.

## Build and local testing

Build with .NET Framework 4.8 MSBuild and the local dependency DLLs in `resources/`:

```powershell
msbuild JumbosJets.sln /p:Configuration=Debug
msbuild JumbosJets.sln /p:Configuration=Release
git diff --check
```

Both configurations stage the complete mod in `JumbosJets/bin/Debug/FinalModFolder/`. Copy that folder to RimWorld's `Mods` directory. Enable Harmony, Vehicle Framework and Vanilla Vehicles Expanded before Jumbos Jets. The staging folder includes the new compiled assembly; copying only the source XML folders is insufficient.

See [sortie validation](docs/sortie-validation.md) for the checks performed and the in-game test checklist.

## Remaining ideas

Ideas
- Sorties / Air Strikes (first implementation above; in-game validation pending)
- Add Jet Noises
- Add the BRRRTT Noise
- Laser Guided Munitions
- Radio in Attack
- Enemy Raids can have air support
- Can shoot down enemey planes with aa and stinger, Can crash on your tile
- AA Turret (SAM, CIWS, WWII flac cannon)
- Event for Hard Wind that blows jets and damages them/moves them
- Wreck for Research
- Jet Fuel and Jet Fuel Refining 
- Ground Loading Boxes like from top gun
- Loading Big Bomb
- Crafting a big bomb
- Aircraft Engine
- aluminum?
- Safe Eject Piolet on crashes
- use of jets attracts mechanoids and mech fights



Planes
- Spooky II
- F-14 Tomcat
- A-1 Sky Raider



Other Mod Support
- Deep Chem fuel?
- CE - Combat Extended?
