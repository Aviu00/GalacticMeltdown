namespace GalacticMeltdown.InputProcessing;

public enum MainControl
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    MoveNe,
    MoveSe,
    MoveSw,
    MoveNw,
    StopTurn,
    DoNothing,
    UseCursor,
    IncreaseViewRange, //for fov testing
    ReduceViewRange, //for fov testing
    ToggleNoClip, //temporary cheat codes
    ToggleXRay, //temporary cheat codes
    Quit
}