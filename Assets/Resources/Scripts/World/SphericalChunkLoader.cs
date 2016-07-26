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
    
    public override List<ChunkLocation> GetLoadedChunks()
    {
        var chunks = new List<ChunkLocation>();
        var pos = new BlockPos(transform.position);

        if (!World.IsValid(pos))
        {
            return chunks;
        }

        var loc = World.GetChunkLocation(pos);

        float distanceSqr = LoadDistance*LoadDistance;

        for (var x = loc.X - LoadDistance; x <= loc.X + LoadDistance; x++)
        {
            for (var z = loc.Z - LoadDistance; z <= loc.Z + LoadDistance; z++)
            {
                for (var y = 0; y < m_world.Chunks.y; y++)
                {
                    if ((loc.X - x)*(loc.X - x) + (loc.Y - y)*(loc.Y - y) + (loc.Z - z)*(loc.Z - z) <= distanceSqr)
                    {
                        chunks.Add(new ChunkLocation(x, y, z));
                    }
                }
            }
        }

        return chunks;
    }
}
