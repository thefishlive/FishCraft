using System;
using System.Collections;
using System.Diagnostics;
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

    private Chunk[,,] m_chunks;

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
        m_chunks = new Chunk[(int)Chunks.x, (int)Chunks.y, (int)Chunks.z];

        // Setup Generators
        foreach (var generator in Generators)
        {
            generator.Setup();
        }

        // Generate world
        for (var x = 0; x < (int)Chunks.x; x++)
        {
            for (var y = 0; y < (int) Chunks.y; y++)
            {
                for (var z = 0; z < (int)Chunks.z; z++)
                {
                    var chunk = m_chunks[x, y, z] = CreateChunk(x, y, z);
                    chunk.GenerateChunk();
                }
            }
        }

        yield return new WaitForFixedUpdate();

        // Build the world render mesh
        for (var x = 0; x < (int)Chunks.x; x++)
        {
            for (var y = 0; y < (int)Chunks.y; y++)
            {
                for (var z = 0; z < (int)Chunks.z; z++)
                {
                    m_chunks[x, y, z].BuildMesh();
                }
            }
        }

        yield return new WaitForFixedUpdate();

        var spawnX = Mathf.FloorToInt(SpawnPosition.x * Chunks.x * ChunkSize.x + ChunkSize.x / 2);
        var spawnY = Mathf.FloorToInt(SpawnPosition.y * Chunks.z * ChunkSize.z + ChunkSize.z / 2);
        
        var spawnPos = new Vector3(
            spawnX,
            GetGroundLevel(spawnX, spawnY),
            spawnY
            );

        spawnPos += new Vector3(0.5f, 2f, 0.5f);
        Instantiate(PlayerPrefab, spawnPos, Quaternion.identity);

        m_running = true;

#if UNITY_EDITOR
        stopwatch.Stop();
        
        Debug.Log("Finished generating world in " + stopwatch.ElapsedMilliseconds + "ms");
#endif

        StartCoroutine(ChunkUpdate());
    }

    private Chunk CreateChunk(int x, int y, int z)
    {
        var chunkObject = new GameObject("Chunk_" + x + "_" + y + "_" + z);
        //chunkObject.SetActive(false);
        chunkObject.AddComponent(typeof (Chunk));
        chunkObject.transform.parent = transform;
        
        chunkObject.transform.position = new Vector3(x * ChunkSize.x, y * ChunkSize.y, z * ChunkSize.y);

        var meshRenderer = chunkObject.GetComponent<MeshRenderer>();
        meshRenderer.materials = GroundMaterials;

        var chunk = chunkObject.GetComponent<Chunk>();
        chunk.WorldGenerator = Generators[y];
        chunk.OptimiseMesh = OptimiseMesh;
        chunk.ChunkSize = ChunkSize;
        chunk.World = this;
        chunk.Location = new Chunk.ChunkLocation {X = x, Y = y, Z = z};
        
        return chunk;
    }

    public void BuildChunks(bool force = false)
    {
        for (var x = 0; x < (int)Chunks.x; x++)
        {
            for (var y = 0; y < (int)Chunks.y; y++)
            {
                for (var z = 0; z < (int)Chunks.z; z++)
                {
                    if (force || m_chunks[x, y, z].IsDirty())
                    {
                        m_chunks[x, y, z].BuildMesh();
                    }
                }
            }
        }
    }

    private IEnumerator ChunkUpdate()
    {
        while (m_running)
        {
            BuildChunks();

            yield return new WaitForSeconds(1);
        }
    }

    public void StopUpdating()
    {
        m_running = false;
    }

    public Chunk GetChunk(BlockPos pos)
    {
        // TODO remove when we get infinite world gen
        if (!IsValid(pos))
        {
            throw new ArgumentException(string.Format("Invalid coordinate {0}", pos));
        }
        
        return m_chunks[(int) (pos.X / ChunkSize.x), (int)(pos.Y / ChunkSize.y), (int)(pos.Z / ChunkSize.z)];
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

    public BlockState GetBlock(BlockPos pos)
    {
        return GetChunk(pos).GetBlock(pos);
    }

    public void SetBlock(BlockPos pos, BlockType block)
    {
        GetChunk(pos).SetBlock(pos, block);
    }

    public bool IsValid(BlockPos pos)
    {
        return !(pos.X < 0 || pos.X > Chunks.x*ChunkSize.x - 1 || pos.Y < 0 || pos.Y > Chunks.y*ChunkSize.y - 1 || pos.Z < 0 || pos.Z > Chunks.z*ChunkSize.z - 1);
    }
}
