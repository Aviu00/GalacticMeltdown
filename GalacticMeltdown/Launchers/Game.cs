using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Menus;
using GalacticMeltdown.UserInterfaceRelated.Rendering;

namespace GalacticMeltdown.Launchers;

public static class Game
{
    private const int Root = 20220319;
    private static bool _playing;
    private static MainMenu _menu;

    public static void Main()
    {
        UserInterface.SetRoot(Root);
        OpenMainMenu();
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
        _menu = new MainMenu();
        UserInterface.AddChild(Root, _menu);
        _menu.Open();
    }

    public static void Quit()
    {
        UserInterface.Forget(Root);
        Renderer.CleanUp();
    }
}