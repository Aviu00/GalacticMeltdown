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

    public static void TakeControl(object obj)
    {
        _inputProcessor.TakeControl(obj);
    }

    public static void YieldControl(object obj)
    {
        _inputProcessor.YieldControl(obj);
    }

    public static void Forget(object obj)
    {
        if (obj is null) return;
        _renderer.Forget(obj);
        _inputProcessor.Forget(obj);
    }

    public static void AddChild(object parent, object child)
    {
        _renderer.AddChild(parent, child);
        _inputProcessor.AddChild(parent, child);
    }
}