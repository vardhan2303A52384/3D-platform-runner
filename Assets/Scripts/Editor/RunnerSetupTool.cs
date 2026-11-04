using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class RunnerSetupTool : EditorWindow
{
    [MenuItem("Tools/3D Endless Runner/Setup Complete Game (Design Overview Match)")]
    public static void SetupCompleteGame()
    {
        if (!Application.isBatchMode)
        {
            bool confirm = EditorUtility.DisplayDialog(
                "Setup Platform Runner (Ultimate Realistic Edition)",
                "This will setup the game with all upgrades:\n\n• Procedural Sound Engine & Dynamic Synth Beats (zero audio files needed!)\n• Realistic Animated Boy with Running, Sliding, Jumping & Death poses\n• Slide Under High Obstacles (duck & roll physics)\n• Keyboard (WASD/Arrows) + Mouse/Touch Swipe Controls\n• Dynamic Cinematic Camera (Lane-Change Banking, Head-Bob & Speed FOV)\n• Screen Shake & Hit Impacts\n• Combo Multiplier & Thrilling Near-Miss Obstacle Dodging\n• 3D Floating Score & Text Popups\n• Procedural Particle Bursts & Footstep Dust Puffs\n• Speed Tiers (Normal -> Fast -> Blazing -> Hypersonic -> Warp)\n• Distance & Score Milestones with celebration fanfares\n• Daily Missions & Quests System\n• Persistent Perk & Upgrade Shop\n• 5 Rich Themed Worlds with World-Specific Obstacles & Moving Hazards\n• 3 Hearts Health System & Revive System\n\nProceed?",
                "Yes, Build Ultimate Game",
                "Cancel"
            );

            if (!confirm) return;
        }

        string scenePath = "Assets/Scenes/SampleScene.unity";
        if (System.IO.File.Exists(scenePath))
        {
            EditorSceneManager.OpenScene(scenePath);
        }

        EnsureTags();
        EnsureFolders();

        // 1. Base Materials
        Material matPlayer = CreateOrLoadMaterial("Mat_Player", new Color(0.0f, 0.88f, 0.92f));
        Material matShield = CreateOrLoadMaterial("Mat_ShieldBubble", new Color(0.1f, 0.7f, 1.0f, 0.45f), true);
        Material matGem = CreateOrLoadMaterial("Mat_Gem", new Color(0.0f, 0.95f, 1.0f));
        Material matCrate = CreateOrLoadMaterial("Mat_Crate", new Color(0.60f, 0.38f, 0.18f));
        Material matSpikes = CreateOrLoadMaterial("Mat_Spikes", new Color(0.40f, 0.42f, 0.45f));

        Material matPowerMagnet = CreateOrLoadMaterial("PowerUps/Mat_Magnet", new Color(1.0f, 0.2f, 0.35f));
        Material matPowerShield = CreateOrLoadMaterial("PowerUps/Mat_Shield", new Color(0.1f, 0.5f, 1.0f));
        Material matPowerSpeed = CreateOrLoadMaterial("PowerUps/Mat_Speed", new Color(1.0f, 0.6f, 0.1f));
        Material matPowerMult = CreateOrLoadMaterial("PowerUps/Mat_Mult", new Color(0.7f, 0.3f, 0.95f));
        Material matPowerStar = CreateOrLoadMaterial("PowerUps/Mat_Star", new Color(1.0f, 0.85f, 0.1f));

        // 2. Props for the 5 Worlds
        GameObject propGrassTree = CreateTreeProp();
        GameObject propGrassRock = CreateRockProp();
        GameObject propJunglePillar = CreateJunglePillarProp();
        GameObject propJungleTree = CreateJungleTreeProp();
        GameObject propCactus = CreateCactusProp();
        GameObject propDesertPyramid = CreatePyramidProp();
        GameObject propIceCrystal = CreateIceCrystalProp();
        GameObject propSnowMound = CreateSnowMoundProp();
        GameObject propMechPillar = CreateMechPillarProp();
        GameObject propMechBox = CreateMechBoxProp();

        // Detail props for each world
        GameObject detailGrass = CreateGrassTuftProp();
        GameObject detailFlower = CreateFlowerProp();
        GameObject detailJungleFern = CreateJungleFernProp();
        GameObject detailJungleMushroom = CreateJungleMushroomProp();
        GameObject detailDesertSkull = CreateDesertSkullProp();
        GameObject detailDesertTumbleweed = CreateDesertTumbleweedProp();
        GameObject detailIceShard = CreateIceShardProp();
        GameObject detailSnowPile = CreateSmallSnowPileProp();
        GameObject detailMechPipe = CreateMechPipeProp();
        GameObject detailMechWire = CreateMechWireBoxProp();

        // Overhead props
        GameObject overheadGrassArch = CreateGrassArchProp();
        GameObject overheadJungleVines = CreateJungleVineArchProp();
        GameObject overheadDesertArch = CreateDesertStoneArchProp();
        GameObject overheadIceArch = CreateIceArchProp();
        GameObject overheadMechArch = CreateMechGantryProp();

        // 3. Configure the 5 World Themes with UNIQUE obstacles
        List<WorldThemeData> worldThemes = new List<WorldThemeData>();

        // === GRASSLAND ===
        // Obstacles: wooden fences, hay bales, wooden barrels
        GameObject grassFence = CreateGrasslandFenceObstacle();
        GameObject grassHayBale = CreateGrasslandHayBaleObstacle();
        GameObject grassBarrel = CreateGrasslandBarrelObstacle();
        GameObject grassGate = CreateGrasslandGateObstacle();

        worldThemes.Add(new WorldThemeData
        {
            worldType = WorldType.Grassland,
            displayName = "Grassland",
            skyColor = new Color(0.48f, 0.72f, 0.95f),
            fogColor = new Color(0.65f, 0.82f, 0.95f),
            fogDensity = 0.010f,
            lightColor = new Color(1.0f, 0.98f, 0.9f),
            lightIntensity = 1.25f,
            trackMaterial = CreateOrLoadMaterial("Worlds/Grassland_Track", new Color(0.36f, 0.44f, 0.32f)),
            sideGroundMaterial = CreateOrLoadMaterial("Worlds/Grassland_Side", new Color(0.25f, 0.68f, 0.22f)),
            laneDividerMaterial = CreateOrLoadMaterial("Worlds/Grassland_Divider", new Color(0.92f, 0.95f, 0.85f)),
            hurdleMaterial = CreateOrLoadMaterial("Worlds/Grassland_Hurdle", new Color(0.58f, 0.38f, 0.18f)),
            barrierMaterial = CreateOrLoadMaterial("Worlds/Grassland_Barrier", new Color(0.45f, 0.50f, 0.45f)),
            coinMaterial = CreateOrLoadMaterial("Worlds/Grassland_Coin", new Color(1.0f, 0.85f, 0.15f)),
            sidePropPrefabs = new GameObject[] { propGrassTree, propGrassRock },
            customObstaclePrefabs = new GameObject[] { grassFence, grassHayBale, grassBarrel, grassGate },
            detailPropPrefabs = new GameObject[] { detailGrass, detailFlower },
            overheadPropPrefabs = new GameObject[] { overheadGrassArch },
            detailDensity = 0.6f,
            overheadDensity = 0.2f,
            hasAmbientParticles = true,
            particleColor = new Color(0.3f, 0.75f, 0.2f)
        });

        // === JUNGLE ===
        // Obstacles: vine walls, fallen logs, temple block
        GameObject jungleVineWall = CreateJungleVineWallObstacle();
        GameObject jungleFallenLog = CreateJungleFallenLogObstacle();
        GameObject jungleTempleBlock = CreateJungleTempleBlockObstacle();
        GameObject jungleBoulderTrap = CreateJungleBoulderObstacle();

        worldThemes.Add(new WorldThemeData
        {
            worldType = WorldType.Jungle,
            displayName = "Jungle",
            skyColor = new Color(0.14f, 0.30f, 0.24f),
            fogColor = new Color(0.18f, 0.36f, 0.28f),
            fogDensity = 0.025f,
            lightColor = new Color(0.85f, 1.0f, 0.85f),
            lightIntensity = 0.95f,
            trackMaterial = CreateOrLoadMaterial("Worlds/Jungle_Track", new Color(0.24f, 0.18f, 0.14f)),
            sideGroundMaterial = CreateOrLoadMaterial("Worlds/Jungle_Side", new Color(0.10f, 0.42f, 0.18f)),
            laneDividerMaterial = CreateOrLoadMaterial("Worlds/Jungle_Divider", new Color(0.55f, 0.85f, 0.45f)),
            hurdleMaterial = CreateOrLoadMaterial("Worlds/Jungle_Hurdle", new Color(0.46f, 0.30f, 0.15f)),
            barrierMaterial = CreateOrLoadMaterial("Worlds/Jungle_Barrier", new Color(0.26f, 0.35f, 0.25f)),
            coinMaterial = CreateOrLoadMaterial("Worlds/Jungle_Coin", new Color(0.25f, 0.95f, 0.55f)),
            sidePropPrefabs = new GameObject[] { propJungleTree, propJunglePillar },
            customObstaclePrefabs = new GameObject[] { jungleVineWall, jungleFallenLog, jungleTempleBlock, jungleBoulderTrap },
            detailPropPrefabs = new GameObject[] { detailJungleFern, detailJungleMushroom },
            overheadPropPrefabs = new GameObject[] { overheadJungleVines },
            detailDensity = 0.8f,
            overheadDensity = 0.35f,
            hasAmbientParticles = true,
            particleColor = new Color(0.4f, 1.0f, 0.3f)
        });

        // === DESERT ===
        // Obstacles: sandstone walls, sand dune barriers, ancient pillars
        GameObject desertSandstoneWall = CreateDesertSandstoneWallObstacle();
        GameObject desertDuneBarrier = CreateDesertDuneBarrierObstacle();
        GameObject desertAncientPillar = CreateDesertAncientPillarObstacle();
        GameObject desertPottery = CreateDesertPotteryObstacle();

        worldThemes.Add(new WorldThemeData
        {
            worldType = WorldType.Desert,
            displayName = "Desert",
            skyColor = new Color(0.92f, 0.72f, 0.48f),
            fogColor = new Color(0.88f, 0.68f, 0.42f),
            fogDensity = 0.014f,
            lightColor = new Color(1.0f, 0.88f, 0.70f),
            lightIntensity = 1.35f,
            trackMaterial = CreateOrLoadMaterial("Worlds/Desert_Track", new Color(0.70f, 0.52f, 0.35f)),
            sideGroundMaterial = CreateOrLoadMaterial("Worlds/Desert_Side", new Color(0.86f, 0.68f, 0.38f)),
            laneDividerMaterial = CreateOrLoadMaterial("Worlds/Desert_Divider", new Color(0.95f, 0.88f, 0.72f)),
            hurdleMaterial = CreateOrLoadMaterial("Worlds/Desert_Hurdle", new Color(0.82f, 0.45f, 0.22f)),
            barrierMaterial = CreateOrLoadMaterial("Worlds/Desert_Barrier", new Color(0.65f, 0.46f, 0.30f)),
            coinMaterial = CreateOrLoadMaterial("Worlds/Desert_Coin", new Color(1.0f, 0.75f, 0.12f)),
            sidePropPrefabs = new GameObject[] { propCactus, propDesertPyramid },
            customObstaclePrefabs = new GameObject[] { desertSandstoneWall, desertDuneBarrier, desertAncientPillar, desertPottery },
            detailPropPrefabs = new GameObject[] { detailDesertSkull, detailDesertTumbleweed },
            overheadPropPrefabs = new GameObject[] { overheadDesertArch },
            detailDensity = 0.4f,
            overheadDensity = 0.15f,
            hasAmbientParticles = true,
            particleColor = new Color(0.9f, 0.75f, 0.4f)
        });

        // === ICE WORLD ===
        // Obstacles: ice walls, frozen pillars, snowdrift barriers
        GameObject iceWall = CreateIceWallObstacle();
        GameObject frozenPillar = CreateFrozenPillarObstacle();
        GameObject snowdriftBarrier = CreateSnowdriftBarrierObstacle();
        GameObject iceSpikes = CreateIceSpikeObstacle();

        worldThemes.Add(new WorldThemeData
        {
            worldType = WorldType.IceWorld,
            displayName = "Ice World",
            skyColor = new Color(0.65f, 0.85f, 0.98f),
            fogColor = new Color(0.75f, 0.90f, 1.0f),
            fogDensity = 0.018f,
            lightColor = new Color(0.85f, 0.94f, 1.0f),
            lightIntensity = 1.15f,
            trackMaterial = CreateOrLoadMaterial("Worlds/Ice_Track", new Color(0.45f, 0.68f, 0.85f)),
            sideGroundMaterial = CreateOrLoadMaterial("Worlds/Ice_Side", new Color(0.92f, 0.96f, 1.0f)),
            laneDividerMaterial = CreateOrLoadMaterial("Worlds/Ice_Divider", new Color(1.0f, 1.0f, 1.0f)),
            hurdleMaterial = CreateOrLoadMaterial("Worlds/Ice_Hurdle", new Color(0.35f, 0.78f, 0.95f)),
            barrierMaterial = CreateOrLoadMaterial("Worlds/Ice_Barrier", new Color(0.22f, 0.55f, 0.85f)),
            coinMaterial = CreateOrLoadMaterial("Worlds/Ice_Coin", new Color(0.35f, 0.95f, 1.0f)),
            sidePropPrefabs = new GameObject[] { propIceCrystal, propSnowMound },
            customObstaclePrefabs = new GameObject[] { iceWall, frozenPillar, snowdriftBarrier, iceSpikes },
            detailPropPrefabs = new GameObject[] { detailIceShard, detailSnowPile },
            overheadPropPrefabs = new GameObject[] { overheadIceArch },
            detailDensity = 0.5f,
            overheadDensity = 0.2f,
            hasAmbientParticles = true,
            particleColor = new Color(0.95f, 0.97f, 1.0f)
        });

        // === MECHANICAL ===
        // Obstacles: robots, gear crushers, laser gates, steam pipes
        GameObject robotObstaclePrefab = CreateRobotObstaclePrefab();
        GameObject gearCrusher = CreateMechGearCrusherObstacle();
        GameObject laserGate = CreateMechLaserGateObstacle();
        GameObject steamPipe = CreateMechSteamPipeObstacle();

        WorldThemeData mechTheme = new WorldThemeData
        {
            worldType = WorldType.Mechanical,
            displayName = "Mechanical",
            skyColor = new Color(0.12f, 0.14f, 0.18f),
            fogColor = new Color(0.18f, 0.20f, 0.25f),
            fogDensity = 0.016f,
            lightColor = new Color(0.95f, 0.85f, 0.75f),
            lightIntensity = 1.1f,
            trackMaterial = CreateOrLoadMaterial("Worlds/Mech_Track", new Color(0.18f, 0.20f, 0.22f)),
            sideGroundMaterial = CreateOrLoadMaterial("Worlds/Mech_Side", new Color(0.28f, 0.30f, 0.35f)),
            laneDividerMaterial = CreateOrLoadMaterial("Worlds/Mech_Divider", new Color(0.98f, 0.85f, 0.12f)),
            hurdleMaterial = CreateOrLoadMaterial("Worlds/Mech_Hurdle", new Color(0.92f, 0.45f, 0.12f)),
            barrierMaterial = CreateOrLoadMaterial("Worlds/Mech_Barrier", new Color(0.38f, 0.42f, 0.46f)),
            coinMaterial = CreateOrLoadMaterial("Worlds/Mech_Coin", new Color(1.0f, 0.55f, 0.15f)),
            sidePropPrefabs = new GameObject[] { propMechPillar, propMechBox },
            customObstaclePrefabs = new GameObject[] { robotObstaclePrefab, gearCrusher, laserGate, steamPipe },
            detailPropPrefabs = new GameObject[] { detailMechPipe, detailMechWire },
            overheadPropPrefabs = new GameObject[] { overheadMechArch },
            detailDensity = 0.5f,
            overheadDensity = 0.3f,
            hasAmbientParticles = true,
            particleColor = new Color(1.0f, 0.6f, 0.1f)
        };
        worldThemes.Add(mechTheme);

        // 4. Create Core Prefabs & Items
        // (Kept as defaults for non-themed fallback)
        GameObject hurdlePrefab = CreateHurdlePrefab(worldThemes[0].hurdleMaterial);
        GameObject barrierPrefab = CreateBarrierPrefab(worldThemes[0].barrierMaterial);
        GameObject cratePrefab = CreateCratePrefab(matCrate);
        GameObject spikesPrefab = CreateSpikesPrefab(matSpikes);

        GameObject coinPrefab = CreateCoinPrefab(worldThemes[0].coinMaterial);
        GameObject gemPrefab = CreateGemPrefab(matGem);

        GameObject pMagnet = CreatePowerUpPrefab("PowerUp_Magnet", PowerUpType.Magnet, matPowerMagnet, PrimitiveType.Cylinder);
        GameObject pShield = CreatePowerUpPrefab("PowerUp_Shield", PowerUpType.Shield, matPowerShield, PrimitiveType.Sphere);
        GameObject pSpeed = CreatePowerUpPrefab("PowerUp_Speed", PowerUpType.SpeedBoost, matPowerSpeed, PrimitiveType.Capsule);
        GameObject pMult = CreatePowerUpPrefab("PowerUp_Multiplier", PowerUpType.ScoreMultiplier, matPowerMult, PrimitiveType.Cube);
        GameObject pStar = CreatePowerUpPrefab("PowerUp_Star", PowerUpType.Invincibility, matPowerStar, PrimitiveType.Sphere);

        GameObject[] obstaclePrefabs = new GameObject[] { hurdlePrefab, barrierPrefab, cratePrefab, spikesPrefab };
        GameObject[] powerUpPrefabs = new GameObject[] { pMagnet, pShield, pSpeed, pMult, pStar };

        GameObject trackPrefab = CreateThemedTrackPrefab(worldThemes[0].trackMaterial, worldThemes[0].sideGroundMaterial, worldThemes[0].laneDividerMaterial);

        // 5. Setup Scene
        SetupScene(matPlayer, matShield, trackPrefab, obstaclePrefabs, coinPrefab, gemPrefab, powerUpPrefabs, worldThemes);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!Application.isBatchMode)
        {
            EditorUtility.DisplayDialog(
                "Success!",
                "Ultimate Realistic Platform Runner created successfully!\n\n• Procedural Sound Engine & High-Energy Cyber Beats\n• Realistic Animated Boy with Running, Sliding, Jumping & Death\n• Slide Under High Barriers (Down / S / Swipe Down)\n• Touch / Mouse Swipe & Keyboard Controls\n• Cinematic Dynamic Camera (Banking, Head-Bob & Speed FOV)\n• Screen Shake & Hit Impacts\n• Combo Multiplier & Near-Miss Dodging\n• 3D Floating Score & Text Popups\n• Speed Tiers & Adrenaline Warping\n• Distance & Score Milestones\n• Daily Missions & Quests\n• Persistent Perk Upgrades\n• 5 Rich Themed Worlds & Moving Obstacles\n\nClick Play (▶) to immerse!",
                "Play Now!"
            );
        }
    }

    // =========================================================================
    // WORLD-SPECIFIC OBSTACLES
    // =========================================================================

    // === GRASSLAND OBSTACLES ===

    private static GameObject CreateGrasslandFenceObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_GrassFence.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Grassland_Hurdle", new Color(0.58f, 0.38f, 0.18f));

        GameObject root = new GameObject("Obstacle_GrassFence");
        root.tag = "Obstacle";

        // Fence posts
        for (int i = -1; i <= 1; i++)
        {
            GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.name = "FencePost";
            post.transform.SetParent(root.transform, false);
            post.transform.localPosition = new Vector3(i * 0.8f, 0.55f, 0f);
            post.transform.localScale = new Vector3(0.12f, 1.1f, 0.12f);
            post.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(post.GetComponent<Collider>());
        }

        // Horizontal planks
        for (int j = 0; j < 3; j++)
        {
            GameObject plank = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plank.name = "Plank";
            plank.transform.SetParent(root.transform, false);
            plank.transform.localPosition = new Vector3(0f, 0.2f + j * 0.35f, 0f);
            plank.transform.localScale = new Vector3(2.0f, 0.10f, 0.06f);
            plank.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(plank.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.55f, 0f);
        col.size = new Vector3(2.0f, 1.1f, 0.5f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateGrasslandHayBaleObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_HayBale.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Grassland_HayBale", new Color(0.85f, 0.72f, 0.32f));

        GameObject root = new GameObject("Obstacle_HayBale");
        root.tag = "Obstacle";

        // Cylinder hay bale
        GameObject bale = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        bale.name = "Bale";
        bale.transform.SetParent(root.transform, false);
        bale.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        bale.transform.localScale = new Vector3(1.2f, 0.5f, 1.2f);
        bale.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        bale.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(bale.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.5f, 0f);
        col.size = new Vector3(1.2f, 1.2f, 1.2f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateGrasslandBarrelObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_Barrel.prefab";
        Material matWood = CreateOrLoadMaterial("Worlds/Grassland_Barrel", new Color(0.52f, 0.32f, 0.15f));
        Material matRing = CreateOrLoadMaterial("Worlds/Grassland_BarrelRing", new Color(0.35f, 0.35f, 0.38f));

        GameObject root = new GameObject("Obstacle_Barrel");
        root.tag = "Obstacle";

        // Barrel body
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        body.name = "BarrelBody";
        body.transform.SetParent(root.transform, false);
        body.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        body.transform.localScale = new Vector3(0.85f, 0.7f, 0.85f);
        body.GetComponent<Renderer>().sharedMaterial = matWood;
        DestroyImmediate(body.GetComponent<Collider>());

        // Metal rings
        for (int i = 0; i < 2; i++)
        {
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Ring";
            ring.transform.SetParent(root.transform, false);
            ring.transform.localPosition = new Vector3(0f, 0.3f + i * 0.8f, 0f);
            ring.transform.localScale = new Vector3(0.92f, 0.04f, 0.92f);
            ring.GetComponent<Renderer>().sharedMaterial = matRing;
            DestroyImmediate(ring.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.7f, 0f);
        col.size = new Vector3(0.9f, 1.4f, 0.9f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateGrasslandGateObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_GrassGate.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Grassland_Hurdle", new Color(0.58f, 0.38f, 0.18f));

        GameObject root = new GameObject("Obstacle_GrassGate");
        root.tag = "Obstacle";

        // Two tall posts
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.transform.SetParent(root.transform, false);
            post.transform.localPosition = new Vector3(i * 0.95f, 1.25f, 0f);
            post.transform.localScale = new Vector3(0.18f, 2.5f, 0.18f);
            post.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(post.GetComponent<Collider>());
        }

        // Top beam
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        beam.transform.SetParent(root.transform, false);
        beam.transform.localPosition = new Vector3(0f, 2.5f, 0f);
        beam.transform.localScale = new Vector3(2.2f, 0.15f, 0.15f);
        beam.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(beam.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.25f, 0f);
        col.size = new Vector3(2.2f, 2.5f, 0.5f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // === JUNGLE OBSTACLES ===

    private static GameObject CreateJungleVineWallObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_JungleVineWall.prefab";
        Material matVine = CreateOrLoadMaterial("Worlds/Jungle_Vine", new Color(0.18f, 0.48f, 0.12f));
        Material matTrunk = CreateOrLoadMaterial("Worlds/Jungle_Hurdle", new Color(0.46f, 0.30f, 0.15f));

        GameObject root = new GameObject("Obstacle_JungleVineWall");
        root.tag = "Obstacle";

        // Thick vine strands
        for (int i = -1; i <= 1; i++)
        {
            GameObject vine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vine.transform.SetParent(root.transform, false);
            vine.transform.localPosition = new Vector3(i * 0.5f, 1.2f, 0f);
            vine.transform.localScale = new Vector3(0.15f, 1.2f, 0.15f);
            vine.transform.rotation = Quaternion.Euler(0f, 0f, i * 8f);
            vine.GetComponent<Renderer>().sharedMaterial = matVine;
            DestroyImmediate(vine.GetComponent<Collider>());
        }

        // Cross vines
        for (int j = 0; j < 2; j++)
        {
            GameObject cross = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cross.transform.SetParent(root.transform, false);
            cross.transform.localPosition = new Vector3(0f, 0.5f + j * 0.9f, 0f);
            cross.transform.localScale = new Vector3(1.8f, 0.12f, 0.12f);
            cross.GetComponent<Renderer>().sharedMaterial = matTrunk;
            DestroyImmediate(cross.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.2f, 0f);
        col.size = new Vector3(1.8f, 2.4f, 0.5f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJungleFallenLogObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_JungleFallenLog.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Jungle_Hurdle", new Color(0.46f, 0.30f, 0.15f));

        GameObject root = new GameObject("Obstacle_JungleFallenLog");
        root.tag = "Obstacle";

        // Main log
        GameObject log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        log.transform.SetParent(root.transform, false);
        log.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        log.transform.localScale = new Vector3(0.55f, 1.1f, 0.55f);
        log.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        log.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(log.GetComponent<Collider>());

        // Moss patches
        Material matMoss = CreateOrLoadMaterial("Worlds/Jungle_Vine", new Color(0.18f, 0.48f, 0.12f));
        GameObject moss = GameObject.CreatePrimitive(PrimitiveType.Cube);
        moss.transform.SetParent(root.transform, false);
        moss.transform.localPosition = new Vector3(0.2f, 0.55f, 0f);
        moss.transform.localScale = new Vector3(0.4f, 0.08f, 0.5f);
        moss.GetComponent<Renderer>().sharedMaterial = matMoss;
        DestroyImmediate(moss.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.35f, 0f);
        col.size = new Vector3(2.2f, 0.7f, 0.7f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJungleTempleBlockObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_JungleTemple.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Jungle_Temple", new Color(0.52f, 0.48f, 0.38f));

        GameObject root = new GameObject("Obstacle_JungleTemple");
        root.tag = "Obstacle";

        // Stacked stone blocks
        for (int j = 0; j < 2; j++)
        {
            float scale = 1.0f - j * 0.2f;
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.transform.SetParent(root.transform, false);
            block.transform.localPosition = new Vector3(0f, 0.3f + j * 0.55f, 0f);
            block.transform.localScale = new Vector3(1.2f * scale, 0.5f, 0.8f * scale);
            block.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(block.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.6f, 0f);
        col.size = new Vector3(1.2f, 1.2f, 0.8f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJungleBoulderObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_JungleBoulder.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Jungle_Barrier", new Color(0.42f, 0.40f, 0.35f));

        GameObject root = new GameObject("Obstacle_JungleBoulder");
        root.tag = "Obstacle";

        GameObject boulder = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        boulder.transform.SetParent(root.transform, false);
        boulder.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        boulder.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        boulder.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(boulder.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.65f, 0f);
        col.size = new Vector3(1.2f, 1.3f, 1.2f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // === DESERT OBSTACLES ===

    private static GameObject CreateDesertSandstoneWallObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_DesertSandstoneWall.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Sandstone", new Color(0.82f, 0.68f, 0.42f));

        GameObject root = new GameObject("Obstacle_DesertSandstoneWall");
        root.tag = "Obstacle";

        // Layered sandstone blocks
        for (int j = 0; j < 3; j++)
        {
            float offset = (j % 2 == 0) ? 0f : 0.15f;
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.transform.SetParent(root.transform, false);
            block.transform.localPosition = new Vector3(offset, 0.25f + j * 0.5f, 0f);
            block.transform.localScale = new Vector3(2.0f, 0.45f, 0.6f);
            block.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(block.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.9f, 0f);
        col.size = new Vector3(2.0f, 1.8f, 0.6f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateDesertDuneBarrierObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_DesertDune.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Side", new Color(0.86f, 0.68f, 0.38f));

        GameObject root = new GameObject("Obstacle_DesertDune");
        root.tag = "Obstacle";

        // Sand dune shape (stretched sphere)
        GameObject dune = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dune.transform.SetParent(root.transform, false);
        dune.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        dune.transform.localScale = new Vector3(2.0f, 0.6f, 1.5f);
        dune.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(dune.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.3f, 0f);
        col.size = new Vector3(1.8f, 0.6f, 1.2f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateDesertAncientPillarObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_DesertPillar.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Sandstone", new Color(0.82f, 0.68f, 0.42f));

        GameObject root = new GameObject("Obstacle_DesertPillar");
        root.tag = "Obstacle";

        // Crumbling pillar
        GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.transform.SetParent(root.transform, false);
        pillar.transform.localPosition = new Vector3(0f, 1.0f, 0f);
        pillar.transform.localScale = new Vector3(0.7f, 1.0f, 0.7f);
        pillar.transform.rotation = Quaternion.Euler(0f, 0f, 5f);
        pillar.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(pillar.GetComponent<Collider>());

        // Base
        GameObject baseBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseBlock.transform.SetParent(root.transform, false);
        baseBlock.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        baseBlock.transform.localScale = new Vector3(1.0f, 0.25f, 1.0f);
        baseBlock.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(baseBlock.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.0f, 0f);
        col.size = new Vector3(0.8f, 2.0f, 0.8f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateDesertPotteryObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_DesertPottery.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Pottery", new Color(0.72f, 0.42f, 0.22f));

        GameObject root = new GameObject("Obstacle_DesertPottery");
        root.tag = "Obstacle";

        // Large pot
        GameObject pot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pot.transform.SetParent(root.transform, false);
        pot.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        pot.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        pot.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(pot.GetComponent<Collider>());

        // Pot neck
        GameObject neck = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        neck.transform.SetParent(root.transform, false);
        neck.transform.localPosition = new Vector3(0f, 0.95f, 0f);
        neck.transform.localScale = new Vector3(0.4f, 0.12f, 0.4f);
        neck.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(neck.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.5f, 0f);
        col.size = new Vector3(0.9f, 1.0f, 0.9f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // === ICE WORLD OBSTACLES ===

    private static GameObject CreateIceWallObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_IceWall.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Barrier", new Color(0.22f, 0.55f, 0.85f));
        Material matFrost = CreateOrLoadMaterial("Worlds/Ice_Frost", new Color(0.85f, 0.92f, 1.0f));

        GameObject root = new GameObject("Obstacle_IceWall");
        root.tag = "Obstacle";

        // Ice block wall
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.SetParent(root.transform, false);
        wall.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        wall.transform.localScale = new Vector3(2.0f, 2.2f, 0.4f);
        wall.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(wall.GetComponent<Collider>());

        // Frost detail on top
        GameObject frost = GameObject.CreatePrimitive(PrimitiveType.Cube);
        frost.transform.SetParent(root.transform, false);
        frost.transform.localPosition = new Vector3(0f, 2.3f, 0f);
        frost.transform.localScale = new Vector3(2.2f, 0.15f, 0.5f);
        frost.GetComponent<Renderer>().sharedMaterial = matFrost;
        DestroyImmediate(frost.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.1f, 0f);
        col.size = new Vector3(2.0f, 2.2f, 0.5f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateFrozenPillarObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_FrozenPillar.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Hurdle", new Color(0.35f, 0.78f, 0.95f));

        GameObject root = new GameObject("Obstacle_FrozenPillar");
        root.tag = "Obstacle";

        // Icicle pillar
        GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.transform.SetParent(root.transform, false);
        pillar.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        pillar.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
        pillar.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(pillar.GetComponent<Collider>());

        // Pointy top
        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        top.transform.SetParent(root.transform, false);
        top.transform.localPosition = new Vector3(0f, 1.85f, 0f);
        top.transform.localScale = new Vector3(0.55f, 0.4f, 0.55f);
        top.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(top.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.9f, 0f);
        col.size = new Vector3(0.7f, 2.0f, 0.7f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateSnowdriftBarrierObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_Snowdrift.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Side", new Color(0.92f, 0.96f, 1.0f));

        GameObject root = new GameObject("Obstacle_Snowdrift");
        root.tag = "Obstacle";

        // Snowdrift shape (flattened sphere)
        GameObject drift = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        drift.transform.SetParent(root.transform, false);
        drift.transform.localPosition = new Vector3(0f, 0.3f, 0f);
        drift.transform.localScale = new Vector3(1.8f, 0.7f, 1.4f);
        drift.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(drift.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.3f, 0f);
        col.size = new Vector3(1.6f, 0.7f, 1.2f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateIceSpikeObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_IceSpikes.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Hurdle", new Color(0.35f, 0.78f, 0.95f));

        GameObject root = new GameObject("Obstacle_IceSpikes");
        root.tag = "Obstacle";

        // Multiple ice spikes jutting from ground
        for (int i = -1; i <= 1; i++)
        {
            GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spike.transform.SetParent(root.transform, false);
            spike.transform.localPosition = new Vector3(i * 0.6f, 0.45f, 0f);
            spike.transform.localScale = new Vector3(0.25f, 0.9f, 0.25f);
            spike.transform.rotation = Quaternion.Euler(0f, 45f, i * 12f);
            spike.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(spike.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.45f, 0f);
        col.size = new Vector3(2.0f, 0.9f, 0.8f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // === MECHANICAL OBSTACLES ===

    private static GameObject CreateMechGearCrusherObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_MechGearCrusher.prefab";
        Material matGear = CreateOrLoadMaterial("Worlds/Mech_Gear", new Color(0.45f, 0.48f, 0.52f));
        Material matAccent = CreateOrLoadMaterial("Worlds/Mech_Divider", new Color(0.98f, 0.85f, 0.12f));

        GameObject root = new GameObject("Obstacle_MechGearCrusher");
        root.tag = "Obstacle";

        // Central gear body
        GameObject gear = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        gear.transform.SetParent(root.transform, false);
        gear.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        gear.transform.localScale = new Vector3(1.4f, 0.2f, 1.4f);
        gear.GetComponent<Renderer>().sharedMaterial = matGear;
        DestroyImmediate(gear.GetComponent<Collider>());

        // Gear teeth
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f;
            float rad = angle * Mathf.Deg2Rad;
            float x = Mathf.Cos(rad) * 0.7f;
            float z = Mathf.Sin(rad) * 0.7f;

            GameObject tooth = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tooth.transform.SetParent(root.transform, false);
            tooth.transform.localPosition = new Vector3(x, 0.6f, z);
            tooth.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            tooth.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            tooth.GetComponent<Renderer>().sharedMaterial = matAccent;
            DestroyImmediate(tooth.GetComponent<Collider>());
        }

        // Support pillar
        GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pillar.transform.SetParent(root.transform, false);
        pillar.transform.localPosition = new Vector3(0f, 0.3f, 0f);
        pillar.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        pillar.GetComponent<Renderer>().sharedMaterial = matGear;
        DestroyImmediate(pillar.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.6f, 0f);
        col.size = new Vector3(1.6f, 0.6f, 1.6f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateMechLaserGateObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_MechLaserGate.prefab";
        Material matFrame = CreateOrLoadMaterial("Worlds/Mech_Barrier", new Color(0.38f, 0.42f, 0.46f));
        Material matLaser = CreateOrLoadMaterial("Worlds/Mech_Laser", new Color(1.0f, 0.15f, 0.15f));

        GameObject root = new GameObject("Obstacle_MechLaserGate");
        root.tag = "Obstacle";

        // Frame pillars
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.transform.SetParent(root.transform, false);
            post.transform.localPosition = new Vector3(i * 1.0f, 1.25f, 0f);
            post.transform.localScale = new Vector3(0.2f, 2.5f, 0.2f);
            post.GetComponent<Renderer>().sharedMaterial = matFrame;
            DestroyImmediate(post.GetComponent<Collider>());
        }

        // Laser beams (horizontal red bars)
        for (int j = 0; j < 4; j++)
        {
            GameObject laser = GameObject.CreatePrimitive(PrimitiveType.Cube);
            laser.name = "LaserBeam";
            laser.transform.SetParent(root.transform, false);
            laser.transform.localPosition = new Vector3(0f, 0.35f + j * 0.55f, 0f);
            laser.transform.localScale = new Vector3(1.8f, 0.06f, 0.06f);
            laser.GetComponent<Renderer>().sharedMaterial = matLaser;
            DestroyImmediate(laser.GetComponent<Collider>());
        }

        // Top bar
        GameObject topBar = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topBar.transform.SetParent(root.transform, false);
        topBar.transform.localPosition = new Vector3(0f, 2.5f, 0f);
        topBar.transform.localScale = new Vector3(2.4f, 0.15f, 0.15f);
        topBar.GetComponent<Renderer>().sharedMaterial = matFrame;
        DestroyImmediate(topBar.GetComponent<Collider>());

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.25f, 0f);
        col.size = new Vector3(2.0f, 2.5f, 0.4f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateMechSteamPipeObstacle()
    {
        string path = "Assets/Prefabs/Obstacle_MechSteamPipe.prefab";
        Material matPipe = CreateOrLoadMaterial("Worlds/Mech_Barrier", new Color(0.38f, 0.42f, 0.46f));
        Material matValve = CreateOrLoadMaterial("Worlds/Mech_Hurdle", new Color(0.92f, 0.45f, 0.12f));

        GameObject root = new GameObject("Obstacle_MechSteamPipe");
        root.tag = "Obstacle";

        // Horizontal pipe
        GameObject pipe = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        pipe.transform.SetParent(root.transform, false);
        pipe.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        pipe.transform.localScale = new Vector3(0.5f, 1.0f, 0.5f);
        pipe.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        pipe.GetComponent<Renderer>().sharedMaterial = matPipe;
        DestroyImmediate(pipe.GetComponent<Collider>());

        // Valve wheel
        GameObject valve = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        valve.transform.SetParent(root.transform, false);
        valve.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        valve.transform.localScale = new Vector3(0.4f, 0.04f, 0.4f);
        valve.GetComponent<Renderer>().sharedMaterial = matValve;
        DestroyImmediate(valve.GetComponent<Collider>());

        // Support brackets
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject bracket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bracket.transform.SetParent(root.transform, false);
            bracket.transform.localPosition = new Vector3(i * 0.65f, 0.25f, 0f);
            bracket.transform.localScale = new Vector3(0.12f, 0.5f, 0.12f);
            bracket.GetComponent<Renderer>().sharedMaterial = matPipe;
            DestroyImmediate(bracket.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.45f, 0f);
        col.size = new Vector3(2.0f, 0.9f, 0.6f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // =========================================================================
    // DETAIL PROPS (small environment scatter)
    // =========================================================================

    private static GameObject CreateGrassTuftProp()
    {
        string path = "Assets/Prefabs/Props/Detail_GrassTuft.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Grassland_GrassTuft", new Color(0.22f, 0.62f, 0.18f));

        GameObject root = new GameObject("Detail_GrassTuft");
        for (int i = -1; i <= 1; i++)
        {
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.transform.SetParent(root.transform, false);
            blade.transform.localPosition = new Vector3(i * 0.08f, 0.15f, 0f);
            blade.transform.localScale = new Vector3(0.04f, 0.35f, 0.04f);
            blade.transform.rotation = Quaternion.Euler(0f, 0f, i * 15f);
            blade.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(blade.GetComponent<Collider>());
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateFlowerProp()
    {
        string path = "Assets/Prefabs/Props/Detail_Flower.prefab";
        Material matStem = CreateOrLoadMaterial("Worlds/Grassland_GrassTuft", new Color(0.22f, 0.62f, 0.18f));
        Material matPetal = CreateOrLoadMaterial("Worlds/Grassland_Flower", new Color(0.95f, 0.35f, 0.45f));

        GameObject root = new GameObject("Detail_Flower");

        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cube);
        stem.transform.SetParent(root.transform, false);
        stem.transform.localPosition = new Vector3(0f, 0.15f, 0f);
        stem.transform.localScale = new Vector3(0.03f, 0.3f, 0.03f);
        stem.GetComponent<Renderer>().sharedMaterial = matStem;
        DestroyImmediate(stem.GetComponent<Collider>());

        GameObject petal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        petal.transform.SetParent(root.transform, false);
        petal.transform.localPosition = new Vector3(0f, 0.32f, 0f);
        petal.transform.localScale = new Vector3(0.15f, 0.1f, 0.15f);
        petal.GetComponent<Renderer>().sharedMaterial = matPetal;
        DestroyImmediate(petal.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJungleFernProp()
    {
        string path = "Assets/Prefabs/Props/Detail_JungleFern.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Jungle_Vine", new Color(0.18f, 0.48f, 0.12f));

        GameObject root = new GameObject("Detail_JungleFern");
        for (int i = -2; i <= 2; i++)
        {
            GameObject frond = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frond.transform.SetParent(root.transform, false);
            frond.transform.localPosition = new Vector3(i * 0.1f, 0.22f, 0f);
            frond.transform.localScale = new Vector3(0.06f, 0.45f, 0.15f);
            frond.transform.rotation = Quaternion.Euler(0f, 0f, i * 22f);
            frond.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(frond.GetComponent<Collider>());
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJungleMushroomProp()
    {
        string path = "Assets/Prefabs/Props/Detail_JungleMushroom.prefab";
        Material matStem = CreateOrLoadMaterial("Worlds/Jungle_MushroomStem", new Color(0.85f, 0.82f, 0.72f));
        Material matCap = CreateOrLoadMaterial("Worlds/Jungle_MushroomCap", new Color(0.82f, 0.22f, 0.18f));

        GameObject root = new GameObject("Detail_JungleMushroom");

        GameObject mushStem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        mushStem.transform.SetParent(root.transform, false);
        mushStem.transform.localPosition = new Vector3(0f, 0.12f, 0f);
        mushStem.transform.localScale = new Vector3(0.08f, 0.12f, 0.08f);
        mushStem.GetComponent<Renderer>().sharedMaterial = matStem;
        DestroyImmediate(mushStem.GetComponent<Collider>());

        GameObject mushCap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mushCap.transform.SetParent(root.transform, false);
        mushCap.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        mushCap.transform.localScale = new Vector3(0.25f, 0.12f, 0.25f);
        mushCap.GetComponent<Renderer>().sharedMaterial = matCap;
        DestroyImmediate(mushCap.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateDesertSkullProp()
    {
        string path = "Assets/Prefabs/Props/Detail_DesertSkull.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Bone", new Color(0.92f, 0.88f, 0.78f));

        GameObject root = new GameObject("Detail_DesertSkull");
        GameObject skull = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        skull.transform.SetParent(root.transform, false);
        skull.transform.localPosition = new Vector3(0f, 0.1f, 0f);
        skull.transform.localScale = new Vector3(0.2f, 0.15f, 0.18f);
        skull.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(skull.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateDesertTumbleweedProp()
    {
        string path = "Assets/Prefabs/Props/Detail_DesertTumbleweed.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Tumbleweed", new Color(0.65f, 0.52f, 0.28f));

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Detail_DesertTumbleweed";
        go.transform.localScale = new Vector3(0.35f, 0.3f, 0.35f);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateIceShardProp()
    {
        string path = "Assets/Prefabs/Props/Detail_IceShard.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Hurdle", new Color(0.35f, 0.78f, 0.95f));

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Detail_IceShard";
        go.transform.localScale = new Vector3(0.1f, 0.4f, 0.1f);
        go.transform.rotation = Quaternion.Euler(12f, 25f, 8f);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateSmallSnowPileProp()
    {
        string path = "Assets/Prefabs/Props/Detail_SnowPile.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Side", new Color(0.92f, 0.96f, 1.0f));

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Detail_SnowPile";
        go.transform.localScale = new Vector3(0.5f, 0.15f, 0.5f);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateMechPipeProp()
    {
        string path = "Assets/Prefabs/Props/Detail_MechPipe.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Mech_Barrier", new Color(0.38f, 0.42f, 0.46f));

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Detail_MechPipe";
        go.transform.localScale = new Vector3(0.12f, 0.5f, 0.12f);
        go.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        go.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateMechWireBoxProp()
    {
        string path = "Assets/Prefabs/Props/Detail_MechWireBox.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Mech_Gear", new Color(0.45f, 0.48f, 0.52f));
        Material matAccent = CreateOrLoadMaterial("Worlds/Mech_Divider", new Color(0.98f, 0.85f, 0.12f));

        GameObject root = new GameObject("Detail_MechWireBox");

        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.SetParent(root.transform, false);
        box.transform.localPosition = new Vector3(0f, 0.2f, 0f);
        box.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        box.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(box.GetComponent<Collider>());

        GameObject light = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        light.transform.SetParent(root.transform, false);
        light.transform.localPosition = new Vector3(0f, 0.38f, 0f);
        light.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
        light.GetComponent<Renderer>().sharedMaterial = matAccent;
        DestroyImmediate(light.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // =========================================================================
    // OVERHEAD PROPS (arches, vine canopies, etc.)
    // =========================================================================

    private static GameObject CreateGrassArchProp()
    {
        string path = "Assets/Prefabs/Props/Overhead_GrassArch.prefab";
        Material matWood = CreateOrLoadMaterial("Worlds/Grassland_Hurdle", new Color(0.58f, 0.38f, 0.18f));
        Material matLeaf = CreateOrLoadMaterial("Worlds/Grassland_Side", new Color(0.25f, 0.68f, 0.22f));

        GameObject root = new GameObject("Overhead_GrassArch");

        // Two side posts
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cube);
            post.transform.SetParent(root.transform, false);
            post.transform.localPosition = new Vector3(i * 5.5f, 2.5f, 0f);
            post.transform.localScale = new Vector3(0.3f, 5.0f, 0.3f);
            post.GetComponent<Renderer>().sharedMaterial = matWood;
            DestroyImmediate(post.GetComponent<Collider>());
        }

        // Arch top with leaves
        GameObject archTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        archTop.transform.SetParent(root.transform, false);
        archTop.transform.localPosition = new Vector3(0f, 5.2f, 0f);
        archTop.transform.localScale = new Vector3(11.5f, 0.3f, 0.5f);
        archTop.GetComponent<Renderer>().sharedMaterial = matWood;
        DestroyImmediate(archTop.GetComponent<Collider>());

        // Leaf canopy
        GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        canopy.transform.SetParent(root.transform, false);
        canopy.transform.localPosition = new Vector3(0f, 5.5f, 0f);
        canopy.transform.localScale = new Vector3(4f, 1.2f, 2f);
        canopy.GetComponent<Renderer>().sharedMaterial = matLeaf;
        DestroyImmediate(canopy.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJungleVineArchProp()
    {
        string path = "Assets/Prefabs/Props/Overhead_JungleVines.prefab";
        Material matVine = CreateOrLoadMaterial("Worlds/Jungle_Vine", new Color(0.18f, 0.48f, 0.12f));
        Material matTrunk = CreateOrLoadMaterial("Worlds/Jungle_Hurdle", new Color(0.46f, 0.30f, 0.15f));

        GameObject root = new GameObject("Overhead_JungleVines");

        // Thick trunk pillars on sides
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.transform.SetParent(root.transform, false);
            trunk.transform.localPosition = new Vector3(i * 5f, 2.5f, 0f);
            trunk.transform.localScale = new Vector3(0.6f, 2.5f, 0.6f);
            trunk.GetComponent<Renderer>().sharedMaterial = matTrunk;
            DestroyImmediate(trunk.GetComponent<Collider>());
        }

        // Vine canopy across top
        GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        canopy.transform.SetParent(root.transform, false);
        canopy.transform.localPosition = new Vector3(0f, 5.5f, 0f);
        canopy.transform.localScale = new Vector3(6f, 1.8f, 3f);
        canopy.GetComponent<Renderer>().sharedMaterial = matVine;
        DestroyImmediate(canopy.GetComponent<Collider>());

        // Hanging vines
        for (int v = -2; v <= 2; v++)
        {
            GameObject vine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            vine.transform.SetParent(root.transform, false);
            vine.transform.localPosition = new Vector3(v * 1.2f, 4.2f, 0f);
            vine.transform.localScale = new Vector3(0.04f, 0.6f + Mathf.Abs(v) * 0.15f, 0.04f);
            vine.GetComponent<Renderer>().sharedMaterial = matVine;
            DestroyImmediate(vine.GetComponent<Collider>());
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateDesertStoneArchProp()
    {
        string path = "Assets/Prefabs/Props/Overhead_DesertArch.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Desert_Sandstone", new Color(0.82f, 0.68f, 0.42f));

        GameObject root = new GameObject("Overhead_DesertArch");

        // Stone pillars
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.transform.SetParent(root.transform, false);
            pillar.transform.localPosition = new Vector3(i * 5.5f, 2.5f, 0f);
            pillar.transform.localScale = new Vector3(0.8f, 5.0f, 0.8f);
            pillar.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(pillar.GetComponent<Collider>());
        }

        // Arch top (weathered sandstone)
        GameObject archTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        archTop.transform.SetParent(root.transform, false);
        archTop.transform.localPosition = new Vector3(0f, 5.2f, 0f);
        archTop.transform.localScale = new Vector3(12f, 0.5f, 1.0f);
        archTop.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(archTop.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateIceArchProp()
    {
        string path = "Assets/Prefabs/Props/Overhead_IceArch.prefab";
        Material mat = CreateOrLoadMaterial("Worlds/Ice_Barrier", new Color(0.22f, 0.55f, 0.85f));
        Material matFrost = CreateOrLoadMaterial("Worlds/Ice_Frost", new Color(0.85f, 0.92f, 1.0f));

        GameObject root = new GameObject("Overhead_IceArch");

        // Frozen pillars
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.transform.SetParent(root.transform, false);
            pillar.transform.localPosition = new Vector3(i * 5f, 2.5f, 0f);
            pillar.transform.localScale = new Vector3(0.5f, 2.5f, 0.5f);
            pillar.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(pillar.GetComponent<Collider>());
        }

        // Ice bridge across
        GameObject bridge = GameObject.CreatePrimitive(PrimitiveType.Cube);
        bridge.transform.SetParent(root.transform, false);
        bridge.transform.localPosition = new Vector3(0f, 5.3f, 0f);
        bridge.transform.localScale = new Vector3(11f, 0.35f, 0.8f);
        bridge.GetComponent<Renderer>().sharedMaterial = matFrost;
        DestroyImmediate(bridge.GetComponent<Collider>());

        // Icicles hanging
        for (int ic = -3; ic <= 3; ic++)
        {
            GameObject icicle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            icicle.transform.SetParent(root.transform, false);
            icicle.transform.localPosition = new Vector3(ic * 1.3f, 4.8f, 0f);
            icicle.transform.localScale = new Vector3(0.06f, 0.3f + Random.Range(0f, 0.3f), 0.06f);
            icicle.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(icicle.GetComponent<Collider>());
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateMechGantryProp()
    {
        string path = "Assets/Prefabs/Props/Overhead_MechGantry.prefab";
        Material matMetal = CreateOrLoadMaterial("Worlds/Mech_Barrier", new Color(0.38f, 0.42f, 0.46f));
        Material matAccent = CreateOrLoadMaterial("Worlds/Mech_Divider", new Color(0.98f, 0.85f, 0.12f));

        GameObject root = new GameObject("Overhead_MechGantry");

        // Steel support beams
        for (int i = -1; i <= 1; i += 2)
        {
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beam.transform.SetParent(root.transform, false);
            beam.transform.localPosition = new Vector3(i * 5.5f, 3f, 0f);
            beam.transform.localScale = new Vector3(0.25f, 6f, 0.25f);
            beam.GetComponent<Renderer>().sharedMaterial = matMetal;
            DestroyImmediate(beam.GetComponent<Collider>());
        }

        // Cross beam
        GameObject crossBeam = GameObject.CreatePrimitive(PrimitiveType.Cube);
        crossBeam.transform.SetParent(root.transform, false);
        crossBeam.transform.localPosition = new Vector3(0f, 6.2f, 0f);
        crossBeam.transform.localScale = new Vector3(11.5f, 0.2f, 0.3f);
        crossBeam.GetComponent<Renderer>().sharedMaterial = matMetal;
        DestroyImmediate(crossBeam.GetComponent<Collider>());

        // Warning stripes
        for (int s = -2; s <= 2; s++)
        {
            GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.transform.SetParent(root.transform, false);
            stripe.transform.localPosition = new Vector3(s * 2f, 6.35f, 0f);
            stripe.transform.localScale = new Vector3(0.8f, 0.08f, 0.35f);
            stripe.GetComponent<Renderer>().sharedMaterial = matAccent;
            DestroyImmediate(stripe.GetComponent<Collider>());
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // =========================================================================
    // EXISTING METHODS (tags, folders, materials, old prefabs, character, scene)
    // =========================================================================

    private static void EnsureTags()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        string[] requiredTags = new string[] { "Obstacle", "Coin" };
        foreach (string tag in requiredTags)
        {
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue.Equals(tag))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
            }
        }
        tagManager.ApplyModifiedProperties();
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials")) AssetDatabase.CreateFolder("Assets", "Materials");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Worlds")) AssetDatabase.CreateFolder("Assets/Materials", "Worlds");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/PowerUps")) AssetDatabase.CreateFolder("Assets/Materials", "PowerUps");
        if (!AssetDatabase.IsValidFolder("Assets/Materials/Boy")) AssetDatabase.CreateFolder("Assets/Materials", "Boy");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Props")) AssetDatabase.CreateFolder("Assets/Prefabs", "Props");
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/PowerUps")) AssetDatabase.CreateFolder("Assets/Prefabs", "PowerUps");
    }

    private static Material CreateOrLoadMaterial(string relativePath, Color color, bool transparent = false)
    {
        string path = $"Assets/Materials/{relativePath}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null) shader = Shader.Find("Mobile/Diffuse");
            mat = new Material(shader);
            mat.color = color;
            if (transparent)
            {
                mat.SetFloat("_Mode", 3);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
            AssetDatabase.CreateAsset(mat, path);
        }
        else
        {
            mat.color = color;
            EditorUtility.SetDirty(mat);
        }
        return mat;
    }

    // --- Existing Collectibles & Obstacle Prefabs (kept as fallback) ---

    private static GameObject CreateRobotObstaclePrefab()
    {
        string path = "Assets/Prefabs/Obstacle_Robot.prefab";
        Material matBody = CreateOrLoadMaterial("Worlds/Mech_RobotBody", new Color(0.20f, 0.22f, 0.26f));
        Material matHead = CreateOrLoadMaterial("Worlds/Mech_RobotHead", new Color(0.32f, 0.36f, 0.40f));
        Material matEye = CreateOrLoadMaterial("Worlds/Mech_RobotEye", new Color(1.0f, 0.05f, 0.05f));
        Material matJoint = CreateOrLoadMaterial("Worlds/Mech_RobotJoint", new Color(0.95f, 0.80f, 0.12f));

        GameObject root = new GameObject("Obstacle_Robot");
        root.tag = "Obstacle";

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 1.0f, 0f);
        col.size = new Vector3(1.3f, 2.0f, 1.1f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        RobotObstacle robot = root.AddComponent<RobotObstacle>();

        // Robot Body Container
        GameObject bodyObj = new GameObject("Body");
        bodyObj.transform.SetParent(root.transform, false);

        // Torso
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torso.name = "Torso";
        torso.transform.SetParent(bodyObj.transform, false);
        torso.transform.localPosition = new Vector3(0f, 0.9f, 0f);
        torso.transform.localScale = new Vector3(1.0f, 1.0f, 0.8f);
        torso.GetComponent<Renderer>().sharedMaterial = matBody;
        DestroyImmediate(torso.GetComponent<Collider>());

        // Head
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(bodyObj.transform, false);
        head.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        head.transform.localScale = new Vector3(0.72f, 0.55f, 0.65f);
        head.GetComponent<Renderer>().sharedMaterial = matHead;
        DestroyImmediate(head.GetComponent<Collider>());

        // Glowing Eye / Visor
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Cube);
        eye.name = "EyeVisor";
        eye.transform.SetParent(head.transform, false);
        eye.transform.localPosition = new Vector3(0f, 0.05f, 0.34f);
        eye.transform.localScale = new Vector3(0.55f, 0.16f, 0.08f);
        Renderer eyeRend = eye.GetComponent<Renderer>();
        eyeRend.sharedMaterial = matEye;
        DestroyImmediate(eye.GetComponent<Collider>());

        // Antenna
        GameObject antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        antenna.name = "Antenna";
        antenna.transform.SetParent(head.transform, false);
        antenna.transform.localPosition = new Vector3(0f, 0.42f, 0f);
        antenna.transform.localScale = new Vector3(0.06f, 0.18f, 0.06f);
        antenna.GetComponent<Renderer>().sharedMaterial = matJoint;
        DestroyImmediate(antenna.GetComponent<Collider>());

        // Left Arm Pivot
        GameObject leftArmPivot = new GameObject("LeftArm");
        leftArmPivot.transform.SetParent(bodyObj.transform, false);
        leftArmPivot.transform.localPosition = new Vector3(-0.62f, 1.0f, 0f);

        GameObject leftArmMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftArmMesh.transform.SetParent(leftArmPivot.transform, false);
        leftArmMesh.transform.localPosition = new Vector3(0f, -0.2f, 0f);
        leftArmMesh.transform.localScale = new Vector3(0.18f, 0.35f, 0.18f);
        leftArmMesh.GetComponent<Renderer>().sharedMaterial = matJoint;
        DestroyImmediate(leftArmMesh.GetComponent<Collider>());

        // Right Arm Pivot
        GameObject rightArmPivot = new GameObject("RightArm");
        rightArmPivot.transform.SetParent(bodyObj.transform, false);
        rightArmPivot.transform.localPosition = new Vector3(0.62f, 1.0f, 0f);

        GameObject rightArmMesh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightArmMesh.transform.SetParent(rightArmPivot.transform, false);
        rightArmMesh.transform.localPosition = new Vector3(0f, -0.2f, 0f);
        rightArmMesh.transform.localScale = new Vector3(0.18f, 0.35f, 0.18f);
        rightArmMesh.GetComponent<Renderer>().sharedMaterial = matJoint;
        DestroyImmediate(rightArmMesh.GetComponent<Collider>());

        // Base / Treads
        GameObject treads = GameObject.CreatePrimitive(PrimitiveType.Cube);
        treads.name = "Treads";
        treads.transform.SetParent(root.transform, false);
        treads.transform.localPosition = new Vector3(0f, 0.18f, 0f);
        treads.transform.localScale = new Vector3(1.15f, 0.35f, 1.0f);
        treads.GetComponent<Renderer>().sharedMaterial = matHead;
        DestroyImmediate(treads.GetComponent<Collider>());

        // Hook up RobotObstacle
        robot.robotBody = bodyObj.transform;
        robot.leftArm = leftArmPivot.transform;
        robot.rightArm = rightArmPivot.transform;
        robot.eyeRenderer = eyeRend;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static void BuildBoyCharacter(GameObject player, PlayerController pc, CharacterController cc)
    {
        Material matSkin = CreateOrLoadMaterial("Boy/Mat_BoySkin", new Color(1.0f, 0.82f, 0.70f));
        Material matHair = CreateOrLoadMaterial("Boy/Mat_BoyHair", new Color(0.28f, 0.18f, 0.10f));
        Material matHeadband = CreateOrLoadMaterial("Boy/Mat_BoyHeadband", new Color(0.95f, 0.22f, 0.22f));
        Material matJacket = CreateOrLoadMaterial("Boy/Mat_BoyJacket", new Color(0.92f, 0.16f, 0.16f));
        Material matJeans = CreateOrLoadMaterial("Boy/Mat_BoyJeans", new Color(0.18f, 0.36f, 0.68f));
        Material matShoes = CreateOrLoadMaterial("Boy/Mat_BoyShoes", new Color(0.95f, 0.95f, 0.95f));
        Material matEyeWhite = CreateOrLoadMaterial("Boy/Mat_BoyEyeWhite", new Color(0.98f, 0.98f, 0.98f));
        Material matEyePupil = CreateOrLoadMaterial("Boy/Mat_BoyEyePupil", new Color(0.12f, 0.10f, 0.08f));
        Material matShoeLace = CreateOrLoadMaterial("Boy/Mat_BoyShoeLace", new Color(0.95f, 0.25f, 0.25f));

        GameObject boyRoot = new GameObject("BoyModel");
        boyRoot.transform.SetParent(player.transform, false);
        boyRoot.transform.localPosition = Vector3.zero;

        BoyCharacterAnimator animator = boyRoot.AddComponent<BoyCharacterAnimator>();
        animator.playerController = pc;
        animator.characterController = cc;

        // Body Root (bounces with running)
        GameObject bodyRoot = new GameObject("BodyRoot");
        bodyRoot.transform.SetParent(boyRoot.transform, false);
        animator.bodyRoot = bodyRoot.transform;

        // === TORSO (Red Hoodie with zipper detail) ===
        GameObject torso = GameObject.CreatePrimitive(PrimitiveType.Cube);
        torso.name = "Torso";
        torso.transform.SetParent(bodyRoot.transform, false);
        torso.transform.localPosition = new Vector3(0f, 0.95f, 0f);
        torso.transform.localScale = new Vector3(0.62f, 0.72f, 0.38f);
        torso.GetComponent<Renderer>().sharedMaterial = matJacket;
        DestroyImmediate(torso.GetComponent<Collider>());
        animator.torso = torso.transform;

        // Jacket zipper stripe
        GameObject zipper = GameObject.CreatePrimitive(PrimitiveType.Cube);
        zipper.name = "Zipper";
        zipper.transform.SetParent(torso.transform, false);
        zipper.transform.localPosition = new Vector3(0f, 0f, 0.51f);
        zipper.transform.localScale = new Vector3(0.06f, 0.85f, 0.02f);
        zipper.GetComponent<Renderer>().sharedMaterial = matEyeWhite;
        DestroyImmediate(zipper.GetComponent<Collider>());

        // Backpack on back of torso
        Material matPack = CreateOrLoadMaterial("Boy/Mat_BoyBackpack", new Color(0.18f, 0.20f, 0.24f));
        GameObject backpack = GameObject.CreatePrimitive(PrimitiveType.Cube);
        backpack.name = "Backpack";
        backpack.transform.SetParent(torso.transform, false);
        backpack.transform.localPosition = new Vector3(0f, 0.05f, -0.62f);
        backpack.transform.localScale = new Vector3(0.75f, 0.78f, 0.42f);
        backpack.GetComponent<Renderer>().sharedMaterial = matPack;
        DestroyImmediate(backpack.GetComponent<Collider>());
        animator.backpack = backpack.transform;

        GameObject packPocket = GameObject.CreatePrimitive(PrimitiveType.Cube);
        packPocket.name = "Pocket";
        packPocket.transform.SetParent(backpack.transform, false);
        packPocket.transform.localPosition = new Vector3(0f, -0.2f, -0.52f);
        packPocket.transform.localScale = new Vector3(0.78f, 0.42f, 0.22f);
        packPocket.GetComponent<Renderer>().sharedMaterial = matHeadband;
        DestroyImmediate(packPocket.GetComponent<Collider>());

        // Hood on back of neck
        GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hood.name = "Hood";
        hood.transform.SetParent(bodyRoot.transform, false);
        hood.transform.localPosition = new Vector3(0f, 1.35f, -0.15f);
        hood.transform.localScale = new Vector3(0.38f, 0.25f, 0.2f);
        hood.GetComponent<Renderer>().sharedMaterial = matJacket;
        DestroyImmediate(hood.GetComponent<Collider>());
        animator.hood = hood.transform;

        // === HEAD (more detailed with face) ===
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(bodyRoot.transform, false);
        head.transform.localPosition = new Vector3(0f, 1.55f, 0f);
        head.transform.localScale = new Vector3(0.48f, 0.50f, 0.46f);
        head.GetComponent<Renderer>().sharedMaterial = matSkin;
        DestroyImmediate(head.GetComponent<Collider>());
        animator.head = head.transform;

        // Hair (styled spiky)
        GameObject hair = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hair.name = "Hair";
        hair.transform.SetParent(head.transform, false);
        hair.transform.localPosition = new Vector3(0f, 0.28f, -0.06f);
        hair.transform.localScale = new Vector3(1.05f, 0.48f, 1.08f);
        hair.GetComponent<Renderer>().sharedMaterial = matHair;
        DestroyImmediate(hair.GetComponent<Collider>());

        // Hair fringe (front spikes)
        GameObject fringe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fringe.name = "HairFringe";
        fringe.transform.SetParent(head.transform, false);
        fringe.transform.localPosition = new Vector3(0f, 0.2f, 0.42f);
        fringe.transform.localScale = new Vector3(0.85f, 0.25f, 0.25f);
        fringe.GetComponent<Renderer>().sharedMaterial = matHair;
        DestroyImmediate(fringe.GetComponent<Collider>());
        animator.hairFringe = fringe.transform;

        // Headband
        GameObject headband = GameObject.CreatePrimitive(PrimitiveType.Cube);
        headband.name = "Headband";
        headband.transform.SetParent(head.transform, false);
        headband.transform.localPosition = new Vector3(0f, 0.08f, 0f);
        headband.transform.localScale = new Vector3(1.04f, 0.14f, 1.04f);
        headband.GetComponent<Renderer>().sharedMaterial = matHeadband;
        DestroyImmediate(headband.GetComponent<Collider>());

        // Headphones resting on neck/ears
        Material matHeadphones = CreateOrLoadMaterial("Boy/Mat_BoyHeadphones", new Color(0.12f, 0.12f, 0.14f));
        Material matPhoneAccent = CreateOrLoadMaterial("Boy/Mat_BoyHeadphoneAccent", new Color(0.0f, 0.85f, 1.0f));

        GameObject phoneArch = GameObject.CreatePrimitive(PrimitiveType.Cube);
        phoneArch.name = "HeadphoneArch";
        phoneArch.transform.SetParent(head.transform, false);
        phoneArch.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        phoneArch.transform.localScale = new Vector3(1.15f, 0.07f, 0.15f);
        phoneArch.GetComponent<Renderer>().sharedMaterial = matHeadphones;
        DestroyImmediate(phoneArch.GetComponent<Collider>());

        for (int ear = -1; ear <= 1; ear += 2)
        {
            GameObject phoneCup = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            phoneCup.name = ear < 0 ? "LeftHeadphone" : "RightHeadphone";
            phoneCup.transform.SetParent(head.transform, false);
            phoneCup.transform.localPosition = new Vector3(ear * 0.54f, 0.02f, 0f);
            phoneCup.transform.localScale = new Vector3(0.24f, 0.08f, 0.24f);
            phoneCup.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            phoneCup.GetComponent<Renderer>().sharedMaterial = matPhoneAccent;
            DestroyImmediate(phoneCup.GetComponent<Collider>());
        }

        // Eyes
        for (int e = -1; e <= 1; e += 2)
        {
            // Eye white
            GameObject eyeWhite = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eyeWhite.name = e < 0 ? "LeftEye" : "RightEye";
            eyeWhite.transform.SetParent(head.transform, false);
            eyeWhite.transform.localPosition = new Vector3(e * 0.22f, -0.02f, 0.42f);
            eyeWhite.transform.localScale = new Vector3(0.22f, 0.2f, 0.1f);
            eyeWhite.GetComponent<Renderer>().sharedMaterial = matEyeWhite;
            DestroyImmediate(eyeWhite.GetComponent<Collider>());

            // Pupil
            GameObject pupil = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pupil.name = e < 0 ? "LeftPupil" : "RightPupil";
            pupil.transform.SetParent(eyeWhite.transform, false);
            pupil.transform.localPosition = new Vector3(0f, 0f, 0.35f);
            pupil.transform.localScale = new Vector3(0.5f, 0.55f, 0.3f);
            pupil.GetComponent<Renderer>().sharedMaterial = matEyePupil;
            DestroyImmediate(pupil.GetComponent<Collider>());
            if (e < 0) animator.leftPupil = pupil.transform;
            else animator.rightPupil = pupil.transform;
        }

        // Mouth (small cube)
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mouth.name = "Mouth";
        mouth.transform.SetParent(head.transform, false);
        mouth.transform.localPosition = new Vector3(0f, -0.22f, 0.45f);
        mouth.transform.localScale = new Vector3(0.28f, 0.06f, 0.04f);
        mouth.GetComponent<Renderer>().sharedMaterial = matEyePupil;
        DestroyImmediate(mouth.GetComponent<Collider>());

        // Nose
        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        nose.name = "Nose";
        nose.transform.SetParent(head.transform, false);
        nose.transform.localPosition = new Vector3(0f, -0.08f, 0.5f);
        nose.transform.localScale = new Vector3(0.1f, 0.08f, 0.08f);
        nose.GetComponent<Renderer>().sharedMaterial = matSkin;
        DestroyImmediate(nose.GetComponent<Collider>());

        // Ears
        for (int ear = -1; ear <= 1; ear += 2)
        {
            GameObject earObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            earObj.name = ear < 0 ? "LeftEar" : "RightEar";
            earObj.transform.SetParent(head.transform, false);
            earObj.transform.localPosition = new Vector3(ear * 0.5f, -0.05f, 0f);
            earObj.transform.localScale = new Vector3(0.1f, 0.14f, 0.08f);
            earObj.GetComponent<Renderer>().sharedMaterial = matSkin;
            DestroyImmediate(earObj.GetComponent<Collider>());
        }

        // === LEFT ARM (with articulated elbow joint) ===
        GameObject leftArm = new GameObject("LeftArmPivot");
        leftArm.transform.SetParent(bodyRoot.transform, false);
        leftArm.transform.localPosition = new Vector3(-0.42f, 1.25f, 0f);
        animator.leftArm = leftArm.transform;

        // Upper arm (jacket sleeve)
        GameObject leftUpperArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftUpperArm.name = "UpperArm";
        leftUpperArm.transform.SetParent(leftArm.transform, false);
        leftUpperArm.transform.localPosition = new Vector3(0f, -0.15f, 0f);
        leftUpperArm.transform.localScale = new Vector3(0.16f, 0.20f, 0.16f);
        leftUpperArm.GetComponent<Renderer>().sharedMaterial = matJacket;
        DestroyImmediate(leftUpperArm.GetComponent<Collider>());

        // Forearm pivot (elbow joint)
        GameObject leftForearmPivot = new GameObject("ForearmPivot");
        leftForearmPivot.transform.SetParent(leftArm.transform, false);
        leftForearmPivot.transform.localPosition = new Vector3(0f, -0.28f, 0f);
        animator.leftForearm = leftForearmPivot.transform;

        // Forearm (skin)
        GameObject leftForearm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftForearm.name = "Forearm";
        leftForearm.transform.SetParent(leftForearmPivot.transform, false);
        leftForearm.transform.localPosition = new Vector3(0f, -0.12f, 0f);
        leftForearm.transform.localScale = new Vector3(0.13f, 0.16f, 0.13f);
        leftForearm.GetComponent<Renderer>().sharedMaterial = matSkin;
        DestroyImmediate(leftForearm.GetComponent<Collider>());

        // Hand
        GameObject leftHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leftHand.name = "Hand";
        leftHand.transform.SetParent(leftForearmPivot.transform, false);
        leftHand.transform.localPosition = new Vector3(0f, -0.26f, 0f);
        leftHand.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
        leftHand.GetComponent<Renderer>().sharedMaterial = matSkin;
        DestroyImmediate(leftHand.GetComponent<Collider>());
        animator.leftHand = leftHand.transform;

        // Wrist sweatband accessory
        GameObject leftWristBand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftWristBand.name = "Wristband";
        leftWristBand.transform.SetParent(leftForearmPivot.transform, false);
        leftWristBand.transform.localPosition = new Vector3(0f, -0.20f, 0f);
        leftWristBand.transform.localScale = new Vector3(0.14f, 0.04f, 0.14f);
        leftWristBand.GetComponent<Renderer>().sharedMaterial = matHeadband;
        DestroyImmediate(leftWristBand.GetComponent<Collider>());

        // === RIGHT ARM (with articulated elbow joint) ===
        GameObject rightArm = new GameObject("RightArmPivot");
        rightArm.transform.SetParent(bodyRoot.transform, false);
        rightArm.transform.localPosition = new Vector3(0.42f, 1.25f, 0f);
        animator.rightArm = rightArm.transform;

        GameObject rightUpperArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightUpperArm.name = "UpperArm";
        rightUpperArm.transform.SetParent(rightArm.transform, false);
        rightUpperArm.transform.localPosition = new Vector3(0f, -0.15f, 0f);
        rightUpperArm.transform.localScale = new Vector3(0.16f, 0.20f, 0.16f);
        rightUpperArm.GetComponent<Renderer>().sharedMaterial = matJacket;
        DestroyImmediate(rightUpperArm.GetComponent<Collider>());

        // Forearm pivot (elbow joint)
        GameObject rightForearmPivot = new GameObject("ForearmPivot");
        rightForearmPivot.transform.SetParent(rightArm.transform, false);
        rightForearmPivot.transform.localPosition = new Vector3(0f, -0.28f, 0f);
        animator.rightForearm = rightForearmPivot.transform;

        GameObject rightForearm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightForearm.name = "Forearm";
        rightForearm.transform.SetParent(rightForearmPivot.transform, false);
        rightForearm.transform.localPosition = new Vector3(0f, -0.12f, 0f);
        rightForearm.transform.localScale = new Vector3(0.13f, 0.16f, 0.13f);
        rightForearm.GetComponent<Renderer>().sharedMaterial = matSkin;
        DestroyImmediate(rightForearm.GetComponent<Collider>());

        GameObject rightHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rightHand.name = "Hand";
        rightHand.transform.SetParent(rightForearmPivot.transform, false);
        rightHand.transform.localPosition = new Vector3(0f, -0.26f, 0f);
        rightHand.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);
        rightHand.GetComponent<Renderer>().sharedMaterial = matSkin;
        DestroyImmediate(rightHand.GetComponent<Collider>());
        animator.rightHand = rightHand.transform;

        // === LEFT LEG (with knee joint) ===
        GameObject leftLeg = new GameObject("LeftLegPivot");
        leftLeg.transform.SetParent(boyRoot.transform, false);
        leftLeg.transform.localPosition = new Vector3(-0.17f, 0.62f, 0f);
        animator.leftLeg = leftLeg.transform;

        // Upper thigh (jeans)
        GameObject leftThigh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftThigh.name = "Thigh";
        leftThigh.transform.SetParent(leftLeg.transform, false);
        leftThigh.transform.localPosition = new Vector3(0f, -0.15f, 0f);
        leftThigh.transform.localScale = new Vector3(0.18f, 0.20f, 0.18f);
        leftThigh.GetComponent<Renderer>().sharedMaterial = matJeans;
        DestroyImmediate(leftThigh.GetComponent<Collider>());

        // Lower leg pivot (knee)
        GameObject leftLowerLeg = new GameObject("LeftLowerLeg");
        leftLowerLeg.transform.SetParent(leftLeg.transform, false);
        leftLowerLeg.transform.localPosition = new Vector3(0f, -0.32f, 0f);
        animator.leftLowerLeg = leftLowerLeg.transform;

        // Shin (jeans)
        GameObject leftShin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftShin.name = "Shin";
        leftShin.transform.SetParent(leftLowerLeg.transform, false);
        leftShin.transform.localPosition = new Vector3(0f, -0.12f, 0f);
        leftShin.transform.localScale = new Vector3(0.15f, 0.18f, 0.15f);
        leftShin.GetComponent<Renderer>().sharedMaterial = matJeans;
        DestroyImmediate(leftShin.GetComponent<Collider>());

        // Shoe
        GameObject leftShoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftShoe.name = "Shoe";
        leftShoe.transform.SetParent(leftLowerLeg.transform, false);
        leftShoe.transform.localPosition = new Vector3(0f, -0.28f, 0.04f);
        leftShoe.transform.localScale = new Vector3(0.18f, 0.10f, 0.30f);
        leftShoe.GetComponent<Renderer>().sharedMaterial = matShoes;
        DestroyImmediate(leftShoe.GetComponent<Collider>());
        animator.leftShoe = leftShoe.transform;

        // Shoe sole (darker)
        GameObject leftSole = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftSole.name = "Sole";
        leftSole.transform.SetParent(leftShoe.transform, false);
        leftSole.transform.localPosition = new Vector3(0f, -0.45f, 0f);
        leftSole.transform.localScale = new Vector3(1.1f, 0.3f, 1.05f);
        leftSole.GetComponent<Renderer>().sharedMaterial = matEyePupil;
        DestroyImmediate(leftSole.GetComponent<Collider>());

        // Shoe lace accent
        GameObject leftLace = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftLace.name = "Lace";
        leftLace.transform.SetParent(leftShoe.transform, false);
        leftLace.transform.localPosition = new Vector3(0f, 0.35f, 0.15f);
        leftLace.transform.localScale = new Vector3(0.5f, 0.15f, 0.4f);
        leftLace.GetComponent<Renderer>().sharedMaterial = matShoeLace;
        DestroyImmediate(leftLace.GetComponent<Collider>());

        // === RIGHT LEG (with knee joint) ===
        GameObject rightLeg = new GameObject("RightLegPivot");
        rightLeg.transform.SetParent(boyRoot.transform, false);
        rightLeg.transform.localPosition = new Vector3(0.17f, 0.62f, 0f);
        animator.rightLeg = rightLeg.transform;

        GameObject rightThigh = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightThigh.name = "Thigh";
        rightThigh.transform.SetParent(rightLeg.transform, false);
        rightThigh.transform.localPosition = new Vector3(0f, -0.15f, 0f);
        rightThigh.transform.localScale = new Vector3(0.18f, 0.20f, 0.18f);
        rightThigh.GetComponent<Renderer>().sharedMaterial = matJeans;
        DestroyImmediate(rightThigh.GetComponent<Collider>());

        GameObject rightLowerLeg = new GameObject("RightLowerLeg");
        rightLowerLeg.transform.SetParent(rightLeg.transform, false);
        rightLowerLeg.transform.localPosition = new Vector3(0f, -0.32f, 0f);
        animator.rightLowerLeg = rightLowerLeg.transform;

        GameObject rightShin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightShin.name = "Shin";
        rightShin.transform.SetParent(rightLowerLeg.transform, false);
        rightShin.transform.localPosition = new Vector3(0f, -0.12f, 0f);
        rightShin.transform.localScale = new Vector3(0.15f, 0.18f, 0.15f);
        rightShin.GetComponent<Renderer>().sharedMaterial = matJeans;
        DestroyImmediate(rightShin.GetComponent<Collider>());

        GameObject rightShoe = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightShoe.name = "Shoe";
        rightShoe.transform.SetParent(rightLowerLeg.transform, false);
        rightShoe.transform.localPosition = new Vector3(0f, -0.28f, 0.04f);
        rightShoe.transform.localScale = new Vector3(0.18f, 0.10f, 0.30f);
        rightShoe.GetComponent<Renderer>().sharedMaterial = matShoes;
        DestroyImmediate(rightShoe.GetComponent<Collider>());
        animator.rightShoe = rightShoe.transform;

        GameObject rightSole = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightSole.name = "Sole";
        rightSole.transform.SetParent(rightShoe.transform, false);
        rightSole.transform.localPosition = new Vector3(0f, -0.45f, 0f);
        rightSole.transform.localScale = new Vector3(1.1f, 0.3f, 1.05f);
        rightSole.GetComponent<Renderer>().sharedMaterial = matEyePupil;
        DestroyImmediate(rightSole.GetComponent<Collider>());

        GameObject rightLace = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightLace.name = "Lace";
        rightLace.transform.SetParent(rightShoe.transform, false);
        rightLace.transform.localPosition = new Vector3(0f, 0.35f, 0.15f);
        rightLace.transform.localScale = new Vector3(0.5f, 0.15f, 0.4f);
        rightLace.GetComponent<Renderer>().sharedMaterial = matShoeLace;
        DestroyImmediate(rightLace.GetComponent<Collider>());
    }

    private static GameObject CreateGemPrefab(Material mat)
    {
        string path = "Assets/Prefabs/Gem.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Gem";
        go.tag = "Coin";
        go.transform.localScale = new Vector3(0.55f, 0.75f, 0.55f);
        go.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
        go.GetComponent<Renderer>().sharedMaterial = mat;

        DestroyImmediate(go.GetComponent<Collider>());
        SphereCollider col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.8f;

        go.AddComponent<Gem>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreatePowerUpPrefab(string name, PowerUpType type, Material mat, PrimitiveType prim)
    {
        string path = $"Assets/Prefabs/PowerUps/{name}.prefab";
        GameObject go = GameObject.CreatePrimitive(prim);
        go.name = name;
        go.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        go.GetComponent<Renderer>().sharedMaterial = mat;

        DestroyImmediate(go.GetComponent<Collider>());
        SphereCollider col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.9f;

        PowerUpItem item = go.AddComponent<PowerUpItem>();
        item.powerUpType = type;
        item.duration = type == PowerUpType.Invincibility ? 8f : 10f;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateCratePrefab(Material mat)
    {
        string path = "Assets/Prefabs/Obstacle_Crate.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Obstacle_Crate";
        go.tag = "Obstacle";
        go.transform.localScale = new Vector3(1.4f, 1.3f, 1.4f);
        go.GetComponent<Renderer>().sharedMaterial = mat;

        Obstacle obs = go.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateSpikesPrefab(Material mat)
    {
        string path = "Assets/Prefabs/Obstacle_Spikes.prefab";
        GameObject root = new GameObject("Obstacle_Spikes");
        root.tag = "Obstacle";

        for (int i = -1; i <= 1; i++)
        {
            GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spike.transform.SetParent(root.transform, false);
            spike.transform.localPosition = new Vector3(i * 0.7f, 0.3f, 0f);
            spike.transform.localScale = new Vector3(0.45f, 0.65f, 0.45f);
            spike.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
            spike.GetComponent<Renderer>().sharedMaterial = mat;
            DestroyImmediate(spike.GetComponent<Collider>());
        }

        BoxCollider col = root.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.35f, 0f);
        col.size = new Vector3(2.2f, 0.7f, 0.8f);

        Obstacle obs = root.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateHurdlePrefab(Material mat)
    {
        string path = "Assets/Prefabs/Obstacle_Hurdle.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Obstacle_Hurdle";
        go.tag = "Obstacle";
        go.transform.localScale = new Vector3(2.2f, 0.75f, 0.5f);
        go.GetComponent<Renderer>().sharedMaterial = mat;

        Obstacle obs = go.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.LowHurdle;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateBarrierPrefab(Material mat)
    {
        string path = "Assets/Prefabs/Obstacle_Barrier.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Obstacle_Barrier";
        go.tag = "Obstacle";
        go.transform.localScale = new Vector3(2.2f, 2.5f, 0.5f);
        go.GetComponent<Renderer>().sharedMaterial = mat;

        Obstacle obs = go.AddComponent<Obstacle>();
        obs.obstacleType = Obstacle.ObstacleType.HighBarrier;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateCoinPrefab(Material mat)
    {
        string path = "Assets/Prefabs/Coin.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Coin";
        go.tag = "Coin";
        go.transform.localScale = new Vector3(0.7f, 0.08f, 0.7f);
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        go.GetComponent<Renderer>().sharedMaterial = mat;

        DestroyImmediate(go.GetComponent<Collider>());
        SphereCollider col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.75f;

        go.AddComponent<Coin>();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateThemedTrackPrefab(Material roadMat, Material sideMat, Material lineMat)
    {
        string path = "Assets/Prefabs/TrackSegment.prefab";
        GameObject root = new GameObject("TrackSegment");
        TrackSegment segment = root.AddComponent<TrackSegment>();
        segment.length = 30f;

        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
        road.name = "RoadBody";
        road.transform.SetParent(root.transform, false);
        road.transform.localPosition = new Vector3(0f, -0.2f, 15f);
        road.transform.localScale = new Vector3(9f, 0.4f, 30f);
        road.GetComponent<Renderer>().sharedMaterial = roadMat;
        road.GetComponent<Renderer>().receiveShadows = false;
        segment.roadRenderer = road.GetComponent<Renderer>();

        GameObject line1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line1.name = "Divider_Left";
        line1.transform.SetParent(root.transform, false);
        line1.transform.localPosition = new Vector3(-1.5f, 0.01f, 15f);
        line1.transform.localScale = new Vector3(0.12f, 0.02f, 30f);
        line1.GetComponent<Renderer>().sharedMaterial = lineMat;
        line1.GetComponent<Renderer>().receiveShadows = false;
        DestroyImmediate(line1.GetComponent<Collider>());
        segment.leftDividerRenderer = line1.GetComponent<Renderer>();

        GameObject line2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        line2.name = "Divider_Right";
        line2.transform.SetParent(root.transform, false);
        line2.transform.localPosition = new Vector3(1.5f, 0.01f, 15f);
        line2.transform.localScale = new Vector3(0.12f, 0.02f, 30f);
        line2.GetComponent<Renderer>().sharedMaterial = lineMat;
        line2.GetComponent<Renderer>().receiveShadows = false;
        DestroyImmediate(line2.GetComponent<Collider>());
        segment.rightDividerRenderer = line2.GetComponent<Renderer>();

        // Raised Curbs along road edge
        GameObject curbLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curbLeft.name = "Curb_Left";
        curbLeft.transform.SetParent(root.transform, false);
        curbLeft.transform.localPosition = new Vector3(-4.55f, 0.05f, 15f);
        curbLeft.transform.localScale = new Vector3(0.35f, 0.18f, 30f);
        curbLeft.GetComponent<Renderer>().sharedMaterial = lineMat;
        curbLeft.GetComponent<Renderer>().receiveShadows = false;
        DestroyImmediate(curbLeft.GetComponent<Collider>());
        segment.leftCurbRenderer = curbLeft.GetComponent<Renderer>();

        GameObject curbRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curbRight.name = "Curb_Right";
        curbRight.transform.SetParent(root.transform, false);
        curbRight.transform.localPosition = new Vector3(4.55f, 0.05f, 15f);
        curbRight.transform.localScale = new Vector3(0.35f, 0.18f, 30f);
        curbRight.GetComponent<Renderer>().sharedMaterial = lineMat;
        curbRight.GetComponent<Renderer>().receiveShadows = false;
        DestroyImmediate(curbRight.GetComponent<Collider>());
        segment.rightCurbRenderer = curbRight.GetComponent<Renderer>();

        // Open wide flanks for expansive horizon (walls on both sides removed)
        GameObject leftFlank = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftFlank.name = "Flank_Left";
        leftFlank.transform.SetParent(root.transform, false);
        leftFlank.transform.localPosition = new Vector3(-25f, -0.25f, 15f);
        leftFlank.transform.localScale = new Vector3(40f, 0.4f, 30f);
        leftFlank.GetComponent<Renderer>().sharedMaterial = sideMat;
        leftFlank.GetComponent<Renderer>().receiveShadows = false;
        segment.leftFlankRenderer = leftFlank.GetComponent<Renderer>();

        GameObject rightFlank = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightFlank.name = "Flank_Right";
        rightFlank.transform.SetParent(root.transform, false);
        rightFlank.transform.localPosition = new Vector3(25f, -0.25f, 15f);
        rightFlank.transform.localScale = new Vector3(40f, 0.4f, 30f);
        rightFlank.GetComponent<Renderer>().sharedMaterial = sideMat;
        rightFlank.GetComponent<Renderer>().receiveShadows = false;
        segment.rightFlankRenderer = rightFlank.GetComponent<Renderer>();

        // Distant Scenic Mountain / Canyon Walls on both sides removed per user request
        segment.leftRidgeRenderer = null;
        segment.rightRidgeRenderer = null;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    // --- Side Props ---

    private static GameObject CreateTreeProp()
    {
        string path = "Assets/Prefabs/Props/Prop_GrassTree.prefab";
        GameObject root = new GameObject("Prop_GrassTree");
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root.transform, false);
        trunk.transform.localPosition = new Vector3(0f, 1f, 0f);
        trunk.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
        trunk.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Grassland_Hurdle", new Color(0.45f, 0.28f, 0.15f));
        DestroyImmediate(trunk.GetComponent<Collider>());

        GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage.transform.SetParent(root.transform, false);
        foliage.transform.localPosition = new Vector3(0f, 2.5f, 0f);
        foliage.transform.localScale = new Vector3(1.8f, 1.8f, 1.8f);
        foliage.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Grassland_Side", new Color(0.2f, 0.6f, 0.2f));
        DestroyImmediate(foliage.GetComponent<Collider>());

        // Second layer of leaves for fullness
        GameObject foliage2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage2.transform.SetParent(root.transform, false);
        foliage2.transform.localPosition = new Vector3(0.3f, 2.8f, 0.2f);
        foliage2.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        foliage2.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Grassland_GrassTuft", new Color(0.22f, 0.62f, 0.18f));
        DestroyImmediate(foliage2.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateRockProp()
    {
        string path = "Assets/Prefabs/Props/Prop_GrassRock.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Prop_GrassRock";
        go.transform.localScale = new Vector3(1.4f, 1.0f, 1.4f);
        go.transform.rotation = Quaternion.Euler(15f, 35f, 10f);
        go.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Grassland_Barrier", new Color(0.45f, 0.48f, 0.45f));
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateJungleTreeProp()
    {
        string path = "Assets/Prefabs/Props/Prop_JungleTree.prefab";
        GameObject root = new GameObject("Prop_JungleTree");
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root.transform, false);
        trunk.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        trunk.transform.localScale = new Vector3(0.5f, 1.6f, 0.5f);
        trunk.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Jungle_Hurdle", new Color(0.35f, 0.22f, 0.12f));
        DestroyImmediate(trunk.GetComponent<Collider>());

        GameObject foliage1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage1.transform.SetParent(root.transform, false);
        foliage1.transform.localPosition = new Vector3(0f, 3.2f, 0f);
        foliage1.transform.localScale = new Vector3(2.4f, 1.6f, 2.4f);
        foliage1.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Jungle_Side", new Color(0.08f, 0.38f, 0.14f));
        DestroyImmediate(foliage1.GetComponent<Collider>());

        // Dense secondary canopy
        GameObject foliage2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        foliage2.transform.SetParent(root.transform, false);
        foliage2.transform.localPosition = new Vector3(-0.5f, 3.6f, 0.3f);
        foliage2.transform.localScale = new Vector3(1.6f, 1.2f, 1.6f);
        foliage2.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Jungle_Vine", new Color(0.18f, 0.48f, 0.12f));
        DestroyImmediate(foliage2.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateJunglePillarProp()
    {
        string path = "Assets/Prefabs/Props/Prop_JunglePillar.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Prop_JunglePillar";
        go.transform.localScale = new Vector3(0.9f, 2.0f, 0.9f);
        go.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Jungle_Barrier", new Color(0.28f, 0.35f, 0.28f));
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateCactusProp()
    {
        string path = "Assets/Prefabs/Props/Prop_DesertCactus.prefab";
        GameObject root = new GameObject("Prop_DesertCactus");
        Material mat = CreateOrLoadMaterial("Worlds/Desert_CactusMat", new Color(0.32f, 0.58f, 0.28f));

        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.transform.SetParent(root.transform, false);
        stem.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        stem.transform.localScale = new Vector3(0.45f, 1.4f, 0.45f);
        stem.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(stem.GetComponent<Collider>());

        GameObject b1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        b1.transform.SetParent(root.transform, false);
        b1.transform.localPosition = new Vector3(0.6f, 1.6f, 0f);
        b1.transform.localScale = new Vector3(0.3f, 0.7f, 0.3f);
        b1.transform.rotation = Quaternion.Euler(0f, 0f, 25f);
        b1.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(b1.GetComponent<Collider>());

        // Second arm for fuller cactus
        GameObject b2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        b2.transform.SetParent(root.transform, false);
        b2.transform.localPosition = new Vector3(-0.5f, 1.9f, 0f);
        b2.transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
        b2.transform.rotation = Quaternion.Euler(0f, 0f, -30f);
        b2.GetComponent<Renderer>().sharedMaterial = mat;
        DestroyImmediate(b2.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreatePyramidProp()
    {
        string path = "Assets/Prefabs/Props/Prop_DesertPyramid.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Prop_DesertPyramid";
        go.transform.localScale = new Vector3(2.5f, 2.0f, 2.5f);
        go.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
        go.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Desert_Barrier", new Color(0.68f, 0.48f, 0.30f));
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateIceCrystalProp()
    {
        string path = "Assets/Prefabs/Props/Prop_IceCrystal.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = "Prop_IceCrystal";
        go.transform.localScale = new Vector3(0.6f, 2.2f, 0.6f);
        go.transform.rotation = Quaternion.Euler(15f, 20f, 15f);
        go.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Ice_Barrier", new Color(0.30f, 0.70f, 0.95f));
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateSnowMoundProp()
    {
        string path = "Assets/Prefabs/Props/Prop_SnowMound.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Prop_SnowMound";
        go.transform.localScale = new Vector3(2.2f, 0.6f, 2.2f);
        go.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Ice_Side", new Color(0.94f, 0.97f, 1.0f));
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateMechPillarProp()
    {
        string path = "Assets/Prefabs/Props/Prop_MechPillar.prefab";
        GameObject root = new GameObject("Prop_MechPillar");
        GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        col.transform.SetParent(root.transform, false);
        col.transform.localPosition = new Vector3(0f, 2f, 0f);
        col.transform.localScale = new Vector3(0.8f, 2f, 0.8f);
        col.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Mech_Side", new Color(0.28f, 0.30f, 0.35f));
        DestroyImmediate(col.GetComponent<Collider>());

        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.transform.SetParent(root.transform, false);
        ring.transform.localPosition = new Vector3(0f, 2.2f, 0f);
        ring.transform.localScale = new Vector3(0.95f, 0.2f, 0.95f);
        ring.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Mech_Divider", new Color(0.98f, 0.85f, 0.12f));
        DestroyImmediate(ring.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        return prefab;
    }

    private static GameObject CreateMechBoxProp()
    {
        string path = "Assets/Prefabs/Props/Prop_MechBox.prefab";
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Prop_MechBox";
        go.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        go.GetComponent<Renderer>().sharedMaterial = CreateOrLoadMaterial("Worlds/Mech_Barrier", new Color(0.38f, 0.42f, 0.46f));
        DestroyImmediate(go.GetComponent<Collider>());

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        DestroyImmediate(go);
        return prefab;
    }

    // --- Scene Setup ---

    private static void SetupScene(Material matPlayer, Material matShield, GameObject trackPrefab, GameObject[] obstaclePrefabs, GameObject coinPrefab, GameObject gemPrefab, GameObject[] powerUpPrefabs, List<WorldThemeData> themes)
    {
        // 1. Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            mainCam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        mainCam.transform.position = new Vector3(0f, 4.5f, -6.5f);
        mainCam.clearFlags = CameraClearFlags.SolidColor;

        // 2. Light
        Light dirLight = Object.FindObjectOfType<Light>();
        if (dirLight == null || dirLight.type != LightType.Directional)
        {
            GameObject lightObj = new GameObject("Directional Light");
            dirLight = lightObj.AddComponent<Light>();
            dirLight.type = LightType.Directional;
        }
        dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        dirLight.shadows = LightShadows.None;

        GameObject oldShadow = GameObject.Find("GroundContactShadow");
        if (oldShadow != null) DestroyImmediate(oldShadow);

        // 3. WorldManager
        GameObject wmObj = GameObject.Find("WorldManager");
        if (wmObj != null) DestroyImmediate(wmObj);

        wmObj = new GameObject("WorldManager");
        WorldManager wm = wmObj.AddComponent<WorldManager>();
        wm.directionalLight = dirLight;
        wm.runnerCamera = mainCam;
        wm.worlds = themes;
        wm.selectedWorld = WorldType.Grassland;

        // 4. Ambient Particle Controller
        GameObject existingParticles = GameObject.Find("AmbientParticles");
        if (existingParticles != null) DestroyImmediate(existingParticles);

        GameObject particleObj = new GameObject("AmbientParticles");
        AmbientParticleController particleCtrl = particleObj.AddComponent<AmbientParticleController>();
        wm.ambientParticles = particleCtrl;

        // 5. Player & Boy Character
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null) DestroyImmediate(existingPlayer);

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0f, 0f);

        CharacterController cc = player.AddComponent<CharacterController>();
        cc.center = new Vector3(0f, 0.95f, 0f);
        cc.radius = 0.45f;
        cc.height = 1.9f;

        PlayerController pc = player.AddComponent<PlayerController>();
        pc.forwardSpeed = 12f;
        pc.laneDistance = 3f;

        // Build Enhanced Animated Boy Model inside Player
        BuildBoyCharacter(player, pc, cc);

        // Set particle controller target
        particleCtrl.followTarget = player.transform;

        // Shield Bubble Child
        GameObject shieldObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shieldObj.name = "ShieldBubble";
        shieldObj.transform.SetParent(player.transform, false);
        shieldObj.transform.localPosition = new Vector3(0f, 0.95f, 0f);
        shieldObj.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        shieldObj.GetComponent<Renderer>().sharedMaterial = matShield;
        DestroyImmediate(shieldObj.GetComponent<Collider>());
        shieldObj.SetActive(false);
        pc.shieldVisual = shieldObj;

        // Camera Follow
        CameraFollow camFollow = mainCam.GetComponent<CameraFollow>();
        if (camFollow == null) camFollow = mainCam.gameObject.AddComponent<CameraFollow>();
        camFollow.target = player.transform;
        camFollow.offset = new Vector3(0f, 4.5f, -6.5f);

        // 6. Track Spawner
        GameObject spawnerObj = GameObject.Find("TrackSpawner");
        if (spawnerObj != null) DestroyImmediate(spawnerObj);

        spawnerObj = new GameObject("TrackSpawner");
        TrackSpawner spawner = spawnerObj.AddComponent<TrackSpawner>();
        spawner.player = player.transform;
        spawner.trackPrefab = trackPrefab;
        spawner.obstaclePrefabs = obstaclePrefabs;
        spawner.coinPrefab = coinPrefab;
        spawner.gemPrefab = gemPrefab;
        spawner.powerUpPrefabs = powerUpPrefabs;
        spawner.laneDistance = 3f;
        spawner.segmentLength = 30f;
        spawner.segmentsAhead = 6;
        spawner.initialSafeSegments = 2;

        // 7. GameManager
        GameObject gmObj = GameObject.Find("GameManager");
        if (gmObj != null) DestroyImmediate(gmObj);

        gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();
        gm.player = player.transform;

        // 7b. Advanced Realism & Game Juice Engines
        if (Object.FindObjectOfType<SoundManager>() == null)
        {
            GameObject soundObj = new GameObject("SoundManager");
            soundObj.AddComponent<SoundManager>();
        }
        if (Object.FindObjectOfType<ScreenShakeEffect>() == null)
        {
            GameObject shakeObj = new GameObject("ScreenShakeEffect");
            shakeObj.AddComponent<ScreenShakeEffect>();
        }
        if (Object.FindObjectOfType<ComboSystem>() == null)
        {
            GameObject comboObj = new GameObject("ComboSystem");
            comboObj.AddComponent<ComboSystem>();
        }
        if (Object.FindObjectOfType<CollectionVFX>() == null)
        {
            GameObject vfxObj = new GameObject("CollectionVFX");
            vfxObj.AddComponent<CollectionVFX>();
        }
        if (Object.FindObjectOfType<FloatingTextSpawner>() == null)
        {
            GameObject textObj = new GameObject("FloatingTextSpawner");
            textObj.AddComponent<FloatingTextSpawner>();
        }
        if (Object.FindObjectOfType<SpeedTier>() == null)
        {
            GameObject speedObj = new GameObject("SpeedTier");
            speedObj.AddComponent<SpeedTier>();
        }
        if (Object.FindObjectOfType<MilestoneSystem>() == null)
        {
            GameObject milestoneObj = new GameObject("MilestoneSystem");
            milestoneObj.AddComponent<MilestoneSystem>();
        }
        if (Object.FindObjectOfType<DailyMissionSystem>() == null)
        {
            GameObject missionObj = new GameObject("DailyMissionSystem");
            missionObj.AddComponent<DailyMissionSystem>();
        }
        if (Object.FindObjectOfType<PerkShopManager>() == null)
        {
            GameObject perkObj = new GameObject("PerkShopManager");
            perkObj.AddComponent<PerkShopManager>();
        }

        // 8. Complete UI
        SetupUI(gmObj);

        wm.ApplyWorldTheme(WorldType.Grassland);
    }

    private static void SetupUI(GameObject gmObj)
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas != null) DestroyImmediate(canvas.gameObject);

        GameObject canvasObj = new GameObject("Canvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();

        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        // ==========================================
        // 1. IN-GAME HUD PANEL (Matching Infographic)
        // ==========================================
        GameObject hudPanel = new GameObject("HUDPanel");
        hudPanel.transform.SetParent(canvasObj.transform, false);
        RectTransform hudRect = hudPanel.AddComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.sizeDelta = Vector2.zero;

        // Coins (Top-Left)
        GameObject coinObj = new GameObject("CoinText");
        coinObj.transform.SetParent(hudPanel.transform, false);
        Text coinText = coinObj.AddComponent<Text>();
        coinText.font = font;
        coinText.fontSize = 36;
        coinText.fontStyle = FontStyle.Bold;
        coinText.color = new Color(1.0f, 0.85f, 0.2f);
        coinText.text = "● 0";
        RectTransform coinRect = coinObj.GetComponent<RectTransform>();
        coinRect.anchorMin = new Vector2(0f, 1f);
        coinRect.anchorMax = new Vector2(0f, 1f);
        coinRect.pivot = new Vector2(0f, 1f);
        coinRect.anchoredPosition = new Vector2(40f, -35f);
        coinRect.sizeDelta = new Vector2(250f, 45f);

        Outline coinOutline = coinObj.AddComponent<Outline>();
        coinOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        coinOutline.effectDistance = new Vector2(2f, -2f);

        // Score (Top-Left)
        GameObject scoreObj = new GameObject("ScoreText");
        scoreObj.transform.SetParent(hudPanel.transform, false);
        Text scoreText = scoreObj.AddComponent<Text>();
        scoreText.font = font;
        scoreText.fontSize = 36;
        scoreText.fontStyle = FontStyle.Bold;
        scoreText.color = Color.white;
        scoreText.text = "★ 00000";
        RectTransform scoreRect = scoreObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0f, 1f);
        scoreRect.anchorMax = new Vector2(0f, 1f);
        scoreRect.pivot = new Vector2(0f, 1f);
        scoreRect.anchoredPosition = new Vector2(40f, -85f);
        scoreRect.sizeDelta = new Vector2(250f, 45f);

        Outline scoreOutline = scoreObj.AddComponent<Outline>();
        scoreOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        scoreOutline.effectDistance = new Vector2(2f, -2f);

        // Gems (Top-Left)
        GameObject gemObj = new GameObject("GemText");
        gemObj.transform.SetParent(hudPanel.transform, false);
        Text gemText = gemObj.AddComponent<Text>();
        gemText.font = font;
        gemText.fontSize = 36;
        gemText.fontStyle = FontStyle.Bold;
        gemText.color = new Color(0.2f, 0.9f, 1.0f);
        gemText.text = "◆ 5";
        RectTransform gemRect = gemObj.GetComponent<RectTransform>();
        gemRect.anchorMin = new Vector2(0f, 1f);
        gemRect.anchorMax = new Vector2(0f, 1f);
        gemRect.pivot = new Vector2(0f, 1f);
        gemRect.anchoredPosition = new Vector2(40f, -135f);
        gemRect.sizeDelta = new Vector2(250f, 45f);

        Outline gemOutline = gemObj.AddComponent<Outline>();
        gemOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        gemOutline.effectDistance = new Vector2(2f, -2f);

        // Hearts / Lives (Top-Right)
        GameObject heartsObj = new GameObject("HeartsText");
        heartsObj.transform.SetParent(hudPanel.transform, false);
        Text heartsText = heartsObj.AddComponent<Text>();
        heartsText.font = font;
        heartsText.fontSize = 44;
        heartsText.fontStyle = FontStyle.Bold;
        heartsText.color = new Color(1.0f, 0.22f, 0.22f);
        heartsText.alignment = TextAnchor.MiddleRight;
        heartsText.text = "♥ ♥ ♥";
        RectTransform heartsRect = heartsObj.GetComponent<RectTransform>();
        heartsRect.anchorMin = new Vector2(1f, 1f);
        heartsRect.anchorMax = new Vector2(1f, 1f);
        heartsRect.pivot = new Vector2(1f, 1f);
        heartsRect.anchoredPosition = new Vector2(-120f, -35f);
        heartsRect.sizeDelta = new Vector2(220f, 50f);

        Outline heartsOutline = heartsObj.AddComponent<Outline>();
        heartsOutline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        heartsOutline.effectDistance = new Vector2(2f, -2f);

        // Pause Button (Top-Right)
        GameObject btnPauseObj = new GameObject("BtnPause");
        btnPauseObj.transform.SetParent(hudPanel.transform, false);
        Image btnPauseImg = btnPauseObj.AddComponent<Image>();
        btnPauseImg.color = new Color(0.2f, 0.24f, 0.3f, 0.9f);
        Button btnPause = btnPauseObj.AddComponent<Button>();
        RectTransform btnPauseRect = btnPauseObj.GetComponent<RectTransform>();
        btnPauseRect.anchorMin = new Vector2(1f, 1f);
        btnPauseRect.anchorMax = new Vector2(1f, 1f);
        btnPauseRect.pivot = new Vector2(1f, 1f);
        btnPauseRect.anchoredPosition = new Vector2(-40f, -35f);
        btnPauseRect.sizeDelta = new Vector2(55f, 55f);

        GameObject pauseIconObj = new GameObject("Icon");
        pauseIconObj.transform.SetParent(btnPauseObj.transform, false);
        Text pauseIconText = pauseIconObj.AddComponent<Text>();
        pauseIconText.font = font;
        pauseIconText.fontSize = 30;
        pauseIconText.fontStyle = FontStyle.Bold;
        pauseIconText.color = Color.white;
        pauseIconText.alignment = TextAnchor.MiddleCenter;
        pauseIconText.text = "❚❚";
        RectTransform pauseIconRect = pauseIconObj.GetComponent<RectTransform>();
        pauseIconRect.sizeDelta = new Vector2(55f, 55f);

        // World Badge (Top-Center)
        GameObject badgeObj = new GameObject("WorldBadgeText");
        badgeObj.transform.SetParent(hudPanel.transform, false);
        Text badgeText = badgeObj.AddComponent<Text>();
        badgeText.font = font;
        badgeText.fontSize = 24;
        badgeText.fontStyle = FontStyle.Bold;
        badgeText.color = new Color(0.85f, 0.9f, 1f, 0.85f);
        badgeText.alignment = TextAnchor.MiddleCenter;
        badgeText.text = "GRASSLAND";
        RectTransform badgeRect = badgeObj.GetComponent<RectTransform>();
        badgeRect.anchorMin = new Vector2(0.5f, 1f);
        badgeRect.anchorMax = new Vector2(0.5f, 1f);
        badgeRect.pivot = new Vector2(0.5f, 1f);
        badgeRect.anchoredPosition = new Vector2(0f, -25f);
        badgeRect.sizeDelta = new Vector2(300f, 35f);

        // Active Power-Up Countdown (Center-Top)
        GameObject pupBadgeObj = new GameObject("PowerUpBadgeText");
        pupBadgeObj.transform.SetParent(hudPanel.transform, false);
        Text pupBadgeText = pupBadgeObj.AddComponent<Text>();
        pupBadgeText.font = font;
        pupBadgeText.fontSize = 28;
        pupBadgeText.fontStyle = FontStyle.Bold;
        pupBadgeText.color = new Color(1.0f, 0.88f, 0.2f);
        pupBadgeText.alignment = TextAnchor.MiddleCenter;
        pupBadgeText.text = "🧲 MAGNET : 10s";
        RectTransform pupBadgeRect = pupBadgeObj.GetComponent<RectTransform>();
        pupBadgeRect.anchorMin = new Vector2(0.5f, 1f);
        pupBadgeRect.anchorMax = new Vector2(0.5f, 1f);
        pupBadgeRect.pivot = new Vector2(0.5f, 1f);
        pupBadgeRect.anchoredPosition = new Vector2(0f, -70f);
        pupBadgeRect.sizeDelta = new Vector2(400f, 40f);
        pupBadgeObj.AddComponent<Outline>().effectColor = Color.black;
        pupBadgeObj.SetActive(false);

        // Status Message (Mid screen)
        GameObject statusObj = new GameObject("StatusMessageText");
        statusObj.transform.SetParent(hudPanel.transform, false);
        Text statusText = statusObj.AddComponent<Text>();
        statusText.font = font;
        statusText.fontSize = 38;
        statusText.fontStyle = FontStyle.Bold;
        statusText.color = new Color(1.0f, 0.4f, 0.4f);
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.text = "SHIELD BROKEN!";
        RectTransform statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.anchoredPosition = new Vector2(0f, 100f);
        statusRect.sizeDelta = new Vector2(500f, 60f);
        statusObj.AddComponent<Outline>().effectColor = Color.black;
        statusObj.SetActive(false);

        // ==========================================
        // 2. PAUSE MENU PANEL
        // ==========================================
        GameObject pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvasObj.transform, false);
        Image pauseBg = pausePanel.AddComponent<Image>();
        pauseBg.color = new Color(0.05f, 0.08f, 0.12f, 0.94f);
        RectTransform pauseRect = pausePanel.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(0.5f, 0.5f);
        pauseRect.anchorMax = new Vector2(0.5f, 0.5f);
        pauseRect.pivot = new Vector2(0.5f, 0.5f);
        pauseRect.sizeDelta = new Vector2(480f, 420f);

        GameObject pauseTitleObj = new GameObject("PauseTitle");
        pauseTitleObj.transform.SetParent(pausePanel.transform, false);
        Text pauseTitle = pauseTitleObj.AddComponent<Text>();
        pauseTitle.font = font;
        pauseTitle.fontSize = 46;
        pauseTitle.fontStyle = FontStyle.Bold;
        pauseTitle.color = Color.white;
        pauseTitle.alignment = TextAnchor.MiddleCenter;
        pauseTitle.text = "GAME PAUSED";
        RectTransform pauseTitleRect = pauseTitleObj.GetComponent<RectTransform>();
        pauseTitleRect.anchoredPosition = new Vector2(0f, 130f);
        pauseTitleRect.sizeDelta = new Vector2(400f, 60f);

        Button btnResume = CreateMenuButton(pausePanel.transform, font, "▶ RESUME", new Color(0.12f, 0.72f, 0.55f), 0f, 40f);
        Button btnPauseRestart = CreateMenuButton(pausePanel.transform, font, "🔄 RETRY", new Color(0.95f, 0.55f, 0.15f), 0f, -35f);
        Button btnPauseHome = CreateMenuButton(pausePanel.transform, font, "🏠 HOME", new Color(0.25f, 0.45f, 0.85f), 0f, -110f);

        pausePanel.SetActive(false);

        // ==========================================
        // 3. GAME OVER PANEL
        // ==========================================
        GameObject goPanel = new GameObject("GameOverPanel");
        goPanel.transform.SetParent(canvasObj.transform, false);
        Image panelBg = goPanel.AddComponent<Image>();
        panelBg.color = new Color(0.07f, 0.10f, 0.16f, 0.96f);
        RectTransform panelRect = goPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(560f, 540f);

        GameObject titleObj = new GameObject("GameOverTitle");
        titleObj.transform.SetParent(goPanel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = font;
        titleText.fontSize = 54;
        titleText.fontStyle = FontStyle.Bold;
        titleText.color = new Color(1.0f, 0.85f, 0.15f);
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "GAME OVER";
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0f, 210f);
        titleRect.sizeDelta = new Vector2(500f, 65f);

        GameObject finalScoreObj = new GameObject("FinalScoreText");
        finalScoreObj.transform.SetParent(goPanel.transform, false);
        Text finalScoreText = finalScoreObj.AddComponent<Text>();
        finalScoreText.font = font;
        finalScoreText.fontSize = 52;
        finalScoreText.fontStyle = FontStyle.Bold;
        finalScoreText.color = Color.white;
        finalScoreText.alignment = TextAnchor.MiddleCenter;
        finalScoreText.text = "02345";
        RectTransform finalScoreRect = finalScoreObj.GetComponent<RectTransform>();
        finalScoreRect.anchoredPosition = new Vector2(0f, 140f);
        finalScoreRect.sizeDelta = new Vector2(480f, 60f);

        GameObject bestScoreObj = new GameObject("BestScoreText");
        bestScoreObj.transform.SetParent(goPanel.transform, false);
        Text bestScoreText = bestScoreObj.AddComponent<Text>();
        bestScoreText.font = font;
        bestScoreText.fontSize = 26;
        bestScoreText.color = new Color(0.95f, 0.85f, 0.2f);
        bestScoreText.alignment = TextAnchor.MiddleCenter;
        bestScoreText.text = "BEST SCORE: 05678";
        RectTransform bestRect = bestScoreObj.GetComponent<RectTransform>();
        bestRect.anchoredPosition = new Vector2(0f, 85f);
        bestRect.sizeDelta = new Vector2(480f, 40f);

        GameObject goCoinsObj = new GameObject("CoinsCollectedText");
        goCoinsObj.transform.SetParent(goPanel.transform, false);
        Text goCoinsText = goCoinsObj.AddComponent<Text>();
        goCoinsText.font = font;
        goCoinsText.fontSize = 22;
        goCoinsText.color = new Color(1f, 0.85f, 0.2f);
        goCoinsText.alignment = TextAnchor.MiddleCenter;
        goCoinsText.text = "● COINS: 0";
        RectTransform goCoinsRect = goCoinsObj.GetComponent<RectTransform>();
        goCoinsRect.anchoredPosition = new Vector2(-120f, 40f);
        goCoinsRect.sizeDelta = new Vector2(240f, 35f);

        GameObject goGemsObj = new GameObject("GemsCollectedText");
        goGemsObj.transform.SetParent(goPanel.transform, false);
        Text goGemsText = goGemsObj.AddComponent<Text>();
        goGemsText.font = font;
        goGemsText.fontSize = 22;
        goGemsText.color = new Color(0.2f, 0.9f, 1f);
        goGemsText.alignment = TextAnchor.MiddleCenter;
        goGemsText.text = "◆ GEMS: 0";
        RectTransform goGemsRect = goGemsObj.GetComponent<RectTransform>();
        goGemsRect.anchoredPosition = new Vector2(120f, 40f);
        goGemsRect.sizeDelta = new Vector2(240f, 35f);

        Button btnRevive = CreateMenuButton(goPanel.transform, font, "💚 REVIVE (◆ 5)", new Color(0.18f, 0.78f, 0.28f), 0f, -25f);
        Button btnHome = CreateMenuButton(goPanel.transform, font, "🏠 HOME", new Color(0.25f, 0.50f, 0.92f), 0f, -100f);
        Button btnRetry = CreateMenuButton(goPanel.transform, font, "🔄 RETRY", new Color(0.98f, 0.50f, 0.15f), 0f, -175f);

        goPanel.SetActive(false);

        // ==========================================
        // 4. WORLD SELECTION MENU PANEL
        // ==========================================
        GameObject wsPanel = new GameObject("WorldSelectPanel");
        wsPanel.transform.SetParent(canvasObj.transform, false);
        Image wsBg = wsPanel.AddComponent<Image>();
        wsBg.color = new Color(0.06f, 0.08f, 0.12f, 0.88f);
        RectTransform wsRect = wsPanel.GetComponent<RectTransform>();
        wsRect.anchorMin = Vector2.zero;
        wsRect.anchorMax = Vector2.one;
        wsRect.sizeDelta = Vector2.zero;

        GameObject wsTitleObj = new GameObject("WSTitle");
        wsTitleObj.transform.SetParent(wsPanel.transform, false);
        Text wsTitle = wsTitleObj.AddComponent<Text>();
        wsTitle.font = font;
        wsTitle.fontSize = 52;
        wsTitle.fontStyle = FontStyle.Bold;
        wsTitle.color = Color.white;
        wsTitle.alignment = TextAnchor.MiddleCenter;
        wsTitle.text = "SELECT YOUR WORLD";
        RectTransform wsTitleRect = wsTitleObj.GetComponent<RectTransform>();
        wsTitleRect.anchoredPosition = new Vector2(0f, 320f);
        wsTitleRect.sizeDelta = new Vector2(800f, 80f);

        GameObject wsSubObj = new GameObject("WSSubtitle");
        wsSubObj.transform.SetParent(wsPanel.transform, false);
        Text wsSub = wsSubObj.AddComponent<Text>();
        wsSub.font = font;
        wsSub.fontSize = 24;
        wsSub.color = new Color(0.75f, 0.8f, 0.9f);
        wsSub.alignment = TextAnchor.MiddleCenter;
        wsSub.text = "Choose a realm below to begin your run!\nControls: A/D or Arrows = Shift Lanes  •  Space/W = Jump  •  S/Down = Fast Fall  •  Esc/P = Pause";
        RectTransform wsSubRect = wsSubObj.GetComponent<RectTransform>();
        wsSubRect.anchoredPosition = new Vector2(0f, 240f);
        wsSubRect.sizeDelta = new Vector2(1000f, 60f);

        Button btnGrass = CreateWorldSelectButton(wsPanel.transform, font, "🌿 GRASSLAND", new Color(0.2f, 0.65f, 0.25f), -360f, 40f);
        Button btnJungle = CreateWorldSelectButton(wsPanel.transform, font, "🌴 JUNGLE", new Color(0.12f, 0.45f, 0.22f), 0f, 40f);
        Button btnDesert = CreateWorldSelectButton(wsPanel.transform, font, "🏜️ DESERT", new Color(0.85f, 0.58f, 0.2f), 360f, 40f);
        Button btnIce = CreateWorldSelectButton(wsPanel.transform, font, "❄️ ICE WORLD", new Color(0.25f, 0.65f, 0.92f), -180f, -120f);
        Button btnMech = CreateWorldSelectButton(wsPanel.transform, font, "⚙️ MECHANICAL", new Color(0.35f, 0.40f, 0.48f), 180f, -120f);

        // ==========================================
        // 5. UIManager Hookup
        // ==========================================
        UIManager uiManager = canvasObj.AddComponent<UIManager>();
        uiManager.hudPanel = hudPanel;
        uiManager.pausePanel = pausePanel;
        uiManager.worldSelectPanel = wsPanel;
        uiManager.gameOverPanel = goPanel;

        uiManager.scoreText = scoreText;
        uiManager.coinText = coinText;
        uiManager.gemText = gemText;
        uiManager.heartsText = heartsText;
        uiManager.pauseButton = btnPause;
        uiManager.currentWorldBadgeText = badgeText;
        uiManager.powerUpBadgeText = pupBadgeText;
        uiManager.statusMessageText = statusText;

        uiManager.resumeButton = btnResume;
        uiManager.pauseRestartButton = btnPauseRestart;
        uiManager.pauseHomeButton = btnPauseHome;

        uiManager.finalScoreText = finalScoreText;
        uiManager.highScoreText = bestScoreText;
        uiManager.gameOverCoinsText = goCoinsText;
        uiManager.gameOverGemsText = goGemsText;
        uiManager.reviveButton = btnRevive;
        uiManager.retryButton = btnRetry;
        uiManager.homeButton = btnHome;

        uiManager.grasslandButton = btnGrass;
        uiManager.jungleButton = btnJungle;
        uiManager.desertButton = btnDesert;
        uiManager.iceWorldButton = btnIce;
        uiManager.mechanicalButton = btnMech;

        uiManager.ShowWorldSelect();
    }

    private static Button CreateMenuButton(Transform parent, Font font, string title, Color btnColor, float posX, float posY)
    {
        GameObject btnObj = new GameObject("Btn_" + title.Replace(" ", ""));
        btnObj.transform.SetParent(parent, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = btnColor;
        Button btn = btnObj.AddComponent<Button>();
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(posX, posY);
        rect.sizeDelta = new Vector2(320f, 60f);

        GameObject txtObj = new GameObject("Label");
        txtObj.transform.SetParent(btnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = font;
        txt.fontSize = 24;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = title;
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.sizeDelta = new Vector2(320f, 60f);

        Outline outline = txtObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.6f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return btn;
    }

    private static Button CreateWorldSelectButton(Transform parent, Font font, string title, Color btnColor, float posX, float posY)
    {
        GameObject btnObj = new GameObject("Btn_" + title.Replace(" ", ""));
        btnObj.transform.SetParent(parent, false);
        Image img = btnObj.AddComponent<Image>();
        img.color = btnColor;
        Button btn = btnObj.AddComponent<Button>();
        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(posX, posY);
        rect.sizeDelta = new Vector2(310f, 100f);

        GameObject txtObj = new GameObject("Label");
        txtObj.transform.SetParent(btnObj.transform, false);
        Text txt = txtObj.AddComponent<Text>();
        txt.font = font;
        txt.fontSize = 28;
        txt.fontStyle = FontStyle.Bold;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.text = title;
        RectTransform txtRect = txtObj.GetComponent<RectTransform>();
        txtRect.sizeDelta = new Vector2(310f, 100f);

        Outline outline = txtObj.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.7f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        return btn;
    }
}
