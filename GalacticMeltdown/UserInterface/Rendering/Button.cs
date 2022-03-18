using System;

namespace GalacticMeltdown.UserInterface.Rendering;

public readonly struct Button
{
    private readonly Action _action;
    private string TextLeft { get; }
    private string TextRight { get; }

    public Button(string textLeft, string textRight, Action action)
    {
        TextLeft = textLeft;
        TextRight = textRight;
        _action = action;
    }

    public string MakeText(int width)
    {
        const string ellipsis = "...";
        const string separator = "  ";
        const string noSpaceForRightText = $"{separator}{ellipsis}";
        int maxLeftStringLength = width - noSpaceForRightText.Length;
        string screenText;
        if (TextRight.Length == 0)
        {
            screenText = TextLeft.Length > width
                ? TextLeft.Substring(0, width - ellipsis.Length) + ellipsis
                : TextLeft.PadRight(width);
        }
        else if (TextLeft.Length == 0)
        {
            screenText = TextRight.Length > width
                ? ellipsis + TextRight.Substring(TextRight.Length - (width - ellipsis.Length))
                : TextRight.PadLeft(width);
        }
        else if (TextLeft.Length >= maxLeftStringLength)
        {
            screenText = TextLeft.Substring(0, maxLeftStringLength) + noSpaceForRightText;
        }
        else
        {
            screenText = TextLeft;
            screenText += separator;
            int spaceLeft = width - screenText.Length;
            screenText += TextRight.Length > spaceLeft
                ? TextRight.Substring(0, spaceLeft - ellipsis.Length) + ellipsis
                : TextRight.PadLeft(spaceLeft);
        }

        return screenText;
    }
    
    public void Press() => _action?.Invoke();
}