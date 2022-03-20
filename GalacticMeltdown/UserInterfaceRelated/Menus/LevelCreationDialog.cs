using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public class LevelCreationDialog : Dialog
{
    private readonly Action<string, int?> _sender;

    private InputLine _nameLine;
    private InputLine _seedLine;
    
    public LevelCreationDialog(Action<string, int?> sender)
    {
        _sender = sender;
    }

    protected override void SendInfo()
    {
        base.SendInfo();
        int? seed = null;
        if (int.TryParse(_seedLine.Text, out int tempSeed))
        {
            seed = tempSeed;
        }
        _sender(_nameLine.Text, seed);
    }

    public override void Open()
    {
        LineView = new LineView();

        _nameLine = new InputLine();
        UserInterface.AddChild(this, _nameLine);

        _seedLine = new InputLine(char.IsDigit);
        UserInterface.AddChild(this, _seedLine);

        LineView.SetLines(new List<ListLine>
        {
            new TextLine("Name"), _nameLine, new TextLine("Seed"), _seedLine,
        });
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Selection,
            new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Select, LineView.PressCurrent},
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.SpecialAction, SendInfo}
            }));
        base.Open();
    }
}