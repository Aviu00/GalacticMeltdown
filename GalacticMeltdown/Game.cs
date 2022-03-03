using System;
using System.Collections.Generic;
using System.Linq;
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

    public static void StartLevel(Map level)
    {
        Renderer.ClearViews();
        InputProcessor.ClearBindings();
        var session = new PlaySession(level);
        session.Start();
    }

    private static void OpenMainMenu()
    {
        OpenBasicMenu(new Button("Select level", "", OpenLevelMenu), new Button("Quit", "", Quit));
    }

    private static void OpenLevelMenu()
    {
        var levelMenu = new LevelManagementView();
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

    private static void OpenMenu(View view, Dictionary<SelectionControl, Action> bindings)
    {
        if (_menus.Any()) Renderer.RemoveLastView();
        _menus.Push(view);
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Selection, bindings);
        Renderer.AddView(view, 0, 0, 1, 1);
        Renderer.Redraw();
    }

    private static void CloseOpenedMenu()
    {
        Renderer.RemoveLastView();
        InputProcessor.RemoveLastBinding();
        _menus.Pop();
        Renderer.AddView(_menus.Peek(), 0, 0, 1, 1);
    }

    private static void Quit()
    {
        _playing = false;
        InputProcessor.StopProcessLoop();
    }
}