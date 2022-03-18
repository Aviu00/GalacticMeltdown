namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public abstract class SelectableListLine : ListLine
{
    public abstract void Select();

    public abstract void Deselect();
}