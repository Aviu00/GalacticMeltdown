namespace GalacticMeltdown
{
    public interface IMovable
    {
        MoveBehavior MoveBehavior { get; }

        public void Move(int relX, int relY);
    }
}