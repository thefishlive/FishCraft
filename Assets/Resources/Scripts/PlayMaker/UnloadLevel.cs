using HutongGames.PlayMaker;
using UnityEngine.SceneManagement;


[ActionCategory(ActionCategory.Level)]
[Tooltip("Unloads a Level by Name. NOTE: Before you can unload a level, you have to add it to the list of levels defined in File->Build Settings...")]
public class UnloadLevel : FsmStateAction
{
    [RequiredField]
    [Tooltip("The name of the level to load. NOTE: Must be in the list of levels defined in File->Build Settings... ")]
    public FsmString LevelName;
    
    [Tooltip("Event to send when the level has loaded. NOTE: This only makes sense if the FSM is still in the scene!")]
    public FsmEvent LoadedEvent;
    
    public override void OnEnter()
    {
        SceneManager.UnloadScene(LevelName.Value);

        Fsm.Event(LoadedEvent);
        Finish();
    }
}
