
using System;
using System.Collections.Generic;

public class BlockState
{
    private static readonly Dictionary<Type, Model> s_modelTypes = new Dictionary<Type, Model>();

    public BlockType Type { get; set; }

    public BlockState(BlockType type)
    {
        Type = type;
    }

    public Model GetModel()
    {
        if (s_modelTypes.ContainsKey(Type.ModelType)) return s_modelTypes[Type.ModelType];
        var model = (Model) Activator.CreateInstance(Type.ModelType, this);
        s_modelTypes.Add(Type.ModelType, model);
        return model;
    }
}
