using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using GalacticMeltdown.Actors;
using GalacticMeltdown.Items;

namespace GalacticMeltdown.Data;
using AmmoDictionary = Dictionary<string, (int reloadAmount, int reloadEnergy, int minDamage, int maxDamage)>;

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
            if(!Enum.TryParse(node.Name, out ItemCategory itemCategory)) continue;
            string id = "";
            string name = "";
            char symbol = ' ';
            int minHitDamage = 0;
            int maxHitDamage = 0;
            int hitEnergy = 0;
            int ammoCapacity = 0;
            BodyPart bodyPart = BodyPart.Hands;
            AmmoDictionary ammoList = null;
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
                    case "MinHitDamage":
                        minHitDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MaxHitDamage":
                        maxHitDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "HitEnergy":
                        hitEnergy = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "AmmoCapacity":
                        ammoCapacity = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "AmmoTypes":
                        ammoList = ParseAmmoDictionary(locNode);
                        break;
                    case "BodyPart":
                        Enum.TryParse(locNode.InnerText, out bodyPart);
                        break;
                }
            }

            ItemData itemData = itemCategory switch
            {
                ItemCategory.UsableItem => new UsableItemData(symbol, name, id, itemCategory),
                ItemCategory.WearableItem => new WearableItemData(symbol, name, id, itemCategory, bodyPart),
                ItemCategory.WeaponItem => new WeaponItemData(symbol, name, id, itemCategory, minHitDamage, maxHitDamage, hitEnergy,
                    ammoCapacity, ammoList),
                ItemCategory.RangedWeaponItem => new RangedWeaponItemData(symbol, name, id, itemCategory, minHitDamage, maxHitDamage,
                    hitEnergy, ammoCapacity, ammoList),
                _ => new ItemData(symbol, name, id, itemCategory)
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
            int reloadAmount = 1;
            int reloadEnergy = 0;
            int minDamage = 0;
            int maxDamage = 0;
            foreach (XmlNode ammoNode in locNode)
            {
                switch (locNode.Name)
                {
                    case "Id":
                        ammoId = locNode.InnerText;
                        break;
                    case "ReloadAmount":
                        reloadAmount = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "ReloadEnergy":
                        reloadAmount = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MinDamage":
                        minDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MaxDamage":
                        maxDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                }
            }

            dictionary.Add(ammoId, (reloadAmount, reloadEnergy, minDamage, maxDamage));
        }
        return dictionary;
    }
}

public record ItemData(char Symbol, string Name, string Id, ItemCategory Category);

public record WeaponItemData(char Symbol, string Name, string Id, ItemCategory Category, int MinHitDamage,
    int MaxHitDamage, int HitEnergy, int AmmoCapacity, AmmoDictionary AmmoTypes) : ItemData(Symbol, Name, Id, Category);

public record RangedWeaponItemData(char Symbol, string Name, string Id, ItemCategory Category, int MinHitDamage,
        int MaxHitDamage, int HitEnergy, int AmmoCapacity, AmmoDictionary AmmoTypes) //Hit chance not yet included
    : WeaponItemData(Symbol, Name, Id, Category, MinHitDamage, MaxHitDamage, HitEnergy, AmmoCapacity, AmmoTypes);

public record WearableItemData (char Symbol, string Name, string Id, ItemCategory Category, BodyPart BodyPart) 
    : ItemData(Symbol, Name, Id, Category); //WIP

public record UsableItemData(char Symbol, string Name, string Id, ItemCategory Category) : ItemData(Symbol, Name, Id,
    Category); //WIP