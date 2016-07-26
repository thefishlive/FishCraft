using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using Debug = UnityEngine.Debug;
#endif

public class World : MonoBehaviour
{
    [Header("World Generation")]
    [Tooltip("Number of chunks to generate")]
    public Vector3 Chunks = new Vector3(6, 2, 6);

    [Tooltip("The world generators for this world, indexed by chunk y layer")]
    public WorldGenerator[] Generators;
    
    [Tooltip("The size of each chunk, in blocks")]
    public Vector3 ChunkSize = new Vector3(16, 16, 16);
    
    [Header("Player Spawn")]
    [Tooltip("The prefab of the player object")]
    public GameObject PlayerPrefab = default(GameObject);

    [Tooltip("The chunk to spawn the player on")]
    public Vector2 SpawnPosition = Vector2.zero;

    [Header("Rendering")]
    [Tooltip("The materials for each chunk")]
    public Material[] GroundMaterials;

    [Header("Debug")]
    [Tooltip("Cull out invisible faces in the mesh")]
    public bool OptimiseMesh = true;

    /// <summary>
    /// All of the currently loaded chunks in this world
    /// </summary>
    private Dictionary<ChunkLocation, Chunk> m_chunks;
    /// <summary>
    /// If this world is currently updating
    /// </summary>
    private bool m_running;

	// Use this for initialization
	private void Start ()
    {
#if UNITY_EDITOR
        Debug.Assert(Generators.Length == (int) Chunks.y, "Not enough generators for the chunks");
#endif

        StartCoroutine(GenerateWorld());
    }

    private IEnumerator GenerateWorld()
    {
#if UNITY_EDITOR
        var stopwatch = new Stopwatch();
        stopwatch.Start();
#endif
        m_chunks = new Dictionary<ChunkLocation, Chunk>();
        yield return new WaitForFixedUpdate();

        var spawnX = Mathf.FloorToInt(SpawnPosition.x * Chunks.x * ChunkSize.x + ChunkSize.x / 2);
        var spawnY = Mathf.FloorToInt(SpawnPosition.y * Chunks.z * ChunkSize.z + ChunkSize.z / 2);

        var spawnPos = new Vector3(
            spawnX,
            0.0f,
            spawnY
            );

        spawnPos += new Vector3(0.5f, 2f, 0.5f);

        var player = (GameObject) Instantiate(PlayerPrefab, spawnPos, Quaternion.identity);
        var chunkLoader = player.GetComponent<ChunkLoader>();

        // Setup Generators
        foreach (var generator in Generators)
        {
            generator.Setup();
        }
        
        // Generate world
        foreach (var loc in chunkLoader.GetLoadedChunks())
        {
            var chunk = CreateChunk(loc);
            m_chunks.Add(loc, chunk);
            chunk.Loaded = true;
            chunk.GenerateChunk();
        }

        yield return new WaitForFixedUpdate();

        // Build the world render mesh
        var chunks = new List<Chunk>(m_chunks.Values);
        foreach (var chunk in chunks)
        {
            chunk.BuildMesh();
        }

        yield return new WaitForFixedUpdate();

        spawnPos = new Vector3(
            spawnX,
            GetGroundLevel(spawnX, spawnY),
            spawnY
            );

        spawnPos += new Vector3(0.5f, 2f, 0.5f);
        player.transform.position = spawnPos;
        player.GetComponent<PlayerCameraController>().EnableCamera();

        m_running = true;

#if UNITY_EDITOR
        stopwatch.Stop();
        
        Debug.Log("Finished generating world in " + stopwatch.ElapsedMilliseconds + "ms");
#endif

        StartCoroutine(ChunkUpdate());
    }

    private Chunk CreateChunk(ChunkLocation loc)
    {
        var chunkObject = new GameObject("Chunk_" + loc.X + "_" + loc.Y + "_" + loc.Z);
        chunkObject.AddComponent(typeof (Chunk));
        chunkObject.transform.parent = transform;
        
        chunkObject.transform.position = new Vector3(loc.X * ChunkSize.x, loc.Y * ChunkSize.y, loc.Z * ChunkSize.y);

        var meshRenderer = chunkObject.GetComponent<MeshRenderer>();
        meshRenderer.materials = GroundMaterials;

        var chunk = chunkObject.GetComponent<Chunk>();
        chunk.WorldGenerator = Generators[loc.Y];
        chunk.OptimiseMesh = OptimiseMesh;
        chunk.ChunkSize = ChunkSize;
        chunk.World = this;
        chunk.Location = loc;
        
        return chunk;
    }

    public void BuildChunks(bool force = false)
    {
        var chunks = new List<Chunk>(m_chunks.Values);
        foreach (var chunk in chunks.Where(chunk => chunk.Loaded && (force || chunk.IsDirty())))
        {
            chunk.BuildMesh();
        }
    }

    private void CleanupChunks()
    {
        var chunks = new List<Chunk>(m_chunks.Values);
        foreach (var chunk in chunks.Where(chunk => !chunk.Loaded))
        {
            m_chunks.Remove(chunk.Location);
            Destroy(chunk.gameObject);
        }
    }

    private IEnumerator ChunkUpdate()
    {
        while (m_running)
        {
            yield return new WaitForSeconds(1);

            CleanupChunks();
            BuildChunks();
        }
    }

    public void StopUpdating()
    {
        m_running = false;
    }

    public Chunk GetChunk(BlockPos pos, bool createChunk = true)
    {
        if (!IsValid(pos))
        {
            throw new ArgumentException(string.Format("Invalid block position {0}", pos));
        }

        return GetChunk(GetChunkLocation(pos));
    }

    public Chunk GetChunk(ChunkLocation pos, bool createChunk = true)
    {
        if (!IsValid(pos))
        {
            throw new ArgumentException(string.Format("Invalid chunk position {0}", pos));
        }

        if (!m_chunks.ContainsKey(pos))
        {
            if (!createChunk)
                return null;

            var chunk = CreateChunk(pos);
            m_chunks.Add(pos, chunk);
            chunk.GenerateChunk();
            chunk.Loaded = false;
        }

        return m_chunks[pos];
    }

    public ChunkLocation GetChunkLocation(BlockPos pos)
    {
        if (!IsValid(pos))
        {
            throw new ArgumentException(string.Format("Invalid block position {0}", pos));
        }

        return new ChunkLocation
        (
            Mathf.FloorToInt(pos.X/ChunkSize.x),
            Mathf.FloorToInt(pos.Y/ChunkSize.y),
            Mathf.FloorToInt(pos.Z/ChunkSize.z)
        );
    }

    public int GetGroundLevel(int x, int z)
    {
        for (var y = (int) Chunks.y - 1; y >= 0; y--)
        {
            var level = GetChunk(new BlockPos((int)(x / ChunkSize.x), y, (int)(z / ChunkSize.z))).GetGroundLevel(x, z);

            if (level == -1)
                continue;

            return (int) ((y * ChunkSize.y) + level);
        }

        return -1;
    }

    public BlockState GetBlock(BlockPos pos, bool createChunk = true)
    {
        return GetChunk(pos, createChunk).GetBlock(pos);
    }

    public void SetBlock(BlockPos pos, BlockType block, bool createChunk = true)
    {
        GetChunk(pos, createChunk).SetBlock(pos, block);
    }

    public bool IsValid(BlockPos pos)
    {
        if (ReferenceEquals(pos, null))
        {
            return false;
        }

        return pos.Y >= 0 && pos.Y <= (ChunkSize.y * Chunks.y) - 1;
    }

    public bool IsValid(ChunkLocation pos)
    {
        return pos.Y >= 0 && pos.Y <= (Chunks.y - 1);
    }
}
