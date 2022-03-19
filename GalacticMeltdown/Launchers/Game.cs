using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.Menus;
using GalacticMeltdown.UserInterfaceRelated.Rendering;

namespace GalacticMeltdown.Launchers;

public static class Game
{
    private const int Root = 20220319;
    private static MainMenu _menu;
    private static PlaySession _session;

    public static void Main()
    {
        UserInterface.SetRoot(Root);
        OpenMainMenu();
    }

    public static void StartLevel(Level level, string savePath)
    {
        UserInterface.Forget(_menu);
        PlaySession session = new(level, savePath);
        UserInterface.AddChild(Root, session);
        session.Start();
    }

    private static void OpenMainMenu()
    {
        UserInterface.Forget(_session);
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