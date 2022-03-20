using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Menus;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    private const int SaveInterval = 30;
    
    private readonly string _levelName;
    private static Player _player;
    private static IControllable _controlledObject;
    private static Level _level;
    private static LevelView _levelView;

    private Dictionary<MainControl, Action> _mainActions;
    private Dictionary<CursorControl, Action> _cursorActions;

    public PlaySession(Level level, string levelName)
    {
        new Counter(level, SaveInterval, SaveInterval, counter =>
        {
            SaveLevel();
            counter.ResetTimer();
        });
        _levelName = levelName;
        _level = level;
        _player = _level.Player;
        _player.SetControlFunc(() =>
        {
            UserInterface.PlayAnimations();
            UserInterface.TakeControl(this);
        });
        _controlledObject = _player;
        _levelView = _level.LevelView;
        _levelView.SetFocus(_player);
        SetControlDicts();
    }

    public void Start()
    {
        UserInterface.SetView(this, new MainScreenView(_levelView, _level.OverlayView));
        UserInterface.SetController(this,
            new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, _mainActions)));
        UserInterface.SetTask(this, MapTurn);
    }

    private void MapTurn()
    {
        if (!_level.DoTurn())
        {
            if (_level.PlayerWon)
            {
            }
            else
            {
            }
            return;
        }
        UserInterface.SetTask(this, MapTurn);
    }

    public void SaveLevel()
    {
        FilesystemLevelManager.SaveLevel(_level, _levelName);
    }

    private void MoveControlled(int deltaX, int deltaY)
    {
        if (!_controlledObject.TryMove(deltaX, deltaY)) return;
        if (_controlledObject is Actor) UserInterface.YieldControl(this);
    }

    private void OpenPauseMenu()
    {
        PauseMenu pauseMenu = new PauseMenu(this);
        UserInterface.AddChild(this, pauseMenu);
        pauseMenu.Open();
    }
}