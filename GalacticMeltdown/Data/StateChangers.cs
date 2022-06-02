using System;
using System.Collections.Generic;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Actors.Effects;

namespace GalacticMeltdown.Data;

public enum StateChangerType
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

public static class StateChangerData
{
    public static readonly Dictionary<StateChangerType, Action<Actor, int, int>> StateChangers = new()
    {
        {
            StateChangerType.Heal, (actor, power, _) => { actor.Hp += power; }
        },
        {
            StateChangerType.Damage, (actor, power, _) => { actor.Hit(power, true, true); }
        },
        {
            StateChangerType.Stun, (actor, power, _) => { actor.Energy -= power; }
        },
        {
            StateChangerType.ModifyDefence, (actor, power, _) => { actor.Defence += power; }
        },
        {
            StateChangerType.ModifyDexterity, (actor, power, _) => { actor.Dexterity += power; }
        },
        {
            StateChangerType.ModifyMaxHealth, (actor, power, _) => { actor.MaxHp += power; }
        },
        {
            StateChangerType.ModifyMaxEnergy, (actor, power, _) => { actor.MaxEnergy += power; }
        },
        {
            StateChangerType.ModifyViewRange, (actor, power, _) => { actor.ViewRange += power; }
        },
        {
            StateChangerType.ModifyStrength, (actor, power, _) => { actor.Strength += power; }
        },
        {
            StateChangerType.ModifySpeed, (actor, power, _) => { actor.MoveSpeed += power; }
        },
        {
            StateChangerType.RegenerationEffect, (actor, power, duration) =>
            {
                ContinuousEffect effect = new ContinuousEffect(actor, power, duration, StateChangerType.Heal);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.PoisonEffect, (actor, power, duration) =>
            {
                ContinuousEffect effect = new ContinuousEffect(actor, power, duration, StateChangerType.Damage);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifyMaxEnergyEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifyMaxEnergy);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifyMaxHealthEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifyMaxHealth);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifyDefenceEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifyDefence);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifyDexterityEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifyDexterity);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifyStrengthEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifyStrength);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifySpeedEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifySpeed);
                actor.AddEffect(effect);
            }
        },
        {
            StateChangerType.ModifyViewRangeEffect, (actor, power, duration) =>
            {
                FlatEffect effect = new FlatEffect(actor, power, duration, StateChangerType.ModifyViewRange);
                actor.AddEffect(effect);
            }
        }
    };

    public static readonly Dictionary<StateChangerType, Func<int, int, string>> StateChangerDescriptions = new()
    {
        {
            StateChangerType.Heal, (power, _) => $"Heals {power} HP"
        },
        {
            StateChangerType.Damage, (power, _) => $"Deals {power} damage"
        },
        {
            StateChangerType.Stun, (power, _) => $"Stuns for {power} energy"
        },
        {
            StateChangerType.ModifyDefence, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" defence by {Math.Abs(power)}"
        },
        {
            StateChangerType.ModifyDexterity, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" dexterity by {Math.Abs(power)}"
        },
        {
            StateChangerType.ModifyMaxHealth, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" max HP by {Math.Abs(power)}"
        },
        {
            StateChangerType.ModifyMaxEnergy, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" max energy by {Math.Abs(power)}"
        },
        {
            StateChangerType.ModifyViewRange, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" view range by {Math.Abs(power)}"
        },
        {
            StateChangerType.ModifyStrength, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" strength by {Math.Abs(power)}"
        },
        {
            StateChangerType.ModifySpeed, (power, _) =>
                (power < 0 ? "Decreases" : "Increases") + $" move speed by {Math.Abs(power)}"
        },
        {
            StateChangerType.RegenerationEffect, (power, duration) =>
                $"Regenerates {power} HP each turn for a total of {duration} turns"
        },
        {
            StateChangerType.PoisonEffect, (power, duration) =>
                $"Applies poison effect, dealing {power} damage each turn for a total of {duration} turns"
        },
        {
            StateChangerType.ModifyMaxEnergyEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifyMaxEnergy, power, duration)
        },
        {
            StateChangerType.ModifyMaxHealthEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifyMaxHealth, power, duration)
        },
        {
            StateChangerType.ModifyDefenceEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifyDefence, power, duration)
        },
        {
            StateChangerType.ModifyDexterityEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifyDexterity, power, duration)
        },
        {
            StateChangerType.ModifyStrengthEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifyStrength, power, duration)
        },
        {
            StateChangerType.ModifySpeedEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifySpeed, power, duration)
        },
        {
            StateChangerType.ModifyViewRangeEffect, (power, duration) =>
                GetModificationEffectDescription(StateChangerType.ModifyViewRange, power, duration)
        }
    };

    private static string GetModificationEffectDescription(StateChangerType baseStateChanger, int power,
        int duration) => StateChangerDescriptions[baseStateChanger](power, duration) + $" for {duration} turns";
}