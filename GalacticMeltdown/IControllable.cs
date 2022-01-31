namespace GalacticMeltdown;

public interface IControllable
{
    public bool TryMove(int deltaX, int deltaY);
}