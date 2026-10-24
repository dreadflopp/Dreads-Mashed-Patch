# Model and bounds ownership decisions

[Internal references](README.md) · [ModelBoundsHandler](../../DreadsMashedPatch/PropertyHandlers/General/ModelBoundsHandler.cs)

An accepted model-filename change brings bounds from the same source override.
A bounds-only edit can forward independently. Model metadata and alternate-texture
changes do not claim bounds. This is conditional ownership, not one unconditional
atomic model-plus-bounds value.

| Record/surface | Shared behavior | Specialized boundary and reason |
|---|---|---|
| Ordinary ModelAndBounds registrations | Shared model copying and filename/bounds coordination | Other record fields retain their own handlers. Use executable registrations/inventory for the complete record list rather than duplicating it here. |
| ARMO WorldModelAndBounds | Gendered dropped/world-model filenames coordinate bounds | Icons, armature and metadata retain separate policies. Worn models do not define dropped-object extents. |
| SLGM | Shared model with optional bounds | Preserve nullable/absent bounds rather than inventing required extents. |
| STAT | Main model coordinates bounds | LOD remains its own value. |
| WEAP | Main world model coordinates bounds | ScopeModel does not own the world-object extents. |
| TREE | Uses the same model/bounds handler | Its default forwarding restriction resolves this context from the effective winner; disabling it restores ordinary model/bounds forwarding. Wind data is a separate value with no geometry compatibility heuristic. |

Do not add another static-only model copier or light-only bounds writer. One
implementation path must apply each selected property. Tests:
[ModelBoundsHandlerTests](../../DreadsMashedPatch.Tests/ModelBoundsHandlerTests.cs),
[ModelHandlerRegressionTests](../../DreadsMashedPatch.Tests/ModelHandlerRegressionTests.cs),
[AppearanceAndTreeForwardingTests](../../DreadsMashedPatch.Tests/AppearanceAndTreeForwardingTests.cs).
