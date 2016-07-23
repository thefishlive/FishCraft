using System;
using UnityEngine;

public class BlockType
{
    private const float c_textureConstant = 32/512f;

    public static readonly BlockType Air = new BlockType("blocks.air");
    public static readonly BlockType Grass = new BlockType("blocks.grass", false, new []
    {
        new Vector2(0, 1 - (1 * c_textureConstant)),
        new Vector2(0, 1 - (1 * c_textureConstant)),
        new Vector2(0, 1 - (0 * c_textureConstant)),
        new Vector2(0, 1 - (2 * c_textureConstant)),
        new Vector2(0, 1 - (1 * c_textureConstant)),
        new Vector2(0, 1 - (1 * c_textureConstant))
    });
    public static readonly BlockType Dirt = new BlockType("blocks.dirt", false, new[]
    {
        new Vector2(0, 1 - (2 * c_textureConstant)),
    });
    public static readonly BlockType Stone = new BlockType("blocks.stone", false, new[]
    {
        new Vector2(0, 1 - (3 * c_textureConstant)),
    });
    public static readonly BlockType Wood = new BlockType("blocks.wood", false, new[]
    {
        new Vector2(0, 1 - (3 * c_textureConstant)),
    });
    public static readonly BlockType Door = new BlockType("blocks.door", false, new[]
    {
        new Vector2(0, 1 - (4 * c_textureConstant)),
    });

    public string Name { get; private set; }
    public Vector2[] Uvs { get; private set; }
    public bool Transparent { get; private set; }
    public Type StateType { get; private set; }
    
    public BlockType(string name, bool isTransparent = true, Vector2[] uvs = null, Type stateType = null)
    {
        Name = name;
        Uvs = uvs;
        Transparent = isTransparent;

        if (stateType == null)
            stateType = typeof (BlockState);
        else if (stateType.IsSubclassOf(typeof(BlockState)))
            throw new ArgumentException("State type must dirive from BlockState");

        StateType = stateType;
    }

    public Vector2 this[Direction direction]
    {
        get
        {
            if (Uvs == null || Uvs.Length == 0)
            {
                return Vector2.zero;
            }

            return Uvs.Length == 1 ? Uvs[0] : Uvs[(int) direction];
        }
    }

    public BlockState CreateState()
    {
        return Activator.CreateInstance(StateType, this) as BlockState;
    }
}
