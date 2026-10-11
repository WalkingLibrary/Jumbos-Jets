# Laser designation and guided runs

## Steam copy draft

Equip a drafted pawn with the laser designator and mark a target for close air support. The operator must hold position and maintain a clear line of sight while aiming. A green beam shows the active designation, and the marked area appears in green while choosing a Warthog attack line.

If any cell in the selected strafing line falls within 10 cells of an active designation, the whole run receives the guided accuracy bonus. The Warthog uses its tighter guided scatter value for every impact in that run. The operator can stop aiming by issuing a move order; confirmed runs keep the accuracy planned when they were accepted.

## Image brief

Make a two-panel comparison. In the first, show a pawn holding the designator with a visible green beam and a 10-cell-radius green marked area. In the second, show a Warthog attack line intersecting that area, with the full line highlighted to indicate that the whole run is guided. Add a small unguided-versus-guided scatter comparison only if it can be represented clearly and accurately.

## Current implementation facts

- The laser designator has a 120-cell targeting range and 25 uses before its charges are depleted.
- It is an equippable item. A drafted pawn aims it through its weapon command.
- Aiming consumes one charge, keeps the pawn stationary, requires line of sight, and can be canceled with a move order.
- The active designation is shared with the sortie targeting preview on the same map.
- Guidance radius is currently 10 cells. One line cell inside the radius guides the entire run.
- The guided scatter value is currently 2.5 cells; unguided scatter is 7 cells.
- Accuracy is selected when the run is confirmed. Later stopping the designation does not change queued impacts.

## Release checks

Confirm that the green beam, green area outline, charge use, move-to-cancel behavior, and whole-run accuracy bonus all work in the release build.
