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
        }
        Renderer.CleanUp();
    }

    private static void OpenMainMenu()
    {
        var session = new PlaySession();
        session.Start();
    }
}