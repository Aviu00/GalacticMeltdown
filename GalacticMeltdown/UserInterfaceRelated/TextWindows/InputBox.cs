using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated.TextWindows;

public class InputBox
{
    private Action<string> _send;
    
    public InputBox(Action<string> send)
    {
        _send = send;
    }

    public void Open()
    {
        InputView view = new();
        UserInterface.SetViewPositioner(this, new ExactViewPositioner(view));
        UserInterface.SetController(this,
            new TextInputHandler(view.AddCharacter,
                UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.TextInput,
                    new Dictionary<TextInputControl, Action>
                    {
                        {TextInputControl.DeleteCharacter, view.DeleteCharacter},
                        {TextInputControl.Back, () => UserInterface.Forget(this)},
                        {TextInputControl.FinishInput, () => _send(view.Text)},
                    })));
        UserInterface.TakeControl(this);
    }
}