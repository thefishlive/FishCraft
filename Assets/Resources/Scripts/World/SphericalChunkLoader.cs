using System.Collections.Generic;
using UnityEngine;

public class SphericalChunkLoader : ChunkLoader
{
    [Tooltip("The distance in chunks to load for this loader")]
    [Range(1, 16)]
    public int LoadDistance = 8;

    private World World
    {
        get
        {
            if (m_world != null)
            {
                return m_world;
            }


            var worldObj = GameObject.FindWithTag("World");

            if (worldObj == null)
            {
                Debug.LogError("Could not find world game object");
                return null;
            }

            return m_world = worldObj.GetComponent<World>();
        }
    }

    private World m_world;
    
    public override List<Chunk.ChunkLocation> GetLoadedChunks()
    {
        var chunks = new List<Chunk.ChunkLocation>();
        var loc = World.GetChunkLocation(new BlockPos(transform.position));

        float distanceSqr = LoadDistance*LoadDistance;

        for (var x = loc.X - LoadDistance; x <= loc.X + LoadDistance; x++)
        {
            for (var z = loc.Z - LoadDistance; z <= loc.Z + LoadDistance; z++)
            {
                if (!((loc.X - x)*(loc.X - x) + (loc.Z - z)*(loc.Z - z) <= distanceSqr)) continue;

                for (var y = 0; y < m_world.Chunks.y; y++)
                {
                    chunks.Add(new Chunk.ChunkLocation { X = x, Y = y, Z = z });
                }
            }
        }

        return chunks;
    }
}
