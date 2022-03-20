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
    private InputLine _nameLine;
    private InputLine _seedLine;
    private InputLine _diffMulLine;
    
    public LevelCreationDialog(Action<List<object>> sender) : base(sender)
    {
        LineView = new LineView();
        _nameLine = new InputLine();
        _seedLine = new InputLine(char.IsDigit);
        _diffMulLine = new InputLine(char.IsDigit);
        LineView.SetLines(new List<ListLine>
        {
            new TextLine("Name"),
            _nameLine,
            new TextLine("Seed"),
            _seedLine,
            new TextLine("Difficulty multiplier"),
            _diffMulLine,
        });
        Controller = new ActionHandler(UtilityFunctions.JoinDictionaries(
            DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>
            {
                {SelectionControl.Back, Close},
                {SelectionControl.Select, LineView.PressCurrent},
                {SelectionControl.Down, LineView.SelectNext},
                {SelectionControl.Up, LineView.SelectPrev},
                {SelectionControl.SpecialAction, SendInfo}
            }));
    }

    protected override void SendInfo()
    {
        base.SendInfo();
        int? seed = null;
        if (_seedLine.Text.Length != 0 && int.TryParse(_seedLine.Text, out int tempSeed))
        {
            seed = tempSeed;
        }
        int? multiplier = null;
        if (_diffMulLine.Text.Length != 0 && int.TryParse(_diffMulLine.Text, out int tempMul))
        {
            multiplier = tempMul;
        }
        Sender(new List<object> {_nameLine.Text, seed, multiplier});
    }
}