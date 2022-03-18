using GalacticMeltdown.UserInterfaceRelated.InputProcessing;
using GalacticMeltdown.UserInterfaceRelated.Rendering;
using GalacticMeltdown.Views;

namespace GalacticMeltdown.UserInterfaceRelated;

public static class UserInterface
{
    private static Renderer _renderer;
    private static InputProcessor _inputProcessor;

    static UserInterface()
    {
        _renderer = new Renderer();
        _inputProcessor = new InputProcessor();
    }

    public static void SetView(object sender, View view)
    {
        _renderer.SetView(sender, view);
    }

    public static void SetController(object sender, Controller controller)
    {
        _inputProcessor.SetController(sender, controller);
    }

    public static void SetRoot(object root)
    {
        _renderer.SetRoot(root);
        _inputProcessor.SetRoot(root);
    }

    public static void TakeControl(object sender)
    {
        _inputProcessor.TakeControl(sender);
    }

    public static void YieldControl(object sender)
    {
        _inputProcessor.YieldControl(sender);
    }

    public static void Forget(object sender)
    {
        _inputProcessor.Forget(sender);
        _renderer.Forget(sender);
    }

    public static void AddChild(object parent, object child)
    {
        _inputProcessor.AddChild(parent, child);
        _renderer.AddChild(parent, child);
    }
}