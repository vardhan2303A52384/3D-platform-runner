using UnityEngine;

/// <summary>
/// Dynamic visual slipstream and power-up aura attached to the player.
/// Emits clean aerodynamic wind streaks during high-speed sprints and vibrant energy trails during active power-ups.
/// Avoids distracting ribbon clutter during normal pace for maximum realism.
/// </summary>
public class PlayerTrailEffect : MonoBehaviour
{
    private TrailRenderer trail;
    private PlayerController player;

    [Header("Colors")]
    public Color slipstreamColor = new Color(0.9f, 0.95f, 1.0f, 0.22f);
    public Color speedColor = new Color(0.0f, 0.95f, 1.0f, 0.75f);
    public Color invincibleColor = new Color(1.0f, 0.85f, 0.1f, 0.85f);
    public Color magnetColor = new Color(1.0f, 0.25f, 0.5f, 0.75f);

    private void Start()
    {
        player = GetComponentInParent<PlayerController>();
        trail = GetComponent<TrailRenderer>();

        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
        }

        trail.time = 0.35f;
        trail.startWidth = 0.35f;
        trail.endWidth = 0.02f;
        trail.minVertexDistance = 0.1f;

        Shader shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        trail.material = new Material(shader);
        trail.startColor = slipstreamColor;
        trail.endColor = new Color(slipstreamColor.r, slipstreamColor.g, slipstreamColor.b, 0f);
    }

    private void Update()
    {
        if (player == null)
        {
            player = GetComponentInParent<PlayerController>();
            if (player == null) return;
        }

        if (trail == null) return;

        bool hasPowerUp = player.isInvincible || player.hasSpeedBoost || player.hasMagnet;
        bool isHighSpeed = player.forwardSpeed > 17f;

        if (player.IsDead || !player.isRunning || (!hasPowerUp && !isHighSpeed))
        {
            trail.emitting = false;
            return;
        }

        trail.emitting = true;

        Color targetColor = slipstreamColor;
        if (player.isInvincible) targetColor = invincibleColor;
        else if (player.hasSpeedBoost) targetColor = speedColor;
        else if (player.hasMagnet) targetColor = magnetColor;

        trail.startColor = Color.Lerp(trail.startColor, targetColor, Time.deltaTime * 6f);
        trail.endColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

        // Dynamic width & duration scaling with forward velocity
        float speedRatio = Mathf.Clamp01(player.forwardSpeed / player.maxSpeed);
        trail.startWidth = Mathf.Lerp(0.2f, 0.45f, speedRatio);
        trail.time = Mathf.Lerp(0.25f, 0.45f, speedRatio);
    }
}
