using UnityEngine;

/// <summary>
/// Screen shake manager for dramatic impacts, near-misses, landings, and explosions.
/// Can be attached to the Camera or used as a singleton.
/// </summary>
public class ScreenShakeEffect : MonoBehaviour
{
    public static ScreenShakeEffect Instance { get; private set; }

    [Header("Shake Tuning")]
    public float defaultDuration = 0.25f;
    public float defaultMagnitude = 0.15f;
    public float dampingSpeed = 2.5f;

    private float shakeDuration = 0f;
    private float shakeMagnitude = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    public Vector3 CurrentShakeOffset => shakeOffset;

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
        if (shakeDuration > 0f)
        {
            float offsetX = Random.Range(-1f, 1f) * shakeMagnitude;
            float offsetY = Random.Range(-1f, 1f) * shakeMagnitude;
            shakeOffset = new Vector3(offsetX, offsetY, 0f);

            shakeDuration -= Time.deltaTime * dampingSpeed;
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, Time.deltaTime * dampingSpeed * 2f);
        }
        else
        {
            shakeDuration = 0f;
            shakeMagnitude = 0f;
            shakeOffset = Vector3.zero;
        }
    }

    public void Shake(float duration = -1f, float magnitude = -1f)
    {
        shakeDuration = duration > 0f ? duration : defaultDuration;
        shakeMagnitude = magnitude > 0f ? magnitude : defaultMagnitude;
    }

    public void ShakeHit()
    {
        Shake(0.35f, 0.3f);
    }

    public void ShakePowerUp()
    {
        Shake(0.18f, 0.12f);
    }

    public void ShakeNearMiss()
    {
        Shake(0.12f, 0.08f);
    }

    public void ShakeLanding()
    {
        Shake(0.1f, 0.06f);
    }

    public void ShakeDeath()
    {
        Shake(0.55f, 0.45f);
    }
}

