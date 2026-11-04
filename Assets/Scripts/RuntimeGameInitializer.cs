using UnityEngine;

/// <summary>
/// Automatically bootstraps and configures all advanced systems (Audio, VFX, Combos, Shake, Missions)
/// immediately when the game runs, ensuring zero setup is required by the user!
/// </summary>
public static class RuntimeGameInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeSystems()
    {
        // 1. Procedural Audio Engine
        if (SoundManager.Instance == null)
        {
            GameObject soundObj = new GameObject("[SoundManager]");
            soundObj.AddComponent<SoundManager>();
            Object.DontDestroyOnLoad(soundObj);
        }

        // 2. Screen Shake Engine
        if (ScreenShakeEffect.Instance == null)
        {
            GameObject shakeObj = new GameObject("[ScreenShakeEffect]");
            shakeObj.AddComponent<ScreenShakeEffect>();
            Object.DontDestroyOnLoad(shakeObj);
        }

        // 3. Combo & Near-Miss Engine
        if (ComboSystem.Instance == null)
        {
            GameObject comboObj = new GameObject("[ComboSystem]");
            comboObj.AddComponent<ComboSystem>();
            Object.DontDestroyOnLoad(comboObj);
        }

        // 4. Procedural Particle VFX Engine
        if (CollectionVFX.Instance == null)
        {
            GameObject vfxObj = new GameObject("[CollectionVFX]");
            vfxObj.AddComponent<CollectionVFX>();
            Object.DontDestroyOnLoad(vfxObj);
        }

        // 5. World Floating Text Spawner
        if (FloatingTextSpawner.Instance == null)
        {
            GameObject textObj = new GameObject("[FloatingTextSpawner]");
            textObj.AddComponent<FloatingTextSpawner>();
            Object.DontDestroyOnLoad(textObj);
        }

        // 6. Speed Tier & Dynamic FOV Warper
        if (SpeedTier.Instance == null)
        {
            GameObject speedObj = new GameObject("[SpeedTier]");
            speedObj.AddComponent<SpeedTier>();
            Object.DontDestroyOnLoad(speedObj);
        }

        // 7. Distance & Score Milestone Engine
        if (MilestoneSystem.Instance == null)
        {
            GameObject milestoneObj = new GameObject("[MilestoneSystem]");
            milestoneObj.AddComponent<MilestoneSystem>();
            Object.DontDestroyOnLoad(milestoneObj);
        }

        // 8. Daily Quests & Challenges
        if (DailyMissionSystem.Instance == null)
        {
            GameObject missionObj = new GameObject("[DailyMissionSystem]");
            missionObj.AddComponent<DailyMissionSystem>();
            Object.DontDestroyOnLoad(missionObj);
        }

        // 9. Perk & Upgrade Shop Manager
        if (PerkShopManager.Instance == null)
        {
            GameObject perkObj = new GameObject("[PerkShopManager]");
            perkObj.AddComponent<PerkShopManager>();
            Object.DontDestroyOnLoad(perkObj);
        }

        // 10. Hook up Player enhancements
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (player.GetComponent<PlayerTrailEffect>() == null)
            {
                player.AddComponent<PlayerTrailEffect>();
            }
            if (player.GetComponent<PlayerGroundShadow>() == null)
            {
                player.AddComponent<PlayerGroundShadow>();
            }
        }
    }
}

