using System;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public partial class PlaySession
{
    private static Player _player;
    private static IControllable _controlledObject;
    private static Map _map;
    private static WorldView _worldView;

    public PlaySession(Map level)
    {
        _map = level;
        _player = _map.Player;
        _controlledObject = _player;
        _worldView = new WorldView(_map);
        _worldView.AddTileRevealingObject(_player);
        _worldView.SetFocus(_player);
    }

    public void Start()
    {
        Renderer.AddView(_worldView, 0, 0, 0.8, 1);
        Renderer.AddView(new OverlayView(_map), 0.8, 0, 1, 1);
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
}
