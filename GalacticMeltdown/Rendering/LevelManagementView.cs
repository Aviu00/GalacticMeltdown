using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Data;

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

public class LevelManagementView : View
{
    public override event ViewChangedEventHandler NeedRedraw;
    public override event CellsChangedEventHandler CellsChanged;

    private List<LevelButtonInfo> _menuLevels;
    private List<MenuButtonInfo> _managementButtonInfos;
    
    private int _levelIndex;
    private int _managementIndex;
    private bool _isManagementSelected;
    
    private int _topVisibleLevelButtonIndex;
    private int _selectedLevelButtonY;

    public LevelManagementView()
    {
        RefreshLevelList();
        _managementButtonInfos = new List<MenuButtonInfo>
        {
            new MenuButtonInfo(new Button("Create", "", CreateLevel)),
            new MenuButtonInfo(new Button("Delete", "", DeleteLevel)),
        };
        _managementIndex = 0;
    }

    private void CreateLevel()
    {
        // TODO: open dialog asking for name and seed (optional)
        int seed = Random.Shared.Next(0, 1000000000);
        Game.CreateLevel(seed);
    }

    private void DeleteLevel()
    {
        if (!_menuLevels.Any())
        {
            // TODO: error animation
            return;
        }
        // TODO: confirmation dialog
        if (!FilesystemLevelManager.RemoveLevel(_menuLevels[_levelIndex].LevelInfo.Path))
        {
            RefreshLevelList();
            return;
        }
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
        Level level = FilesystemLevelManager.GetLevel(path);
        if (level is null)
        {
            RefreshLevelList();
            // TODO: failure animation
            return;
        }
        Game.StartLevel(level);
    }
    
    public override ViewCellData GetSymbol(int x, int y)
    {
        char symbol;
        ConsoleColor fgColor;
        ConsoleColor bgColor;
        if (y == _managementButtonInfos.Count)
        {
            symbol = '#';
            fgColor = DataHolder.Colors.MenuBorderColor;
            bgColor = ConsoleColor.Black;
        }
        else if (y < _managementButtonInfos.Count)
        {
            symbol = _managementButtonInfos[y].RenderedText[x];
            fgColor = DataHolder.Colors.TextColor;
            bgColor = _isManagementSelected && y == _managementIndex
                ? DataHolder.Colors.BackgroundColorSelected
                : DataHolder.Colors.BackgroundColorUnselected;
            
        }
        else
        {
            if (Height - y > _menuLevels.Count) return new ViewCellData(null, null);
            symbol = _menuLevels[Height - (y - _topVisibleLevelButtonIndex) - 1].RenderedText[x];
            fgColor = DataHolder.Colors.TextColor;;
            bgColor = _selectedLevelButtonY == y 
                ? DataHolder.Colors.BackgroundColorSelected 
                : DataHolder.Colors.BackgroundColorUnselected;
        }

        return new ViewCellData(symbol == ' ' ? null : (symbol, fgColor), bgColor);
    }
    
    public void PressCurrent()
    {
        if (_isManagementSelected) _managementButtonInfos[_managementIndex].Button.Press();
        else _menuLevels[_levelIndex].LevelButton.Press();
    }

    public void SelectNext()
    {
        if (_isManagementSelected) _managementIndex = (_managementIndex + 1) % _managementButtonInfos.Count;
        else if (_menuLevels.Any()) _levelIndex = (_levelIndex + 1) % _menuLevels.Count;  // Avoid zero division
        UpdateOutVars();
        NeedRedraw?.Invoke(this);
    }

    public void SelectPrev()
    {
        if (_isManagementSelected)
        {
            _managementIndex -= 1;
            if (_managementIndex < 0) _managementIndex = _managementButtonInfos.Count - 1;
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
        for (int i = 0; i < _managementButtonInfos.Count; i++)
        {
            _managementButtonInfos[i].RenderedText = _managementButtonInfos[i].Button.MakeText(Width);
        }
    }

    private void RefreshLevelList()
    {
        List<LevelInfo> levelInfos = FilesystemLevelManager.GetLevelButtonInfo();
        _menuLevels = new List<LevelButtonInfo>(levelInfos.Count);
        foreach (var levelInfo in levelInfos)
        {
            _menuLevels.Add(new(new Button(levelInfo.Name, $"seed: {levelInfo.Seed}",
                () => TryStartLevel(levelInfo.Path)), levelInfo));
        }
        _levelIndex = 0;
        _isManagementSelected = _menuLevels.Count == 0;  // can't select a level when none exist
        if (!(Width == 0 && Height == 0))  // View has size
        {
            UpdateOutVars();
            CalculateVisibleButtonText();
        }
        NeedRedraw?.Invoke(this);
    }
    
    private void UpdateOutVars()
    {
        _topVisibleLevelButtonIndex = Math.Max(0, _levelIndex - (Height - (_managementButtonInfos.Count + 1)) + 1);
        _selectedLevelButtonY = _menuLevels.Any() ? Height - (_levelIndex - _topVisibleLevelButtonIndex) - 1 : -1;
    }
}