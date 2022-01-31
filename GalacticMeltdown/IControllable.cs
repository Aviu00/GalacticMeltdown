namespace GalacticMeltdown;

public interface IControllable : IHasCoords
{
    public bool TryMove(int deltaX, int deltaY);
}