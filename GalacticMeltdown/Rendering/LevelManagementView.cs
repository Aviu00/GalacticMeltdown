using System;
using System.Collections.Generic;
using System.Linq;

namespace GalacticMeltdown.Rendering;


internal class LevelButtonInfo
{
    public Button LevelButton { get; }
    public LevelInfo LevelInfo { get; }
    public string RenderedText { get; set; }

    public LevelButtonInfo(Button button, LevelInfo levelInfo)
    {
        LevelButton = button;
        LevelInfo = levelInfo;
        RenderedText = "";
    }
}

internal class ManagementButtonInfo
{
    public Button LevelButton { get; }
    public string RenderedText { get; set; }

    public ManagementButtonInfo(Button button)
    {
        LevelButton = button;
        RenderedText = "";
    }
}

public class LevelManagementView : View
{
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;

    private List<LevelButtonInfo> _menuLevels;
    private readonly List<Button> _managementButtons;
    
    private int _levelIndex;
    private int _managementIndex;
    private bool _isManagementSelected;
    private int _topVisibleLevelButtonIndex;
    private string[] _managementButtonText;
    private int _selectedLevelButtonY;
    
    private const ConsoleColor TextColor = ConsoleColor.Magenta;
    private const ConsoleColor BackgroundColorUnselected = ConsoleColor.Black;
    private const ConsoleColor BackgroundColorSelected = ConsoleColor.DarkGray;
    private const ConsoleColor ManagementButtonsBorder = ConsoleColor.Yellow;

    public LevelManagementView()
    {
        List<LevelInfo> levelInfos = FilesystemLevelManager.GetLevelButtonInfo();
        _menuLevels = new List<LevelButtonInfo>(levelInfos.Count);
        foreach (var levelInfo in levelInfos)
        {
            _menuLevels.Add(new(new Button(levelInfo.Name, $"seed: {levelInfo.Seed}",
                () => TryStartLevel(levelInfo.Path)), levelInfo));
        }

        _managementButtons = new List<Button>
        {
            new Button("Create", "", null),
            new Button("Delete", "", DeleteLevel)
        };
        _managementButtonText = new string[_managementButtons.Count];
        
        _levelIndex = 0;
        _managementIndex = 0;
        
        _isManagementSelected = _menuLevels.Count == 0;  // can't select a level when none exist
    }

    private void DeleteLevel()
    {
        if (!_menuLevels.Any())
        {
            // TODO: error animation
            return;
        }
        // TODO: confirmation dialog
        FilesystemLevelManager.RemoveLevel(_menuLevels[_levelIndex].LevelInfo.Path);
        _menuLevels.RemoveAt(_levelIndex);
        if (!_menuLevels.Any())  // If there are no levels left, don't let the user select one
        {
            _isManagementSelected = true;
            _levelIndex = 0;
        }
        else if (_levelIndex >= _menuLevels.Count)
        {
            _levelIndex = _menuLevels.Count - 1;
        }
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
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
            if (Height - y > _menuLevels.Count) return new ViewCellData(null, null);
            symbol = _menuLevels[Height - (y - _topVisibleLevelButtonIndex) - 1].RenderedText[x];
            fgColor = TextColor;
            bgColor = _selectedLevelButtonY == y ? BackgroundColorSelected : BackgroundColorUnselected;
        }

        return new ViewCellData(symbol == ' ' ? null : (symbol, fgColor), bgColor);
    }
    
    public void PressCurrent()
    {
        if (_isManagementSelected) _managementButtons[_managementIndex].Press();
        else _menuLevels[_levelIndex].LevelButton.Press();
    }

    public void SelectNext()
    {
        if (_isManagementSelected) _managementIndex = (_managementIndex + 1) % _managementButtons.Count;
        else if (_menuLevels.Any()) _levelIndex = (_levelIndex + 1) % _menuLevels.Count;  // Avoid zero division
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    public void SelectPrev()
    {
        if (_isManagementSelected)
        {
            _managementIndex -= 1;
            if (_managementIndex < 0) _managementIndex = _managementButtons.Count - 1;
        }
        else if (_menuLevels.Any())
        {
            _levelIndex -= 1;
            if (_levelIndex < 0) _levelIndex = _menuLevels.Count - 1;
        }
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    public void SwitchButtonGroup()
    {
        // can't select a level when none exist
        _isManagementSelected = _menuLevels.Any() ? !_isManagementSelected : true;
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
        for (int i = 0; i < _menuLevels.Count; i++)
        {
            _menuLevels[i].RenderedText = _menuLevels[i].LevelButton.MakeText(Width);
        }
        for (int i = 0; i < _managementButtonText.Length; i++)
        {
            _managementButtonText[i] = _managementButtons[i].MakeText(Width);
        }
    }
    
    private void UpdateOutVars()
    {
        _topVisibleLevelButtonIndex = Math.Max(0, _levelIndex - (Height - (_managementButtons.Count + 1)) + 1);
        _selectedLevelButtonY = _menuLevels.Any() ? Height - (_levelIndex - _topVisibleLevelButtonIndex) - 1 : -1;
    }
}