using System;

namespace GalacticMeltdown;

public readonly struct Button
{
    public string TextLeft { get; }
    public string TextRight { get; }
    private readonly Action _action;

    public Button(string textLeft, string textRight, Action action)
    {
        TextLeft = textLeft;
        TextRight = textRight;
        _action = action;
    }

    public void Press() => _action?.Invoke();
}