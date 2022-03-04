using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public partial class PlaySession
{
    private string _savePath;
    private static Player _player;
    private static IControllable _controlledObject;
    private static Level _level;
    private static LevelView _levelView;

    public PlaySession(Level level, string savePath)
    {
        _savePath = savePath;
        _level = level;
        _player = _level.Player;
        _controlledObject = _player;
        _levelView = _level.LevelView;
        _levelView.AddTileRevealingObject(_player);
        _levelView.SetFocus(_player);
    }

    public void Start()
    {
        Renderer.AddView(_levelView, 0, 0, 0.8, 1);
        Renderer.AddView(_level.OverlayView, 0.8, 0, 1, 1);
        Renderer.Redraw();
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Player, PlayerActions);
    }
    
    private static void MoveControlled(int deltaX, int deltaY)
    {
        if (_controlledObject.TryMove(deltaX, deltaY))  // Temporary, keeps screen up to date
        {
            Renderer.Redraw();  // TODO: Redraw should happen after a MoveMade event instead
        }
    }

    private static void StopSession()
    {
        Renderer.ClearViews();
        InputProcessor.ClearBindings();
        InputProcessor.StopProcessLoop();
    }
}
