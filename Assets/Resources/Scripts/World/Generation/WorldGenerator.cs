using UnityEngine;

public abstract class WorldGenerator : MonoBehaviour
{
    public abstract void Setup();

    public abstract BlockType GetBlockType(BlockPos pos);
}
