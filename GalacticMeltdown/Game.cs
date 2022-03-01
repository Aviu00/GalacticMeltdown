using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public static class Game
{
    private static bool _playing;
    static void Main()
    {
        _playing = true;
        var mainMenu = new MainMenu();
        mainMenu.Start();
        var session = new PlaySession();
        session.Start();
        Renderer.CleanUp();
    }
}