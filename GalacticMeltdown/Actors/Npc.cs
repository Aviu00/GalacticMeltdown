using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public abstract class Npc : Actor
{
    [JsonProperty] protected override string ActorName => "Npc";
    [JsonProperty] protected HashSet<Actor> Targets { get; set; }
    [JsonIgnore] public Actor CurrentTarget { get; set; }

    [JsonProperty] private readonly string _id;

    [JsonProperty] protected SortedSet<Behavior> Behaviors;

    [JsonConstructor]
    protected Npc()
    {
    }
    protected Npc(int maxHp, int maxEnergy, int dexterity, int defence, int viewRange, int x, int y, Level level)
        : base(maxHp, maxEnergy, dexterity, defence, viewRange, x, y, level)
    {
        _id = UtilityFunctions.RandomString(16);
    }

    protected bool IsPointVisible(int x, int y)
    {
        if (UtilityFunctions.GetDistance(x, y, X, Y) > GetViewRange())
        {
            return false;
        }

        return Algorithms.BresenhamGetPointsOnLine(X, Y, x, y)
            .All(coord =>
            {
                Tile tile = Level.GetTile(coord.x, coord.y);
                return tile is not null && Level.GetTile(coord.x, coord.y).IsTransparent;
            });
    }

    public void MoveNpcTo(int x, int y) => MoveTo(x, y);

    public override void TakeAction()
    {
        Behaviors?.Any(behavior => behavior.TryAct());
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj);
    }

    public override bool Hit(int damage, bool ignoreDexterity, bool ignoreDefence)
    {
        bool hit = base.Hit(damage, ignoreDexterity, ignoreDefence);
        if(hit)
            SendAffected();
        return hit;
    }
}