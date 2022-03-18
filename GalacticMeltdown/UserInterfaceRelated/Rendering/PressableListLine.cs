namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public abstract class PressableListLine : ListLine
{
    public abstract void Select();

    public abstract void Deselect();

    public abstract void Press();
}