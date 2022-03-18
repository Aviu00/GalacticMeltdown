using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.Launchers;

public static class Game
{
    private static bool _playing;
    private static MenuView _menu;

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

    public static void StartLevel(Level level, string savePath)
    {
        Renderer.ClearViews();
        InputProcessor.ClearBindings();
        var session = new PlaySession(level, savePath);
        session.Start();
    }

    private static void OpenMainMenu()
    {
        _menu = new MenuView();
        Renderer.AddView(_menu, (0, 0, 1, 1));
        _menu.OpenBasicMenu(new Button("Select level", "", _menu.OpenLevelMenu), new Button("Quit", "", Quit));
    }

    private static void Quit()
    {
        _playing = false;
        InputProcessor.StopProcessLoop();
    }
}