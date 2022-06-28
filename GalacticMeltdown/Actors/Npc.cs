using System;
using System.Collections.Generic;
using System.Linq;
using GalacticMeltdown.ActorActions;
using GalacticMeltdown.Behaviors;
using GalacticMeltdown.Data;
using GalacticMeltdown.LevelRelated;
using GalacticMeltdown.Utility;
using Newtonsoft.Json;

namespace GalacticMeltdown.Actors;

public abstract class Npc : Actor
{
    [JsonProperty] protected override string ActorName => "Npc";
    protected NpcTypeData TypeData;
    [JsonIgnore] public override (char symbol, ConsoleColor color) SymbolData => (TypeData.Symbol, TypeData.Color);
    [JsonIgnore] public string Name => TypeData.Name;
    [JsonIgnore] public override ConsoleColor? BgColor => TypeData.BgColor;
    [JsonIgnore] protected int AlertRadius => TypeData.AlertRadius;
    [JsonProperty] public readonly string TypeId;
    [JsonProperty] protected Counter AlertCounter;
    [JsonProperty] public HashSet<Actor> Targets { get; protected set; }
    [JsonIgnore] public Actor CurrentTarget { get; set; }

    [JsonProperty] protected SortedSet<Behavior> Behaviors;

    [JsonConstructor]
    protected Npc()
    {
    }

    protected Npc(NpcTypeData typeData, int x, int y, Level level)
        : base(typeData.MaxHp, typeData.MaxEnergy, typeData.Dexterity, typeData.Defence, typeData.ViewRange, x, y, level)
    {
        TypeData = typeData;
        TypeId = typeData.Id;
        Targets = new HashSet<Actor> {Level.Player};
        AlertCounter = new Counter(Level, 1, 30);
        Died += AlertCounter.RemoveCounter;

        if (TypeData.Behaviors == null) return;

        Behaviors = new SortedSet<Behavior>();
        foreach (BehaviorData behaviorData in TypeData.Behaviors)
        {
            Behavior behavior = behaviorData switch
            {
                MovementStrategyData movementStrategyData => new MovementStrategy(movementStrategyData, this),
                MeleeAttackStrategyData meleeAttackStrategyData => new MeleeAttackStrategy(meleeAttackStrategyData,
                    this),
                RangeAttackStrategyData rangeAttackStrategyData => new RangeAttackStrategy(rangeAttackStrategyData,
                    this),
                SelfEffectStrategyData selfEffectStrategyData => new SelfEffectStrategy(selfEffectStrategyData, this),
                _ => null
            };

            if (behavior is null) continue;
            Behaviors.Add(behavior);
        }
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

    public override ActorActionInfo TakeAction()
    {
        if (!IsActive) return null;
        CurrentTarget = Targets.Where(target => IsPointVisible(target.X, target.Y))
            .MinBy(target => UtilityFunctions.GetDistance(target.X, target.Y, X, Y));
        if (CurrentTarget is null || !AlertCounter.FinishedCounting)
            return Behaviors?.Select(behavior => behavior.TryAct()).FirstOrDefault(el => el is not null);

        Level.AlertEnemies(X, Y, AlertRadius, CurrentTarget);
        AlertCounter.ResetTimer();
        return Behaviors?.Select(behavior => behavior.TryAct()).FirstOrDefault(el => el is not null);
    }

    public override bool Hit(int damage, bool ignoreDexterity, bool ignoreDefence)
    {
        bool hit = base.Hit(damage, ignoreDexterity, ignoreDefence);
        if(hit)
            SendAffected();
        return hit;
    }

    public override void Alert(Actor target)
    {
        if (Behaviors is null) return;
        foreach (var behavior in Behaviors.OfType<MovementStrategy>())
        {
            behavior.PreviousTarget = target;
        }
    }
    
    public List<string> GetDescription()
    {
        List<string> description = new()
        {
            Name,
            "",
            $"HP: {Hp}/{MaxHp}",
            $"Energy: {Energy}/{MaxEnergy}",
            $"Def: {Defence}",
            $"Dex: {Dexterity}",
            $"View Range: {ViewRange}",
            $"Alert Radius: {AlertRadius}"
        };
        if (Behaviors is null) return description;
        foreach (var behavior in Behaviors)
        {
            description.AddRange(behavior.GetDescription());
        }
        return description;
    }
}