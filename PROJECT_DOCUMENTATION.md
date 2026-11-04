# 3D PLATFORM RUNNER GAME USING UNITY
## Complete Game Development & Technical Architecture Manual
**A Comprehensive Guide to Game Design, C# Scripting, Kinematics, Procedural Audio, and Systems Architecture**

---

### Course & Academic Information
* **Course Title**: Game Design Using Unity
* **Level**: B. Tech III Year, I Semester — Computer Science & Engineering (CSE)
* **Institution**: Department of Computer Science & Artificial Intelligence, SR University, Warangal, Telangana, India
* **Engine & Version**: Unity 2021.3.15f1 LTS (64-bit)
* **Scripting Language**: C# (.NET Standard 2.1 / C# 9.0)

---

# TABLE OF CONTENTS
1. [Project Analysis & Asset Inventory](#1-project-analysis--asset-inventory)
2. [Game Overview & Core Mechanics](#2-game-overview--core-mechanics)
3. [Complete Project Folder Structure](#3-complete-project-folder-structure)
4. [Complete C# Script Explanations](#4-complete-c-script-explanations)
5. [Script-to-Action Mapping Matrix](#5-script-to-action-mapping-matrix)
6. [Animation System: Procedural Kinematics vs. Animator](#6-animation-system-procedural-kinematics-vs-animator)
7. [Unity GameObjects & Component Architecture](#7-unity-gameobjects--component-architecture)
8. [Step-by-Step Game Development Process (Steps 1–18)](#8-step-by-step-game-development-process)
9. [Script Communication & Data Flow](#9-script-communication--data-flow)
10. [Unity Inspector Configuration Guide](#10-unity-inspector-configuration-guide)
11. [Debugging & Troubleshooting Guide](#11-debugging--troubleshooting-guide)
12. [Systematic Testing & Results Matrix](#12-systematic-testing--results-matrix)
13. [Existing Features vs. Future Improvements](#13-existing-features-vs-future-improvements)
14. [List of Missing or Incomplete Implementations](#14-list-of-missing-or-incomplete-implementations)

---

# 1. PROJECT ANALYSIS & ASSET INVENTORY

### 1.1 Technical Specifications & Engine Environment
* **Unity Editor Version**: `2021.3.15f1` LTS (Revision `e8e88683f834`).
* **Target Platforms**: Standalone PC (Windows 64-bit / macOS / Linux) & Mobile (Android / iOS).
* **Render Pipeline**: Built-in Render Pipeline (Standard Forward Rendering with Soft Shadow calculations and custom exponential fog).
* **Physics Engine**: Unity 3D PhysX Engine (utilizing kinematic `CharacterController` capsule sweep collision paired with trigger volumes for items).
* **Audio Architecture**: Real-time Procedural Audio Synthesis via native C# PCM wave generation buffers without external audio assets.
* **Input Architecture**: Dual Input System combining Legacy Input Manager (`Input.GetAxisRaw`, `Input.GetKeyDown`, `KeyCode`) with Pointer/Touch coordinate differentials for swipe detection.

### 1.2 Package Dependencies (`Packages/manifest.json`)
The project utilizes lean, production-ready Unity modules:
* `com.unity.ugui` (`1.0.0`): Unity UI Canvas system used for HUD, floating text popups, shop interfaces, and overlays.
* `com.unity.textmeshpro` (`3.0.6`): High-definition SDF font rendering engine.
* `com.unity.modules.physics` (`1.0.0`): PhysX 3D subsystem (Colliders, Raycasts, CharacterController).
* `com.unity.modules.audio` (`1.0.0`): Low-latency DSP audio playback system.
* `com.unity.modules.particlesystem` (`1.0.0`): Shuriken particle system for environment atmospheres.
* `com.unity.modules.animation` (`1.0.0`): Transform hierarchy and procedural joint matrix evaluation.
* `com.coplaydev.unity-mcp` (Beta): Development workflow integration bridge.

### 1.3 Complete Inventory of Game Assets

#### A. Scenes (`Assets/Scenes/`)
* `SampleScene.unity`: The primary game scene containing the complete runtime environment, including dynamic directional lighting, follow camera, pooled track hierarchy, UI Canvas, and runtime singleton manager coordinators.

#### B. Complete C# Scripts Inventory (31 Scripts Total)
* **Core Player & Kinematics**:
  1. `PlayerController.cs`: Main player locomotion, 3-lane lateral stepping, jumping, fast-falling, slide hitbox shrinking, power-up timers, and 3-hearts health system.
  2. `BoyCharacterAnimator.cs`: Biomechanical procedural skeletal kinematics (arm pumping, elbow bends, knee strides, ankle roll, pelvic twist, landing squash/stretch).
  3. `PlayerTrailEffect.cs`: Neon ribbon trail renderer attached to the player with dynamic color/width mapping based on active power-ups and velocity.
  4. `PlayerGroundShadow.cs`: Contact ambient occlusion shadow controller.
* **Endless World & Track Systems**:
  5. `TrackSpawner.cs`: Endless modular track instantiator, segment recycler, and object pool manager.
  6. `TrackSegment.cs`: Individual 30-meter track chunk controller containing beveled roads, lane dividers, curbs, mountain ridges, and prop hooks.
  7. `WorldManager.cs`: Biome transition controller handling skybox color, ambient lighting, directional sun angle, and fog density.
  8. `WorldType.cs`: Global enumeration of the 5 biomes (`Grassland`, `Jungle`, `Desert`, `IceWorld`, `Mechanical`).
  9. `WorldThemeData.cs`: Scriptable data structure holding biome materials, lighting colors, custom obstacles, and side prop prefabs.
* **Obstacles & Environmental Hazards**:
  10. `Obstacle.cs`: Core obstacle component with collision detection, hazard classification, and invulnerability checks.
  11. `MovingObstacle.cs`: Dynamic patrol hazard that ping-pongs or oscillates horizontally across lanes.
  12. `RobotObstacle.cs`: Mechanical biome enemy featuring floating hover dynamics, scanning arm joints, and pulsing red eye glow.
* **Collectibles, Currency & Power-Ups**:
  13. `Coin.cs`: Standard currency pickup with continuous rotation, bobbing, and magnetic attraction mechanics.
  14. `Gem.cs`: Rare diamond pickup providing high scores and revive currency.
  15. `PowerUpType.cs`: Enum defining `Magnet`, `Shield`, `SpeedBoost`, `ScoreMultiplier`, and `Invincibility`.
  16. `PowerUpItem.cs`: Interactive 3D pickup crystal triggering specific power-up buffs on the player.
* **Game Management, UI & Audio**:
  17. `GameManager.cs`: Master state machine (`WorldSelect`, `Playing`, `Paused`, `GameOver`), score tracking, high-score serialization, and revives.
  18. `UIManager.cs`: Screen-space UI manager displaying 3 hearts, score, coins, speedometers, active power-up cards, and modal dialogs.
  19. `SoundManager.cs`: 100% procedural waveform audio synthesis engine generating all 12 sound effects and dynamic multi-track electronic BGM.
  20. `CameraFollow.cs`: Dynamic cinematic third-person camera with lane-banking roll tilt, running head-bob, and speed-proportional FOV expansion.
  21. `ScreenShakeEffect.cs`: Perlin-noise trauma camera shake engine for obstacle hits, shield breaks, and speed tier shifts.
  22. `FloatingTextSpawner.cs`: World-space 3D text popup engine displaying score gains, combo streaks, and near-miss alerts.
* **Meta-Systems, Progression & Upgrades**:
  23. `ComboSystem.cs`: Exponential combo multiplier rewarding rapid coin streaks and close-proximity near-miss dodges.
  24. `SpeedTier.cs`: Multi-tier speed progression tracker (Normal, Fast, Blazing, Hypersonic, Warp) with camera distortion triggers.
  25. `MilestoneSystem.cs`: Distance/score achievement monitor awarding celebration fanfares and screen banners.
  26. `DailyMissionSystem.cs`: Persistent quest tracker managing daily objectives and diamond rewards.
  27. `PerkShopManager.cs`: Persistent upgrade shop allowing players to spend coins to upgrade Magnet Radius, Shield Duration, Extra Hearts, and Multipliers.
  28. `CollectionVFX.cs`: Lightweight particle effect coordinator.
  29. `AmbientParticleController.cs`: Spawns theme-specific ambient particles (leaves, fireflies, sand dust, snowflakes, electrical sparks).
  30. `RuntimeGameInitializer.cs`: Automatic bootstrapping engine using `[RuntimeInitializeOnLoadMethod]` ensuring zero-configuration execution.
* **Editor Tooling**:
  31. `RunnerSetupTool.cs` (in `Assets/Scripts/Editor/`): Full Unity Editor setup automation script generating the entire scene, materials, prefabs, boy character hierarchy, and UI Canvas with one click via `Tools -> 3D Endless Runner -> Setup Complete Game`.

#### C. Prefabs Inventory (`Assets/Prefabs/`)
* **Environment**: `TrackSegment.prefab`
* **Collectibles**: `Coin.prefab`, `Gem.prefab`
* **Core Obstacles**: `Obstacle_Crate.prefab`, `Obstacle_Spikes.prefab`, `Obstacle_Hurdle.prefab`, `Obstacle_Barrier.prefab`
* **Power-Ups**: `PowerUp_Magnet.prefab`, `PowerUp_Shield.prefab`, `PowerUp_Speed.prefab`, `PowerUp_Multiplier.prefab`, `PowerUp_Star.prefab`
* **Themed World Obstacles**:
  * *Grassland*: `Obstacle_GrassFence.prefab`, `Obstacle_HayBale.prefab`, `Obstacle_Barrel.prefab`, `Obstacle_GrassGate.prefab`
  * *Jungle*: `Obstacle_JungleVineWall.prefab`, `Obstacle_JungleLog.prefab`, `Obstacle_JungleTempleBlock.prefab`, `Obstacle_JungleBoulder.prefab`
  * *Desert*: `Obstacle_DesertSandstoneWall.prefab`, `Obstacle_DesertDune.prefab`, `Obstacle_DesertPillar.prefab`, `Obstacle_DesertPottery.prefab`
  * *Ice World*: `Obstacle_IceWall.prefab`, `Obstacle_FrozenPillar.prefab`, `Obstacle_Snowdrift.prefab`, `Obstacle_IceSpikes.prefab`
  * *Mechanical*: `Obstacle_Robot.prefab`, `Obstacle_MechCrusher.prefab`, `Obstacle_MechLaserGate.prefab`, `Obstacle_MechSteamPipe.prefab`
* **Scenery Props**: `Prop_GrassTree.prefab`, `Prop_GrassRock.prefab`, `Prop_JunglePillar.prefab`, `Prop_Cactus.prefab`, `Prop_DesertPyramid.prefab`, `Prop_IceCrystal.prefab`, `Prop_MechPillar.prefab`, etc.

---

# 2. GAME OVERVIEW & CORE MECHANICS

### 2.1 Game Concept
**PlatformRunner3D** is an endless 3D action runner where the player character continuously advances along an infinite forward track divided into three discrete lanes. The player must use fast reflexes to switch lanes, jump over grounded hurdles, slide underneath elevated barriers, and dodge moving obstacles.

```
                         [ GAMEPLAY FLOWCHART ]

                       ┌─────────────────────────┐
                       │       Game Launch       │
                       └────────────┬────────────┘
                                    │
                                    ▼
                       ┌─────────────────────────┐
                       │    World Select Menu    │
                       └────────────┬────────────┘
                                    │ Start Game
                                    ▼
                    ┌───────────────────────────────┐
             ┌─────►│     Continuous Forward Run    │◄────────┐
             │      └───────────────┬───────────────┘         │
             │                      │                         │
             │         ┌────────────┴────────────┐            │
             │         │  Detect Player Input    │            │
             │         └────────────┬────────────┘            │
             │         /            │            \            │
             │    [Swipe L/R]   [Swipe Up]   [Swipe Down]     │
             │        │             │             │           │
             │   Shift Lane       Jump        Slide/Duck      │
             │   (-3, 0, 3)     (Parabola)    (H: 0.85m)      │
             │        │             │             │           │
             │         └────────────┼─────────────┘           │
             │                      ▼                         │
             │         ┌─────────────────────────┐            │
             │         │   Collision Assessment  │            │
             │         └────────────┬────────────┘            │
             │          /           │           \             │
             │    (Hit Coin)   (Hit Hazard)  (Near Miss)      │
             │        │             │             │           │
             │   +Coin Balance  {Shield?}    +Combo Multiplier│
             │   +Score Points   /      \    +Floating Text   │
             │        │       (Yes)     (No)      │           │
             │        │         │        │        │           │
             │        │    Break Shield Health-1  │           │
             │        │     1s Invuln   ScreenShk │           │
             │        │         │        │        │           │
             │        │         │     {HP > 0?}   │           │
             │        │         │     /       \   │           │
             │        │       (Yes) (Yes)     (No)│           │
             └────────┴─────────┴─────┘        │  │           │
                                               ▼  └───────────┘
                                    ┌─────────────────────┐
                                    │      Game Over      │
                                    └──────────┬──────────┘
                                               │
                                    ┌──────────┴──────────┐
                                    │ Revive / Perk Shop  │
                                    └─────────────────────┘
```

### 2.2 Objective & Win/Loss Conditions
* **Objective**: Survive as long as possible, maximize total score, collect coins/diamonds, complete daily missions, and unlock persistent perk upgrades.
* **Win Condition**: Endless format; personal victory is achieved by breaking the local high score and mastering higher speed tiers.
* **Loss Condition**: Depleting all 3 Hearts through unshielded obstacle collisions.

### 2.3 Mathematical Mechanics Formulations
1. **Kinematic Jumping Equation**:
   The vertical launch velocity $v_y$ required to reach the exact jump height $h_{\text{jump}} = 2.2\,\text{m}$ under gravity $g = -26\,\text{m/s}^2$ is calculated using Torricelli’s theorem:
   $$v_y = \sqrt{2 \cdot |g| \cdot h_{\text{jump}}} = \sqrt{2 \cdot 26 \cdot 2.2} \approx 10.70\,\text{m/s}$$
2. **Slide Bounding-Box Contraction**:
   * Normal State: Height $H = 2.0\,\text{m}$, Center $Y = 1.0\,\text{m}$.
   * Slide State: Height $H = 0.85\,\text{m}$, Center $Y = 0.45\,\text{m}$.
   This dynamic reduction allows the player to safely clear obstacles with an overhead clearance plane between $0.9\,\text{m}$ and $2.2\,\text{m}$.
3. **Lateral Lane Stepping**:
   Lanes are centered at coordinates $X \in \{-3.0, 0.0, +3.0\}$. Lateral position interpolates smoothly:
   $$X_{t+\Delta t} = \text{MoveTowards}(X_t, X_{\text{target}}, v_{\text{lane}} \cdot \Delta t), \quad v_{\text{lane}} = 18\,\text{m/s}$$

---

# 3. COMPLETE PROJECT FOLDER STRUCTURE

```
PlatformRunner3D/
├── Assets/
│   ├── Prefabs/                 # Reusable GameObject templates
│   │   ├── PowerUps/            # Magnet, Shield, Speed, Multiplier, Star prefabs
│   │   ├── Props/               # World-specific scenery props (trees, rocks, arches)
│   │   ├── TrackSegment.prefab  # Core 30m modular road piece with curbs & ridges
│   │   ├── Coin.prefab          # Rotating coin pickup with trigger collider
│   │   ├── Gem.prefab           # Diamond pickup for bonus score & revive economy
│   │   └── Obstacle_*.prefab    # Low hurdles, high barriers, crates, spikes, robots
│   ├── Scenes/
│   │   └── SampleScene.unity    # Primary production scene
│   ├── Scripts/                 # C# source code files (30 runtime scripts)
│   │   ├── Editor/
│   │   │   └── RunnerSetupTool.cs # Automated scene builder & generator
│   │   ├── PlayerController.cs
│   │   ├── BoyCharacterAnimator.cs
│   │   ├── SoundManager.cs
│   │   ├── GameManager.cs
│   │   ├── UIManager.cs
│   │   ├── TrackSpawner.cs
│   │   ├── TrackSegment.cs
│   │   ├── ComboSystem.cs
│   │   ├── PerkShopManager.cs
│   │   └── ... (all 30 scripts)
│   ├── Materials/               # PBR & Unlit shaders and color materials
│   │   ├── Boy/                 # Skin, Jacket, Jeans, Shoes, Headphones materials
│   │   ├── Worlds/              # Theme-specific road, divider, and obstacle materials
│   │   └── PowerUps/            # Glowing power-up materials
│   └── Settings/                # Preset configurations
├── Packages/
│   └── manifest.json            # Unity Package Manager dependency declarations
└── ProjectSettings/             # Engine settings (TagManager, Physics, Quality)
```

---

# 4. COMPLETE C# SCRIPT EXPLANATIONS

Below is a detailed technical examination of the core scripts driving the game.

### 4.1 `PlayerController.cs`
* **Attachment**: Attached to the root `Player` GameObject.
* **Required Component**: `[RequireComponent(typeof(CharacterController))]`.
* **Core Responsibilities**:
  1. Controls forward running, smooth lateral lane switching, jumping, fast-falling, and crouch sliding.
  2. Dynamically alters the `CharacterController` dimensions during slides.
  3. Manages the 3-Hearts health system, temporary damage invulnerability, and power-up states.
  4. Detects dual inputs (keyboard keys `A`/`D`/`Space`/`S` and touch/pointer swipe vectors).
* **Key Code Snippet & Analysis**:
```csharp
// Excerpt from PlayerController.cs: Slide hitbox morphing
private void StartSlide()
{
    if (isSliding) return;
    isSliding = true;
    slideTimer = slideDuration;

    if (controller != null)
    {
        controller.height = slideControllerHeight; // Contract to 0.85m
        controller.center = slideControllerCenter; // Lower center to (0, 0.45, 0)
    }

    if (SoundManager.Instance != null) SoundManager.Instance.PlaySlide();
    if (CollectionVFX.Instance != null) CollectionVFX.Instance.PlaySlideDust(transform.position);
}
```
* **Failure Impact**: Removing this script disables character movement, input processing, health, and player physics, breaking the game.

---

### 4.2 `BoyCharacterAnimator.cs`
* **Attachment**: Attached to the `BoySkeletonRoot` child under `Player`.
* **Core Responsibilities**:
  1. Procedurally calculates trigonometric skeletal rotations for 12 distinct body parts.
  2. Bends upper arms and articulates forearm elbow joints up to $85^\circ$ as the character pumps its arms while running.
  3. Flexes thigh pivots and lower-leg knee joints up to $60^\circ$ during striding.
  4. Rotates shoes for ankle toe-roll and applies pelvic counter-twist opposing shoulder motion.
  5. Dynamically compresses the body scale along the $Y$-axis upon ground impact to simulate landing squash and stretch.
* **Key Code Snippet & Analysis**:
```csharp
// Excerpt from BoyCharacterAnimator.cs: Arm & Elbow kinematics
float armForwardAngle = -sinWave * armSwingAngle;
float armBackAngle = sinWave * armSwingAngle;

if (leftArm != null)
    leftArm.localRotation = initialLeftArmRot * Quaternion.Euler(armForwardAngle - 10f, 0f, -10f);
if (rightArm != null)
    rightArm.localRotation = initialRightArmRot * Quaternion.Euler(armBackAngle - 10f, 0f, 10f);

// Elbow flexion increases as arm swings forward
float leftElbowBend = Mathf.Lerp(30f, 85f, Mathf.InverseLerp(-1f, 1f, -sinWave));
float rightElbowBend = Mathf.Lerp(30f, 85f, Mathf.InverseLerp(-1f, 1f, sinWave));

if (leftForearm != null) leftForearm.localRotation = Quaternion.Euler(-leftElbowBend, 0f, 0f);
if (rightForearm != null) rightForearm.localRotation = Quaternion.Euler(-rightElbowBend, 0f, 0f);
```
* **Failure Impact**: The player model remains in a static T-pose or rigid block stance without running, jumping, sliding, or death poses.

---

### 4.3 `SoundManager.cs`
* **Attachment**: Attached to the `[SoundManager]` persistent GameObject.
* **Core Responsibilities**:
  1. Eliminates reliance on external audio assets by synthesizing real-time PCM waveforms via `AudioClip.Create`.
  2. Generates multi-voice sound effects for Coin collection, Gem pickup, Jump whoosh, Slide swoosh, Shield breaking, and Milestone fanfares.
  3. Synthesizes an interactive 8-bar electronic background music track with bassline and synth chords directly into an audio buffer.
* **Failure Impact**: The game runs silently with zero audio feedback.

---

### 4.4 `TrackSpawner.cs` & `TrackSegment.cs`
* **Attachment**: `TrackSpawner` is on the scene coordinator; `TrackSegment` is on each instantiated 30m track piece.
* **Core Responsibilities**:
  1. Maintains a queue of active 30m segments ahead of the player.
  2. Recycles passed segments behind the camera to prevent memory allocation spikes.
  3. Automatically places low hurdles, high barriers, crates, spikes, moving obstacles, coins, and gems based on seed data.
  4. Swaps materials, curbs, and scenery props when biomes transition.
* **Failure Impact**: No ground spawns under the player, causing the character to fall into empty space.

---

### 4.5 `GameManager.cs` & `UIManager.cs`
* **Attachment**: Attached to `[GameManager]` and `[UIManager]` in the Canvas hierarchy.
* **Core Responsibilities**:
  1. Manages high-level game states: `WorldSelect`, `Playing`, `Paused`, and `GameOver`.
  2. Calculates continuous distance score: $\text{Score} = \text{Distance} + (\text{Coins} \times 10) + (\text{Gems} \times 50)$.
  3. Updates the HUD (3 Hearts display, live score counter, speed tiers, power-up timers).
  4. Persists high scores and gem currency using `PlayerPrefs`.
* **Failure Impact**: Scores do not record, UI remains frozen, and Game Over conditions never resolve.

---

### 4.6 `ComboSystem.cs` & `PerkShopManager.cs`
* **Attachment**: Attached to singletons `[ComboSystem]` and `[PerkShopManager]`.
* **Core Responsibilities**:
  1. `ComboSystem`: Monitors coin pickup timestamps and obstacle near-miss distances ($<1.6\,\text{m}$), rewarding players with up to $5\times$ score multipliers.
  2. `PerkShopManager`: Manages persistent upgrades for Magnet Radius, Shield Duration, Extra Hearts, and Coin Value, saving progress across play sessions.
* **Failure Impact**: The score remains flat, near-miss mechanics are disabled, and upgrade progression is lost.

---

# 5. SCRIPT-TO-ACTION MAPPING MATRIX

This table maps every user action and game event to its controlling script, method, and GameObject:

| Game Action | Script File | Function / Method | Target GameObject | Operational Result |
| :--- | :--- | :--- | :--- | :--- |
| **Forward Running** | `PlayerController.cs` | `MovePlayer()` | `Player` | Translates character forward along the $+Z$ axis via `controller.Move()` |
| **Lane Switching (Left/Right)** | `PlayerController.cs` | `ShiftLane(int direction)` | `Player` | Clamps lane between 0 and 2; smoothly shifts $X$ position toward $-3, 0, +3$ |
| **Jumping** | `PlayerController.cs` | `Jump()` | `Player` | Applies vertical velocity $v_y = 10.7\,\text{m/s}$; plays procedural jump audio |
| **Fast-Fall Mid-Air** | `PlayerController.cs` | `MovePlayer()` | `Player` | Increases gravity from $-26\,\text{m/s}^2$ to $-45\,\text{m/s}^2$ on Down input |
| **Crouch Slide** | `PlayerController.cs` | `StartSlide()` | `Player` | Contracts collider height to $0.85\,\text{m}$ for $0.75\,\text{s}$; ducks under barriers |
| **Biomechanical Arm & Knee Flex** | `BoyCharacterAnimator.cs`| `ApplyRunAnimation()` | `BoySkeletonRoot` | Calculates elbow flexion ($30^\circ\text{--}85^\circ$) and knee bending ($0^\circ\text{--}60^\circ$) |
| **Ground Landing Squash** | `BoyCharacterAnimator.cs`| `Update()` | `BoySkeletonRoot` | Compresses body scale along $Y$-axis on landing, then smoothly recovers |
| **Coin Collection** | `Coin.cs` | `Collect()` | `Coin(Clone)` | Plays procedural chime, triggers 3D floating text popup, adds coin |
| **Magnet Attraction** | `Coin.cs` | `CheckMagnetAttraction()`| `Coin(Clone)` | Accelerates coin toward player at $22\,\text{m/s}$ if Magnet is active |
| **Gem Pickup** | `Gem.cs` | `Collect()` | `Gem(Clone)` | Awards $+1$ diamond currency and $+50$ bonus score |
| **Hazard Collision** | `PlayerController.cs` | `TakeDamage(int damage)` | `Player` | Deducts 1 heart, triggers $1.5\,\text{s}$ invulnerability flashing and screen shake |
| **Shield Protection** | `PlayerController.cs` | `TakeDamage(int damage)` | `Player` | Absorbs lethal hit, shatters force field bubble, prevents heart loss |
| **Camera Follow & Banking** | `CameraFollow.cs` | `LateUpdate()` | `Main Camera` | Tracks player with smooth damping, banks roll tilt, and adds head-bob |
| **Dynamic Speed FOV Warping**| `SpeedTier.cs` | `Update()` | `[SpeedTier]` | Expands camera FOV from $60^\circ$ to $78^\circ$ as speed increases |
| **Endless Track Spawning** | `TrackSpawner.cs` | `SpawnSegment()` | `[TrackSpawner]` | Instantiates 30m segment ahead and enqueues it for recycling |
| **Score & Distance Update** | `GameManager.cs` | `Update()` | `[GameManager]` | Computes distance run and adds coin/gem bonuses to HUD |
| **Game Over Trigger** | `GameManager.cs` | `TriggerGameOver()` | `[GameManager]` | Displays Game Over screen, checks high score, halts gameplay |
| **Player Revive** | `GameManager.cs` | `RevivePlayer()` | `[GameManager]` | Consumes 5 gems, restores 1 heart, grants $2.5\,\text{s}$ invulnerability |
| **Perk Upgrade Purchase** | `PerkShopManager.cs` | `UpgradeMagnet()`, etc. | `[PerkShopManager]`| Deducts coins, increments perk tier, and saves to `PlayerPrefs` |

---

# 6. ANIMATION SYSTEM: PROCEDURAL KINEMATICS VS. ANIMATOR

### 6.1 Understanding Unity Animation Concepts
1. **Animation Clip (`.anim`)**: A pre-recorded asset containing keyframed positions, rotations, and scales for specific transforms over time.
2. **Animator Component**: A Unity engine component attached to a GameObject that references an Animator Controller to play and blend animation clips.
3. **Animator Controller (`.controller`)**: A visual finite state machine (FSM) defining states (e.g., Idle, Run, Jump), transitions, blend trees, and parameter rules (`Speed`, `isGrounded`, `isSliding`).

### 6.2 Implementation in this Project: Procedural Kinematics
Instead of using imported FBX files or static keyframed clips, this project features an advanced **Procedural Kinematic Skeletal Rig** (`BoyCharacterAnimator.cs`).

```
                 [ PROCEDURAL KINEMATICS STATE MACHINE ]

                            ┌─────────────────┐
                            │    Start / Run  │
                            └────────┬────────┘
                                     │
                    ┌────────────────┴────────────────┐
                    │                                 │
     isGrounded == false                       isSliding == true
                    │                                 │
                    ▼                                 ▼
         ┌─────────────────────┐           ┌─────────────────────┐
         │     Jump Pose       │           │     Slide Pose      │
         │ Arms raise upward   │           │ Arms extend backward│
         │ Knees tuck forward  │           │ Body tilts low      │
         └──────────┬──────────┘           └──────────┬──────────┘
                    │                                 │
             isGrounded == true                slideTimer <= 0
                    │                                 │
                    └────────────────┬────────────────┘
                                     ▼
                          ┌─────────────────────┐
                          │    Landing Squash   │
                          │ Body scale Y com-   │
                          │ presses by 0.15m    │
                          └──────────┬──────────┘
                                     │
                             landingSquash -> 0
                                     │
                                     ▼
                          ┌─────────────────────┐
                          │    Run Animation    │
                          │ Dynamic elbow flex  │
                          │ Knee stride swing   │
                          │ Pelvic twist        │
                          └─────────────────────┘
```

#### Why Procedural Kinematics?
* **Zero Asset Overhead**: Requires no heavy external animation libraries or complex blend trees.
* **Dynamic Speed Adaptability**: The running cadence automatically scales with the player's actual velocity ($v / 12\,\text{m/s}$), ensuring feet never slide along the track.
* **Integrated Secondary Motion**: Pelvic counter-rotation, elbow flexion, dynamic leaning, and landing squash/stretch adapt in real time to physics inputs.

#### How to Add a Traditional Mecanim Rig (If Desired)
If you wish to replace procedural kinematics with custom 3D model FBX clips:
1. Import your humanoid FBX model with animations (Run, Jump, Slide, Fall, Die).
2. Attach an **Animator** component to the model root.
3. Create an **Animator Controller** (`PlayerAnimatorController.controller`) in `Assets/Animations/`.
4. Define parameters: `float Speed`, `bool isGrounded`, `bool isSliding`, `trigger Die`.
5. Connect states using transitions with `Has Exit Time = false`.
6. Update `PlayerController.cs` to set parameters via `animator.SetFloat("Speed", forwardSpeed)`, `animator.SetBool("isSliding", isSliding)`, etc.

---

# 7. UNITY GAMEOBJECTS & COMPONENT ARCHITECTURE

### 7.1 Key GameObjects in the Hierarchy
1. **`Player` (Parent)**:
   * **Components**: `CharacterController`, `PlayerController`, `PlayerTrailEffect`.
   * **Role**: Handles movement coordinates, collision bounds, health, and input processing.
2. **`BoySkeletonRoot` (Child of Player)**:
   * **Components**: `BoyCharacterAnimator`.
   * **Sub-Children**: Head (with Headphones and Spiky Hair), Torso (with Backpack and Zipper), Left/Right Arm Pivots (with upper arm, elbow pivot, forearm, hand), Left/Right Leg Pivots (with thigh, knee joint, shin, shoe, sole, lace).
3. **`Main Camera`**:
   * **Components**: `Camera`, `AudioListener`, `CameraFollow`, `ScreenShakeEffect`.
   * **Role**: Tracks the player with lane-banking roll tilt, speed FOV adjustments, and screen shake.
4. **`[Managers]` (Root Coordinator)**:
   * **Components**: `GameManager`, `SoundManager`, `UIManager`, `TrackSpawner`, `ComboSystem`, `PerkShopManager`, `MilestoneSystem`, `DailyMissionSystem`, `SpeedTier`.
5. **`Canvas (HUD)`**:
   * **Components**: `Canvas`, `CanvasScaler`, `GraphicRaycaster`, `UIManager`.
   * **Sub-Panels**: Top-Left HUD (Score, Coins, Gems, World Badge), Top-Right HUD (3 Hearts, Pause Button), Bottom Overlays (Active Power-Up Timers, Speedometer, Status Messages), Modal Screens (Pause Menu, Game Over Screen, Perk Shop Overlay).

### 7.2 Core Unity Components Explained
* **`Transform`**: Defines the position ($X, Y, Z$), rotation, and scale of an object in 3D space.
* **`CharacterController`**: A specialized kinematic capsule collision driver that moves via `controller.Move()` without standard rigid-body slipping or unintended physics bounce.
* **`BoxCollider` / `SphereCollider`**: Trigger volumes that detect when the player overlaps with coins, gems, power-ups, or obstacles.
* **`MeshFilter` & `MeshRenderer`**: Instantiates and displays 3D geometric meshes using assigned shader materials.
* **`AudioSource` & `AudioListener`**: Handles playback of synthesized PCM audio buffers in the 3D scene.
* **`TrailRenderer`**: Generates a smooth polygon ribbon trail behind the moving player, color-coded to active power-ups.

---

# 8. STEP-BY-STEP GAME DEVELOPMENT PROCESS

This guide outlines how the game was constructed, step by step:

1. **Project Initialization**:
   * Create a new Unity 3D project using Unity Hub (Version 2021.3 LTS). Set color space to Linear for vibrant visuals.
2. **Folder Structure Setup**:
   * Under `Assets/`, create dedicated folders: `Scenes`, `Scripts`, `Prefabs`, `Materials`, `Audio`, and `Settings`.
3. **Player GameObject Construction**:
   * In the Hierarchy, create an empty GameObject named `Player`. Attach a `CharacterController` component ($R = 0.45\,\text{m}, H = 2.0\,\text{m}$).
4. **Procedural Rigging of the Character**:
   * Build the stylized boy character hierarchy under `Player` using geometric primitives with custom PBR materials (jacket, skin, jeans, shoes, headphones, backpack). Attach `BoyCharacterAnimator.cs`.
5. **Camera Configuration**:
   * Align `Main Camera` behind the runner ($Y = 4.2\,\text{m}, Z = -6.5\,\text{m}$). Attach `CameraFollow.cs` to enable lane-banking roll tilt and dynamic FOV warping.
6. **Kinematic Locomotion Scripting**:
   * Create `PlayerController.cs`. Implement lateral lane interpolation across $X \in \{-3, 0, 3\}$, forward velocity acceleration, and kinematic jump curves.
7. **Slide & Hitbox Morphing**:
   * Implement slide mechanics in `PlayerController.cs`. Contract collider height to $0.85\,\text{m}$ upon pressing `Down` or swiping downward.
8. **Obstacle Prefab Authoring**:
   * Build low hurdles (wooden fences, hay bales), high barriers (overhead beams, laser gates), crates, and spikes. Add `BoxCollider` trigger components and attach `Obstacle.cs`.
9. **Modular Track Generation**:
   * Build `TrackSegment.prefab` with road bodies, raised curbs, and distant mountain ridges. Write `TrackSpawner.cs` to dynamically pool and recycle segments.
10. **Collectibles & Magnetic Flying Physics**:
    * Create `Coin.prefab` and `Gem.prefab`. Attach `Coin.cs` and `Gem.cs` with rotation, bobbing, and magnet-attraction logic.
11. **Power-Up System Implementation**:
    * Create prefabs for Magnet, Shield, Speed Boost, Multiplier, and Invincibility. Link pickup events in `PowerUpItem.cs` to player buff states.
12. **Procedural Waveform Audio Engine**:
    * Develop `SoundManager.cs`. Synthesize sine, square, sawtooth, and noise waveforms directly into `AudioClip` buffers at runtime.
13. **Screen Shake & Hit Feedback**:
    * Implement `ScreenShakeEffect.cs` to add camera trauma shakes on hazard collisions and speed tier shifts.
14. **Scoring, Combos & Near-Miss Dodging**:
    * Write `ComboSystem.cs`. Monitor pickup timestamps and check distances to obstacles ($<1.6\,\text{m}$) to award combo bonuses.
15. **HUD & User Interface**:
    * Construct a Canvas with 3-Hearts health indicators, score counters, power-up fill bars, speedometer, and modal dialogs in `UIManager.cs`.
16. **Game Over & Revive Loop**:
    * Connect death events to `GameManager.cs`. Display the Game Over summary screen and implement gem-based revives.
17. **Progression & Upgrade Shop**:
    * Develop `PerkShopManager.cs` and `DailyMissionSystem.cs`. Persist coin balances, completed quests, and perk tiers via `PlayerPrefs`.
18. **Testing, Optimization & Build**:
    * Profile the game in the Unity Profiler. Verify stable $60+\,\text{FPS}$ performance and zero runtime garbage collection spikes before exporting standalone executables.

---

# 9. SCRIPT COMMUNICATION & DATA FLOW

The diagram below illustrates how components interact during an active run:

```
[ User Input (Keys / Swipe) ]
              │
              ▼
    [ PlayerController ] ──────────────► [ BoyCharacterAnimator ]
              │                                    │
              │ (controller.Move)                  │ (Calculates Bone Rotations)
              ▼                                    ▼
       [ Physics Scene ]                  [ Visual 3D Skeleton ]
              │
              ▼ (Trigger Overlap)
    ┌────────────────────────┬────────────────────────┐
    │                        │                        │
    ▼                        ▼                        ▼
 [ Coin / Gem ]         [ Obstacle ]           [ PowerUpItem ]
    │                        │                        │
    ▼                        ▼                        ▼
 ComboSystem.Register()   Player.TakeDamage()    Player.ActivatePowerUp()
    │                        │                        │
    ▼                        ▼                        ▼
 GameManager.AddCoin()   GameManager.EndGame()   UIManager.ShowPowerUp()
    │                        │                        │
    ▼                        ▼                        ▼
 [ SoundManager ]         [ ScreenShake ]        [ HUD Update ]
 (Chime / Synth)         (Trauma Shake)          (Card Countdown)
```

### Communication Mechanisms Used:
1. **Singleton Access (`Instance`)**: Subsystems like `SoundManager.Instance`, `GameManager.Instance`, and `UIManager.Instance` allow globally accessible method execution without slow runtime object lookups.
2. **C# Action Events**: `ComboSystem` broadcasts events (`OnComboMultiplierChanged`, `OnNearMissRegistered`) that `UIManager` listens to, decoupling gameplay rules from UI display code.
3. **Direct Component Calls (`GetComponent`)**: Trigger colliders extract `other.GetComponent<PlayerController>()` on contact to initiate pickups and damage routines.
4. **Data Persistence (`PlayerPrefs`)**: `PerkShopManager` and `GameManager` save and load values from disk via `PlayerPrefs.SetInt()` and `PlayerPrefs.GetInt()`.

---

# 10. UNITY INSPECTOR CONFIGURATION GUIDE

### 10.1 Key GameObject Configurations

#### `Player` Root GameObject
* **Tag**: `Player` | **Layer**: `Default`
* **`CharacterController`**:
  * Slope Limit: $45^\circ$, Step Offset: $0.3\,\text{m}$, Skin Width: $0.05\,\text{m}$, Min Move Distance: $0$
  * Center: $(0, 1.0, 0)$, Radius: $0.45\,\text{m}$, Height: $2.0\,\text{m}$
* **`PlayerController` Component**:
  * Forward Speed: $12$, Max Speed: $26$, Speed Increase Rate: $0.08$
  * Lane Distance: $3.0$, Lane Change Speed: $18$
  * Jump Height: $2.2$, Gravity: $-26$, Fast Fall Gravity: $-45$
  * Slide Duration: $0.75$, Max Health: $3$

#### `BoySkeletonRoot` GameObject
* **`BoyCharacterAnimator` Component**:
  * Body Root: `BoySkeletonRoot/Torso`
  * Head: `BoySkeletonRoot/Torso/Head`
  * Left Arm: `LeftArmPivot`, Right Arm: `RightArmPivot`
  * Left Forearm: `LeftArmPivot/ForearmPivot`, Right Forearm: `RightArmPivot/ForearmPivot`
  * Left Leg: `LeftLegPivot`, Right Leg: `RightLegPivot`
  * Left Lower Leg: `LeftLowerLeg`, Right Lower Leg: `RightLowerLeg`
  * Left Shoe: `LeftShoe`, Right Shoe: `RightShoe`

#### `Main Camera` GameObject
* **`CameraFollow` Component**:
  * Target: `Player`
  * Offset: $(0, 4.2, -6.5)$
  * Smooth Speed: $12$, Vertical Air Smooth: $4.5$
  * Lane Roll Angle: $4.0^\circ$, Head Bob Speed: $14$

### 10.2 Common Setup Mistakes & Solutions
* **Mistake**: Player falls through the track at launch.
  * *Fix*: Ensure the road mesh on `TrackSegment.prefab` has an active `BoxCollider` or `MeshCollider` and that `Player` has a `CharacterController`.
* **Mistake**: Character slides visually but still collides with high barriers.
  * *Fix*: Verify in `PlayerController.cs` that `controller.height` is contracted to $0.85\,\text{m}$ and `controller.center` is lowered to $(0, 0.45, 0)$.
* **Mistake**: `NullReferenceException` in `SoundManager` or `UIManager`.
  * *Fix*: Ensure `RuntimeGameInitializer.cs` is present in the project to automatically bootstrap all manager singletons.

---

# 11. DEBUGGING & TROUBLESHOOTING GUIDE

| Issue Description | Likely Root Cause | Investigation Window | Troubleshooting & Resolution |
| :--- | :--- | :--- | :--- |
| **Player does not move forward** | `isRunning` is false or `Time.timeScale` is set to 0 | Inspector / Console | Check if `GameManager.StartGame()` was called; verify `Time.timeScale == 1`. |
| **Lane change snaps instantly** | Using direct transform coordinate assignment | Script Editor | Use `Mathf.MoveTowards(currentX, targetX, laneSpeed * Time.deltaTime)`. |
| **Jump input is ignored** | Character is not registered as grounded | Scene / Inspector | Check `controller.isGrounded`. If ground is slightly misaligned, set ground check velocity to $-2\,\text{m/s}$. |
| **Character floats or descends slowly** | Gravity parameter is too weak | Inspector (`PlayerController`) | Ensure gravity is set to $-26\,\text{m/s}^2$ and fast-fall gravity to $-45\,\text{m/s}^2$. |
| **Coins do not trigger collection** | Collider is missing `Is Trigger` | Inspector (`Coin.prefab`) | Enable `Is Trigger` on the Coin's `SphereCollider`. |
| **Audio is silent** | `AudioListener` is missing or volume is muted | Hierarchy (`Main Camera`) | Verify that `Main Camera` has an `AudioListener` component and volume is unmuted. |
| **Camera stutters during movement** | Camera logic runs in `Update()` instead of `LateUpdate()`| Script Editor (`CameraFollow.cs`) | Move all camera position tracking into `LateUpdate()` to ensure the player moves first. |
| **Track segments spawn with gaps** | `segmentLength` does not match prefab mesh bounds | Inspector (`TrackSpawner`) | Measure the road mesh along $Z$; ensure `segmentLength` matches the mesh length ($30.0\,\text{m}$). |
| **High barrier kills sliding player** | High barrier collider reaches the track floor | Inspector (`Obstacle_Barrier`) | Set barrier collider center $Y \ge 1.4\,\text{m}$ and size $Y \le 1.6\,\text{m}$ to leave an opening below $1.0\,\text{m}$. |
| **Shop upgrades do not persist** | `PlayerPrefs.Save()` was not called | Script Editor (`PerkShopManager.cs`)| Call `PlayerPrefs.Save()` immediately after updating perk tiers. |

---

# 12. SYSTEMATIC TESTING & RESULTS MATRIX

| Test ID | Test Case Scenario | Input / Action | Expected Result | Actual Result | Status |
| :---: | :--- | :--- | :--- | :--- | :---: |
| **TC-01** | Game Initialization | Launch scene and click "Play" | Game state shifts to `Playing`; runner accelerates down track; HUD shows 3 hearts | Verified; run begins cleanly at $12\,\text{m/s}$ with HUD active | **PASS** |
| **TC-02** | 3-Lane Lateral Stepping | Press `A`/`D` or Swipe Left/Right | Character shifts smoothly between lanes $-3, 0, +3$; clamps at boundaries | Verified; lateral speed is $18\,\text{m/s}$ without coordinate snapping | **PASS** |
| **TC-03** | Jump Clearance | Press `Space` or Swipe Up at hurdle | Character leaps $2.2\,\text{m}$ in a parabola, clearing the low hurdle cleanly | Verified; clears hurdle with knee joint flexion | **PASS** |
| **TC-04** | Fast Fall Execution | Press `S` or Swipe Down while jumping | Character immediately accelerates downward under $-45\,\text{m/s}^2$ gravity | Verified; rapid ground recovery confirmed | **PASS** |
| **TC-05** | Slide Hitbox Contraction | Press `S` or Swipe Down at high barrier| Collider height contracts to $0.85\,\text{m}$; runner ducks underneath barrier safely | Verified; clears barrier without collision damage | **PASS** |
| **TC-06** | Coin Pickup & Sound Chime | Touch a coin on the track | Coin disappears, coin balance increments, procedural audio chime plays | Verified; floating score popup displays value | **PASS** |
| **TC-07** | Magnet Power-Up Attraction| Pick up Magnet power-up crystal | Coins within $14\,\text{m}$ fly toward the player at $22\,\text{m/s}$ | Verified; coins track smoothly toward the runner | **PASS** |
| **TC-08** | Shield Protection | Hit obstacle while Shield is active | Shield bubble shatters, camera shakes, hearts remain at 3, invulnerability flashes | Verified; hit absorbed with zero heart loss | **PASS** |
| **TC-09** | Damage & Heart Loss | Hit obstacle without shield | Health drops from 3 to 2 hearts, player flashes for $1.5\,\text{s}$, combo meter resets | Verified; heart icon updates on HUD | **PASS** |
| **TC-10** | Speed Tier Progression | Travel past $500\,\text{m}$ | Speed reaches tier 3 (Blazing); camera FOV expands from $60^\circ$ to $70^\circ$ | Verified; HUD status message announces tier | **PASS** |
| **TC-11** | Game Over & Revive | Lose all 3 hearts, click "Revive" | Game pauses on death; clicking Revive consumes 5 gems and restores 1 heart | Verified; revive restores player with brief invulnerability | **PASS** |
| **TC-12** | Perk Persistence | Purchase Magnet upgrade, restart game | Coin balance decrements; Magnet radius permanently increases in next run | Verified; data persists across restarts via `PlayerPrefs` | **PASS** |

---

# 13. EXISTING FEATURES VS. FUTURE IMPROVEMENTS

### 13.1 Implemented & Verified Features (Present in Project)
* Kinematic 3-lane navigation with non-slip `CharacterController`.
* Parabolic jumping with mid-air fast fall.
* Crouch sliding with dynamic bounding-box contraction ($2.0\,\text{m} \rightarrow 0.85\,\text{m}$).
* Procedural skeletal kinematics (12 joints, elbow/knee bending, ankle roll, landing squash/stretch).
* 100% procedural waveform audio synthesis (12 SFX + dynamic electronic BGM).
* Modular endless track recycling across 5 biomes (Grassland, Jungle, Desert, Ice, Mechanical).
* 3-Hearts health system with damage invulnerability.
* 5 power-up types (Magnet, Shield, Speed Boost, Multiplier, Invincibility).
* Dynamic camera with lane-banking roll tilt, running head-bob, and speed FOV warping.
* Screen shake trauma system for impacts.
* Combo multiplier and near-miss obstacle dodging detection.
* 3D world-space floating text popups.
* Persistent Perk Upgrade Shop and Daily Missions system.

### 13.2 Proposed Future Enhancements
* **Online Leaderboards**: Cloud database synchronization (e.g., Firebase or Unity Gaming Services) for global rankings.
* **Character Customization**: Unlockable runner skins, outfits, and accessories in the Perk Shop.
* **Vertical Track Elevation**: Elevated multi-tier highways, ramps, and underground subway sections.
* **Mobile Haptic Feedback**: Haptic engine vibration pulses on near-misses, impacts, and speed tier shifts.

---

# 14. LIST OF MISSING OR INCOMPLETE IMPLEMENTATIONS

To maintain technical transparency, the following items are noted:
1. **Mecanim FBX Animation Assets**: The project does not include pre-baked humanoid animation clips (`.anim`) or an `AnimatorController`. Animation is handled entirely via procedural C# kinematics in `BoyCharacterAnimator.cs`.
2. **External Audio AudioClips**: There are no `.wav`, `.mp3`, or `.ogg` files in `Assets/Audio/`. All audio is synthesized procedurally in `SoundManager.cs`.
3. **Bake-Only Lightmaps**: Real-time lighting is used across biomes. Static lightmaps are omitted to support endless procedural track generation.
4. **Third-Party Network SDKs**: Multi-device cloud sync and in-app purchases (IAP) are not included; economy and high scores use local `PlayerPrefs` storage.

---
*End of Documentation Manual. Developed for SR University — Game Design Using Unity.*

