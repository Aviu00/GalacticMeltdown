using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class MainMenu : TextWindow
{
    public MainMenu()
    {
        LineView.SetLines(new List<ListLine>
        {
            new Button("Select level", "", OpenLevelMenu),
            new Button("Help", "", OpenKeyHelp),
            new Button("Quit", "", Game.Quit)
        });
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.Select, LineView.PressCurrent}
            }));
    }

    private void OpenLevelMenu()
    {
        LevelMenu levelMenu = new();
        UserInterface.AddChild(this, levelMenu);
        levelMenu.Open();
    }

    private void OpenKeyHelp()
    {
        InfoWindow infoWindow = new(new List<string>
        {
            "Galactic Meltdown",
            "This game is turn-based: every entity has a set amount of energy that it can use to make one or several " 
            + "turns in one \"game step\". At the end of each game step all entities restore their energy.",
            "The amount restored is the maximum possible amount available to the entity.",
            "",
            "Key bindings",
            "Use 1-9 or arrows to move, hit, or open doors",
            "Use Enter to select a cell using the cursor",
            "In game:",
            "    Esc: open pause menu",
            "    O: get a cursor for opening doors",
            "    P: get a cursor for picking up items",
            "    U: get a cursor to shoot",
            "    D: skip a turn",
            "    S: stop until the end of this game step",
            "    E: open inventory",
            "    A: open ammo selection menu",
            "    /: open console",
        });
        UserInterface.AddChild(this, infoWindow);
        infoWindow.Open();
    }
}