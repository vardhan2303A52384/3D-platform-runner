using System;
using UnityEngine;

/// <summary>
/// Tracks consecutive coin streaks and near-miss obstacle dodges.
/// Multiplies coin rewards and total score, creating high-skill excitement!
/// </summary>
public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }

    [Header("Combo Configuration")]
    public float comboWindowTime = 2.4f;
    public int maxComboMultiplier = 5;
    public float nearMissDistance = 1.6f;
    public int nearMissComboBonus = 3;
    public float nearMissCooldown = 0.6f;

    [Header("Live State")]
    public int currentStreak = 0;
    public int comboMultiplier = 1;
    public float comboTimer = 0f;
    public int totalNearMisses = 0;

    public event Action<int> OnComboMultiplierChanged;
    public event Action<int> OnNearMissRegistered;
    public event Action OnComboExpired;

    private float lastNearMissCheckTime = -10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.isPlaying) return;

        // Drain combo timer
        if (comboTimer > 0f)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ExpireCombo();
            }
        }

        CheckNearMisses();
    }

    public void RegisterCoinPickup()
    {
        currentStreak++;
        comboTimer = comboWindowTime;

        int oldMult = comboMultiplier;
        if (currentStreak >= 25) comboMultiplier = 5;
        else if (currentStreak >= 18) comboMultiplier = 4;
        else if (currentStreak >= 10) comboMultiplier = 3;
        else if (currentStreak >= 4) comboMultiplier = 2;
        else comboMultiplier = 1;

        comboMultiplier = Mathf.Min(comboMultiplier, maxComboMultiplier);

        if (comboMultiplier != oldMult)
        {
            OnComboMultiplierChanged?.Invoke(comboMultiplier);
            if (FloatingTextSpawner.Instance != null && PlayerController.ActivePlayer != null)
            {
                FloatingTextSpawner.Instance.SpawnComboText(PlayerController.ActivePlayer.transform.position + Vector3.up * 1.5f, comboMultiplier);
            }
        }
    }

    private void CheckNearMisses()
    {
        if (PlayerController.ActivePlayer == null || PlayerController.ActivePlayer.IsDead) return;
        if (!PlayerController.ActivePlayer.isRunning) return;
        if (Time.time - lastNearMissCheckTime < nearMissCooldown) return;

        Vector3 playerPos = PlayerController.ActivePlayer.transform.position;
        Collider[] nearby = Physics.OverlapSphere(playerPos, nearMissDistance);

        foreach (var col in nearby)
        {
            if (col.CompareTag("Obstacle") || col.GetComponent<Obstacle>() != null)
            {
                float zDiff = col.transform.position.z - playerPos.z;
                // Passed right beside obstacle
                if (zDiff < 0.4f && zDiff > -1.8f)
                {
                    float xDist = Mathf.Abs(col.transform.position.x - playerPos.x);
                    if (xDist > 0.65f && xDist <= nearMissDistance)
                    {
                        TriggerNearMiss(col.transform.position);
                        break;
                    }
                }
            }
        }
    }

    private void TriggerNearMiss(Vector3 obstaclePos)
    {
        lastNearMissCheckTime = Time.time;
        totalNearMisses++;
        currentStreak += nearMissComboBonus;
        comboTimer = comboWindowTime;

        if (currentStreak >= 25) comboMultiplier = 5;
        else if (currentStreak >= 18) comboMultiplier = 4;
        else if (currentStreak >= 10) comboMultiplier = 3;
        else if (currentStreak >= 4) comboMultiplier = 2;

        OnNearMissRegistered?.Invoke(totalNearMisses);
        OnComboMultiplierChanged?.Invoke(comboMultiplier);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayNearMiss();
        }

        if (ScreenShakeEffect.Instance != null)
        {
            ScreenShakeEffect.Instance.ShakeNearMiss();
        }

        if (FloatingTextSpawner.Instance != null)
        {
            FloatingTextSpawner.Instance.SpawnNearMissText(PlayerController.ActivePlayer.transform.position);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowStatusMessage($"★ NEAR MISS! +{nearMissComboBonus} COMBO ★");
        }
    }

    private void ExpireCombo()
    {
        if (currentStreak > 0)
        {
            OnComboExpired?.Invoke();
        }
        currentStreak = 0;
        comboMultiplier = 1;
        comboTimer = 0f;
        OnComboMultiplierChanged?.Invoke(1);
    }

    public void ResetForNewRun()
    {
        ExpireCombo();
        totalNearMisses = 0;
    }
}

