namespace GalacticMeltdown.InputProcessing;

public enum PlayerControl
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
    IncreaseViewRange, //for fov testing
    ReduceViewRange, //for fov testing
    ActivateNoClip, //temporary cheat codes
    ActivateXRay, //temporary cheat codes
    Quit
}