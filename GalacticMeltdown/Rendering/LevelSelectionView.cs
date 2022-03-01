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
        _isManagementSelected = false;
    }
    
    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData(null, null);
    }
    
    public void PressCurrent()
    {
        
    }

    public void SelectNext()
    {
        
    }

    public void SelectPrev()
    {
        
    }

    public void SwitchButtonGroup()
    {
        
    }
}