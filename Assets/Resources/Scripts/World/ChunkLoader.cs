using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkLoader : MonoBehaviour
{
    public abstract List<Chunk.ChunkLocation> GetLoadedChunks();
}
