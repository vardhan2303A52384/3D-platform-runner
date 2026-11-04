using UnityEngine;

/// <summary>
/// Persistent Perk & Upgrade Shop. Players can spend collected coins and gems
/// to permanently upgrade their runner capabilities and power-ups!
/// </summary>
public class PerkShopManager : MonoBehaviour
{
    public static PerkShopManager Instance { get; private set; }

    [Header("Upgrade Levels (1-5)")]
    public int magnetLevel = 1;
    public int shieldLevel = 1;
    public int speedBoostLevel = 1;
    public int multiplierLevel = 1;

    private const string MagnetLevelKey = "Perk_MagnetLevel";
    private const string ShieldLevelKey = "Perk_ShieldLevel";
    private const string SpeedLevelKey = "Perk_SpeedLevel";
    private const string MultLevelKey = "Perk_MultLevel";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        LoadPerks();
    }

    private void LoadPerks()
    {
        magnetLevel = PlayerPrefs.GetInt(MagnetLevelKey, 1);
        shieldLevel = PlayerPrefs.GetInt(ShieldLevelKey, 1);
        speedBoostLevel = PlayerPrefs.GetInt(SpeedLevelKey, 1);
        multiplierLevel = PlayerPrefs.GetInt(MultLevelKey, 1);
    }

    public float GetMagnetDuration()
    {
        return 8f + (magnetLevel * 2.5f); // 10.5s to 20.5s
    }

    public float GetMagnetRadius()
    {
        return 12f + (magnetLevel * 2.5f); // 14.5m to 24.5m
    }

    public float GetSpeedBoostDuration()
    {
        return 7f + (speedBoostLevel * 2f);
    }

    public float GetMultiplierDuration()
    {
        return 8f + (multiplierLevel * 2f);
    }

    public int GetExtraMaxHealth()
    {
        return shieldLevel > 3 ? 1 : 0; // Extra heart at higher levels!
    }

    public bool TryUpgradeMagnet()
    {
        int cost = magnetLevel * 40;
        if (GameManager.Instance != null && GameManager.Instance.coins >= cost && magnetLevel < 5)
        {
            GameManager.Instance.coins -= cost;
            magnetLevel++;
            PlayerPrefs.SetInt(MagnetLevelKey, magnetLevel);
            PlayerPrefs.Save();
            if (SoundManager.Instance != null) SoundManager.Instance.PlayMilestone();
            return true;
        }
        return false;
    }

    public bool TryUpgradeShield()
    {
        int cost = shieldLevel * 50;
        if (GameManager.Instance != null && GameManager.Instance.coins >= cost && shieldLevel < 5)
        {
            GameManager.Instance.coins -= cost;
            shieldLevel++;
            PlayerPrefs.SetInt(ShieldLevelKey, shieldLevel);
            PlayerPrefs.Save();
            if (SoundManager.Instance != null) SoundManager.Instance.PlayMilestone();
            return true;
        }
        return false;
    }
}

