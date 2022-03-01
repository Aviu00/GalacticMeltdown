using System.Collections.Generic;

namespace GalacticMeltdown.Rendering;

public class ButtonListView : View
{
    public override event ViewChangedEventHandler ViewChanged;
    public override event CellsChangedEventHandler CellsChanged;
    private LinkedList<Button> _buttons;
    private LinkedListNode<Button> _currentButtonNode;

    public ButtonListView(LinkedList<Button> buttons)
    {
        _buttons = buttons;
        _currentButtonNode = _buttons.First;
    }

    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData(null, null);
    }

    public void PressCurrent()
    {
        _currentButtonNode.Value.Press();
    }

    public void SelectNext()
    {
        _currentButtonNode = _currentButtonNode.Next ?? _buttons.First;
        // TODO: update view
    }

    public void SelectPrev()
    {
        _currentButtonNode = _currentButtonNode.Previous ?? _buttons.Last;
        // TODO: update view
    }
}