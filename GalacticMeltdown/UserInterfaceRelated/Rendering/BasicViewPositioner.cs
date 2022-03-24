namespace GalacticMeltdown.UserInterfaceRelated.Rendering;

public class BasicViewPositioner : ViewPositioner
{
    public override void Resize(int width, int height)
    {
        base.Resize(width, height);
        foreach (var view in ViewPositions.Keys)
        {
            ViewPositions[view] = (0, 0, Width, Height);
        }
    }
}