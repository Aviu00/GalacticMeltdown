using System;
using System.Collections.Generic;
using System.Xml;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.Data;

using AmmoDictionary = Dictionary<string, (int minDamage, int maxDamage,
    LinkedList<ActorStateChangerData> actorStateChangerData)>;

public class ItemTypesExtractor : XmlExtractor
{
    public Dictionary<string, ItemData> ItemTypes { get; }

    public ItemTypesExtractor()
    {
        ItemTypes = new Dictionary<string, ItemData>();
        ParseDocument("Items.xml");
    }

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            if (!Enum.TryParse(node.Name, out ItemCategory itemCategory)) continue;
            string id = "";
            string name = "";
            char symbol = ' ';
            int minHitDamage = 0;
            int maxHitDamage = 0;
            int hitEnergy = 0;
            int shootEnergy = 0;
            int spread = 0;
            BodyPart bodyPart = BodyPart.Hands;
            bool stackable = false;
            AmmoDictionary ammoList = null;
            LinkedList<ActorStateChangerData> actorStateChangerData = null;
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
                    case "Spread":
                        spread = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MinHitDamage":
                        minHitDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MaxHitDamage":
                        maxHitDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "HitEnergy":
                        hitEnergy = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "ShootEnergy":
                        shootEnergy = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "AmmoTypes":
                        ammoList = ParseAmmoDictionary(locNode);
                        break;
                    case "BodyPart":
                        bodyPart = Enum.Parse<BodyPart>(locNode.InnerText);
                        break;
                    case "Stackable":
                        stackable = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "StateChanger":
                        actorStateChangerData ??= new();
                        actorStateChangerData.AddFirst(ActorStateChangerDataExtractor.ParseStateChanger(locNode));
                        break;
                }
            }

            ItemData itemData = itemCategory switch
            {
                ItemCategory.ConsumableItem => new ConsumableItemData(symbol, name, id, itemCategory, stackable,
                    actorStateChangerData),
                ItemCategory.EquippableItem => new WearableItemData(symbol, name, id, itemCategory, bodyPart,
                    actorStateChangerData),
                ItemCategory.WeaponItem => new WeaponItemData(symbol, name, id, itemCategory, minHitDamage,
                    maxHitDamage, hitEnergy, ammoList, actorStateChangerData),
                ItemCategory.RangedWeaponItem => new RangedWeaponItemData(symbol, name, id, itemCategory, minHitDamage,
                    maxHitDamage, hitEnergy, shootEnergy, spread, ammoList, actorStateChangerData),
                _ => new ItemData(symbol, name, id, itemCategory, stackable)
            };
            ItemTypes.Add(itemData.Id, itemData);
        }
    }

    private AmmoDictionary ParseAmmoDictionary(XmlNode node)
    {
        AmmoDictionary dictionary = new();
        foreach (XmlNode locNode in node)
        {
            string ammoId = "";
            int minDamage = 0;
            int maxDamage = 0;
            LinkedList<ActorStateChangerData> actorStateChangerData = null;
            foreach (XmlNode ammoNode in locNode)
            {
                switch (ammoNode.Name)
                {
                    case "Id":
                        ammoId = ammoNode.InnerText;
                        break;
                    case "MinDamage":
                        minDamage = Convert.ToInt32(ammoNode.InnerText);
                        break;
                    case "MaxDamage":
                        maxDamage = Convert.ToInt32(ammoNode.InnerText);
                        break;
                    case "StateChanger":
                        actorStateChangerData ??= new();
                        actorStateChangerData.AddLast(ActorStateChangerDataExtractor.ParseStateChanger(ammoNode));
                        break;
                }
            }

            dictionary.Add(ammoId, (minDamage, maxDamage, actorStateChangerData));
        }

        return dictionary;
    }
}

public record ItemData(char Symbol, string Name, string Id, ItemCategory Category, bool Stackable);

public record WearableItemData
(char Symbol, string Name, string Id, ItemCategory Category, BodyPart BodyPart,
    LinkedList<ActorStateChangerData> ActorStateChangerData) : ItemData(Symbol, Name, Id, Category, false);

public record WeaponItemData(char Symbol, string Name, string Id, ItemCategory Category, int MinHitDamage,
    int MaxHitDamage, int HitEnergy, AmmoDictionary AmmoTypes,
    LinkedList<ActorStateChangerData> ActorStateChangerData) : WearableItemData(Symbol, Name, Id,
    Category, BodyPart.Hands, ActorStateChangerData);

public record RangedWeaponItemData(char Symbol, string Name, string Id, ItemCategory Category, int MinHitDamage,
        int MaxHitDamage, int HitEnergy, int ShootEnergy, int Spread, AmmoDictionary AmmoTypes,
        LinkedList<ActorStateChangerData> ActorStateChangerData)
    : WeaponItemData(Symbol, Name, Id, Category, MinHitDamage, MaxHitDamage, HitEnergy, AmmoTypes,
        ActorStateChangerData);

public record ConsumableItemData(char Symbol, string Name, string Id, ItemCategory Category, bool Stackable,
        LinkedList<ActorStateChangerData> ActorStateChangerData)
    : ItemData(Symbol, Name, Id, Category, Stackable);