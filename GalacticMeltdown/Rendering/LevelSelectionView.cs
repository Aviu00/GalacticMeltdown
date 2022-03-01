using System.Collections.Generic;

namespace GalacticMeltdown.Rendering;

public class LevelSelectionView : View
{
    public override event ViewChangedEventHandler ViewChanged;
    public override event CellsChangedEventHandler CellsChanged;
    private LinkedList<Button> _levelButtons;
    private LinkedList<Button> _managementButtons;

    public LevelSelectionView()
    {
        
    }
    
    public override ViewCellData GetSymbol(int x, int y)
    {
        return new ViewCellData(null, null);
    }
}