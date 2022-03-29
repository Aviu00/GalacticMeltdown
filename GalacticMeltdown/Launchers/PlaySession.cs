using System;
using System.Collections.Generic;
using System.Threading;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.Items;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing.ControlTypes;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.UserInterfaceRelated.TextWindows;
using GalacticMeltdown.UserInterfaceRelated.TextWindows.ItemManagement;
using GalacticMeltdown.Utility;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Launchers;

public partial class PlaySession
{
    private const int SaveInterval = 100;
    
    private readonly string _levelName;
    private readonly int _levelSeed;
    private Player _player;
    private IControllable _controlledObject;
    private Level _level;
    private LevelView _levelView;
    private Counter _saveCounter;

    private Dictionary<MainControl, Action> _mainActions;

    public PlaySession(Level level, string levelName, int levelSeed)
    {
        _saveCounter = new Counter(level, SaveInterval, SaveInterval);
        _levelName = levelName;
        _levelSeed = levelSeed;
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
        UserInterface.SetViewPositioner(this,
            new MainViewPositioner(_levelView, _level.OverlayView, _level.MinimapView));
        UserInterface.SetController(this,
            new ActionHandler(UtilityFunctions.JoinDictionaries(DataHolder.CurrentBindings.Main, _mainActions)));
        UserInterface.SetTask(this, MapTurn);
    }

    private void MapTurn()
    {
        _level.DoTurn();
        if (_level.PlayerWon is null)
        {
            if (_saveCounter.FinishedCounting)
            {
                SaveLevel();
                _saveCounter.ResetTimer();
            }
            UserInterface.SetTask(this, MapTurn);
        }
        else
        {
            UserInterface.PlayAnimations();
            Thread.Sleep(400);
            EndGameMessage endGameMessage = new(_level.PlayerWon.Value ? "You won" : "You died");
            UserInterface.AddChild(this, endGameMessage);
            endGameMessage.Open();
            UserInterface.SetTask(this, Game.OpenMainMenu);
        }
    }

    private void SaveLevel()
    {
        FilesystemLevelManager.SaveLevel(_level, _levelName, _levelSeed);
    }

    private void MoveControlled(int deltaX, int deltaY)
    {
        if (!_controlledObject.TryMove(deltaX, deltaY)) return;
        if (_controlledObject is Actor) UserInterface.YieldControl(this);
    }

    private void OpenPauseMenu()
    {
        PauseMenu pauseMenu = new(SaveAndQuit);
        UserInterface.AddChild(this, pauseMenu);
        pauseMenu.Open();
    }
    
    private void SaveAndQuit()
    {
        _level.AbortTurn();
        SaveLevel();
        Game.OpenMainMenu();
    }

    private void OpenInventory()
    {
        InventoryMenu inventoryMenu = new(_player);
        UserInterface.AddChild(this, inventoryMenu);
        inventoryMenu.Open();
    }
    
    private void OpenAmmoSelectionDialog()
    {
        if (!(_player.Equipment[BodyPart.Hands] is WeaponItem weapon && weapon.AmmoTypes is not null)) return;
        AmmoDialog dialog = new(_player, id => _player.ChosenAmmoId = id);
        UserInterface.AddChild(this, dialog);
        dialog.Open();
    }

    private void ProcessCommand(string command)
    {
        command = command.TrimStart();
        string[] words = command.Split(' ', 1);
        if (words.Length == 0) return;
        switch (words[0])
        {
            case "exit":
                SaveAndQuit();
                break;
            default:
                break;
        }
    }
}