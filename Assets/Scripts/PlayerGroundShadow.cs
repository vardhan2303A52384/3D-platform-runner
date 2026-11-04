using UnityEngine;

/// <summary>
/// Dynamic soft contact ambient occlusion ground shadow for the runner.
/// Projects an authentic physical contact shadow directly onto the track surface beneath the player.
/// Dynamically scales, diffuses, and fades with player jump height and adapts shape during slides.
/// </summary>
public class PlayerGroundShadow : MonoBehaviour
{
    private GameObject shadowObj;
    private Transform shadowTransform;
    private Material shadowMaterial;
    private PlayerController player;

    [Header("Shadow Tuning")]
    public float groundY = 0.025f;
    public Vector2 groundedScale = new Vector2(0.85f, 1.05f);
    public Vector2 peakJumpScale = new Vector2(1.25f, 1.45f);
    public float groundedAlpha = 0.52f;
    public float airborneAlpha = 0.15f;
    public Color shadowColor = new Color(0.05f, 0.07f, 0.10f);

    private void Awake()
    {
        // Ground contact shadow removed
        enabled = false;
        GameObject contactShadowObj = GameObject.Find("GroundContactShadow");
        if (contactShadowObj != null)
        {
            Destroy(contactShadowObj);
        }
        Destroy(this);
    }

    private void Start()
    {
        if (shadowObj != null)
        {
            Destroy(shadowObj);
        }
        Destroy(this);
    }

    private void CreateShadowObject()
    {
        shadowObj = new GameObject("GroundContactShadow");
        shadowTransform = shadowObj.transform;
        shadowTransform.SetParent(null); // Keep world space orientation without inheriting player rotation

        MeshFilter mf = shadowObj.AddComponent<MeshFilter>();
        MeshRenderer mr = shadowObj.AddComponent<MeshRenderer>();

        // Build a flat horizontal quad
        Mesh mesh = new Mesh();
        mesh.name = "ShadowQuad";
        mesh.vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0f, -0.5f),
            new Vector3( 0.5f, 0f, -0.5f),
            new Vector3(-0.5f, 0f,  0.5f),
            new Vector3( 0.5f, 0f,  0.5f)
        };
        mesh.uv = new Vector2[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f)
        };
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        mesh.RecalculateNormals();
        mf.sharedMesh = mesh;

        // Build procedural soft gaussian falloff texture
        Texture2D shadowTex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        shadowTex.wrapMode = TextureWrapMode.Clamp;
        shadowTex.filterMode = FilterMode.Bilinear;

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float u = (x - 31.5f) / 31.5f;
                float v = (y - 31.5f) / 31.5f;
                float dist = Mathf.Sqrt(u * u + v * v);
                float alpha = Mathf.Clamp01(1f - dist);
                alpha = Mathf.SmoothStep(0f, 1f, alpha);
                alpha = Mathf.Pow(alpha, 1.6f);
                shadowTex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        shadowTex.Apply();

        Shader shader = Shader.Find("Particles/Standard Unlit");
        if (shader == null) shader = Shader.Find("Mobile/Particles/Alpha Blended");
        if (shader == null) shader = Shader.Find("Sprites/Default");

        shadowMaterial = new Material(shader);
        shadowMaterial.mainTexture = shadowTex;
        shadowMaterial.color = new Color(shadowColor.r, shadowColor.g, shadowColor.b, groundedAlpha);

        mr.sharedMaterial = shadowMaterial;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows = false;
    }

    private void LateUpdate()
    {
        if (shadowTransform == null) return;

        if (player != null && player.IsDead)
        {
            if (shadowObj.activeSelf) shadowObj.SetActive(false);
            return;
        }

        if (!shadowObj.activeSelf) shadowObj.SetActive(true);

        // Position shadow directly beneath player at track surface
        Vector3 playerPos = transform.position;
        shadowTransform.position = new Vector3(playerPos.x, groundY, playerPos.z);
        shadowTransform.rotation = Quaternion.identity;

        // Calculate height above track
        float heightAboveGround = Mathf.Max(0f, playerPos.y);
        float normalizedHeight = Mathf.Clamp01(heightAboveGround / 2.4f);

        // Scale and diffusion: expands slightly when high up (penumbra)
        Vector2 targetScale = Vector2.Lerp(groundedScale, peakJumpScale, normalizedHeight);

        // Slide posture elongation
        if (player != null && player.isSliding)
        {
            targetScale.y *= 1.45f;
            targetScale.x *= 0.85f;
        }

        shadowTransform.localScale = new Vector3(targetScale.x, 1f, targetScale.y);

        // Opacity attenuation: darker when grounded, fades with elevation
        float targetAlpha = Mathf.Lerp(groundedAlpha, airborneAlpha, normalizedHeight);
        if (shadowMaterial != null)
        {
            Color c = shadowColor;
            c.a = targetAlpha;
            shadowMaterial.color = c;
        }
    }

    private void OnDestroy()
    {
        if (shadowObj != null)
        {
            Destroy(shadowObj);
        }
    }
}
