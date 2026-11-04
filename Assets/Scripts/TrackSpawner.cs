using System.Collections.Generic;
using UnityEngine;

public class TrackSpawner : MonoBehaviour
{
    public static TrackSpawner Instance { get; private set; }

    [Header("Prefabs")]
    [Tooltip("The base track segment prefab.")]
    public GameObject trackPrefab;
    [Tooltip("Obstacle prefabs to spawn on the track (e.g. hurdle, tall barrier, crate, spikes).")]
    public GameObject[] obstaclePrefabs;
    [Tooltip("Collectible coin prefab.")]
    public GameObject coinPrefab;
    [Tooltip("Collectible rare gem (diamond) prefab.")]
    public GameObject gemPrefab;
    [Tooltip("Power-up pickup prefabs (Magnet, Shield, Speed, 2x, Star).")]
    public GameObject[] powerUpPrefabs;

    [Header("Track Configuration")]
    public Transform player;
    public float laneDistance = 3f;
    public float segmentLength = 30f;
    public int segmentsAhead = 6;
    public int initialSafeSegments = 2;

    private float nextSpawnZ = 0f;
    private int spawnedSegmentCount = 0;
    private readonly Queue<GameObject> activeSegments = new Queue<GameObject>();

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
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        SpawnInitialRunway();
    }

    private void Update()
    {
        if (player == null) return;

        float thresholdZ = nextSpawnZ - (segmentsAhead * segmentLength * 0.75f);
        if (player.position.z >= thresholdZ)
        {
            SpawnSegment(false);
            RecycleOldSegment();
        }
    }

    public void SpawnInitialRunway()
    {
        ClearAllSegments();
        nextSpawnZ = 0f;
        spawnedSegmentCount = 0;

        for (int i = 0; i < segmentsAhead; i++)
        {
            SpawnSegment(spawnedSegmentCount < initialSafeSegments);
        }
    }

    private void SpawnSegment(bool isSafe)
    {
        if (trackPrefab == null)
        {
            Debug.LogWarning("TrackSpawner: TrackPrefab is not assigned!");
            return;
        }

        Vector3 spawnPos = new Vector3(0f, 0f, nextSpawnZ);
        GameObject newSegment = Instantiate(trackPrefab, spawnPos, Quaternion.identity, transform);
        activeSegments.Enqueue(newSegment);

        TrackSegment segmentScript = newSegment.GetComponent<TrackSegment>();
        if (segmentScript != null)
        {
            segmentLength = segmentScript.length;

            WorldThemeData currentTheme = WorldManager.Instance != null ? WorldManager.Instance.GetCurrentTheme() : null;
            segmentScript.SetupSegment(laneDistance, obstaclePrefabs, coinPrefab, gemPrefab, powerUpPrefabs, currentTheme, isSafe);
        }

        nextSpawnZ += segmentLength;
        spawnedSegmentCount++;
    }

    private void RecycleOldSegment()
    {
        if (activeSegments.Count > segmentsAhead + 1)
        {
            GameObject oldSegment = activeSegments.Dequeue();
            if (oldSegment != null)
            {
                Destroy(oldSegment);
            }
        }
    }

    public void ClearAllSegments()
    {
        while (activeSegments.Count > 0)
        {
            GameObject seg = activeSegments.Dequeue();
            if (seg != null)
            {
                Destroy(seg);
            }
        }
    }
}
