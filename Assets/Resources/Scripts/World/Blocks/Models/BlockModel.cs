using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BlockModel : Model
{
    private static readonly Vector2 s_topLeft = Vector2.zero;
    private static readonly Vector2 s_topRight = new Vector2(32f / 512f, 0);
    private static readonly Vector2 s_bottomLeft = new Vector2(0, -32f / 512f);
    private static readonly Vector2 s_bottomRight = new Vector2(32f / 512f, -32f / 512f);

    public BlockModel(BlockState state) : base(state)
    {
    }

    public override void BuildMesh(World world, Chunk chunk, BlockPos pos, ref int id, ref List<Vertex> verticies, ref List<int> indicies)
    {
        var worldPos = chunk.ToWorldPosition(pos);
        var blockType = world.GetBlock(worldPos).Type;
        var i = id;


        // Bottom Face
        if (!chunk.OptimiseMesh || (pos.Y == 0 && chunk.Location.Y == 0) || world.GetBlock(worldPos + new BlockPos(0, -1, 0)).Type.Transparent)
        {
            verticies.AddRange(new[]
            {
                new Vertex { Pos = new Vector3(pos.X, pos.Y, pos.Z), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Down] },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y, pos.Z), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Down] + s_topRight },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y, pos.Z+1), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Down] + s_bottomRight },
                new Vertex { Pos = new Vector3(pos.X, pos.Y, pos.Z+1), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Down] + s_bottomLeft },
            });

            indicies.AddRange(new[]
            {
                0, 1, 2,
                0, 2, 3,
            }.Select(index => i + index));
            i += 4;
        }

        // Top Face
        if (!chunk.OptimiseMesh || (pos.Y == (int)world.ChunkSize.y - 1 && chunk.Location.Y == (int)world.Chunks.y - 1) || world.GetBlock(worldPos + new BlockPos(0, 1, 0)).Type.Transparent)
        {
            verticies.AddRange(new[]
            {
                new Vertex { Pos = new Vector3(pos.X, pos.Y+1, pos.Z), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Up] },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y+1, pos.Z), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Up] + s_topRight },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y+1, pos.Z+1), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Up] + s_bottomRight },
                new Vertex { Pos = new Vector3(pos.X, pos.Y+1, pos.Z+1), Tangent = new Vector4(1f, 0f, 0f, -1f), Uv = blockType[Direction.Up] + s_bottomLeft },
                });

            indicies.AddRange(new[]
            {
                0, 2, 1,
                0, 3, 2,
            }.Select(index => i + index));
            i += 4;
        }

        // Left Face
        if (!chunk.OptimiseMesh || (pos.X == 0 && chunk.Location.X == 0) || world.GetBlock(worldPos + new BlockPos(-1, 0, 0)).Type.Transparent)
        {
            verticies.AddRange(new[]
            {
                new Vertex { Pos = new Vector3(pos.X, pos.Y, pos.Z), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Left] + s_bottomLeft },
                new Vertex { Pos = new Vector3(pos.X, pos.Y+1, pos.Z), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Left] + s_topRight },
                new Vertex { Pos = new Vector3(pos.X, pos.Y, pos.Z+1), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Left] + s_bottomRight },
                new Vertex { Pos = new Vector3(pos.X, pos.Y+1, pos.Z+1), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Left] + s_topLeft },
            });

            indicies.AddRange(new[]
            {
                0, 3, 1,
                0, 2, 3,
            }.Select(index => i + index));
            i += 4;
        }

        // Right Face
        if (!chunk.OptimiseMesh || (pos.X == (int)world.ChunkSize.x - 1 && chunk.Location.X == (int)world.Chunks.x - 1) || world.GetBlock(worldPos + new BlockPos(1, 0, 0)).Type.Transparent)
        {
            verticies.AddRange(new[]
            {
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y, pos.Z), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Right] + s_bottomRight },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y+1, pos.Z), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Right] + s_topLeft },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y, pos.Z+1), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Right] + s_bottomLeft },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y+1, pos.Z+1), Tangent = new Vector4(0f, 0f, 1f, -1f), Uv = blockType[Direction.Right] + s_topRight },
            });

            indicies.AddRange(new[]
            {
                0, 3, 2,
                0, 1, 3,
            }.Select(index => i + index));
            i += 4;
        }

        // Front Face
        if (!chunk.OptimiseMesh || (pos.Z == 0 && chunk.Location.Z == 0) || world.GetBlock(worldPos + new BlockPos(0, 0, -1)).Type.Transparent)
        {
            verticies.AddRange(new[]
            {
                new Vertex { Pos = new Vector3(pos.X, pos.Y, pos.Z), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Forward] + s_bottomLeft },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y, pos.Z), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Forward] + s_bottomRight },
                new Vertex { Pos = new Vector3(pos.X, pos.Y+1, pos.Z), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Forward] },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y+1, pos.Z), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Forward] + s_topRight },
            });

            indicies.AddRange(new[]
            {
                0, 3, 1,
                0, 2, 3,
            }.Select(index => i + index));
            i += 4;
        }

        // Back Face
        if (!chunk.OptimiseMesh || (pos.Z == (int)world.ChunkSize.z - 1 && chunk.Location.Z == (int)world.Chunks.z - 1) || world.GetBlock(worldPos + new BlockPos(0, 0, 1)).Type.Transparent)
        {
            verticies.AddRange(new[]
            {
                new Vertex { Pos = new Vector3(pos.X, pos.Y, pos.Z+1), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Backwards] + s_bottomLeft },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y, pos.Z+1), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Backwards] + s_bottomRight},
                new Vertex { Pos = new Vector3(pos.X, pos.Y+1, pos.Z+1), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Backwards] },
                new Vertex { Pos = new Vector3(pos.X+1, pos.Y+1, pos.Z+1), Tangent = new Vector4(0f, 1f, 0f, -1f), Uv = blockType[Direction.Backwards] + s_topRight },
            });

            indicies.AddRange(new[]
            {
                1, 2, 0,
                1, 3, 2
            }.Select(index => i + index));
            i += 4;
        }

        id = i;
    }
}
