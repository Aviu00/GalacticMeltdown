using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown.Rendering;

public class LevelManagementView : View
{
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;
    private readonly List<Button> _levelButtons;
    private readonly List<Button> _managementButtons;
    private int _levelIndex;
    private int _managementIndex;
    private bool _isManagementSelected;
    private int _topVisibleLevelButtonIndex;
    private string[] _levelButtonText;
    private string[] _managementButtonText;
    private int _selectedLevelButtonY;
    
    private const ConsoleColor TextColor = ConsoleColor.Magenta;
    private const ConsoleColor BackgroundColorUnselected = ConsoleColor.Black;
    private const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkGray;
    private const ConsoleColor ManagementButtonsBorder = ConsoleColor.Yellow;

    public LevelManagementView()
    {
        List<LevelInfo> levels = FilesystemLevelManager.GetLevelButtonInfo();
        _levelButtons = new List<Button>(levels.Count);
        foreach (var levelInfo in levels)
        {
            _levelButtons.Add(new Button(levelInfo.Name, $"seed: {levelInfo.Seed}",
                () => TryStartLevel(levelInfo.Path)));
        }

        _levelButtonText = new string[_levelButtons.Count];

        _managementButtons = new List<Button>
        {
            new Button("Create", "", null),
            new Button("Delete", "", null)
        };
        _managementButtonText = new string[_managementButtons.Count];
        
        _levelIndex = 0;
        _managementIndex = 0;
        
        _isManagementSelected = levels.Count == 0;  // can't select a level when none exist
    }

    private void TryStartLevel(string path)
    {
        // TODO: error checking, animation on error.
        Game.StartLevel(FilesystemLevelManager.GetLevel(path));
    }
    
    public override ViewCellData GetSymbol(int x, int y)
    {
        char symbol;
        ConsoleColor fgColor;
        ConsoleColor bgColor;
        if (y == _managementButtons.Count)
        {
            symbol = '#';
            fgColor = ManagementButtonsBorder;
            bgColor = ConsoleColor.Black;
        }
        else if (y < _managementButtons.Count)
        {
            symbol = _managementButtonText[y][x];
            fgColor = TextColor;
            bgColor = _isManagementSelected && y == _managementIndex
                ? BackgroundColorSelected
                : BackgroundColorUnselected;
            
        }
        else
        {
            if (Height - y > _levelButtonText.Length) return new ViewCellData(null, null);
            symbol = _levelButtonText[Height - (y - _topVisibleLevelButtonIndex) - 1][x];
            fgColor = TextColor;
            bgColor = _selectedLevelButtonY == y ? BackgroundColorSelected : BackgroundColorUnselected;
        }

        return new ViewCellData(symbol == ' ' ? null : (symbol, fgColor), bgColor);
    }
    
    public void PressCurrent()
    {
        if (_isManagementSelected) _managementButtons[_managementIndex].Press();
        else _levelButtons[_levelIndex].Press();
    }

    public void SelectNext()
    {
        if (_isManagementSelected) _managementIndex = (_managementIndex + 1) % _managementButtons.Count;
        else _levelIndex = (_levelIndex + 1) % _levelButtons.Count;
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    public void SelectPrev()
    {
        if (_isManagementSelected) _managementIndex = (_managementButtons.Count + _managementIndex - 1) % _managementButtons.Count;
        else _levelIndex = (_levelButtons.Count + _levelIndex - 1) % _levelButtons.Count;
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    public void SwitchButtonGroup()
    {
        // can't select a level when none exist
        _isManagementSelected = _levelButtons.Any() ? !_isManagementSelected : false;
        NeedRedraw?.Invoke(this);
    }
    
    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        UpdateOutVars();
        CalculateVisibleButtonText();
    }
    
    private void CalculateVisibleButtonText()
    {
        for (int i = 0; i < _levelButtonText.Length; i++)
        {
            _levelButtonText[i] = _levelButtons[i].MakeText(Width);
        }
        for (int i = 0; i < _managementButtonText.Length; i++)
        {
            _managementButtonText[i] = _managementButtons[i].MakeText(Width);
        }
    }
    
    private void UpdateOutVars()
    {
        _topVisibleLevelButtonIndex = Math.Max(0, _levelIndex - (Height - (_managementButtons.Count + 1)) + 1);
        _selectedLevelButtonY = Height - (_levelIndex - _topVisibleLevelButtonIndex) - 1;
    }
}