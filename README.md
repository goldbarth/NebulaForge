# Nebula Forge

**An N-body orbit simulation, a procedural celestial body generator and a custom Unity editor toolchain in one project.**

Unity 2021.3.21f1 (URP) / C#

Nebula Forge is a sandbox for building and playing with your own solar systems.
You author celestial bodies with a custom editor window, shape their surfaces with layered procedural noise, tune their physical parameters (mass, surface gravity, initial velocity) and then watch the resulting N-body gravity simulation play out in real time.
An orbit debug display predicts and draws the future trajectory of every body before you ever press Play, so you can dial in stable orbits instead of guessing.

The almighty and likeable Sebastian Lague served as inspiration :)

---

## Table of Contents

- [Feature Overview](#feature-overview)
- [The Orbit Simulation](#the-orbit-simulation)
- [The Orbit Debugger](#the-orbit-debugger)
- [The Celestial Object Generator (Editor Tool)](#the-celestial-object-generator-editor-tool)
- [Procedural Surface Generation](#procedural-surface-generation)
- [Runtime Interaction (The "Game" Part)](#runtime-interaction-the-game-part)
- [Rendering and Effects](#rendering-and-effects)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Keyboard and Mouse Reference](#keyboard-and-mouse-reference)
- [Tuning Guide: Getting a Stable Orbit](#tuning-guide-getting-a-stable-orbit)
- [Known Limits and Ideas for the Future](#known-limits-and-ideas-for-the-future)
- [Credits](#credits)

---

## Feature Overview

| Area | What it does |
| --- | --- |
| **N-body gravity simulation** | Every celestial object attracts every other one. No parent/child orbit fakery, no keyframed paths. |
| **Orbit debugger** | Forward-simulates all bodies for N steps and draws the predicted trajectories in the Scene view, in edit mode and at runtime. |
| **Burst/Jobs orbit debugger** | A second implementation of the same prediction running as a Burst compiled `IJob` for a much larger step budget. |
| **Relative reference frames** | Predict orbits relative to any chosen central body, so a moon's path around its planet is readable instead of a smeared spiral. |
| **Celestial Object Generator window** | A custom `EditorWindow` (MVP architecture) to browse, edit and live-update every generated body in the scene. |
| **Object creation wizard** | A second window that spawns a fully wired celestial object (Rigidbody, collider, mesh holder, surface generator, material, gradient texture, line renderer) from a parameter form. |
| **Procedural planets** | Cube-to-sphere mesh generation with up to 3 stacked noise layers, simple and rigid noise filters, masking, and a gradient driven terrain shader. |
| **Mass/gravity coupling** | Mass and surface gravity are derived from each other via the body's radius, so editing one keeps the other physically consistent. |
| **Runtime editing UI** | Select a planet in play mode and change its velocity, surface gravity and mass with sliders while the simulation keeps running. |
| **Time control** | Global time scale, either fixed or manually driven by a slider. |
| **Collision events** | Planets explode on impact with a rogue body, and anything falling into the black hole disappears past the event horizon. |
| **Two cameras** | A top down overview camera and a free flying FPS camera with variable speed. |
| **URP visuals** | Shader Graph based terrain gradient, stylized water, cloud atmosphere, black hole distortion, VFX Graph explosions, particle star field, FPS counter and graph. |

---

## The Orbit Simulation

The core of the project is a straightforward but genuine N-body integrator.

`OrbitSimulation` runs in `FixedUpdate` and performs a two phase update over every `CelestialObject` registered in the `CelestialObjectManager`:

1. **Update all velocities.**
   For each body, the gravitational acceleration contributed by every *other* body is accumulated:
   `a = G * m / r^2 * direction`
   This is Newton's law of universal gravitation reduced to acceleration, so the attracted body's own mass cancels out.
2. **Update all positions.**
   Only after every velocity has been updated are the positions integrated with `rigidbody.MovePosition(position + velocity * dt)`.

Splitting the two phases matters.
If positions were moved while velocities were still being calculated, bodies later in the list would be pulled toward already-moved bodies, which breaks symmetry and injects energy into the system.

**Universe constants** live in a single struct so the simulation and the debugger can never drift apart:

```csharp
public const float GravitationalConstant = 0.01f;   // not 6.674e-11, on purpose
public const float PhysicsTimeStep       = 0.01f;   // fixed dt, also assigned to Time.fixedDeltaTime
```

The real gravitational constant was deliberately not used.
With `G = 6.674e-11` every mass in the scene would need to be scaled up absurdly to produce visible motion, so a scene friendly constant is used instead and all masses stay readable numbers.

**Mass and surface gravity are two views of the same thing.**
`CelestialObject` keeps them consistent using the body's generated radius:

```csharp
Mass           = surfaceGravity * radius * radius / G;   // set gravity -> mass follows
SurfaceGravity = G * Mass / (radius * radius);           // set mass    -> gravity follows
```

Set the surface gravity and the mass is recomputed, set the mass and the surface gravity is recomputed.
The `Rigidbody` mass is kept in sync as well.

A body tagged `BlackHole` has its velocity zeroed every step, which pins it as the anchor of the system while it still pulls on everything else.

---

## The Orbit Debugger

Getting a stable orbit by trial and error is painful.
The orbit debugger removes the guesswork by simulating the future and drawing it.

It copies each `CelestialObject` into a lightweight `VirtualBody` struct (position, velocity, mass) so nothing in the real scene is touched, then runs the exact same integration loop as the live simulation for a configurable number of steps and draws the resulting path segments in the body's own material colour.

Because both components are marked `[ExecuteAlways]`, the predicted orbits are visible in the Scene view while the game is *not* running.
You can drag a planet, change its initial velocity, and immediately see whether the resulting orbit closes into an ellipse, spirals into the sun or escapes the system entirely.

### Parameters

| Parameter | Effect |
| --- | --- |
| `Draw Orbits` | Master toggle for the prediction. |
| `Num Steps` | How far into the future to simulate. More steps means longer arcs and more cost. |
| `Time Step` | Integration step size of the prediction. Smaller is more accurate, larger reaches further for the same step count. |
| `Relative To Body` | Draw all paths in the reference frame of a chosen body instead of world space. |
| `Central Body` | The body that defines that reference frame. |

### Reference frames

Without a reference frame, a moon orbiting a planet that itself orbits a sun draws as a long looping spiral, which tells you very little about the moon's actual orbit.
With `Relative To Body` enabled, the offset the reference body travelled is subtracted from every drawn point and the reference body itself is pinned to its start position.
The result is the classic clean ellipse of the moon around its planet.

### Two implementations

- **`OrbitDebugDisplay`** is the plain managed version. Simple, easy to read, fine for moderate step counts.
- **`OrbitDebugDisplayJob`** packs the same work into `CalculateOrbitsJob`, an `[BurstCompile]` `IJob` operating on `NativeArray<VirtualBody>`, `NativeArray<Vector3>` draw points and `NativeArray<Color>` path colours.
  Burst compilation makes far higher step counts practical, which is what you want when you are checking whether an orbit is stable over many revolutions rather than over the next few seconds.

Both were kept intentionally close to each other in structure so the job version stays readable next to its reference implementation.

---

## The Celestial Object Generator (Editor Tool)

`Tools > Celestial Object Generator` opens the main authoring window.
It is a custom `EditorWindow` built on an MVP split:

- **Model:** `ObjectSettings`, a `ScriptableObject` holding everything about a body's appearance.
- **View:** `ObjectGeneratorWindow`, which owns the layout, the serialized properties and the GUI.
- **Presenter:** `WindowPresenter`, which brokers every change between the two via events so neither side calls into the other directly.

### Layout

**Sidebar.**
Every `ObjectSettings` asset under `Assets/ObjectInstances/` is discovered through `AssetDatabase` and listed as a button, grouped into *Terrestrial Bodies* and *Sphere Bodies*.
The currently attached asset is highlighted.
Clicking a button attaches that settings asset to the selected generator and regenerates the mesh immediately.
The sidebar width adapts to the longest asset name.

**Selection dropdown.**
A popup listing every celestial body currently registered in the scene.
Selecting an entry selects the object in the Hierarchy, and selecting an object in the Hierarchy moves the dropdown, so the window and the scene selection stay synchronised in both directions.

**General tab.**
Asset reference and material (read only, they are driven by the asset), plus `Resolution` (2 to 255) and `Radius` (0 to 1000).
For terrestrial bodies the elevation `Gradient` is exposed here too.

**Surface tab.**
Surface shape type, plus the full elevation layer stack.
For a solid sphere the tool tells you plainly that noise layers do not apply.

**Footer.**
`Update Asset` marks the settings dirty, saves the asset database and regenerates the object.
`Auto Update` re-generates on an interval (roughly every two seconds) so you can drag sliders and watch the planet reshape itself.
The tooltip is honest about the cost: auto update is expensive and should only be on while you are actively tuning.
`New` opens the creation window.

### Object types

| Type | Use for |
| --- | --- |
| `SolidSphere` | Plain spheres for shader driven bodies: suns, water shells, cloud atmospheres, gas giants. No elevation. The gradient becomes the surface colour. |
| `TerrestrialBody` | Planets with real displaced terrain, driven by the noise layer stack and coloured by elevation gradient. |

### Guard rails

The window refuses to edit while the editor is in play mode and says so, and it shows a friendly hint when no object is selected in the Hierarchy instead of throwing null references.

---

## Creating a New Celestial Object

`Tools > Create Celestial Object` opens a compact form that builds a complete, simulation ready body in one click.

Parameters: **Name**, **Type**, **Resolution**, **Radius**, **Gradient**, **Surface Gravity**, **Mass**, **Initial Velocity**.

What gets created:

- A root `GameObject` parented under the `OrbitSimulation`, carrying a `Rigidbody` (with Unity gravity disabled, since gravity is simulated manually) and a `CelestialObject` seeded with your mass, surface gravity and initial velocity.
- A `ColliderHolder` child with a `SphereCollider` matching the radius.
- A `MeshHolder` and `Mesh` child with a `MeshRenderer`.
- A `Surface` child with the `ObjectGenerator`, which generates the mesh immediately.
- A `LineRenderer` child pre-tinted with your gradient.
- A new folder under `Assets/ObjectInstances/<Name>/` containing the generated `ObjectSettings` asset, a material (URP Lit for a solid sphere, the `TerrainGradient_SG` Shader Graph for a terrestrial body) and the gradient texture exported as a PNG.
- A default noise layer, because at least one layer must exist for the generator to run.

Name collisions against existing scene objects and existing asset folders are both rejected with a clear message.

`CelestialObjectUpdater` watches `EditorApplication.hierarchyChanged` and keeps the `CelestialObjectManager` registry in sync, adding new bodies and removing deleted ones (including cleaning up their generated assets).

---

## Procedural Surface Generation

Planets are built as a **cube sphere**: six `TerrainFace` meshes, one per cube direction, each mapped onto the sphere.

Mapping is done with the seam free formulation rather than a plain `normalize()`, which keeps vertex density much more even across the face edges:

```csharp
x' = x * sqrt(1 - y²/2 - z²/2 + y²z²/3)
```

Elevation is then applied per vertex by the noise stack, and the final vertex position becomes `direction * radius * (1 + elevation)`.

### Noise layers

Up to **3 stacked layers**, each independently toggleable.

**Layer 1** is special: its raw value can be used as a **mask** for any later layer.
Enable `Use First Layer As Mask` on layer 2 or 3 and that layer's contribution is multiplied by the first layer's value.
This is how you get mountain ranges that only appear on continents rather than sprouting out of the ocean floor.

### Filter types

| Filter | Character |
| --- | --- |
| `Simple` | Smooth fractal Simplex noise. Rolling hills, continents, general terrain. |
| `Rigid` | Ridged noise with sharp creases. Mountain ridges, canyons, fractured crusts. Adds a `Weight Multiplier` that controls how pronounced the grooves are. |

### Per layer parameters

| Parameter | Range | Meaning |
| --- | --- | --- |
| `Seed` | 0 to 500 | Reproducible generation. Same seed, same world. |
| `Noise Strength` | 0 to 10 | How far the noise displaces the surface. |
| `Number Of Layers` | 1 to 8 | Fractal octaves stacked on top of each other. |
| `Base Roughness` | 0 to 5 | Frequency of the first octave. Higher means less smooth overall. |
| `Roughness` | -2.5 to 10 | Frequency multiplier between octaves (lacunarity). |
| `Persistence` | 0 to 5 | Amplitude falloff between octaves. Drives how detailed versus how dominant the fine noise is. |
| `Center` | Vector3 | Offset into the noise field. Shifts the whole pattern. |
| `Ground Level` | 0 to 5 | Raises the floor, pushing terrain below this level back into the sphere. Good for carving oceans. |

### Colouring

The generator tracks the minimum and maximum elevation produced across the whole mesh and feeds that range into the material as `_Elevation_Min_Max`.
The `Gradient` you author is baked into a 50 pixel wide texture and saved next to the asset, so the shader can map any elevation to a colour with the extremes always landing on the ends of your gradient no matter how you retune the noise.

---

## Runtime Interaction (The "Game" Part)

Press Play and the project turns into an interactive sandbox.

**Selection.**
`SelectionManager` raycasts against a dedicated layer, either from the mouse cursor or from the screen centre dot, depending on the `Using Center Dot Interaction` toggle.
Hovering a body swaps its material texture to the highlight texture, clicking selects it, and `TAB` deselects it and restores the original texture.

**Live property editing.**
Selecting a body opens a panel with sliders for **velocity magnitude**, **surface gravity** and **mass**.
Because mass and gravity are coupled through the radius, dragging one visibly moves the other.
Changes take effect on the running simulation instantly, so you can watch a stable orbit decay into a death spiral in real time by adding mass to the sun.

**Live readout.**
Position, velocity magnitude, surface gravity, mass and the current time scale are displayed for the selected body.

**Time control.**
A toggle switches between the fixed time scale and manual control, and a slider drives the manual value.

**Cameras.**
`F` switches between the top down overview camera and the free flying camera, swapping the matching canvas and locking or releasing the cursor at the same time.
The free camera flies with `WASD`, rises and falls with `Space` and `C`, rolls with `Q` and `E`, and its speed is scroll wheel adjustable between 400 and 1400.

**Guided hints.**
`HintManager` walks a first time user through the interaction loop step by step (hover, click, adjust, deselect, free view) and then gets out of the way permanently.

**Collisions.**
A planet struck by the rogue brown planet triggers a staged explosion VFX before the body is removed.
Anything that falls into the black hole is destroyed shortly after crossing the event horizon.

**Quit.**
`ESC` opens a confirm/cancel quit panel and pauses the simulation while it is open.

---

## Rendering and Effects

Everything runs on the **Universal Render Pipeline**.

- **Terrain gradient shader** (`Shader Graphs/TerrainGradient_SG`) maps generated elevation to the baked gradient texture.
- **Stylized water** as a separate solid sphere shell slightly larger than the terrain body.
- **Cloud atmosphere** as another transparent shell.
- **Black hole** shader with a distortion effect.
- **Stylized explosion** as both a prefab and a VFX Graph effect.
- **Particle star field** rendered on the transparent queue with a rotation lock, so the stars behave as a backdrop rather than a nearby particle cloud. Inspired by the skybox free star rendering technique from *Outer Wilds*.
- **Rotating asteroid belt**.
- **FPS counter and graph** overlay for keeping an eye on the cost of high step count orbit prediction.

---

## Architecture

```
Assets/_Scripts/
├── SolarSystem/          Universe constants, CelestialObject, OrbitSimulation, CelestialObjectManager
├── DebugDisplay/         OrbitDebugDisplay, OrbitDebugDisplayJob, VirtualBody, Jobs/CalculateOrbitsJob
├── Planet/               ObjectGenerator, TerrainFace, SurfaceGeneration/*, Editor/ObjectEditor
├── PlanetSettings/       ObjectSettings (ScriptableObject), NoiseSettings/* (filters, factory, Simplex)
├── CustomEditorWindow/   ObjectGeneratorWindow (View), WindowPresenter, Dependencies/* (tabs, sidebar, creation)
├── UX/                   SelectionManager, UserInput, DrawProperties, HintManager
├── CameraController/     CameraManager, FpsCameraController
├── CollisionHandler/     PlanetCollision, BlackHoleCollision
├── Particles/            StarParticlesManager, RotateBelt
└── HelpersAndExtensions/ Singleton, folder paths, MinMax, custom attributes and drawers, event managers
```

Notable patterns:

- **MVP** for the editor window, keeping GUI code free of asset mutation logic.
- **Factory** (`NoiseFilterFactory`) selecting the noise filter implementation behind the `INoiseFilter` interface.
- **Generic singleton** for the simulation, the body registry and the selection manager.
- **Static event managers** decoupling editor selection and play mode state changes from the components that react to them.
- **Custom property attributes** (`ConditionalHide`, `ReadOnly`) with matching drawers, so the inspector only shows the fields that are actually relevant for the chosen filter type.
- **`[ExecuteAlways]` / `[ExecuteInEditMode]`** throughout, which is what makes edit time orbit prediction and live regeneration possible in the first place.

---

## Getting Started

1. Open the project with **Unity 2021.3.21f1**.
2. Open `Assets/Scenes/SolarSystemSim.unity`.
3. Press Play to explore the existing system, or stay in edit mode to author one.

To build your own system:

1. `Tools > Create Celestial Object`, fill in the form, hit **Create**.
2. Select the new body and open `Tools > Celestial Object Generator` to shape its surface.
3. Select the `OrbitDebugDisplay` (or the job version) and enable `Draw Orbits`.
4. Adjust the body's `Initial Velocity` and mass until the predicted path in the Scene view closes into the orbit you want.
5. Press Play.

---

## Keyboard and Mouse Reference

| Input | Action |
| --- | --- |
| Mouse hover | Highlight a celestial body |
| Left click | Select the highlighted body and open its property panel |
| `TAB` | Deselect and close the panel |
| `F` | Toggle between top down and free view camera |
| `W` `A` `S` `D` | Fly (free view) |
| `Space` / `C` | Ascend / descend (free view) |
| `Q` / `E` | Roll left / right (free view) |
| Scroll wheel | Adjust fly speed (400 to 1400) |
| `ESC` | Open the quit confirmation and pause |

---

## Tuning Guide: Getting a Stable Orbit

The orbit debugger exists precisely for this, and the workflow is short:

1. **Anchor the system.** Give the central body a large mass and leave its initial velocity at zero. Tagging it `BlackHole` pins it hard.
2. **Place the orbiting body** at the distance you want.
3. **Set the initial velocity perpendicular to the line joining the two bodies.** A velocity pointing at or away from the centre gives you a plunge or an escape, never an orbit.
4. **Read the prediction.** Too slow and the path spirals inward, too fast and it opens into an escape trajectory. The target is a closed ellipse.
5. **Adjust mass or velocity, not both at once.** The two trade off against each other and changing both makes the result impossible to reason about.
6. **Raise `Num Steps`** once the orbit looks roughly right, to confirm it stays closed over many revolutions rather than only over the first one.
7. **Use `Relative To Body`** for moons and any nested orbit, otherwise you are reading the combined motion instead of the orbit you care about.

Reference values for the shipped scene: `G = 0.01`, physics step `0.01`, planet radii in the tens, masses in the hundreds to thousands, initial velocities in the single digits.

---

## Known Limits and Ideas for the Future

This was built under a real time budget, and it shows in places.
Being upfront about that:

- The integrator is semi-implicit Euler. It is stable enough for the scales here, but a Verlet or Runge-Kutta integrator would hold long running orbits far better.
- Noise layers are capped at 3 by the editor window.
- Terrain meshes have no colliders. Unity's convex mesh colliders cap out at 255 vertices, which a generated planet blows past immediately, so collision uses a sphere approximation.
- The creation window still warns that celestial object creation is under development.
- Editing is disabled during play mode, so authored changes have to happen in edit mode.
- Orbit prediction cost grows with body count times step count. The Burst job version pushes that ceiling a long way out, but it is still the most expensive thing in the scene.

---

## Credits

### Sebastian Lague - Inspiration:
- [Sebastian Lague](https://www.youtube.com/user/Cercopithecan) - Youtube, Main Channel
    - [Coding Adventure: Solar System](https://www.youtube.com/watch?v=7axImc1sxa0&list=PLFt_AvWsXl0ehjAfLFsp1PGaatzAwo0uK&index=10) - Youtube
    - [Procedural Planets](https://www.youtube.com/watch?v=QN39W020LqU&list=PLFt_AvWsXl0d8k4NUZU2ZdU6f2Nc6oZ4h) - Youtube
    - [Procedural Planets](https://github.com/SebLague/Procedural-Planets) GitHub - MIT License

### Infallible Code:
- [Tutorial: Selecting Objects with Raycast](https://www.youtube.com/watch?v=Hj7AZkyojdo) - Youtube

### Nicholas Veselov:
- [FPS Counter and Graph](https://nvjob.github.io/unity/nvjob-fps-counter-and-graph) - nvjob.github.io - MIT License

### Mobius Digital:
- [Star Rendering With No Skybox](https://www.youtube.com/watch?v=Ipl7EVDsExk) - the star field technique from *Outer Wilds*

### DK:
- [DK2go](https://github.com/DK2go) Who was involved in the beginning, but had to pull out due to a job offer.

### Reference reading:
- [Gravitational constant](https://en.wikipedia.org/wiki/Gravitational_constant)
- [Orbital speed](https://en.wikipedia.org/wiki/Orbital_speed)
- [Two-body problem](https://en.wikipedia.org/wiki/Two-body_problem)
- [Three-body problem](https://en.wikipedia.org/wiki/Three-body_problem)
- [N-body simulation](https://en.wikipedia.org/wiki/N-body_simulation)

---

## License

See [LICENSE](LICENSE).
