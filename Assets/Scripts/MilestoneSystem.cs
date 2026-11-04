using System;
using UnityEngine;

/// <summary>
/// Monitors distance, coins, and high score progress to trigger rewarding milestones.
/// </summary>
public class MilestoneSystem : MonoBehaviour
{
    public static MilestoneSystem Instance { get; private set; }

    [Header("Intervals")]
    public int distanceInterval = 150;
    public int bigDistanceInterval = 500;
    public int coinInterval = 30;

    private int lastDistanceMilestone = 0;
    private int lastCoinMilestone = 0;
    private bool beatHighScore = false;

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

        int dist = GameManager.Instance.distanceScore;
        int coins = GameManager.Instance.coins;
        int score = GameManager.Instance.totalScore;

        // 1. Distance Milestones
        int currentDistMilestone = (dist / distanceInterval) * distanceInterval;
        if (currentDistMilestone > lastDistanceMilestone && currentDistMilestone > 0)
        {
            lastDistanceMilestone = currentDistMilestone;

            bool isBig = (currentDistMilestone % bigDistanceInterval == 0);
            string message = isBig ? $"🏆 {currentDistMilestone}m MASTER RUNNER! 🏆" : $"★ {currentDistMilestone}m REACHED! ★";

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMilestone();
            }

            if (ScreenShakeEffect.Instance != null)
            {
                ScreenShakeEffect.Instance.Shake(0.2f, isBig ? 0.18f : 0.08f);
            }

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowStatusMessage(message);
            }

            if (isBig && GameManager.Instance != null)
            {
                GameManager.Instance.AddGem(1); // Gift 1 gem on big milestone!
                if (FloatingTextSpawner.Instance != null && PlayerController.ActivePlayer != null)
                {
                    FloatingTextSpawner.Instance.SpawnGemText(PlayerController.ActivePlayer.transform.position, 1);
                }
            }
        }

        // 2. Coin Milestones
        int currentCoinMilestone = (coins / coinInterval) * coinInterval;
        if (currentCoinMilestone > lastCoinMilestone && currentCoinMilestone > 0)
        {
            lastCoinMilestone = currentCoinMilestone;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowStatusMessage($"💰 {currentCoinMilestone} COINS COLLECTED! 💰");
            }
        }

        // 3. High Score Overcome
        if (!beatHighScore && GameManager.Instance.highScore > 0 && score > GameManager.Instance.highScore)
        {
            beatHighScore = true;
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayMilestone();
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowStatusMessage("👑 NEW ALL-TIME HIGH SCORE! 👑");
            }
            if (ScreenShakeEffect.Instance != null)
            {
                ScreenShakeEffect.Instance.Shake(0.3f, 0.15f);
            }
        }
    }

    public void ResetForNewRun()
    {
        lastDistanceMilestone = 0;
        lastCoinMilestone = 0;
        beatHighScore = false;
    }
}

