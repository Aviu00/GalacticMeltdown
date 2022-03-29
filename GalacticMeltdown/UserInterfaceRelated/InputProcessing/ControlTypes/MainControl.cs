namespace GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;

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
    PickUpItem,
    OpenInventory,
    InteractWithDoors,
    UseCursor,
    Shoot,
    IncreaseViewRange, //for fov testing
    ReduceViewRange, //for fov testing
    ToggleNoClip, //temporary cheat codes
    ToggleXRay, //temporary cheat codes
    OpenPauseMenu,
    OpenAmmoSelection,
    OpenConsole,
}