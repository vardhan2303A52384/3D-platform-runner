using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Daily challenge & quest system. Keeps players coming back for dynamic objectives and gem rewards!
/// </summary>
public class DailyMissionSystem : MonoBehaviour
{
    public static DailyMissionSystem Instance { get; private set; }

    [Serializable]
    public class Mission
    {
        public string id;
        public string description;
        public MissionType type;
        public int targetAmount;
        public int currentProgress;
        public bool isCompleted;
        public int gemReward;
    }

    public enum MissionType
    {
        CollectCoins,
        TravelDistance,
        CollectGems,
        UsePowerUps,
        NearMisses,
        ScorePoints
    }

    public List<Mission> todaysMissions = new List<Mission>();

    private const string LastMissionDateKey = "DailyMission_LastDate";
    private int powerUpsUsedThisRun = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CheckDailyReset();
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isPlaying) return;

        UpdateMissionProgress(MissionType.TravelDistance, GameManager.Instance.distanceScore);
        UpdateMissionProgress(MissionType.ScorePoints, GameManager.Instance.totalScore);
        UpdateMissionProgress(MissionType.CollectCoins, GameManager.Instance.coins);
    }

    public void CheckDailyReset()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string lastDate = PlayerPrefs.GetString(LastMissionDateKey, "");

        if (lastDate != today)
        {
            GenerateNewDailyMissions();
            PlayerPrefs.SetString(LastMissionDateKey, today);
            SaveMissions();
        }
        else
        {
            LoadMissions();
        }
    }

    private void GenerateNewDailyMissions()
    {
        todaysMissions.Clear();

        todaysMissions.Add(new Mission
        {
            id = "m_coins",
            description = "Collect 40 coins in runs",
            type = MissionType.CollectCoins,
            targetAmount = 40,
            currentProgress = 0,
            isCompleted = false,
            gemReward = 2
        });

        todaysMissions.Add(new Mission
        {
            id = "m_dist",
            description = "Travel 400 meters total",
            type = MissionType.TravelDistance,
            targetAmount = 400,
            currentProgress = 0,
            isCompleted = false,
            gemReward = 3
        });

        todaysMissions.Add(new Mission
        {
            id = "m_nearmiss",
            description = "Execute 3 Near Miss dodges",
            type = MissionType.NearMisses,
            targetAmount = 3,
            currentProgress = 0,
            isCompleted = false,
            gemReward = 4
        });
    }

    public void UpdateMissionProgress(MissionType type, int amount)
    {
        foreach (var m in todaysMissions)
        {
            if (m.type == type && !m.isCompleted)
            {
                m.currentProgress = Mathf.Max(m.currentProgress, amount);
                if (m.currentProgress >= m.targetAmount)
                {
                    m.isCompleted = true;
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.AddGem(m.gemReward);
                    }
                    if (SoundManager.Instance != null)
                    {
                        SoundManager.Instance.PlayMilestone();
                    }
                    if (UIManager.Instance != null)
                    {
                        UIManager.Instance.ShowStatusMessage($"🎯 MISSION COMPLETE! +{m.gemReward} GEMS 🎯");
                    }
                }
                SaveMissions();
            }
        }
    }

    public void OnPowerUpCollected()
    {
        powerUpsUsedThisRun++;
        UpdateMissionProgress(MissionType.UsePowerUps, powerUpsUsedThisRun);
    }

    public void OnNearMissRegistered(int totalNearMisses)
    {
        UpdateMissionProgress(MissionType.NearMisses, totalNearMisses);
    }

    public void OnGemCollected()
    {
        if (GameManager.Instance != null)
        {
            UpdateMissionProgress(MissionType.CollectGems, GameManager.Instance.gems);
        }
    }

    public void ResetRunState()
    {
        powerUpsUsedThisRun = 0;
    }

    private void SaveMissions()
    {
        for (int i = 0; i < todaysMissions.Count; i++)
        {
            string key = $"Mission_{i}_";
            PlayerPrefs.SetInt(key + "Type", (int)todaysMissions[i].type);
            PlayerPrefs.SetInt(key + "Target", todaysMissions[i].targetAmount);
            PlayerPrefs.SetInt(key + "Progress", todaysMissions[i].currentProgress);
            PlayerPrefs.SetInt(key + "Done", todaysMissions[i].isCompleted ? 1 : 0);
            PlayerPrefs.SetInt(key + "Reward", todaysMissions[i].gemReward);
            PlayerPrefs.SetString(key + "Desc", todaysMissions[i].description);
        }
        PlayerPrefs.SetInt("Mission_TotalCount", todaysMissions.Count);
        PlayerPrefs.Save();
    }

    private void LoadMissions()
    {
        todaysMissions.Clear();
        int count = PlayerPrefs.GetInt("Mission_TotalCount", 0);
        if (count == 0)
        {
            GenerateNewDailyMissions();
            return;
        }

        for (int i = 0; i < count; i++)
        {
            string key = $"Mission_{i}_";
            todaysMissions.Add(new Mission
            {
                id = $"m_{i}",
                type = (MissionType)PlayerPrefs.GetInt(key + "Type", 0),
                targetAmount = PlayerPrefs.GetInt(key + "Target", 50),
                currentProgress = PlayerPrefs.GetInt(key + "Progress", 0),
                isCompleted = PlayerPrefs.GetInt(key + "Done", 0) == 1,
                gemReward = PlayerPrefs.GetInt(key + "Reward", 2),
                description = PlayerPrefs.GetString(key + "Desc", "Daily Quest")
            });
        }
    }
}

