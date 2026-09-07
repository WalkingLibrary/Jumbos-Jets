# Sortie validation

## Local validation — September 7, 2026

The implementation targets the DLLs supplied in `resources/`, rather than assuming that the current upstream Vehicle Framework source matches this installation.

| Dependency | Local assembly version |
| --- | --- |
| RimWorld `Assembly-CSharp.dll` | 1.6.9340.29816 |
| Vehicle Framework `Vehicles.dll` | 1.6.0.0 |
| SmashTools | 1.6.0.0 |
| Harmony | 2.3.3.0 |
| Vanilla Vehicles Expanded | 1.0.0.0 |

Assembly versions do not identify the exact Workshop release. Record the installed mod versions from the game's mod list when performing the manual checks below.

- Debug and Release builds: passed using Rider's bundled MSBuild and the .NET Framework 4.8 targeting pack.
- XML parsing and `git diff --check`: passed.
- A temporary managed smoke-check executable loaded the production assembly and shipped sortie XML. Its 28 assertions passed: XML field binding, comp type, centered strafe footprint, map-edge coordinate behavior, and target/parameter signatures for all five Harmony patches against the local dependency DLLs.
- Staged assembly directory contains `JumbosJets.dll` and its symbols, without bundled game or dependency DLLs.
- In-game startup, flight, targeting, damage, save/load, and crash behavior: **not yet tested**. The managed checks do not exercise Unity or apply the patches in a running game.

## In-game checklist

The cleared-aircraft regression also has a standalone check:

```powershell
./Tests/Test-SortieFlight.ps1
```

All 17 checks pass. The test compiles the production flight guard against small framework stand-ins, reproduces the null dereference in `Flying`, and checks that cleared/destroyed aircraft, missing launchers/paths and stopped flights skip movement. Valid sorties and ordinary flight/landing orders still proceed. It does not run Unity or simulate an actual in-game crash.

The movement prefix now skips both sortie handling and the original `MoveForward` after a flight ends during the same tick. Tick, order and targeting entry points also check the aircraft references before using them. In-game fuel exhaustion remains part of step 10 below.

Use a disposable RimWorld 1.6 save with the dependencies above. Record the game version, Workshop versions and results alongside screenshots of the orbit, target preview, impacts and landing.

1. Start the game and verify there are no missing classes, XML errors, texture errors or Harmony patch errors. Check Warthog construction/spawning and all four orientations and color masks.
2. Confirm an uncrewed aircraft cannot start a sortie. Board a capable pilot, fuel the aircraft, and confirm the normal runway restriction still applies.
3. Select a generated home map and another existing map as sortie destinations. Confirm a tile without a local map offers no sortie order and creates no map.
4. Launch to the target and confirm the attack button is disabled during transit. On arrival, observe several complete world-map circuits without landing or repeated arrival messages. Check that fuel decreases once at the normal framework flight rate.
5. From the selected world-map aircraft, click **Strafing run**, inspect the preview and choose a square. Confirm a delayed west-to-east sequence centered on the target, damage to nearby targets, and the cooldown. Confirm the aircraft remains selectable and can make another pass.
6. Test a map-edge target, a roofed square, a fogged square, changing maps while targeting, and canceling targeting. Invalid targets must not order an attack; canceled targeting must not stop the orbit or spend the attack cooldown.
7. Queue a pass, then order a landing or a different flight before all impacts complete. Remaining impacts must stop. Repeat with the normal framework landing command and **Land on this map**, including canceling landing selection and rotating the landing footprint.
8. Save and reload during transit, while circling, during the approach, between impacts and during cooldown. Verify the same aircraft and crew persist, flight fuel continues decreasing, the orbit resumes, and no completed impacts repeat or cooldown resets occur.
9. Remove the target map while circling or while a pass is pending. Confirm the aircraft warns once, stops attacking, does not regenerate the map, and can be redirected using **Launch**.
10. Reduce fuel below 20%, then exhaust it. Confirm the warning is not repeated each tick or after reload, and the framework crash finishes with one aircraft and its crew, without duplicate world objects or continued attacks. Also test loss or incapacitation of the pilot during a pass.
11. Run two Warthogs on separate sorties. Their targets, cooldowns and cancellation must remain independent. Verify another vehicle type retains its normal flight menus and behavior.

The first implementation deliberately uses a fixed west-to-east pass, vanilla explosive damage, existing flight fuel settings and no ammunition resource. Custom flyover visuals, aircraft audio, ammunition and other combat-mod integration remain future work.
