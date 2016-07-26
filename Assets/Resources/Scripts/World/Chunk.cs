using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    public const int BlockAir = 0;
    
    public struct ChunkLocation
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    }
    
    public World World { get; set; }
    public ChunkLocation Location { get; set; }
    public bool Loaded { get; set; }

    [Header("World Generation")]
    [Tooltip("The world generator for this chunk")]
    public WorldGenerator WorldGenerator;

    [Tooltip("The size of each chunk, in blocks")]
    public Vector3 ChunkSize = new Vector3(16, 16, 16);

    [Header("Debug")]
    [Tooltip("Cull out invisible faces in the mesh")]
    public bool OptimiseMesh = true;
    
    private BlockState[,,] m_blocks;
    private bool m_dirty;

    private Mesh m_mesh;

    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private MeshCollider m_meshCollider;

    // Use this for initialization
    private void Start ()
    {
        gameObject.isStatic = true;
	}

    public void MarkDirty()
    {
        m_dirty = true;
    }

    public bool IsDirty()
    {
        return m_dirty;
    }

    public Bounds GetBounds()
    {
        return new Bounds(transform.position + ChunkSize / 2, ChunkSize);
    }

    public int GetGroundLevel(int x, int z)
    {
        for (var y = (int) ChunkSize.y - 1; y >= 0; y--)
        {
            if (m_blocks[x, y, z].Type.Transparent)
            {
                continue;
            }

            return y;
        }

        return -1;
    }

    public void GenerateChunk()
    {
        m_blocks = new BlockState[(int)ChunkSize.x, (int)ChunkSize.y, (int)ChunkSize.z];

        for (var x = 0; x < (int)ChunkSize.x; x++)
        {
            for (var y = 0; y < (int)ChunkSize.y; y++)
            {
                for (var z = 0; z < (int)ChunkSize.z; z++)
                {
                    m_blocks[x, y, z] = WorldGenerator.GetBlockType(ToWorldPosition(new BlockPos(x, y, z))).CreateState();
                }
            }
        }

        MarkDirty();
    }

    public void BuildMesh()
    {
        if (m_blocks == null)
        {
            throw new UnityException("Could not create mesh for chunk that hasn't generated yet");
        }

        m_dirty = false;

        var verticies = new List<Vertex>();
        var indicies = new List<int>();

        var i = 0;

        for (var x = 0; x < (int)ChunkSize.x; x++)
        {
            for (var y = 0; y < (int)ChunkSize.y; y++)
            {
                for (var z = 0; z < (int)ChunkSize.z; z++)
                {
                    var block = m_blocks[x, y, z];

                    if (block.Type.Transparent)
                    {
                        continue;
                    }

                    var model = block.GetModel();
                    model.BuildMesh(World, this, new BlockPos(x, y, z), ref i, ref verticies, ref indicies);
                }
            }
        }

        m_mesh = new Mesh();
        m_mesh.MarkDynamic();

        m_mesh.vertices = verticies.Select(vertex => vertex.Pos).ToArray();
        m_mesh.tangents = verticies.Select(vertex => vertex.Tangent).ToArray();
        m_mesh.uv = verticies.Select(vertex => vertex.Uv).ToArray();
        m_mesh.triangles = indicies.ToArray();
        
        if (OptimiseMesh)
        {
            m_mesh.Optimize();
        }

        m_mesh.RecalculateNormals();
        m_mesh.RecalculateBounds();

        if (m_meshFilter == null)
        {
            m_meshFilter = GetComponent<MeshFilter>();
        }
        if (m_meshCollider == null)
        {
            m_meshCollider = GetComponent<MeshCollider>();
        }
        m_meshFilter.mesh = m_mesh;
        m_meshCollider.sharedMesh = m_mesh;
    }

    public BlockPos ToWorldPosition(BlockPos pos)
    {
        return new BlockPos(
            pos.X + (int) (Location.X * ChunkSize.x),
            pos.Y + (int) (Location.Y * ChunkSize.y),
            pos.Z + (int) (Location.Z * ChunkSize.z)
            );
    }

    public BlockPos ToLocalPosition(BlockPos pos)
    {
        return new BlockPos(
            pos.X - (int)(Location.X * ChunkSize.x),
            pos.Y - (int)(Location.Y * ChunkSize.y),
            pos.Z - (int)(Location.Z * ChunkSize.z)
            );
    }

    private BlockState GetWorldBlock(BlockPos local)
    {
        // If not local
        if (local.X < 0 || local.X > ChunkSize.x - 1 || local.Y < 0 || local.Y > ChunkSize.y - 1 || local.Z < 0 || local.Z > ChunkSize.z - 1)
        {
            return World.GetBlock(ToWorldPosition(local));
        }

        return m_blocks[local.X, local.Y, local.Z];
    }

    public BlockState GetBlock(BlockPos pos)
    {
        var local = ToLocalPosition(pos);
        return m_blocks[local.X, local.Y, local.Z];
    }

    public void SetBlock(BlockPos pos, BlockType block)
    {
        var local = ToLocalPosition(pos);
        m_blocks[local.X, local.Y, local.Z] = block.CreateState();

        if (local.X == 0)
            World.GetChunk(pos + new BlockPos(-1, 0, 0)).MarkDirty();
        else if (local.X == (int) ChunkSize.x - 1)
            World.GetChunk(pos + new BlockPos(1, 0, 0)).MarkDirty();

        if (local.Y == 0)
            World.GetChunk(pos + new BlockPos(0, -1, 0)).MarkDirty();
        else if (local.Y == (int)ChunkSize.y - 1)
            World.GetChunk(pos + new BlockPos(0, 1, 0)).MarkDirty();

        if (local.Z == 0)
            World.GetChunk(pos + new BlockPos(0, 0, -1)).MarkDirty();
        else if (local.Z == (int)ChunkSize.z - 1)
            World.GetChunk(pos + new BlockPos(0, 0, 1)).MarkDirty();

        MarkDirty();
        World.BuildChunks();
    }
}
