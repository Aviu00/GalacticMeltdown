using System;
using System.Collections.Generic;
using GalacticMeltdown.Data;
using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public static class Game
{
    private static bool _playing;
    public static void Main()
    {
        _playing = true;
        while (_playing)
        {
            OpenMainMenu();
            InputProcessor.StartProcessLoop();
        }
        Renderer.CleanUp();
    }

    private static void OpenMainMenu()
    {
        var playButton = new Button("Play", "", null);
        var quitButton = new Button("Quit", "", Quit);
        LinkedList<Button> mainMenuButtons = new();
        mainMenuButtons.AddLast(playButton);
        mainMenuButtons.AddLast(quitButton);
        var mainMenuButtonsView = new ButtonListView(mainMenuButtons);
        Renderer.AddView(mainMenuButtonsView, 0, 0, 1, 1);
        InputProcessor.AddBinding(DataHolder.CurrentBindings.Selection, new Dictionary<SelectionControl, Action>
        {
            {SelectionControl.Up, mainMenuButtonsView.SelectPrev},
            {SelectionControl.Down, mainMenuButtonsView.SelectNext},
            {SelectionControl.Select, mainMenuButtonsView.PressCurrent}
        });
        //var session = new PlaySession();
        //session.Start();
    }

    private static void Quit()
    {
        _playing = false;
        InputProcessor.StopProcessLoop();
    }
}