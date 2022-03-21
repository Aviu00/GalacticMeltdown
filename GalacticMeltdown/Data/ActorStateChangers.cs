using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Actors.Effects;

namespace GalacticMeltdown.Data;

public static partial class DataHolder
{
    public enum ActorStateChangerType
    {
        Heal,
        Damage,
        Stun,
        ModifyDefence,
        ModifyDexterity,
        ModifyStrength,
        ModifyMaxHealth,
        ModifyMaxEnergy,
        ModifyViewRange,
        ModifySpeed,
        RegenerationEffect,
        PoisonEffect,
        ModifyDexterityEffect,
        ModifyDefenceEffect,
        ModifyStrengthEffect,
        ModifySpeedEffect,
        ModifyViewRangeEffect
    }

    public static readonly Dictionary<ActorStateChangerType, Action<Actor, int, int>> ActorStateChangers = new()
    {
        {
            ActorStateChangerType.Heal, (actor, power, _) =>
            {
                actor.Hp += power;
            }
        },
        {
            ActorStateChangerType.Damage, (actor, power, _) =>
            {
                actor.Hit(power, true, true);
            }
        },
        {
            ActorStateChangerType.Stun, (actor, power, _) =>
            {
                actor.Energy -= power;
            }
        },
        {
            ActorStateChangerType.ModifyDefence, (actor, power, _) =>
            {
                actor.Defence += power;
            }
        },
        {
            ActorStateChangerType.ModifyDexterity, (actor, power, _) =>
            {
                actor.Dexterity += power;
            }
        },
        {
            ActorStateChangerType.ModifyMaxHealth, (actor, power, _) =>
            {
                actor.HpLim.MaxValue += power;
            }
        },
        {
            ActorStateChangerType.ModifyMaxEnergy, (actor, power, _) =>
            {
                actor.EnergyLim.MaxValue += power;
            }
        },
        {
            ActorStateChangerType.ModifyViewRange, (actor, power, _) =>
            {
            }
        },
        {
            ActorStateChangerType.ModifyStrength, (actor, power, _) =>
            {
            }
        },
        {
            ActorStateChangerType.ModifySpeed, (actor, power, _) =>
            {
            }
        },
        {
            ActorStateChangerType.RegenerationEffect, (actor, power, duration) =>
            {
                ContinuousEffect effect = new ContinuousEffect(actor, power, duration, ActorStateChangerType.Heal);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.PoisonEffect, (actor, power, duration) =>
            {
                ContinuousEffect effect = new ContinuousEffect(actor, power, duration, ActorStateChangerType.Damage);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.ModifyDefenceEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifyDefence);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.ModifyDexterityEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifyDexterity);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.ModifyStrengthEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifyStrength);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.ModifySpeedEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifySpeed);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.ModifyViewRangeEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifyViewRange);
                actor.AddEffect(effect);
            }
        },
    };
}