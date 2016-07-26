using System;
using System.Collections.Generic;
using InControl;
using Debug = UnityEngine.Debug;

public class ControlsManager
{
    private static readonly Dictionary<Type, PlayerActionSet> s_actionSets = new Dictionary<Type, PlayerActionSet>();

    static ControlsManager()
    {
        RegisterControls(typeof(PlayerMovementControls));
        RegisterControls(typeof(PlayerInteractionControls));
    }
    
    private ControlsManager() { }

    public static void RegisterControls(Type type)
    {
        if (!type.IsSubclassOf(typeof (PlayerActionSet)))
        {
            throw new ArgumentException("Type " + type.Name + " is not a subclass of PlayerActionSet");
        }

        s_actionSets.Add(type, Activator.CreateInstance(type) as PlayerActionSet);
    }

    public static T GetActionSet<T>() where T : PlayerActionSet
    {
        return (T) s_actionSets[typeof(T)];
    }

    public static ICollection<PlayerActionSet> GetActionSets()
    {
        return s_actionSets.Values;
    } 
}
