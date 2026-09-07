# Jumbos Jets

A RimWorld 1.6 aircraft mod using Vehicle Framework and Vanilla Vehicles Expanded.

## A-10 sorties

The first sortie implementation is ready for in-game testing.

1. Fuel the Warthog, board a capable pawn in its pilot seat, and clear its runway.
2. Use the normal **Launch** command, select a world tile with an existing local map, and choose **Sortie over [map]**. You can also issue this order to an aircraft already in flight.
3. The aircraft travels to that tile and then circles its neighboring tiles. It stays selectable on the world map and continues consuming its normal flight fuel.
4. Select it on the world map and click **Strafing run**. Once the aircraft is overhead, this opens the sortie map. Pick a visible, unroofed square; the preview shows the impact areas.
5. After a two-second approach at normal game speed, nine explosions sweep west to east through the selected square. Each impact is two cells apart, with six game ticks between impacts. The aircraft stays airborne, with a ten-second cooldown after the pass.
6. Use **Land on this map** to choose a normal landing position and rotation, or use **Launch** to travel or land elsewhere. Canceling target selection leaves the sortie running. Committing a new flight or landing order cancels the remaining strafe impacts.

Only maps that already exist can be selected; a sortie never generates a map. If its map disappears, attacks stop and the aircraft keeps circling until redirected. A warning appears at 20% fuel. There is no automatic return or free hovering: fuel exhaustion uses Vehicle Framework's crash behavior.

The current pass uses vanilla explosions and can hurt friendly pawns and buildings. Impact centers outside the map or under roofs are skipped. It has no ammunition requirement, custom jet flyover animation, or BRRRT sound yet. Flight fuel settings are unchanged, so the current Warthog configuration burns fuel quickly. Burst damage, spacing, timing and the fuel warning threshold are editable in `JumbosJets/1.6/Defs/VehicleDefs/Tier4/Warthog/Warthog_VehiclePawn.xml`.

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

