using UnityEngine;
using System.Collections.Generic;

public abstract class Model
{
    protected readonly BlockState State;

    protected Model(BlockState state)
    {
        State = state;
    }

    public abstract void BuildMesh(World world, Chunk chunk, BlockPos pos, ref int id, ref List<Vertex> verticies, ref List<int> triangles);
}
