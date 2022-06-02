using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;

public class EnemyTypesExtractor : XmlExtractor
{
    public Dictionary<string, EnemyTypeData> EnemiesTypes { get; }
    
    public EnemyTypesExtractor()
    {
        EnemiesTypes = new Dictionary<string, EnemyTypeData>();
        ParseDocument("Enemies.xml");
    }
    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string id = "";
            string name = "";
            char symbol = ' ';
            ConsoleColor color = ConsoleColor.White;
            ConsoleColor bgColor = ConsoleColor.Black;
            int maxHp = 10;
            int maxEnergy = 20;
            int defence = 0;
            int dexterity  = 0;
            int viewRange = 1;
            int cost = 10;
            int alertRadius = 10;
            IEnumerable<BehaviorData> behaviors = null;
            foreach (XmlNode locNode in node)
            {
                switch (locNode.Name)
                {
                    case "Id":
                        id = locNode.InnerText;
                        break;
                    case "Name":
                        name = locNode.InnerText;
                        break;
                    case "Symbol":
                        symbol = Convert.ToChar(locNode.InnerText);
                        break;
                    case "Color":
                        color = Names.Colors[locNode.InnerText];
                        break;
                    case "BgColor":
                        bgColor = Names.Colors[locNode.InnerText];
                        break;
                    case "MaxEnergy":
                        maxEnergy = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MaxHp":
                        maxHp = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Defence":
                        defence = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Dexterity":
                        dexterity  = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "ViewRange":
                        viewRange = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Cost":
                        cost = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Behaviors":
                        behaviors = ParseBehaviors(locNode);
                        break;
                    case "AlertRadius":
                        alertRadius = Convert.ToInt32(locNode.InnerText);
                        break;
                }
            }

            EnemyTypeData enemiesTypeData = new EnemyTypeData(id, name, symbol, color, bgColor, maxHp, maxEnergy, 
                defence, dexterity , viewRange, cost, alertRadius, behaviors);
            EnemiesTypes.Add(enemiesTypeData.Id, enemiesTypeData);
        }
    }

    private IEnumerable<BehaviorData> ParseBehaviors(XmlNode node)
    {
        LinkedList<BehaviorData> behaviors = new();
        foreach (XmlNode locNode in node)
        {
            switch (locNode.Name)
            {
                case "Movement":
                    behaviors.AddLast(ParseMovementStrategyData(locNode));
                    break;
                case "MeleeAttack":
                    behaviors.AddLast(ParseMeleeAttackStrategyData(locNode));
                    break;
                case "RangeAttack":
                    behaviors.AddLast(ParseRangeAttackStrategyData(locNode));
                    break;
                case "SelfEffect":
                    behaviors.AddLast(SelfEffectStrategyData(locNode));
                    break;
            }
        }

        return behaviors;
    }

    private BehaviorData ParseMovementStrategyData(XmlNode node)
    {
        int? priority = null;
        if (node.ChildNodes.Count > 0 && node.ChildNodes[0]?.Name == "Priority")
            priority = Convert.ToInt32(node.ChildNodes[0].InnerText);
        return new MovementStrategyData(priority);
    }

    private BehaviorData ParseMeleeAttackStrategyData(XmlNode node)
    {
        int? priority = null;
        int minDamage = 0;
        int maxDamage = 0;
        int cooldown = 0;
        LinkedList<ActorStateChangerData> actorStateChangerData = null;
        int meleeAttackCost = 10;
        foreach (XmlNode locNode in node)
        {
            switch (locNode.Name)
            {
                case "Priority":
                    priority = Convert.ToInt32(locNode.InnerText);
                    break;
                case "MinDamage":
                    minDamage = Convert.ToInt32(locNode.InnerText);
                    break;
                case "MaxDamage":
                    maxDamage = Convert.ToInt32(locNode.InnerText);
                    break;
                case "Cooldown":
                    cooldown = Convert.ToInt32(locNode.InnerText);
                    break;
                case "StateChanger":
                    actorStateChangerData ??= new();
                    actorStateChangerData.AddLast(ActorStateChangerDataExtractor.ParseStateChanger(locNode));
                    break;
                case "MeleeAttackCost":
                    meleeAttackCost = Convert.ToInt32(locNode.InnerText);
                    break;
            }
        }

        return new 
            MeleeAttackStrategyData(priority, minDamage, maxDamage, cooldown, meleeAttackCost, actorStateChangerData);
    }
    private BehaviorData ParseRangeAttackStrategyData(XmlNode node)
    {
        int? priority = null;
        int minDamage = 0;
        int maxDamage = 0;
        int cooldown = 0;
        int rangeAttackCost = 10;
        int attackRange = 1;
        int spread = 1;
        LinkedList<ActorStateChangerData> actorStateChangerData = null;
        foreach (XmlNode locNode in node)
        {
            switch (locNode.Name)
            {
                case "Priority":
                    priority = Convert.ToInt32(locNode.InnerText);
                    break;
                case "MinDamage":
                    minDamage = Convert.ToInt32(locNode.InnerText);
                    break;
                case "MaxDamage":
                    maxDamage = Convert.ToInt32(locNode.InnerText);
                    break;
                case "Cooldown":
                    cooldown = Convert.ToInt32(locNode.InnerText);
                    break;
                case "RangeAttackCost":
                    rangeAttackCost = Convert.ToInt32(locNode.InnerText);
                    break;
                case "AttackRange":
                    attackRange = Convert.ToInt32(locNode.InnerText);
                    break;
                case "Spread":
                    spread = Convert.ToInt32(locNode.InnerText);
                    break;
                case "StateChanger":
                    actorStateChangerData ??= new();
                    actorStateChangerData.AddLast(ActorStateChangerDataExtractor.ParseStateChanger(locNode));
                    break;
            }
        }

        return new RangeAttackStrategyData(priority, minDamage, maxDamage, cooldown, rangeAttackCost, attackRange, spread,
            actorStateChangerData);
    }
    private BehaviorData SelfEffectStrategyData(XmlNode node)
    {
        int? priority = null;
        int cooldown = 0;
        int selfEffectCost = 10;
        bool activateWhenTargetIsVisible = false;
        LinkedList<ActorStateChangerData> actorStateChangerData = null;
        foreach (XmlNode locNode in node)
        {
            switch (locNode.Name)
            {
                case "Priority":
                    priority = Convert.ToInt32(locNode.InnerText);
                    break;
                case "Cooldown":
                    cooldown = Convert.ToInt32(locNode.InnerText);
                    break;
                case "SelfEffectCost":
                    selfEffectCost = Convert.ToInt32(locNode.InnerText);
                    break;
                case "ActivateIfTargetIsVisible":
                    activateWhenTargetIsVisible = Convert.ToBoolean(locNode.InnerText);
                    break;
                case "StateChanger":
                    actorStateChangerData ??= new();
                    actorStateChangerData.AddLast(ActorStateChangerDataExtractor.ParseStateChanger(locNode));
                    break;
            }
        }

        return new SelfEffectStrategyData(priority, cooldown, selfEffectCost, activateWhenTargetIsVisible,
            actorStateChangerData);
    }
    
    
}
public record EnemyTypeData(string Id, string Name, char Symbol, ConsoleColor Color,
    ConsoleColor BgColor, int MaxHp, int MaxEnergy,  int Defence, int Dexterity, int ViewRange, int Cost, 
    int AlertRadius, IEnumerable<BehaviorData> Behaviors);

public record BehaviorData(int? Priority);

public record MovementStrategyData(int? Priority) : BehaviorData(Priority);

public record MeleeAttackStrategyData(int? Priority, int MinDamage, int MaxDamage, int Cooldown, int MeleeAttackCost, 
        LinkedList<ActorStateChangerData> ActorStateChangerData)
    : BehaviorData(Priority);

public record RangeAttackStrategyData(int? Priority, int MinDamage, int MaxDamage, int Cooldown, int RangeAttackCost,
        int AttackRange, int Spread, LinkedList<ActorStateChangerData> ActorStateChangerData)
    : BehaviorData(Priority);

public record SelfEffectStrategyData(int? Priority, int Cooldown, int SelfEffectCost, bool ActivateIfTargetIsVisible,
    LinkedList<ActorStateChangerData> ActorStateChangerData) : BehaviorData(Priority);
