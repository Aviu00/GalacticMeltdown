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
            LinkedList<BehaviorData> behaviors = null;
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
                        color = DataHolder.ColorName[locNode.InnerText];
                        break;
                    case "BgColor":
                        bgColor = DataHolder.ColorName[locNode.InnerText];
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

    private LinkedList<BehaviorData> ParseBehaviors(XmlNode node)
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
        ActorStateChangerDataExtractor.ActorStateChangerData actorStateChangerData = null;
        foreach (XmlNode locNode in node)
        {
            switch (node.Name)
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
                    actorStateChangerData = ActorStateChangerDataExtractor.ParseStateChanger(locNode);
                    break;
            }
        }

        return new MeleeAttackStrategyData(priority, minDamage, maxDamage, cooldown, actorStateChangerData);
    }
}
public record EnemyTypeData(string Id, string Name, char Symbol, ConsoleColor Color,
    ConsoleColor BgColor, int MaxHp, int MaxEnergy,  int Defence, int Dexterity, int ViewRange, int Cost, 
    int AlertRadius, LinkedList<BehaviorData> Behaviors);

public record BehaviorData(int? Priority);

public record MovementStrategyData(int? Priority) : BehaviorData(Priority);

public record MeleeAttackStrategyData(int? Priority, int MinDamage, int MaxDamage, int Cooldown,
        ActorStateChangerDataExtractor.ActorStateChangerData ActorStateChangerData)
    : BehaviorData(Priority);
