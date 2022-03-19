using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Launchers;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public class MainMenu
{
    private LineView _lineView;
    private Controller _controller;
    
    public MainMenu()
    {
        _lineView = new LineView();
        _lineView.SetLines(new List<ListLine>()
        {
            new Button("Select level", "", OpenLevelMenu),
            new Button("Quit", "", Game.Quit)
        });
        _controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Down, _lineView.SelectNext},
                {SelectionControl.Up, _lineView.SelectPrev},
                {SelectionControl.Select, _lineView.PressCurrent}
            }));
    }

    public void Open()
    {
        UserInterface.SetView(this, _lineView);
        UserInterface.SetController(this, _controller);
        UserInterface.TakeControl(this);
    }

    public void Close()
    {
        UserInterface.Forget(this);
    }

    private void OpenLevelMenu()
    {
        
    }
}