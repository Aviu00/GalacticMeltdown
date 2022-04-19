using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class PauseMenu : TextWindow
{
    public PauseMenu(Action saveAndQuit)
    {
        LineView.SetLines(new List<ListLine>
        {
            new Button("Back", "", Close),
            new Button("Help", "", OpenHelp),
            new Button("To main menu", "", saveAndQuit)
        });
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>()
        {
            {SelectionControl.Back, Close},
            {SelectionControl.Down, LineView.SelectNext},
            {SelectionControl.Up, LineView.SelectPrev},
            {SelectionControl.Select, LineView.PressCurrent}
        }));
    }
    
    private void OpenHelp()
    {
        InfoWindow infoWindow = new(DataHolder.InfoLines);
        UserInterface.AddChild(this, infoWindow);
        infoWindow.Open();
    }
}