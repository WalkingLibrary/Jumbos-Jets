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
- `Tests/Test-Strafing.ps1`: 2,629 checks passed against production geometry and ammunition helpers using framework stand-ins. Covers all line directions, shallow angles, truncation, precise/scattered runs, unique impact centers, forward progression, a connected shared footprint containing every possible blast, map edges and blocked cells, plus inventory consumption.
- `Tests/Test-SortieFlight.ps1`: 17 lifecycle regression checks passed.
- Both placeholder WAV files and SoundDef clip paths were validated and included in the staged mod.
- Staged assembly directory contains `JumbosJets.dll` and its symbols, without bundled game or dependency DLLs.
- The user reports that the earlier null-reference issue appears resolved, but the initial audio was inaudible and the preview rings overlapped. The installed clips and definitions exist, and the inspected game log contains no missing-clip errors. The earlier audio combined low sample levels with low SoundDef volume and positional attenuation. Playback now uses louder, map-scoped camera audio and disables stock bomb audio for sortie impacts. Audible playback and the new area/scatter behavior still require in-game testing. Managed checks do not exercise Unity or apply the patches in a running game.

## In-game checklist

The cleared-aircraft regression also has a standalone check:

```powershell
./Tests/Test-SortieFlight.ps1
./Tests/Test-Strafing.ps1
```

All 17 checks pass. The test compiles the production flight guard against small framework stand-ins, reproduces the null dereference in `Flying`, and checks that cleared/destroyed aircraft, missing launchers/paths and stopped flights skip movement. Valid sorties and ordinary flight/landing orders still proceed. It does not run Unity or simulate an actual in-game crash.

The movement prefix now skips both sortie handling and the original `MoveForward` after a flight ends during the same tick. Tick, order and targeting entry points also check the aircraft references before using them. In-game fuel exhaustion remains part of step 10 below.

Use a disposable RimWorld 1.6 save with the dependencies above. Record the game version, Workshop versions and results alongside screenshots of the orbit, target preview, impacts and landing.

1. Start the game and verify there are no missing classes, XML errors, texture errors or Harmony patch errors. Check Warthog construction/spawning and all four orientations and color masks.
2. Confirm an uncrewed aircraft cannot start a sortie. Board a capable pilot, fuel the aircraft, and confirm the normal runway restriction still applies.
3. Select a generated home map and another existing map as sortie destinations. Confirm a tile without a local map offers no sortie order and creates no map.
4. Launch to the target and confirm the attack button is disabled during transit. On arrival, observe several complete world-map circuits without landing or repeated arrival messages. Check that fuel decreases once at the normal framework flight rate.
5. Load at least 30 steel, select the world-map aircraft and click **Strafing run**. Click a start, then an endpoint. Test horizontal, vertical, diagonal and shallow-angle lines in both directions. Confirm a long 50-square selection limits its aimed line to 15 squares and a full run schedules 30 impacts (two per aim square). Check there is one shared outline around the possible blast area, with no internal rings. Shots should land unevenly inside it, progress forward, and respect the cooldown.
6. Test a map-edge target, a roofed square, a fogged square, changing maps while targeting, and canceling targeting. Invalid targets must not order an attack; canceled targeting must not stop the orbit or spend the attack cooldown.
7. Queue a pass, then order a landing or a different flight before all impacts complete. Remaining impacts must stop. Repeat with the normal framework landing command and **Land on this map**, including canceling landing selection and rotating the landing footprint.
8. Save and reload during transit, while circling, during the approach, between impacts and during cooldown. Verify the same aircraft and crew persist, flight fuel continues decreasing, the orbit resumes, and no completed impacts repeat or cooldown resets occur.
9. Remove the target map while circling or while a pass is pending. Confirm the aircraft warns once, stops attacking, does not regenerate the map, and can be redirected using **Launch**.
10. Reduce fuel below 20%, then exhaust it. Confirm the warning is not repeated each tick or after reload, and the framework crash finishes with one aircraft and its crew, without duplicate world objects or continued attacks. Also test loss or incapacitation of the pilot during a pass.
11. Run two Warthogs on separate sorties. Their targets, cooldowns and cancellation must remain independent. Verify another vehicle type retains its normal flight menus and behavior.
12. With zero steel, verify the attack is disabled. With three steel, verify the preview is limited to three impacts. Check exactly one steel is consumed per fired impact, across multiple stacks, with other cargo unchanged. Remove steel during the approach or pass: unfunded shots must not fire. Cancel targeting or change orders: unspent steel must remain aboard. Set `steelPerCell` to two and repeat the insufficient/multi-stack cases.
13. Hear the firing clips overlap and the tail continue after the last shot. Pan and zoom the camera: sortie audio should remain audible on the target map. Other maps should not start these sounds. Stock bomb audio should not mask the clips. Try two aircraft on the same map, a single-cell run, fast game speeds and cancellation. Check that missing/blocked impact cells do not consume steel or play firing sounds. Replace the two WAV files, rebuild and restart to verify custom clips load without code changes.
14. Save and reload between impacts of an angled run; check its remaining cells, ammunition and cooldown. Loading an older save with a pending centered burst should cancel only that obsolete burst. Audio should not replay completed shots on load.
15. Repeat runs with `scatterRadius=2.5`, `laserGuidedScatterRadius=1.25`, then `scatterRadius=0` (rebuild/restart between settings). The 2.5 setting should be visibly wider than the 1.25 laser-guided value; zero should tighten the outline and put impacts on aim squares. Confirm that already-queued shots keep their saved positions. Future laser support should reduce scatter through `CompSortie.GetScatterRadius`, so preview and firing agree.

The implementation uses vanilla explosive damage, existing flight fuel settings, steel ammunition and replaceable synthesized audio. Custom flyover visuals and other combat-mod integration remain future work.
