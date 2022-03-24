using System;
using System.Collections.Generic;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class YesNoDialog : ChoiceDialog<bool>
{
    public YesNoDialog(Action<bool> sendInfo, string message) 
        : base(new List<(string text, bool choice)> {("Yes", true), ("No", false)}, sendInfo, message)
    {
    }
}