using System;
using UnityEngine;

[Serializable]
public class WorldThemeData
{
    public WorldType worldType;
    public string displayName;

    [Header("Atmosphere & Lighting")]
    public Color skyColor = new Color(0.2f, 0.4f, 0.6f);
    public Color fogColor = new Color(0.3f, 0.5f, 0.7f);
    [Range(0.001f, 0.05f)]
    public float fogDensity = 0.015f;
    public Color lightColor = Color.white;
    [Range(0.5f, 2f)]
    public float lightIntensity = 1f;

    [Header("Materials")]
    public Material trackMaterial;
    public Material sideGroundMaterial;
    public Material laneDividerMaterial;
    public Material hurdleMaterial;
    public Material barrierMaterial;
    public Material coinMaterial;

    [Header("Side Decorations (Left & Right Flanks)")]
    public GameObject[] sidePropPrefabs;

    [Header("World-Specific Obstacles")]
    public GameObject[] customObstaclePrefabs;

    [Header("Enhanced Environment")]
    [Tooltip("Additional smaller detail props (grass tufts, pebbles, flowers, etc.)")]
    public GameObject[] detailPropPrefabs;
    [Tooltip("Overhead decoration prefabs (arches, wires, clouds, etc.)")]
    public GameObject[] overheadPropPrefabs;
    [Tooltip("Ground texture overlay or pattern prefab on side ground")]
    public Material sideDetailMaterial;
    [Range(0f, 1f)]
    public float detailDensity = 0.5f;
    [Range(0f, 1f)]
    public float overheadDensity = 0.3f;

    [Header("Environment Particles")]
    [Tooltip("Ambient particle color (leaves, sand, snow, sparks, etc.)")]
    public Color particleColor = Color.clear;
    public bool hasAmbientParticles = false;
}
