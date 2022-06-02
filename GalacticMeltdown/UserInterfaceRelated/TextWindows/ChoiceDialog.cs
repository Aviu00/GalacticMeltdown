using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public abstract class ChoiceDialog<T> : TextWindow
{
    private Action<T> _send;
    protected List<(string text, T choice)> Options;
    private string _message;
    private bool _closeOnChoice;

    protected ChoiceDialog(List<(string text, T choice)> options, Action<T> send, string message = null,
        bool closeOnChoice = true)
    {
        _send = send;
        Options = options;
        _message = message;
        _closeOnChoice = closeOnChoice;
        Controller = new ActionController(UtilityFunctions.JoinDictionaries(KeyBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.Select, LineView.PressCurrent},
            }));
        UpdateLines();
    }

    private void UpdateLines()
    {
        List<ListLine> lines = new();
        if (_message is not null) lines.Add(new TextLine(_message));
        lines.AddRange(Options.Select(info => new Button(info.text, "", () => SendChoice(info.choice))));
        LineView.SetLines(lines);
    }

    private void SendChoice(T choice)
    {
        if (_closeOnChoice) Close();
        _send(choice);
    }
}