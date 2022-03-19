using GalacticMeltdown.Actors;

namespace GalacticMeltdown.Events;

public class StatChangeEventArgs
{
    public Stat Stat { get; }

    public StatChangeEventArgs(Stat stat)
    {
        Stat = stat;
    }
}