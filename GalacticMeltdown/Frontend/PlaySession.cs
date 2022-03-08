﻿using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Frontend;

public partial class PlaySession
{
    private readonly string _savePath;
    private static Player _player;
    private static IControllable _controlledObject;
    private static Level _level;
    private static LevelView _levelView;

    private static bool _sessionActive;

    public PlaySession(Level level, string savePath)
    {
        _savePath = savePath;
        _level = level;
        _player = _level.Player;
        _player.SetControlFunc(() =>
        {
            Renderer.PlayAnimations();
            InputProcessor.AddBinding(DataHolder.CurrentBindings.Player, PlayerActions);
            InputProcessor.StartProcessLoop();
        });
        _controlledObject = _player;
        _levelView = _level.LevelView;
        _levelView.SetFocus(_player);
    }

    public void Start()
    {
        Renderer.AddView(_levelView, 0, 0, 0.8, 1);
        Renderer.AddView(_level.OverlayView, 0.8, 0, 1, 1);
        Renderer.Redraw();
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

    private static void MoveControlled(int deltaX, int deltaY)
    {
        if (!_controlledObject.TryMove(deltaX, deltaY)) return;
        if (_controlledObject is Actor)
        {
            InputProcessor.ClearBindings();
            InputProcessor.StopProcessLoop();
        }
    }

    private static void StopSession()
    {
        _sessionActive = false;
        _player.StopTurn();
        Renderer.ClearViews();
        InputProcessor.ClearBindings();
        InputProcessor.StopProcessLoop();
    }
}