﻿using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
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
        UserInterface.SetView(this, new MainScreenView(_levelView, _level.OverlayView));
        UserInterface.SetController(this,
            new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, MainActions)));
        _sessionActive = true;
        while (_sessionActive)
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

                break;
            }
        }
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

    private void StopSession()
    {
        _sessionActive = false;
        _player.StopTurn();
        UserInterface.Forget(this);
        Game.OpenMainMenu();
    }
}