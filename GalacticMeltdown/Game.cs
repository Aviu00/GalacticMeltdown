using GalacticMeltdown.Rendering;

namespace GalacticMeltdown;

public class Game
{
    static void Main()
    {
        var session = new PlaySession();
        session.Start();
        Renderer.CleanUp();
    }
}