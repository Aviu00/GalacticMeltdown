using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class InfoWindow
{
    private TextView _textView;
    private Controller _controller;

    public InfoWindow(List<string> lines)
    {
        _textView = new TextView(lines);
        _controller = new ActionController(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Up, _textView.ScrollPrev},
                {SelectionControl.Down, _textView.ScrollNext}
            }));
    }

    public void Open()
    {
        UserInterface.SetViewPositioner(this, new ExactViewPositioner(_textView));
        UserInterface.SetController(this, _controller);
        UserInterface.TakeControl(this);
    }

    private void Close()
    {
        UserInterface.Forget(this);
    }
}