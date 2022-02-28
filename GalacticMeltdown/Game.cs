using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public class Game
{
    static void Main()
    {
        var mainMenu = new MainMenu();
        mainMenu.Start();
        var session = new PlaySession();
        session.Start();
        Renderer.CleanUp();
    }
}