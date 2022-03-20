using System;
using System.Collections.Generic;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.Menus;

public class LevelCreationDialog : Dialog
{
    private InputLine _nameLine;
    private InputLine _seedLine;
    private InputLine _difficultyMultiplierLine;
    
    public LevelCreationDialog(Action<List<object>> sender) : base(sender)
    {
        LineView = new LineView();
        _nameLine = new InputLine();
        _seedLine = new InputLine();
        _difficultyMultiplierLine = new InputLine();
        LineView.SetLines(new List<ListLine>
        {
            new TextLine("Name"),
            _nameLine,
            new TextLine("Seed"),
            _seedLine,
            new TextLine("Difficulty multiplier"),
            _difficultyMultiplierLine,
        });
    }

    protected override void SendInfo()
    {
        base.SendInfo();
        Sender(new List<object> {_nameLine.Text, _seedLine.Text, _difficultyMultiplierLine.Text});
    }
}