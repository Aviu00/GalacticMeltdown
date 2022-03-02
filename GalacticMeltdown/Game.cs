using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public static class Game
{
    private static bool _playing;
    private static Stack<View> _menus;
    public static void Main()
    {
        _playing = true;
        while (_playing)
        {
            _menus = new Stack<View>();
            OpenMainMenu();
            InputProcessor.StartProcessLoop();
        }
        Renderer.CleanUp();
    }

    public static void GenerateLevel(int? seed)
    {
        
    }

    public static void StartLevel(Map level)
    {
        var session = new PlaySession(level);
        session.Start();
    }

    private static void OpenMainMenu()
    {
        OpenBasicMenu(new Button("Select level", "", OpenLevelMenu), new Button("Quit", "", Quit));
    }

    private static void OpenLevelMenu()
    {
        var levelMenu = new LevelSelectionView();
        OpenMenu(levelMenu, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Up, levelMenu.SelectPrev},
            {SelectionControl.Down, levelMenu.SelectNext},
            {SelectionControl.SwitchButtonGroup, levelMenu.SwitchButtonGroup},
            {SelectionControl.Select, levelMenu.PressCurrent},
            {SelectionControl.Back, CloseOpenedMenu}
        });
    }

    private static void OpenBasicMenu(params Button[] buttons)
    {
        var buttonListView = new ButtonListView(buttons);
        OpenMenu(buttonListView, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Up, buttonListView.SelectPrev},
            {SelectionControl.Down, buttonListView.SelectNext},
            {SelectionControl.Select, buttonListView.PressCurrent}
        });
    }

    private static void OpenMenu(View buttonListView, Dictionary<SelectionControl, Action> bindings)
    {
        _menus.Push(buttonListView);
        Renderer.AddView(buttonListView, 0, 0, 1, 1);
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Selection, bindings);
    }

    private static void CloseOpenedMenu()
    {
        _menus.Pop();
        Renderer.RemoveLastView();
        InputProcessor.RemoveLastBinding();
    }

    private static void Quit()
    {
        _playing = false;
        InputProcessor.StopProcessLoop();
    }
}