# VirtualEnvironment

A Unity 6 project containing a low-poly city scene and a prototype traffic simulation. Vehicles are spawned into configured lanes, follow destination transforms, respond to traffic-light counters, and can be routed through trigger-based path choices.

**GitHub:** [iamchinmay03/VirtualEnvironment](https://github.com/iamchinmay03/VirtualEnvironment)

## Project status

The city traffic demo is included at `Assets/SimplePoly City - Low Poly Assets/Demo/SimplePoly City - Low Poly Assets_Demo Scene.unity`. It contains the city, a configured vehicle spawner, traffic-light cycles, counters, and path triggers. This scene is the first scene in the build settings.

`Assets/Scenes/SampleScene.unity` is the original, minimal template scene; it is retained as a separate scene and is not the traffic demo.

The webcam/ONNX vehicle-counting code is an optional experiment. It is not required by the traffic demo and is not connected to its traffic-light logic.

## Requirements

- Unity Editor **6000.3.11f1** (Unity 6.3), as recorded in `ProjectSettings/ProjectVersion.txt`.
- A supported desktop platform for the included city demo.
- Internet access the first time the project is opened, so Unity can resolve packages in `Packages/manifest.json` and `Packages/packages-lock.json`.
- For optional webcam inference only: a working webcam, camera permission, GPU compute support, a compatible Unity Inference Engine model asset, and a compatible inference output.

## Open and run

1. Clone the repository:

   ```sh
   git clone https://github.com/iamchinmay03/VirtualEnvironment.git
   ```

2. In Unity Hub, select **Add project from disk** and choose the cloned `VirtualEnvironment` folder.
3. Open it with Unity **6000.3.11f1** and let package resolution and asset import finish.
4. Open `Assets/SimplePoly City - Low Poly Assets/Demo/SimplePoly City - Low Poly Assets_Demo Scene.unity` if it is not already open.
5. Press **Play** in the Unity Editor to run the city and traffic simulation.
6. To build, use **File > Build Profiles** (or **File > Build Settings**, depending on the editor UI). The demo scene is enabled and listed first; `SampleScene` remains available as a second scene.

No separate server, credentials, or external service is required for the traffic demo. The GitHub repository is the source-control remote; runtime traffic behavior is local to Unity.

## Traffic simulation

The included demo scene connects the simulation components and assets:

- A `VehicleSpawner` creates vehicles at its configured spawn points and assigns each vehicle the corresponding destination.
- Each lane has a `VehicleCounter` trigger. The spawner selects among lanes with at most two vehicles inside that counter area.
- Spawned prefabs use `VehicleMovement` to travel toward their current destination, stop at a red-light destination, and check for another vehicle ahead.
- `TrafficLight` cycles through yellow, green, yellow, and red. Green duration is based on the number of vehicles counted at the beginning of the cycle; it then starts its configured next light.
- `TrafficLightStarter` starts the configured traffic-light cycles when the scene begins.
- `NextPath` changes a vehicle's destination when it enters a route trigger, selecting from that trigger's configured possible paths.
- `DestroyOnTrigger` removes an object that enters its trigger and can be used at the end of a route.

The spawner's lane lists are aligned by index: `spawnPoints[i]`, `targetDestinations[i]`, and `vehicleCounters[i]` describe one lane. Each lane requires a valid reference for all three. Missing or incomplete lanes are skipped with a warning; a non-positive spawn interval or missing required setup disables the spawner with an error. The demo's spawn interval is configured in the Inspector.

## Main project files

| Path | Purpose |
| --- | --- |
| `Assets/SimplePoly City - Low Poly Assets/Demo/` | Authored city traffic demo scene and its lighting data. |
| `Assets/SimplePoly City - Low Poly Assets/Prefab/Vehicles/` | Vehicle prefabs used by the scene and spawner. |
| `Assets/Script/VehicleSpawner.cs` | Periodic, lane-aware vehicle spawning. |
| `Assets/Script/VehicleMovement.cs` | Vehicle destination following, traffic-light stopping, and forward vehicle detection. |
| `Assets/Script/VehicleCounter.cs` | Tracks vehicles within a trigger and applies the current signal state. |
| `Assets/Script/TrafficLight.cs` | Traffic-light cycle and optional on-object status text. |
| `Assets/Script/TrafficLightStarter.cs` | Starts configured traffic lights when the scene starts. |
| `Assets/Script/NextPath.cs` | Selects a new destination at a route trigger. |
| `Assets/Script/DestroyOnTrigger.cs` | Removes objects entering an exit/cleanup trigger. |
| `Assets/Script/DisableTrafficLightBoxes.cs` | Hides the renderer named `TrafficLight1Box`. |
| `Assets/Model/yolov5n.onnx` and `Assets/Model/CarCounter.cs` | Optional webcam/inference prototype; not needed by the default scene. |
| `Assets/Scenes/SampleScene.unity` | Minimal template scene, separate from the traffic demo. |
| `Assets/Settings/` | Universal Render Pipeline assets and volume profiles. |
| `Assets/Animation/` | Project animation assets, including traffic-light animation. |
| `Assets/TextMesh Pro/` | TextMesh Pro resources and examples. |
| `Assets/TutorialInfo/` | Unity template tutorial/readme assets. |
| `Packages/manifest.json` | Direct Unity package dependencies. |
| `Packages/packages-lock.json` | Resolved Unity package versions. |
| `ProjectSettings/` | Unity editor, player, rendering, input, and build-scene configuration. |

The C# command-buffer interface/implementation files in the repository root are outside Unity's `Assets` folder, so Unity does not import or compile them as part of this project.

## Extending the demo

### Add a vehicle prefab

1. Create or choose a vehicle prefab with a `VehicleMovement` component on its root object.
2. Ensure the prefab has suitable colliders and a `Vehicle` tag if it should be detected by `VehicleMovement`'s forward raycast.
3. Add the prefab to the `Vehicle Prefabs` list on `VehicleManager` in the demo scene.

### Add a spawn lane

1. Create a spawn transform and a destination transform oriented/positioned for the lane.
2. Create a trigger volume with a `VehicleCounter` component for that lane.
3. Add the spawn point, destination, and counter at the same index in their respective `VehicleSpawner` lists.
4. Keep the trigger positioned so a vehicle enters and exits it at the intended point in its route.

### Add or change a traffic signal

1. Put `TrafficLight` on the signal object and provide a `VehicleCounter` among its children.
2. Assign the next signal in the cycle through `Next Traffic Light`.
3. Set the `TrafficLightStarter` list to the starting signal(s) for the cycle.
4. If using the optional animation and status label, configure the expected `ToGreen`/`ToRed` Animator triggers and a child TextMesh Pro text component.

### Add a route choice

Add `NextPath` to a trigger volume and assign one or more transforms to `Possible Paths`. A vehicle entering the trigger will select a destination from that list. Add an exit trigger with `DestroyOnTrigger` if vehicles should be removed at the end of the route.

## Optional webcam inference

`Assets/Model/CarCounter.cs` captures frames from `WebCamTexture`, loads an assigned Unity Inference Engine `ModelAsset`, and attempts to count vehicle classes from the model output. To experiment with it, attach the component to an object, assign its model asset and `VehicleCounter`, grant the application camera permission, and use a compatible compute backend/model output. This is not used to spawn vehicles or control the default traffic-light cycle. Test it on the target device before relying on the reported counts.

## Packages and assets

Unity resolves the packages declared in `Packages/manifest.json`, including the Universal Render Pipeline, Input System, Test Framework, TextMesh Pro/UI-related modules, and Unity Inference Engine. Keep both package manifest and lock file under version control so collaborators open a consistent project.

The repository includes third-party city, font, and Unity sample assets. Check the relevant upstream license/asset terms before redistributing those assets. No root-level project license is included; a public repository alone does not grant additional reuse rights.

## Source control

The repository's `.gitignore` excludes Unity-generated directories, IDE state, and generated Visual Studio project files. Keep source assets, `.meta` files, scenes, packages, and project settings tracked; Unity `.meta` files preserve asset GUID references.

