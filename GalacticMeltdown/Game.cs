using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

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

    public static void StartLevel(Level level, string path)
    {
        Renderer.ClearViews();
        InputProcessor.ClearBindings();
        var session = new PlaySession(level, path);
        session.Start();
    }

    private static void OpenMainMenu()
    {
        _menu = new MenuView();
        Renderer.AddView(_menu, 0, 0, 1, 1);
        _menu.OpenBasicMenu(new Button("Select level", "", _menu.OpenLevelMenu), 
            new Button("Quit", "", Quit));
    }

    public static void CreateLevel(int seed)
    {
        Level level = new MapGenerator(seed).Generate();
        string savePath = FilesystemLevelManager.GetSavePath();
        FilesystemLevelManager.SaveLevel(level, savePath);
        StartLevel(level, savePath);
    }

    private static void Quit()
    {
        _playing = false;
        InputProcessor.StopProcessLoop();
    }
}