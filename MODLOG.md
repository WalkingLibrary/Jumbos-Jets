# RimWorld 1.6 rendering notes

- `MoteDualAttached` is used by vanilla for activity icons (for example `Mote_Clean` and `Mote_Sow`). Its two links do not draw a line between them.
- A spawned mote's `DrawAt` runs in the map's world render pass. `LaserBeamMote.DrawAt` uses `GenDraw.DrawLineBetween` there, so the beam follows camera movement.
- The laser component owns the beam and a sustained `Stance_Cooldown`. Its draw method checks that the pawn is still drafted and holding that stance; canceling aim or undrafting hides the beam immediately.
- RimWorld locks loaded mod DLLs. Verify the active mod DLL hash matches the built DLL after the game exits before testing changes.
