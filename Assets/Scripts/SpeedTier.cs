using UnityEngine;

/// <summary>
/// Controls progressive speed tiers, dynamic FOV warping, and speed sensations.
/// </summary>
public class SpeedTier : MonoBehaviour
{
    public static SpeedTier Instance { get; private set; }

    public enum Tier
    {
        Normal,
        Fast,
        Blazing,
        Hypersonic,
        WarpSpeed
    }

    [Header("Speed Thresholds")]
    public float fastThreshold = 16f;
    public float blazingThreshold = 20f;
    public float hypersonicThreshold = 24f;
    public float warpSpeedThreshold = 28f;

    [Header("FOV Dynamics")]
    public float baseFOV = 60f;
    public float maxFOV = 82f;
    public float fovLerpSpeed = 3.5f;

    public Tier currentTier = Tier.Normal;
    private Tier previousTier = Tier.Normal;

    private Camera runnerCamera;

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
        runnerCamera = Camera.main;
    }

    private void Update()
    {
        if (PlayerController.ActivePlayer == null || PlayerController.ActivePlayer.IsDead || !PlayerController.ActivePlayer.isRunning)
        {
            ResetFOV();
            return;
        }

        float speed = PlayerController.ActivePlayer.forwardSpeed;
        previousTier = currentTier;

        if (speed >= warpSpeedThreshold) currentTier = Tier.WarpSpeed;
        else if (speed >= hypersonicThreshold) currentTier = Tier.Hypersonic;
        else if (speed >= blazingThreshold) currentTier = Tier.Blazing;
        else if (speed >= fastThreshold) currentTier = Tier.Fast;
        else currentTier = Tier.Normal;

        if (currentTier > previousTier)
        {
            OnTierPromoted();
        }

        // Adjust Camera FOV dynamically
        if (runnerCamera != null)
        {
            float ratio = Mathf.InverseLerp(12f, warpSpeedThreshold + 4f, speed);
            float targetFOV = Mathf.Lerp(baseFOV, maxFOV, ratio);
            runnerCamera.fieldOfView = Mathf.Lerp(runnerCamera.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }

        // Adjust music pitch slightly for adrenaline
        if (SoundManager.Instance != null)
        {
            float pitchRatio = Mathf.Lerp(1.0f, 1.25f, Mathf.InverseLerp(12f, warpSpeedThreshold, speed));
            SoundManager.Instance.SetMusicPitch(pitchRatio);
        }
    }

    private void OnTierPromoted()
    {
        string title = currentTier switch
        {
            Tier.Fast => "⚡ SPEED BOOST: FAST!",
            Tier.Blazing => "🔥 SPEED UP: BLAZING!",
            Tier.Hypersonic => "🚀 HYPERSONIC SPEED!",
            Tier.WarpSpeed => "✨ WARP SPEED!",
            _ => ""
        };

        if (UIManager.Instance != null && !string.IsNullOrEmpty(title))
        {
            UIManager.Instance.ShowStatusMessage(title);
        }

        if (ScreenShakeEffect.Instance != null)
        {
            ScreenShakeEffect.Instance.Shake(0.15f, 0.08f);
        }
    }

    public void ResetFOV()
    {
        currentTier = Tier.Normal;
        if (runnerCamera != null)
        {
            runnerCamera.fieldOfView = Mathf.Lerp(runnerCamera.fieldOfView, baseFOV, Time.deltaTime * 5f);
        }
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicPitch(1.0f);
        }
    }
}

