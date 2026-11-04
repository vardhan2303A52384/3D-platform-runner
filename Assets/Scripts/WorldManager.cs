using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

    [Header("Worlds Configuration (5 Playable Worlds)")]
    public List<WorldThemeData> worlds = new List<WorldThemeData>();

    [Header("Current Selection")]
    public WorldType selectedWorld = WorldType.Grassland;

    [Header("References")]
    public Light directionalLight;
    public Camera runnerCamera;
    public AmbientParticleController ambientParticles;

    private const string WorldPrefKey = "Runner_SelectedWorld";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Load saved world choice
        int savedWorld = PlayerPrefs.GetInt(WorldPrefKey, (int)WorldType.Grassland);
        selectedWorld = (WorldType)Mathf.Clamp(savedWorld, 0, 4);
    }

    private void Start()
    {
        if (runnerCamera == null)
        {
            runnerCamera = Camera.main;
        }
        if (directionalLight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    directionalLight = l;
                    break;
                }
            }
        }

        if (directionalLight != null)
        {
            directionalLight.shadows = LightShadows.None;
        }

        // Find or create ambient particles
        if (ambientParticles == null)
        {
            ambientParticles = FindObjectOfType<AmbientParticleController>();
        }

        ApplyWorldTheme(selectedWorld);
    }

    public WorldThemeData GetTheme(WorldType worldType)
    {
        foreach (var w in worlds)
        {
            if (w.worldType == worldType)
                return w;
        }
        return worlds.Count > 0 ? worlds[0] : null;
    }

    public WorldThemeData GetCurrentTheme()
    {
        return GetTheme(selectedWorld);
    }

    public void SelectWorld(WorldType newWorld)
    {
        selectedWorld = newWorld;
        PlayerPrefs.SetInt(WorldPrefKey, (int)newWorld);
        PlayerPrefs.Save();

        ApplyWorldTheme(selectedWorld);
    }

    public void ApplyWorldTheme(WorldType worldType)
    {
        WorldThemeData theme = GetTheme(worldType);
        if (theme == null) return;

        // 1. Atmosphere & Fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = theme.fogColor;
        RenderSettings.fogDensity = theme.fogDensity;

        // 2. Ambient Light for richer environments
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.Lerp(theme.skyColor, theme.lightColor, 0.4f) * 0.6f;

        // 3. Camera Background
        if (runnerCamera != null)
        {
            runnerCamera.clearFlags = CameraClearFlags.SolidColor;
            runnerCamera.backgroundColor = theme.skyColor;
        }

        // 4. Directional Light (Shadows disabled to eliminate ground contact shadows)
        if (directionalLight != null)
        {
            directionalLight.shadows = LightShadows.None;
            directionalLight.color = theme.lightColor;
            directionalLight.intensity = theme.lightIntensity;

            // Adjust light angle per world for mood
            switch (worldType)
            {
                case WorldType.Grassland:
                    directionalLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
                    break;
                case WorldType.Jungle:
                    directionalLight.transform.rotation = Quaternion.Euler(65f, -15f, 0f);
                    break;
                case WorldType.Desert:
                    directionalLight.transform.rotation = Quaternion.Euler(35f, -45f, 0f);
                    break;
                case WorldType.IceWorld:
                    directionalLight.transform.rotation = Quaternion.Euler(40f, -20f, 0f);
                    break;
                case WorldType.Mechanical:
                    directionalLight.transform.rotation = Quaternion.Euler(55f, -35f, 0f);
                    break;
            }
        }

        // 5. Ambient Particles
        if (ambientParticles != null)
        {
            ambientParticles.SetupForWorld(theme);
        }
    }
}
