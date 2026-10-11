# Steam page graphics

Four feature graphics and eight item cards are built from the `graphictemplate.png` layout and generated RimWorld-style scene panels. The finished PNGs are suitable for reviewing and adapting for the Steam Workshop page.

Extracted recipe and construction ingredient icons are available in [`Ingredients/`](Ingredients/README.md).

The vanilla Techprint icon is extracted as [`Techprint.png`](Techprint.png) and used on the Jet Propulsion research card.

| Graphic | Feature |
| --- | --- |
| `AerialSorties.png` | Warthog staying on station and drawing a steel-consuming attack line |
| `LaserGuidance.png` | Green laser designation guiding an entire nearby strafing run |
| `JetPropulsion.png` | Research and manufacture of Warthog aircraft components |
| `JetFuel.png` | Refining chemfuel into dedicated jet fuel |
| `Item-Warthog.png` | Warthog aircraft profile |
| `Item-LaserDesignator.png` | Equippable laser designator |
| `Item-JetEngine.png` | Jet engine |
| `Item-JetIntake.png` | Jet intake |
| `Item-JetExhaust.png` | Jet exhaust |
| `Item-JetFuelTank.png` | Jet fuel tank |
| `Item-JetParts.png` | General aircraft parts |
| `Item-JetFuel.png` | Dedicated jet fuel item |

`Source/` contains the generated scene panels before text was added. Run `Build-FeatureGraphics.ps1` from PowerShell to rebuild the finished PNGs after editing their copy or source panels. The original template is one directory up at `../graphictemplate.png`.
