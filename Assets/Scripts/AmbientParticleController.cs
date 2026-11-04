using UnityEngine;

/// <summary>
/// Spawns and manages ambient floating particles for the current world theme.
/// Grassland: floating leaves. Jungle: fireflies. Desert: sand particles.
/// Ice: snowflakes. Mechanical: sparks.
/// </summary>
public class AmbientParticleController : MonoBehaviour
{
    [Header("References")]
    public Transform followTarget;

    [Header("Particle Settings")]
    public int maxParticles = 30;
    public float spawnRadius = 15f;
    public float spawnHeight = 8f;
    public float particleLifetime = 5f;

    private ParticleSystem ps;
    private WorldType currentWorld;
    private bool isActive = false;

    private void Start()
    {
        if (followTarget == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) followTarget = player.transform;
        }

        CreateParticleSystem();
    }

    private void Update()
    {
        if (followTarget != null)
        {
            transform.position = followTarget.position + new Vector3(0f, 5f, 5f);
        }
    }

    public void SetupForWorld(WorldThemeData theme)
    {
        if (theme == null || ps == null) return;

        if (!theme.hasAmbientParticles)
        {
            ps.Stop();
            isActive = false;
            return;
        }

        var main = ps.main;
        var emission = ps.emission;
        var shape = ps.shape;
        var colorOverLifetime = ps.colorOverLifetime;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        switch (theme.worldType)
        {
            case WorldType.Grassland:
                // Floating leaves / pollen
                main.startColor = new Color(0.3f, 0.75f, 0.2f, 0.8f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.22f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1.0f);
                main.startLifetime = 6f;
                main.gravityModifier = 0.08f;
                emission.rateOverTime = 8f;
                break;

            case WorldType.Jungle:
                // Glowing fireflies / pollen
                main.startColor = new Color(0.4f, 1.0f, 0.3f, 0.9f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.12f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.8f);
                main.startLifetime = 8f;
                main.gravityModifier = -0.02f;
                emission.rateOverTime = 12f;
                break;

            case WorldType.Desert:
                // Sand / dust particles
                main.startColor = new Color(0.9f, 0.75f, 0.4f, 0.6f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.1f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(1.0f, 3.0f);
                main.startLifetime = 3f;
                main.gravityModifier = 0.02f;
                emission.rateOverTime = 20f;
                break;

            case WorldType.IceWorld:
                // Snowflakes
                main.startColor = new Color(0.95f, 0.97f, 1.0f, 0.9f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.18f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1.5f);
                main.startLifetime = 7f;
                main.gravityModifier = 0.12f;
                emission.rateOverTime = 25f;
                break;

            case WorldType.Mechanical:
                // Sparks / embers
                main.startColor = new Color(1.0f, 0.6f, 0.1f, 0.85f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4.0f);
                main.startLifetime = 2f;
                main.gravityModifier = 0.15f;
                emission.rateOverTime = 15f;
                break;
        }

        main.maxParticles = maxParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(spawnRadius * 2f, spawnHeight, spawnRadius);

        ps.Play();
        isActive = true;
        currentWorld = theme.worldType;
    }

    private void CreateParticleSystem()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            ps = gameObject.AddComponent<ParticleSystem>();
        }

        var main = ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.startLifetime = particleLifetime;
        main.maxParticles = maxParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 10f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(spawnRadius * 2f, spawnHeight, spawnRadius);

        // Disable default renderer material, use default particle material
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }

        ps.Stop();
    }
}
