using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown.Rendering;

public class LevelSelectionView : View
{
    public override event ViewChangedEventHandler ViewChanged;
    public override event CellsChangedEventHandler CellsChanged;
    private readonly LinkedList<Button> _levelButtons;
    private readonly LinkedList<Button> _managementButtons;
    private LinkedListNode<Button> _currentLevelNode;
    private LinkedListNode<Button> _currentManagementNode;
    private bool _isManagementSelected;

    public LevelSelectionView()
    {
        List<LevelInfo> levels = FilesystemLevelManager.GetLevelInfo();
        _levelButtons = new LinkedList<Button>();
        foreach (var levelInfo in levels)
        {
            _levelButtons.AddLast(new Button(levelInfo.Name, $"seed: {levelInfo.Seed}", 
                () => TryStartLevel(levelInfo.Path)));
        }

        _managementButtons = new LinkedList<Button>();
        _managementButtons.AddLast(new Button("Create", "", null));
        _managementButtons.AddLast(new Button("Delete", "", null));
        
        _currentLevelNode = _levelButtons.First;
        _currentManagementNode = _managementButtons.First;
        
        _isManagementSelected = levels.Count == 0;  // can't select a level when none exist
    }

    private void TryStartLevel(string path)
    {
        // TODO: error checking, animation on error.
        Game.StartLevel(FilesystemLevelManager.GetLevel(path));
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
        // can't select a level when none exist
        _isManagementSelected = _levelButtons.Any() ? !_isManagementSelected : false;
        // TODO: notify renderer
    }
}