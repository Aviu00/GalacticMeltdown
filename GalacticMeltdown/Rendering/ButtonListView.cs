using System.Collections.Generic;

namespace GalacticMeltdown.Rendering;

public class ButtonListView : View
{
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;
    private readonly List<Button> _buttons;
    private int _currentButtonIndex;

    public ButtonListView(ICollection<Button> buttons)
    {
        // TODO: this should have at least one button, check that
        _buttons = new List<Button>(buttons);
        _currentButtonIndex = 0;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData(null, null);
    }

    public void PressCurrent()
    {
        _buttons[_currentButtonIndex].Press();
    }

    public void SelectNext()
    {
        _currentButtonIndex = (_currentButtonIndex + 1) % _buttons.Count;
        // TODO: update view
    }

    public void SelectPrev()
    {
        _currentButtonIndex = (_buttons.Count + _currentButtonIndex - 1) % _buttons.Count;  // -1 should wrap
        // TODO: update view
    }
}