using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class YesNoDialog : ChoiceDialog<bool>
{
    public YesNoDialog(Action<bool> sendInfo, string message) 
        : base(new List<(string text, bool choice)> {("Yes", true), ("No", false)}, sendInfo, message)
    {
    }
}