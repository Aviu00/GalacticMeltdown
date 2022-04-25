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
        ModifyMaxHealthEffect,
        ModifyMaxEnergyEffect,
        ModifyDexterityEffect,
        ModifyDefenceEffect,
        ModifyStrengthEffect,
        ModifySpeedEffect,
        ModifyViewRangeEffect
    }

    public static readonly Dictionary<ActorStateChangerType, Action<Actor, int, int>> ActorStateChangers = new()
    {
        {
            ActorStateChangerType.Heal, (actor, power, _) => { actor.Hp += power; }
        },
        {
            ActorStateChangerType.Damage, (actor, power, _) => { actor.Hit(power, true, true); }
        },
        {
            ActorStateChangerType.Stun, (actor, power, _) => { actor.Energy -= power; }
        },
        {
            ActorStateChangerType.ModifyDefence, (actor, power, _) => { actor.Defence += power; }
        },
        {
            ActorStateChangerType.ModifyDexterity, (actor, power, _) => { actor.Dexterity += power; }
        },
        {
            ActorStateChangerType.ModifyMaxHealth, (actor, power, _) => { actor.MaxHp += power; }
        },
        {
            ActorStateChangerType.ModifyMaxEnergy, (actor, power, _) => { actor.MaxEnergy += power; }
        },
        {
            ActorStateChangerType.ModifyViewRange, (actor, power, _) => { actor.ViewRange += power; }
        },
        {
            ActorStateChangerType.ModifyStrength, (actor, power, _) => { actor.Strength += power; }
        },
        {
            ActorStateChangerType.ModifySpeed, (actor, power, _) => { actor.MoveSpeed += power; }
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
            ActorStateChangerType.ModifyMaxEnergyEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifyMaxEnergy);
                actor.AddEffect(effect);
            }
        },
        {
            ActorStateChangerType.ModifyMaxHealthEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, ActorStateChangerType.ModifyMaxHealth);
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
        }
    };

    public static readonly Dictionary<ActorStateChangerType, Func<int, int, string>> StateChangerDescriptions = new()
    {
        {
            ActorStateChangerType.Heal, (power, _) => $"Heals {power} HP"
        },
        {
            ActorStateChangerType.Damage, (power, _) => $"Deals {power} damage"
        },
        {
            ActorStateChangerType.Stun, (power, _) => $"Stuns for {power} energy"
        },
        {
            ActorStateChangerType.ModifyDefence, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" defence by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.ModifyDexterity, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" dexterity by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.ModifyMaxHealth, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" max HP by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.ModifyMaxEnergy, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" max energy by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.ModifyViewRange, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" view range by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.ModifyStrength, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" strength by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.ModifySpeed, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" move speed by {Math.Abs(power)}"
        },
        {
            ActorStateChangerType.RegenerationEffect, (power, duration) =>
                $"Regenerates {power} HP each turn for a total of {duration} turns"
        },
        {
            ActorStateChangerType.PoisonEffect, (power, duration) =>
                $"Applies poison effect, dealing {power} damage each turn for a total of {duration} turns"
        },
        {
            ActorStateChangerType.ModifyMaxEnergyEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifyMaxEnergy, power, duration)
        },
        {
            ActorStateChangerType.ModifyMaxHealthEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifyMaxHealth, power, duration)
        },
        {
            ActorStateChangerType.ModifyDefenceEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifyDefence, power, duration)
        },
        {
            ActorStateChangerType.ModifyDexterityEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifyDexterity, power, duration)
        },
        {
            ActorStateChangerType.ModifyStrengthEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifyStrength, power, duration)
        },
        {
            ActorStateChangerType.ModifySpeedEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifySpeed, power, duration)
        },
        {
            ActorStateChangerType.ModifyViewRangeEffect, (power, duration) =>
                GetModificationEffectDescription(ActorStateChangerType.ModifyViewRange, power, duration)
        }
    };

    private static string GetModificationEffectDescription(ActorStateChangerType baseStateChanger, int power,
        int duration) => StateChangerDescriptions[baseStateChanger](power, duration) + $" for {duration} turns";
}