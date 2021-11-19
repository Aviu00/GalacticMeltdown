namespace GalacticMeltdown
{
    public abstract class Entity : GameObject //Entity is an object that can be placed on a tile
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}