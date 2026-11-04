using System.Collections.Generic;
using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    [Header("Segment Configuration")]
    public float length = 30f;
    public Transform itemsContainer;

    [Header("Segment Renderers")]
    public Renderer roadRenderer;
    public Renderer leftDividerRenderer;
    public Renderer rightDividerRenderer;
    public Renderer leftFlankRenderer;
    public Renderer rightFlankRenderer;
    public Renderer leftCurbRenderer;
    public Renderer rightCurbRenderer;
    public Renderer leftRidgeRenderer;
    public Renderer rightRidgeRenderer;

    private readonly List<GameObject> spawnedItems = new List<GameObject>();

    private void Awake()
    {
        DisableGroundShadows();
        RemoveSideWalls();
    }

    /// <summary>
    /// Removes the mountain / canyon side walls from both sides of the environment,
    /// and expands the ground flanks to provide an open, expansive horizon.
    /// </summary>
    public void RemoveSideWalls()
    {
        if (leftRidgeRenderer != null)
        {
            leftRidgeRenderer.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Destroy(leftRidgeRenderer.gameObject);
            }
            else
            {
                DestroyImmediate(leftRidgeRenderer.gameObject);
            }
            leftRidgeRenderer = null;
        }

        if (rightRidgeRenderer != null)
        {
            rightRidgeRenderer.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Destroy(rightRidgeRenderer.gameObject);
            }
            else
            {
                DestroyImmediate(rightRidgeRenderer.gameObject);
            }
            rightRidgeRenderer = null;
        }

        Transform leftRidge = transform.Find("Ridge_Left");
        if (leftRidge != null)
        {
            leftRidge.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Destroy(leftRidge.gameObject);
            }
            else
            {
                DestroyImmediate(leftRidge.gameObject);
            }
        }

        Transform rightRidge = transform.Find("Ridge_Right");
        if (rightRidge != null)
        {
            rightRidge.gameObject.SetActive(false);
            if (Application.isPlaying)
            {
                Destroy(rightRidge.gameObject);
            }
            else
            {
                DestroyImmediate(rightRidge.gameObject);
            }
        }

        // Expand side ground flanks to cover horizon where walls were removed
        Transform leftFlank = transform.Find("Flank_Left");
        if (leftFlank != null)
        {
            leftFlank.localPosition = new Vector3(-25f, -0.25f, 15f);
            leftFlank.localScale = new Vector3(40f, 0.4f, 30f);
        }

        Transform rightFlank = transform.Find("Flank_Right");
        if (rightFlank != null)
        {
            rightFlank.localPosition = new Vector3(25f, -0.25f, 15f);
            rightFlank.localScale = new Vector3(40f, 0.4f, 30f);
        }
    }

    public void DisableGroundShadows()
    {
        if (roadRenderer != null) roadRenderer.receiveShadows = false;
        if (leftDividerRenderer != null) leftDividerRenderer.receiveShadows = false;
        if (rightDividerRenderer != null) rightDividerRenderer.receiveShadows = false;
        if (leftFlankRenderer != null) leftFlankRenderer.receiveShadows = false;
        if (rightFlankRenderer != null) rightFlankRenderer.receiveShadows = false;
        if (leftCurbRenderer != null) leftCurbRenderer.receiveShadows = false;
        if (rightCurbRenderer != null) rightCurbRenderer.receiveShadows = false;
        if (leftRidgeRenderer != null) leftRidgeRenderer.receiveShadows = false;
        if (rightRidgeRenderer != null) rightRidgeRenderer.receiveShadows = false;

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in allRenderers)
        {
            if (r != null)
            {
                r.receiveShadows = false;
            }
        }
    }

    /// <summary>
    /// Populates obstacles, coins, gems, power-ups, and side scenery.
    /// </summary>
    public void SetupSegment(float laneDistance, GameObject[] obstaclePrefabs, GameObject coinPrefab, GameObject gemPrefab, GameObject[] powerUpPrefabs, WorldThemeData theme, bool isSafe)
    {
        ClearSpawnedItems();
        DisableGroundShadows();
        RemoveSideWalls();

        Transform parent = itemsContainer != null ? itemsContainer : transform;

        // Apply World Theme Materials
        if (theme != null)
        {
            if (roadRenderer != null && theme.trackMaterial != null)
                roadRenderer.sharedMaterial = theme.trackMaterial;

            if (leftDividerRenderer != null && theme.laneDividerMaterial != null)
                leftDividerRenderer.sharedMaterial = theme.laneDividerMaterial;

            if (rightDividerRenderer != null && theme.laneDividerMaterial != null)
                rightDividerRenderer.sharedMaterial = theme.laneDividerMaterial;

            if (leftFlankRenderer != null && theme.sideGroundMaterial != null)
                leftFlankRenderer.sharedMaterial = theme.sideGroundMaterial;

            if (rightFlankRenderer != null && theme.sideGroundMaterial != null)
                rightFlankRenderer.sharedMaterial = theme.sideGroundMaterial;

            if (leftCurbRenderer != null && theme.laneDividerMaterial != null)
                leftCurbRenderer.sharedMaterial = theme.laneDividerMaterial;

            if (rightCurbRenderer != null && theme.laneDividerMaterial != null)
                rightCurbRenderer.sharedMaterial = theme.laneDividerMaterial;

            if (leftRidgeRenderer != null && theme.barrierMaterial != null)
                leftRidgeRenderer.sharedMaterial = theme.barrierMaterial;

            if (rightRidgeRenderer != null && theme.barrierMaterial != null)
                rightRidgeRenderer.sharedMaterial = theme.barrierMaterial;

            SpawnSideDecorations(theme, parent);
            SpawnDetailProps(theme, parent);
            SpawnOverheadDecorations(theme, parent);
        }

        if (isSafe)
        {
            // Spawn introductory coins & 1 gem
            if (coinPrefab != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector3 coinPos = transform.position + new Vector3(0f, 0.7f, 6f + (i * 2.5f));
                    GameObject coin = Instantiate(coinPrefab, coinPos, coinPrefab.transform.rotation, parent);
                    ApplyCoinMaterial(coin, theme);
                    spawnedItems.Add(coin);
                }
            }
            if (gemPrefab != null)
            {
                Vector3 gemPos = transform.position + new Vector3(0f, 0.8f, 20f);
                GameObject gem = Instantiate(gemPrefab, gemPos, Quaternion.identity, parent);
                spawnedItems.Add(gem);
            }
            return;
        }

        // Two obstacle rows per 30m segment
        float[] rowZOffsets = new float[] { length * 0.35f, length * 0.75f };

        foreach (float zOffset in rowZOffsets)
        {
            int obstacleCount = Random.value < 0.65f ? 1 : 2;

            List<int> lanes = new List<int> { 0, 1, 2 };
            for (int i = 0; i < lanes.Count; i++)
            {
                int temp = lanes[i];
                int randomIndex = Random.Range(i, lanes.Count);
                lanes[i] = lanes[randomIndex];
                lanes[randomIndex] = temp;
            }

            // Spawn obstacles on chosen lanes
            for (int i = 0; i < obstacleCount; i++)
            {
                int lane = lanes[i];
                float laneX = (lane - 1) * laneDistance;

                GameObject[] pool = (theme != null && theme.customObstaclePrefabs != null && theme.customObstaclePrefabs.Length > 0)
                    ? theme.customObstaclePrefabs
                    : obstaclePrefabs;

                if (pool != null && pool.Length > 0)
                {
                    GameObject chosenPrefab = pool[Random.Range(0, pool.Length)];
                    if (chosenPrefab != null)
                    {
                        Vector3 spawnPos = transform.position + new Vector3(laneX, 0f, zOffset);
                        GameObject obs = Instantiate(chosenPrefab, spawnPos, chosenPrefab.transform.rotation, parent);
                        ApplyObstacleMaterial(obs, theme);

                        // Dynamic moving obstacle challenge at higher distances
                        if (Random.value < 0.28f && GameManager.Instance != null && GameManager.Instance.distanceScore > 120)
                        {
                            MovingObstacle mover = obs.GetComponent<MovingObstacle>();
                            if (mover == null)
                            {
                                mover = obs.AddComponent<MovingObstacle>();
                                mover.pattern = (Random.value < 0.65f) ? MovingObstacle.MovementPattern.SideToSide : MovingObstacle.MovementPattern.Bobbing;
                                mover.distance = 1.05f;
                                mover.speed = Random.Range(1.8f, 2.6f);
                            }
                        }

                        spawnedItems.Add(obs);
                    }
                }
            }

            // In the open lane(s), spawn items
            int freeLane = lanes[obstacleCount];
            float openLaneX = (freeLane - 1) * laneDistance;
            float roll = Random.value;

            if (roll < 0.18f && powerUpPrefabs != null && powerUpPrefabs.Length > 0)
            {
                // Spawn a Power-Up
                GameObject chosenPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
                if (chosenPowerUp != null)
                {
                    Vector3 pupPos = transform.position + new Vector3(openLaneX, 0.9f, zOffset);
                    GameObject pup = Instantiate(chosenPowerUp, pupPos, Quaternion.identity, parent);
                    spawnedItems.Add(pup);
                }
            }
            else if (roll < 0.38f && gemPrefab != null)
            {
                // Spawn rare Gem (💎)
                Vector3 gemPos = transform.position + new Vector3(openLaneX, 0.85f, zOffset);
                GameObject gem = Instantiate(gemPrefab, gemPos, Quaternion.identity, parent);
                spawnedItems.Add(gem);
            }
            else if (coinPrefab != null)
            {
                // Spawn line of coins
                for (int c = -1; c <= 1; c++)
                {
                    Vector3 coinPos = transform.position + new Vector3(openLaneX, 0.75f, zOffset + (c * 2.2f));
                    GameObject coin = Instantiate(coinPrefab, coinPos, Quaternion.identity, parent);
                    ApplyCoinMaterial(coin, theme);
                    spawnedItems.Add(coin);
                }
            }
        }
    }

    private void SpawnSideDecorations(WorldThemeData theme, Transform parent)
    {
        if (theme.sidePropPrefabs == null || theme.sidePropPrefabs.Length == 0) return;

        // More decoration positions for denser environments
        float[] zPositions = new float[] { length * 0.15f, length * 0.4f, length * 0.65f, length * 0.9f };

        foreach (float z in zPositions)
        {
            // Left side decorations
            if (Random.value < 0.85f)
            {
                GameObject prefab = theme.sidePropPrefabs[Random.Range(0, theme.sidePropPrefabs.Length)];
                if (prefab != null)
                {
                    float leftX = Random.Range(-14f, -7f);
                    Vector3 pos = transform.position + new Vector3(leftX, 0f, z + Random.Range(-2f, 2f));
                    Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    float scale = Random.Range(0.8f, 1.3f);
                    GameObject prop = Instantiate(prefab, pos, rot, parent);
                    prop.transform.localScale = prefab.transform.localScale * scale;
                    spawnedItems.Add(prop);
                }
            }

            // Right side decorations
            if (Random.value < 0.85f)
            {
                GameObject prefab = theme.sidePropPrefabs[Random.Range(0, theme.sidePropPrefabs.Length)];
                if (prefab != null)
                {
                    float rightX = Random.Range(7f, 14f);
                    Vector3 pos = transform.position + new Vector3(rightX, 0f, z + Random.Range(-2f, 2f));
                    Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    float scale = Random.Range(0.8f, 1.3f);
                    GameObject prop = Instantiate(prefab, pos, rot, parent);
                    prop.transform.localScale = prefab.transform.localScale * scale;
                    spawnedItems.Add(prop);
                }
            }
        }
    }

    /// <summary>
    /// Spawns small detail props like grass clumps, pebbles, flowers, snowdrifts, etc.
    /// </summary>
    private void SpawnDetailProps(WorldThemeData theme, Transform parent)
    {
        if (theme.detailPropPrefabs == null || theme.detailPropPrefabs.Length == 0) return;

        int detailCount = Mathf.RoundToInt(8 * theme.detailDensity);

        for (int i = 0; i < detailCount; i++)
        {
            if (Random.value > theme.detailDensity) continue;

            GameObject prefab = theme.detailPropPrefabs[Random.Range(0, theme.detailPropPrefabs.Length)];
            if (prefab == null) continue;

            // Scatter along both sides, including close to track edges
            float side = Random.value < 0.5f ? -1f : 1f;
            float x = side * Random.Range(5f, 16f);
            float z = Random.Range(0f, length);

            Vector3 pos = transform.position + new Vector3(x, 0f, z);
            Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            float scale = Random.Range(0.6f, 1.2f);

            GameObject detail = Instantiate(prefab, pos, rot, parent);
            detail.transform.localScale = prefab.transform.localScale * scale;
            spawnedItems.Add(detail);
        }
    }

    /// <summary>
    /// Spawns overhead decorations like arches, wires, hanging vines, etc.
    /// </summary>
    private void SpawnOverheadDecorations(WorldThemeData theme, Transform parent)
    {
        if (theme.overheadPropPrefabs == null || theme.overheadPropPrefabs.Length == 0) return;

        if (Random.value < theme.overheadDensity)
        {
            GameObject prefab = theme.overheadPropPrefabs[Random.Range(0, theme.overheadPropPrefabs.Length)];
            if (prefab != null)
            {
                float z = Random.Range(length * 0.3f, length * 0.7f);
                Vector3 pos = transform.position + new Vector3(0f, 0f, z);
                GameObject overhead = Instantiate(prefab, pos, prefab.transform.rotation, parent);
                spawnedItems.Add(overhead);
            }
        }
    }

    private void ApplyObstacleMaterial(GameObject obs, WorldThemeData theme)
    {
        if (theme == null) return;

        Obstacle obstacleComp = obs.GetComponent<Obstacle>();
        Renderer r = obs.GetComponent<Renderer>();
        if (r == null) r = obs.GetComponentInChildren<Renderer>();

        if (r != null)
        {
            if (obstacleComp != null && obstacleComp.obstacleType == Obstacle.ObstacleType.HighBarrier && theme.barrierMaterial != null)
            {
                r.sharedMaterial = theme.barrierMaterial;
            }
            else if (theme.hurdleMaterial != null)
            {
                r.sharedMaterial = theme.hurdleMaterial;
            }
        }
    }

    private void ApplyCoinMaterial(GameObject coin, WorldThemeData theme)
    {
        if (theme == null || theme.coinMaterial == null) return;
        Renderer r = coin.GetComponent<Renderer>();
        if (r == null) r = coin.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            r.sharedMaterial = theme.coinMaterial;
        }
    }

    public void ClearSpawnedItems()
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            if (spawnedItems[i] != null)
            {
                Destroy(spawnedItems[i]);
            }
        }
        spawnedItems.Clear();
    }

    private void OnDestroy()
    {
        ClearSpawnedItems();
    }
}
