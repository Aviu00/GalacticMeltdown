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
    private readonly string _savePath;
    private static Player _player;
    private static IControllable _controlledObject;
    private static Level _level;
    private static LevelView _levelView;

    private bool _sessionActive;

    private Dictionary<MainControl, Action> MainActions;
    private Dictionary<CursorControl, Action> CursorActions;

    public PlaySession(Level level, string savePath)
    {
        _savePath = savePath;
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
        _sessionActive = true;
        UserInterface.SetView(this, new MainScreenView(_levelView, _level.OverlayView));
        UserInterface.SetController(this,
            new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, MainActions)));
        UserInterface.SetTask(MapTurn);
    }

    private void MapTurn()
    {
        SaveLevel();
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
        if (_sessionActive) UserInterface.SetTask(MapTurn);
    }

    private void SaveLevel()
    {
        FilesystemLevelManager.SaveLevel(_level, _savePath);
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

    public void Stop()
    {
        _sessionActive = false;
        _player.StopTurn();
        UserInterface.Forget(this);
        Game.OpenMainMenu();
    }
}