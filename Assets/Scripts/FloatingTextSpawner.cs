using UnityEngine;

/// <summary>
/// World-space floating texts have been removed per user preference for a clean, distraction-free visual experience.
/// Methods are maintained as silent safe no-ops to preserve API compatibility with Coin, Gem, Combo, and Player systems.
/// </summary>
public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance { get; private set; }

    [Header("Colors (Preserved for compatibility)")]
    public Color coinColor = new Color(1.0f, 0.88f, 0.15f);
    public Color gemColor = new Color(0.0f, 0.95f, 1.0f);
    public Color comboColor = new Color(1.0f, 0.55f, 0.05f);
    public Color nearMissColor = new Color(0.2f, 1.0f, 0.4f);
    public Color damageColor = new Color(1.0f, 0.2f, 0.2f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnCoinText(Vector3 worldPos, int value, int multiplier = 1) { }
    public void SpawnGemText(Vector3 worldPos, int value) { }
    public void SpawnComboText(Vector3 worldPos, int multiplier) { }
    public void SpawnNearMissText(Vector3 worldPos) { }
    public void SpawnDamageText(Vector3 worldPos) { }
}
