using UnityEngine;

public class CaveWorldGenerator : WorldGenerator
{
    private static readonly Perlin s_perlin = new Perlin();

    [Header("World Generation Settings")]
    [Tooltip("The x scale for the world generator")]
    [Range(0, 1.0f)]
    public float XScale = 1.0f;

    [Tooltip("The y scale for the world generator")]
    [Range(0, 1.0f)]
    public float YScale = 1.0f;

    [Tooltip("The y scale for the world generator")]
    [Range(0, 1.0f)]
    public float ZScale = 1.0f;

    [Tooltip("The x size for the world generator")]
    [Range(0, 128.0f)]
    public float XSize = 16.0f;

    [Tooltip("The y size for the world generator")]
    [Range(0, 128.0f)]
    public float YSize = 16.0f;

    [Tooltip("The y size for the world generator")]
    [Range(0, 128.0f)]
    public float ZSize = 16.0f;

    [Tooltip("The number of octaves of perlin noise to use")]
    [Range(1, 16)]
    public int Octaves = 1;

    [Tooltip("The the persistence of the perlin noise")]
    [Range(1, 16)]
    public double Persistence = 1;

    [Tooltip("The threashold for creating a cave")]
    [Range(0, 1)]
    public float CaveThreshold = 0.5f;

    [Header("Debug")]
    [Tooltip("Invert world generation (caves are solid)")]
    public bool InvertGeneration = false;

    private float m_xSeed;
    private float m_ySeed;
    private float m_zSeed;

    public override BlockType GetBlockType(BlockPos pos)
    {
        if (pos.Y == 0)
            return BlockType.Bedrock;

        var noise = s_perlin.OctavePerlin(m_xSeed + (pos.X / XSize) * XScale, m_ySeed + (pos.Y / YSize) * YScale, m_zSeed + (pos.Z / ZSize) * ZScale, Octaves, Persistence);
        return noise > CaveThreshold ? (InvertGeneration ? BlockType.Air : BlockType.Stone) : (InvertGeneration ? BlockType.Stone : BlockType.Air);
    }

    public override void Setup()
    {
        m_xSeed = Random.Range(0, 10000.0f);
        m_ySeed = Random.Range(0, 10000.0f);
        m_zSeed = Random.Range(0, 10000.0f);
    }
}
