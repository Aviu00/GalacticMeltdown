using System.Collections.Generic;

namespace GalacticMeltdown.Rendering;

public class LevelSelectionView : View
{
    public override event ViewChangedEventHandler ViewChanged;
    public override event CellsChangedEventHandler CellsChanged;
    private LinkedList<Button> _levelButtons;
    private LinkedList<Button> _managementButtons;
    private LinkedListNode<Button> _currentLevelNode;
    private LinkedListNode<Button> _currentManagementNode;
    private bool _isManagementSelected;

    public LevelSelectionView()
    {
        var Levels = FilesystemLevelManager.GetLevelInfo();
        _isManagementSelected = false;
    }
    
    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData(null, null);
    }
    
    public void PressCurrent()
    {
        if (_isManagementSelected) _currentManagementNode.Value.Press();
        else _currentLevelNode.Value.Press();
    }

    public void SelectNext()
    {
        if (_isManagementSelected) _currentManagementNode = _currentManagementNode.Next ?? _managementButtons.First;
        else _currentLevelNode = _currentLevelNode.Next ?? _levelButtons.First;
        // TODO: notify renderer
    }

    public void SelectPrev()
    {
        if (_isManagementSelected) _currentManagementNode = _currentManagementNode.Previous ?? _managementButtons.Last;
        else _currentLevelNode = _currentLevelNode.Previous ?? _levelButtons.Last;
        // TODO: notify renderer
    }

    public void SwitchButtonGroup()
    {
        _isManagementSelected = !_isManagementSelected;
        // TODO: notify renderer
    }
}