using System;
using System.Collections.Generic;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;

namespace GalacticMeltdown.Data;

public static class KeyBindings
{
    public static Dictionary<ConsoleKey, MainControl> Main = new()
    {
        {ConsoleKey.UpArrow, MainControl.MoveUp},
        {ConsoleKey.DownArrow, MainControl.MoveDown},
        {ConsoleKey.LeftArrow, MainControl.MoveLeft},
        {ConsoleKey.RightArrow, MainControl.MoveRight},
        {ConsoleKey.D8, MainControl.MoveUp},
        {ConsoleKey.D9, MainControl.MoveNe},
        {ConsoleKey.D6, MainControl.MoveRight},
        {ConsoleKey.D3, MainControl.MoveSe},
        {ConsoleKey.D2, MainControl.MoveDown},
        {ConsoleKey.D1, MainControl.MoveSw},
        {ConsoleKey.D4, MainControl.MoveLeft},
        {ConsoleKey.D7, MainControl.MoveNw},
        {ConsoleKey.D5, MainControl.StopTurn},
        {ConsoleKey.S, MainControl.StopTurn},
        {ConsoleKey.D, MainControl.DoNothing},
        {ConsoleKey.P, MainControl.PickUpItem},
        {ConsoleKey.E, MainControl.OpenInventory},
        {ConsoleKey.A, MainControl.OpenAmmoSelection},
        {ConsoleKey.O, MainControl.InteractWithDoors},
        {ConsoleKey.C, MainControl.UseCursor},
        {ConsoleKey.U, MainControl.Shoot},
        {ConsoleKey.Multiply, MainControl.IncreaseViewRange},
        {ConsoleKey.Subtract, MainControl.ReduceViewRange},
        {ConsoleKey.Escape, MainControl.OpenPauseMenu},
        {ConsoleKey.Divide, MainControl.OpenConsole},
    };

    public static Dictionary<ConsoleKey, SelectionControl> Selection = new()
    {
        {ConsoleKey.UpArrow, SelectionControl.Up},
        {ConsoleKey.DownArrow, SelectionControl.Down},
        {ConsoleKey.LeftArrow, SelectionControl.Left},
        {ConsoleKey.RightArrow, SelectionControl.Right},
        {ConsoleKey.Enter, SelectionControl.Select},
        {ConsoleKey.Escape, SelectionControl.Back},
        {ConsoleKey.Tab, SelectionControl.SpecialAction},
    };

    public static Dictionary<ConsoleKey, CursorControl> Cursor = new()
    {
        {ConsoleKey.UpArrow, CursorControl.MoveUp},
        {ConsoleKey.DownArrow, CursorControl.MoveDown},
        {ConsoleKey.LeftArrow, CursorControl.MoveLeft},
        {ConsoleKey.RightArrow, CursorControl.MoveRight},
        {ConsoleKey.D8, CursorControl.MoveUp},
        {ConsoleKey.D9, CursorControl.MoveNe},
        {ConsoleKey.D6, CursorControl.MoveRight},
        {ConsoleKey.D3, CursorControl.MoveSe},
        {ConsoleKey.D2, CursorControl.MoveDown},
        {ConsoleKey.D1, CursorControl.MoveSw},
        {ConsoleKey.D4, CursorControl.MoveLeft},
        {ConsoleKey.D7, CursorControl.MoveNw},
        {ConsoleKey.Enter, CursorControl.Interact},
        {ConsoleKey.Escape, CursorControl.Back},
        {ConsoleKey.L, CursorControl.ToggleLine},
        {ConsoleKey.F, CursorControl.ToggleFocus},
    };

    public static Dictionary<ConsoleKey, TextInputControl> TextInput = new()
    {
        {ConsoleKey.Backspace, TextInputControl.DeleteCharacter},
        {ConsoleKey.Escape, TextInputControl.Back},
        {ConsoleKey.Enter, TextInputControl.FinishInput},
    };

    public static Dictionary<ConsoleKey, LevelMenuControl> LevelMenu = new()
    {
        {ConsoleKey.UpArrow, LevelMenuControl.SelectPrev},
        {ConsoleKey.DownArrow, LevelMenuControl.SelectNext},
        {ConsoleKey.Enter, LevelMenuControl.Start},
        {ConsoleKey.Escape, LevelMenuControl.GoBack},
        {ConsoleKey.C, LevelMenuControl.Create},
        {ConsoleKey.D, LevelMenuControl.Delete},
    };

    public static Dictionary<ConsoleKey, InventoryControl> InventoryMenu = new()
    {
        {ConsoleKey.UpArrow, InventoryControl.Up},
        {ConsoleKey.DownArrow, InventoryControl.Down},
        {ConsoleKey.LeftArrow, InventoryControl.Left},
        {ConsoleKey.RightArrow, InventoryControl.Right},
        {ConsoleKey.Enter, InventoryControl.Select},
        {ConsoleKey.Escape, InventoryControl.Back},
        {ConsoleKey.Q, InventoryControl.OpenEquipmentMenu},
        {ConsoleKey.C, InventoryControl.OpenCategorySelection},
        {ConsoleKey.Divide, InventoryControl.OpenSearchBox},
    };
}