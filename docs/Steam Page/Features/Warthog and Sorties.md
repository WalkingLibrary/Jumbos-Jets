# The Warthog and aerial sorties

## Steam copy draft

Build the Warthog, a heavy close-air-support aircraft that can remain over a map and attack ground targets without landing. A qualified pilot keeps the aircraft operating while it circles overhead; the aircraft continues to consume fuel while it stays in the air.

When the Warthog is on station above an existing map, order a strafing run and draw an attack line. The run can travel in any direction, including diagonally. The aircraft makes a sequence of explosive impacts along the line and spends steel from its inventory for each impact. Longer runs allow more impacts, up to the aircraft's configured maximum.

## Image brief

Create a wide feature graphic with the Warthog flying above a colony map. Show a simple map overlay with a selected attack line and one shared outline around the possible impact area. Add a small world-map inset to communicate that the aircraft stays airborne over a selected map. Keep the pilot, aircraft silhouette, attack direction, and steel requirement readable at thumbnail size.

## Current implementation facts

- Only the Warthog is currently included as an aircraft.
- The sortie is available while the Warthog is flying, on station over a map, and has an operating pilot.
- The map must already exist. The sortie does not create a new map.
- The player selects a starting cell and endpoint; the line can be horizontal, vertical, diagonal, or another angle.
- Current Warthog values: 30-cell maximum attack line, 20 impacts maximum, 53 damage per impact, and one steel per impact.
- A short line limits the available impact count proportionally. Steel in the vehicle inventory can limit it further.
- Current scatter is 7 cells when unguided and 2.5 cells when guided. Each impact still uses vanilla explosion damage and can damage friendly pawns and structures.
- Flight fuel is consumed while remaining in orbit; there is no free hover or automatic return behavior.

## Release checks

Verify the attack line, impact count, inventory steel cost, damage, and fuel use in the final release build. The README contains older sortie values that do not match the current Warthog XML.
