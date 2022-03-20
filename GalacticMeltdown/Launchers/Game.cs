using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.Menus;

namespace GalacticMeltdown.Launchers;

public static class Game
{
    private const int Root = 20220319;
    private static MainMenu _menu;
    private static PlaySession _session;

    public static void Main()
    {
        UserInterface.SetRoot(Root);
        UserInterface.SetTask(OpenMainMenu);
        UserInterface.Start();
    }

    public static void StartLevel(Level level, string savePath)
    {
        UserInterface.Forget(_menu);
        _session = new PlaySession(level, savePath);
        UserInterface.AddChild(Root, _session);
        UserInterface.SetTask(_session.Start);
    }

    public static void OpenMainMenu()
    {
        UserInterface.Forget(_session);
        _menu = new MainMenu();
        UserInterface.AddChild(Root, _menu);
        UserInterface.SetTask(_menu.Open);
    }

    public static void Quit()
    {
        UserInterface.SetTask(null);
        UserInterface.Forget(Root);
    }
}