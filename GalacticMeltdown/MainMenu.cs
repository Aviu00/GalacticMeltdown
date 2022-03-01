using System;
using System.Collections.Generic;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public class MainMenu
{
    private ButtonListView _buttons;
    
    public void Start()
    {
        InputProcessor.AddBinding(Data.Data.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Up, _buttons.SelectPrev},
            {SelectionControl.Down, _buttons.SelectNext},
            {SelectionControl.Select, _buttons.PressCurrent}
        });
    }
}