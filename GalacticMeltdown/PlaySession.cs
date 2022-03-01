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

    public void Start()
    {
        var map = GenerateMap();
        _player = _map.Player;
        _controlledObject = _player;
        _worldView = new WorldView(map);
        _worldView.AddTileRevealingObject(_player);
        _worldView.SetFocus(_player);
        Renderer.AddView(_worldView, 0, 0, 0.8, 1);
        Renderer.AddView(new OverlayView(map), 0.8, 0, 1, 1);
        Renderer.Redraw();
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Player, PlayerActions);
        InputProcessor.StartProcessLoop();
    }
    
    private static void MoveControlled(int deltaX, int deltaY)
    {
        if (_controlledObject.TryMove(deltaX, deltaY))  // Temporary, keeps screen up to date
        {
            Renderer.Redraw();  // TODO: Redraw should happen after a MoveMade event instead
        }
    }

    private static Map GenerateMap(string seed = null)
    {
        if (seed == null || !int.TryParse(seed, out int mapSeed) || mapSeed < 0)
            mapSeed = Random.Shared.Next(0, 1000000000);
        var tileTypes = new TileTypesExtractor().TileTypes;
        var rooms = new RoomDataExtractor(tileTypes).Rooms;
        MapGenerator mapGen = new MapGenerator(mapSeed, tileTypes, rooms);
        _map = mapGen.Generate();
        rooms = null;
        tileTypes = null;
        mapGen = null;
        GC.Collect();
        return _map;
    }
}
