using GalacticMeltdown.Actors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Launchers;

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
            InputProcessor.AddBinding(DataHolder.CurrentBindings.Main, MainActions);
            InputProcessor.StartProcessLoop();
        });
        _controlledObject = _player;
        _levelView = _level.LevelView;
        _levelView.SetFocus(_player);
    }

    public void Start()
    {
        Renderer.AddViewTemp(new MainScreenView(_levelView, _level.OverlayView));
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
        if (_controlledObject is Actor) GiveBackControl();
    }

    private static void StopSession()
    {
        _sessionActive = false;
        _player.StopTurn();
        Renderer.ClearViews();
        GiveBackControl();
    }

    private static void GiveBackControl()
    {
        InputProcessor.ClearBindings();
        InputProcessor.StopProcessLoop();
    }
}