using InControl;

public class PlayerInteractionControls : PlayerActionSet
{
    public PlayerAction Dig { get; private set; }
    public PlayerAction Place { get; private set; }

    public PlayerInteractionControls()
    {
        Dig = CreatePlayerAction("input.interaction.dig");
        Place = CreatePlayerAction("input.interaction.place");

        SetupKeyboardMappings();
    }

    private void SetupKeyboardMappings()
    {
        Dig.AddDefaultBinding(Mouse.LeftButton);
        Place.AddDefaultBinding(Mouse.RightButton);
    }
}
