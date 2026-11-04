using UnityEngine;

/// <summary>
/// Realistic, performance-friendly procedural visual effects engine for PlatformRunner3D.
/// Generates subtle, physically grounded dust puffs, slide friction plumes, landing impact rings,
/// and collection effects without external texture dependencies.
/// </summary>
public class CollectionVFX : MonoBehaviour
{
    public static CollectionVFX Instance { get; private set; }

    private ParticleSystem footstepPS;
    private ParticleSystem landingPS;
    private ParticleSystem slidePS;
    private ParticleSystem hitPS;
    private ParticleSystem deathPS;
    private ParticleSystem coinPS;
    private ParticleSystem gemPS;

    private Material particleMat;
    private Texture2D softCircleTex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildParticleResources();
        InitializeParticleSystems();
    }

    private void BuildParticleResources()
    {
        // Procedurally generate a soft radial falloff texture
        softCircleTex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        softCircleTex.wrapMode = TextureWrapMode.Clamp;
        softCircleTex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float u = (x - 15.5f) / 15.5f;
                float v = (y - 15.5f) / 15.5f;
                float dist = Mathf.Clamp01(Mathf.Sqrt(u * u + v * v));
                float alpha = Mathf.SmoothStep(1f, 0f, dist);
                alpha = Mathf.Pow(alpha, 1.8f);
                softCircleTex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        softCircleTex.Apply();

        Shader shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Mobile/Particles/Alpha Blended");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        particleMat = new Material(shader);
        particleMat.mainTexture = softCircleTex;
    }

    private void InitializeParticleSystems()
    {
        // 1. Subtle Footstep Dust
        footstepPS = CreateSubtleSystem("FootstepDust", 15, new Color(0.9f, 0.85f, 0.75f, 0.22f));
        var footMain = footstepPS.main;
        footMain.startLifetime = 0.35f;
        footMain.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.22f);
        footMain.startSpeed = 0.4f;

        // 2. Landing Impact Dust Ring
        landingPS = CreateSubtleSystem("LandingDust", 30, new Color(0.92f, 0.88f, 0.8f, 0.35f));
        var landMain = landingPS.main;
        landMain.startLifetime = 0.45f;
        landMain.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.45f);
        landMain.startSpeed = new ParticleSystem.MinMaxCurve(0.6f, 1.8f);

        // 3. Slide Friction Dust
        slidePS = CreateSubtleSystem("SlideDust", 40, new Color(0.88f, 0.84f, 0.78f, 0.25f));
        var slideMain = slidePS.main;
        slideMain.startLifetime = 0.4f;
        slideMain.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
        slideMain.startSpeed = 0.5f;

        // 4. Hit / Damage Puff
        hitPS = CreateSubtleSystem("HitPuff", 20, new Color(1.0f, 0.35f, 0.25f, 0.5f));
        var hitMain = hitPS.main;
        hitMain.startLifetime = 0.3f;
        hitMain.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.4f);
        hitMain.startSpeed = 2.2f;

        // 5. Death Impact
        deathPS = CreateSubtleSystem("DeathPuff", 50, new Color(0.95f, 0.3f, 0.2f, 0.6f));
        var deathMain = deathPS.main;
        deathMain.startLifetime = 0.6f;
        deathMain.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.6f);
        deathMain.startSpeed = 3.5f;

        // 6. Coin Sparkle (subtle warm glow)
        coinPS = CreateSubtleSystem("CoinSparkle", 15, new Color(1.0f, 0.88f, 0.25f, 0.45f));
        var coinMain = coinPS.main;
        coinMain.startLifetime = 0.3f;
        coinMain.startSize = 0.25f;
        coinMain.startSpeed = 0.8f;

        // 7. Gem Sparkle (subtle cyan glimmer)
        gemPS = CreateSubtleSystem("GemSparkle", 20, new Color(0.2f, 0.85f, 1.0f, 0.5f));
        var gemMain = gemPS.main;
        gemMain.startLifetime = 0.35f;
        gemMain.startSize = 0.3f;
        gemMain.startSpeed = 1.0f;
    }

    private ParticleSystem CreateSubtleSystem(string sysName, int maxParticles, Color tint)
    {
        GameObject go = new GameObject(sysName);
        go.transform.SetParent(transform, false);

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ParticleSystemRenderer psr = go.GetComponent<ParticleSystemRenderer>();
        psr.sharedMaterial = particleMat;

        var main = ps.main;
        main.playOnAwake = false;
        main.maxParticles = maxParticles;
        main.startColor = tint;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.05f;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.15f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(tint, 0.0f), new GradientColorKey(tint, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(tint.a, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = grad;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0.0f, 0.5f);
        curve.AddKey(0.3f, 1.0f);
        curve.AddKey(1.0f, 1.4f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, curve);

        return ps;
    }

    // =========================================================================
    // PUBLIC VFX API
    // =========================================================================

    public void PlayFootstepPuff(Vector3 pos)
    {
        if (footstepPS == null) return;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = pos + new Vector3(Random.Range(-0.06f, 0.06f), 0.02f, Random.Range(-0.06f, 0.06f)),
            velocity = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0.1f, 0.35f), Random.Range(-0.25f, -0.05f))
        };
        footstepPS.Emit(emitParams, 2);
    }

    public void PlayLandingDust(Vector3 pos, float impactSpeed = 10f)
    {
        if (landingPS == null) return;
        int count = Mathf.Clamp(Mathf.RoundToInt(impactSpeed * 0.45f), 4, 14);
        float radius = 0.25f;

        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * Mathf.PI * 2f;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0.15f, Mathf.Sin(angle));
            var emitParams = new ParticleSystem.EmitParams
            {
                position = pos + new Vector3(dir.x * radius, 0.02f, dir.z * radius),
                velocity = dir * Random.Range(0.6f, 1.4f)
            };
            landingPS.Emit(emitParams, 1);
        }
    }

    public void PlaySlideDust(Vector3 pos)
    {
        if (slidePS == null) return;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = pos + new Vector3(Random.Range(-0.15f, 0.15f), 0.02f, Random.Range(-0.1f, 0.1f)),
            velocity = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(0.1f, 0.3f), Random.Range(-0.8f, -0.2f))
        };
        slidePS.Emit(emitParams, 2);
    }

    public void PlayDamageEffect(Vector3 pos)
    {
        if (hitPS == null) return;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = pos + Vector3.up * 0.6f,
            velocity = Vector3.up * 1.5f
        };
        hitPS.Emit(emitParams, 12);
    }

    public void PlayDeathEffect(Vector3 pos)
    {
        if (deathPS == null) return;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = pos + Vector3.up * 0.5f,
            velocity = Vector3.up * 2.0f
        };
        deathPS.Emit(emitParams, 25);
    }

    public void PlayCoinCollect(Vector3 pos)
    {
        if (coinPS == null) return;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = pos,
            velocity = Vector3.up * 0.8f
        };
        coinPS.Emit(emitParams, 4);
    }

    public void PlayGemCollect(Vector3 pos)
    {
        if (gemPS == null) return;
        var emitParams = new ParticleSystem.EmitParams
        {
            position = pos,
            velocity = Vector3.up * 1.0f
        };
        gemPS.Emit(emitParams, 8);
    }

    public void PlayPowerUpCollect(Vector3 pos, PowerUpType type)
    {
        PlayGemCollect(pos);
    }
}
