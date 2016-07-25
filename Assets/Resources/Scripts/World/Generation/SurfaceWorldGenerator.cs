using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SurfaceWorldGenerator : WorldGenerator
{
    private static readonly Perlin s_perlin = new Perlin();

    [Header("World Generation Settings")]
    [Tooltip("The height scale for the world generator")]
    [Range(0, 32.0f)]
    public float HeightScale = 16.0f;

    [Tooltip("The height exponential for the world generator")]
    [Range(0.0f, 5.0f)]
    public float HeightExponential = 1.0f;

    [Tooltip("The surface level of the world")]
    [Range(0, 128.0f)]
    public int SurfaceLevel = 48;

    [Tooltip("The x scale for the world generator")]
    [Range(0, 8.0f)]
    public float XScale = 1.0f;

    [Tooltip("The y scale for the world generator")]
    [Range(0, 8.0f)]
    public float ZScale = 1.0f;

    [Tooltip("The x size for the world generator")]
    [Range(0, 128.0f)]
    public float XSize = 16.0f;

    [Tooltip("The y size for the world generator")]
    [Range(0, 128.0f)]
    public float ZSize = 16.0f;

    [Tooltip("The number of octaves of perlin noise to use")]
    [Range(1, 16)]
    public int Octaves = 1;

    [Tooltip("The the persistence of the perlin noise")]
    [Range(1, 16)]
    public double Persistence = 1;
    
    private float m_xSeed;
    private float m_zSeed;

    public override BlockType GetBlockType(BlockPos pos)
    {
        var height = GetHeightAtLocation(pos.X, pos.Z);

        if (height < pos.Y - SurfaceLevel)
        {
            return BlockType.Air;
        }

        return height - 1 < pos.Y - SurfaceLevel ? BlockType.Grass : BlockType.Dirt;
    }

    private double GetHeightAtLocation(int x, int z)
    {
        var noise = s_perlin.OctavePerlin(m_xSeed + x / XSize * XScale, 1.0f, m_zSeed + z / ZSize * ZScale, Octaves, Persistence);
        return Math.Pow(HeightScale * noise, HeightExponential);
    }

    public override void Setup()
    {
        m_xSeed = Random.Range(0, 10000.0f);
        m_zSeed = Random.Range(0, 10000.0f);

        Debug.Log(string.Format("X Seed: {0}, Z Seed: {1}", m_xSeed, m_zSeed));
    }
}
