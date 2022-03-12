namespace GalacticMeltdown.Views;

public partial class LevelView
{
    private Cursor _cursor;

    public Cursor Cursor
    {
        get
        {
            if (_cursor is not null) return _cursor;
            _cursor = new Cursor(_focusObject.X, _focusObject.Y);
            SetCursorBounds();
            return _cursor;
        }
        private set => _cursor = value;
    }

    public void RemoveCursor()
    {
        if (_cursor.InFocus) return;
        Cursor = null;
    }
    
    private void SetCursorBounds()
    {
        if (_cursor is null) return;
        int minX, minY, maxX, maxY;
        if (_cursor.InFocus)
        {
            minX = -1;
            minY = -1;
            (maxX, maxY) = _level.Size;
        }
        else
        {
            (minX, minY, maxX, maxY) = ViewBounds;
        }

        _cursor.SetBounds(minX, minY, maxX, maxY);
    }
}