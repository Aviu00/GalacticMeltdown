using System.ComponentModel;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.UserInterfaceRelated;
using GalacticMeltdown.UserInterfaceRelated.TextWindows;
using GalacticMeltdown.Utility;

namespace GalacticMeltdown.Launchers;

public static class Game
{
    private const int Root = 20220319;
    private static MainMenu _menu;
    private static PlaySession _session;

    public static void Main()
    {
        TypeDescriptor.AddAttributes(typeof((int, int)),
            new TypeConverterAttribute(typeof(TupleConverter<int, int>)));
        UserInterface.SetRoot(Root);
        UserInterface.SetTask(Root, OpenMainMenu);
        UserInterface.Start();
    }

    public static void StartLevel(Level level, string name, int seed)
    {
        UserInterface.Forget(_menu);
        DataHolder.CurrentSeed = seed;
        _session = new PlaySession(level, name);
        UserInterface.AddChild(Root, _session);
        UserInterface.SetTask(Root, _session.Start);
    }

    public static void OpenMainMenu()
    {
        UserInterface.Forget(_session);
        _menu = new MainMenu();
        UserInterface.AddChild(Root, _menu);
        UserInterface.SetTask(Root, _menu.Open);
    }

    public static void Quit()
    {
        UserInterface.Forget(Root);
    }
}