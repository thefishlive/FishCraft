using InControl;

public class PlayerMovementControls : PlayerActionSet
{
    public PlayerAction MoveForward { get; private set; }
    public PlayerAction MoveBackwards { get; private set; }
    public PlayerAction MoveLeft { get; private set; }
    public PlayerAction MoveRight { get; private set; }
    public PlayerTwoAxisAction Move { get; private set; }

    public PlayerAction LookUp { get; private set; }
    public PlayerAction LookDown { get; private set; }
    public PlayerAction LookLeft { get; private set; }
    public PlayerAction LookRight { get; private set; }
    public PlayerTwoAxisAction Look { get; private set; }

    public PlayerAction Jump { get; private set; }

    public PlayerMovementControls()
    {
        MoveForward = CreatePlayerAction("input.movement.foward");
        MoveBackwards = CreatePlayerAction("input.movement.backwards");
        MoveLeft = CreatePlayerAction("input.movement.left");
        MoveRight = CreatePlayerAction("input.movement.right");
        Move = CreateTwoAxisPlayerAction(MoveLeft, MoveRight, MoveBackwards, MoveForward);

        LookUp = CreatePlayerAction("input.look.up");
        LookDown = CreatePlayerAction("input.look.down");
        LookLeft = CreatePlayerAction("input.look.left");
        LookRight = CreatePlayerAction("input.look.right");
        Look = CreateTwoAxisPlayerAction(LookRight, LookLeft, LookUp, LookDown);
        
        Jump = CreatePlayerAction("input.movement.jump");

        SetupKeyboardMappings();
    }

    private void SetupKeyboardMappings()
    {
        MoveForward.AddDefaultBinding(Key.W);
        MoveBackwards.AddDefaultBinding(Key.S);
        MoveLeft.AddDefaultBinding(Key.A);
        MoveRight.AddDefaultBinding(Key.D);

        LookUp.AddDefaultBinding(Mouse.PositiveY);
        LookDown.AddDefaultBinding(Mouse.NegativeY);
        LookLeft.AddDefaultBinding(Mouse.PositiveX);
        LookRight.AddDefaultBinding(Mouse.NegativeX);

        Jump.AddDefaultBinding(Key.Space);
    }
}
