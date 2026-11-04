using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject worldSelectPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("HUD Elements (Top-Left)")]
    public Text scoreText;
    public Text coinText;
    public Text gemText;
    public Text currentWorldBadgeText;

    [Header("HUD Elements (Top-Right)")]
    public Text heartsText;
    public Button pauseButton;
    public Button muteButton;
    public Text muteButtonText;

    [Header("HUD Dynamic Indicators")]
    public Text speedometerText;
    public Text comboBadgeText;

    [Header("Power-Up & Notification Display")]
    public Text powerUpBadgeText;
    public Text statusMessageText;

    [Header("Pause Menu")]
    public Button resumeButton;
    public Button pauseRestartButton;
    public Button pauseHomeButton;

    [Header("Game Over Elements")]
    public Text finalScoreText;
    public Text highScoreText;
    public Text gameOverCoinsText;
    public Text gameOverGemsText;
    public Button reviveButton;
    public Button retryButton;
    public Button homeButton;

    [Header("World Selection Buttons")]
    public Button grasslandButton;
    public Button jungleButton;
    public Button desertButton;
    public Button iceWorldButton;
    public Button mechanicalButton;

    private Coroutine powerUpTimerCoroutine;
    private Coroutine statusMessageCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // HUD
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

        if (muteButton != null)
        {
            muteButton.onClick.AddListener(OnMuteClicked);
            UpdateMuteButtonVisual();
        }

        // Pause Menu
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (pauseRestartButton != null)
            pauseRestartButton.onClick.AddListener(OnRetryClicked);

        if (pauseHomeButton != null)
            pauseHomeButton.onClick.AddListener(OnHomeClicked);

        // Game Over
        if (reviveButton != null)
            reviveButton.onClick.AddListener(OnReviveClicked);

        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);

        if (homeButton != null)
            homeButton.onClick.AddListener(OnHomeClicked);

        // World Select
        if (grasslandButton != null)
            grasslandButton.onClick.AddListener(() => OnWorldButtonClicked(WorldType.Grassland));

        if (jungleButton != null)
            jungleButton.onClick.AddListener(() => OnWorldButtonClicked(WorldType.Jungle));

        if (desertButton != null)
            desertButton.onClick.AddListener(() => OnWorldButtonClicked(WorldType.Desert));

        if (iceWorldButton != null)
            iceWorldButton.onClick.AddListener(() => OnWorldButtonClicked(WorldType.IceWorld));

        if (mechanicalButton != null)
            mechanicalButton.onClick.AddListener(() => OnWorldButtonClicked(WorldType.Mechanical));
    }

    private void Update()
    {
        // Mute toggle hotkey
        if (Input.GetKeyDown(KeyCode.M))
        {
            OnMuteClicked();
        }

        if (GameManager.Instance != null && GameManager.Instance.isPlaying)
        {
            UpdateHUD();
        }
    }

    private void UpdateHUD()
    {
        if (GameManager.Instance == null) return;

        if (scoreText != null)
            scoreText.text = $"★ {GameManager.Instance.totalScore:D5}";

        if (coinText != null)
            coinText.text = $"● {GameManager.Instance.coins}";

        if (gemText != null)
            gemText.text = $"◆ {GameManager.Instance.gems}";

        if (currentWorldBadgeText != null && WorldManager.Instance != null)
        {
            WorldThemeData currentTheme = WorldManager.Instance.GetCurrentTheme();
            if (currentTheme != null)
                currentWorldBadgeText.text = currentTheme.displayName.ToUpper();
        }

        // Real-time speed & combo metrics
        PlayerController player = PlayerController.ActivePlayer;
        if (speedometerText != null && player != null)
        {
            float speedKmh = player.forwardSpeed * 3.6f;
            speedometerText.text = $"⚡ {speedKmh:F0} KM/H";
        }

        if (comboBadgeText != null)
        {
            if (ComboSystem.Instance != null && ComboSystem.Instance.comboMultiplier > 1)
            {
                comboBadgeText.gameObject.SetActive(true);
                comboBadgeText.text = $"🔥 {ComboSystem.Instance.comboMultiplier}X COMBO";
            }
            else
            {
                comboBadgeText.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateHealth(int health)
    {
        if (heartsText == null) return;

        int max = (PlayerController.ActivePlayer != null) ? PlayerController.ActivePlayer.maxHealth : 3;
        health = Mathf.Clamp(health, 0, max);
        string hearts = "";
        for (int i = 0; i < max; i++)
        {
            hearts += (i < health) ? "♥ " : "♡ ";
        }
        heartsText.text = hearts.TrimEnd();
    }

    public void ShowPowerUpActive(PowerUpType type, float duration)
    {
        if (powerUpBadgeText == null) return;

        if (powerUpTimerCoroutine != null)
            StopCoroutine(powerUpTimerCoroutine);

        powerUpTimerCoroutine = StartCoroutine(PowerUpTimerRoutine(type, duration));
    }

    private IEnumerator PowerUpTimerRoutine(PowerUpType type, float duration)
    {
        string icon = type switch
        {
            PowerUpType.Magnet => "🧲 MAGNET",
            PowerUpType.Shield => "🛡 SHIELD",
            PowerUpType.SpeedBoost => "⚡ SPEED BOOST",
            PowerUpType.ScoreMultiplier => "✖2 MULTIPLIER",
            PowerUpType.Invincibility => "★ INVINCIBILITY",
            _ => "POWER-UP"
        };

        if (duration > 900f)
        {
            powerUpBadgeText.gameObject.SetActive(true);
            powerUpBadgeText.text = $"{icon} ACTIVE";
            yield break;
        }

        float remaining = duration;
        powerUpBadgeText.gameObject.SetActive(true);

        while (remaining > 0f)
        {
            powerUpBadgeText.text = $"{icon} : {Mathf.CeilToInt(remaining)}s";
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        powerUpBadgeText.gameObject.SetActive(false);
    }

    public void ShowStatusMessage(string message)
    {
        if (statusMessageText == null) return;

        if (statusMessageCoroutine != null)
            StopCoroutine(statusMessageCoroutine);

        statusMessageCoroutine = StartCoroutine(StatusMessageRoutine(message));
    }

    private IEnumerator StatusMessageRoutine(string message)
    {
        statusMessageText.gameObject.SetActive(true);
        statusMessageText.text = message;
        yield return new WaitForSeconds(1.8f);
        statusMessageText.gameObject.SetActive(false);
    }

    // --- Panel Transitions ---

    public void ShowWorldSelect()
    {
        if (worldSelectPanel != null) worldSelectPanel.SetActive(true);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void ShowHUD()
    {
        if (worldSelectPanel != null) worldSelectPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        UpdateHUD();
    }

    public void ShowPauseMenu()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void HidePauseMenu()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void ShowGameOver(int finalScore, int highScore, int coins, int gems)
    {
        if (worldSelectPanel != null) worldSelectPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (finalScoreText != null)
            finalScoreText.text = $"{finalScore:D5}";

        if (highScoreText != null)
            highScoreText.text = $"BEST: {highScore:D5}";

        if (gameOverCoinsText != null)
            gameOverCoinsText.text = $"COINS: {coins}";

        if (gameOverGemsText != null)
            gameOverGemsText.text = $"GEMS: {gems}";

        if (reviveButton != null)
        {
            Text btnText = reviveButton.GetComponentInChildren<Text>();
            if (btnText != null)
            {
                btnText.text = gems >= 5 ? "REVIVE (◆ 5)" : "REVIVE (FREE)";
            }
        }
    }

    public void OnMuteClicked()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.ToggleMute();
            UpdateMuteButtonVisual();
        }
    }

    private void UpdateMuteButtonVisual()
    {
        if (muteButtonText != null && SoundManager.Instance != null)
        {
            muteButtonText.text = SoundManager.Instance.isMuted ? "🔇 SOUND OFF" : "🔊 SOUND ON";
        }
    }

    // --- Button Callbacks ---

    public void OnWorldButtonClicked(WorldType world)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartRun(world);
        }
    }

    public void OnPauseClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PauseGame();
        }
    }

    public void OnResumeClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    public void OnReviveClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TryRevive();
        }
    }

    public void OnRetryClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartCurrentRun();
        }
    }

    public void OnHomeClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OpenWorldSelect();
        }
    }
}
