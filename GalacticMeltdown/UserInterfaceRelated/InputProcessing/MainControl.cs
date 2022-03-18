namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing;

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
    InteractWithDoors,
    UseCursor,
    IncreaseViewRange, //for fov testing
    ReduceViewRange, //for fov testing
    ToggleNoClip, //temporary cheat codes
    ToggleXRay, //temporary cheat codes
    Quit
}