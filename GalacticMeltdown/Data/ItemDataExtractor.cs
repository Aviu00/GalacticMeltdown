using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;
using AmmoDictionary = Dictionary<string, (int reloadAmount, int reloadEnergy, int minDamage, int maxDamage)>;

public class ItemDataExtractor : XmlExtractor
{
    public Dictionary<string, ItemData> ItemData { get; }

    public ItemDataExtractor()
    {
        ItemData = new Dictionary<string, ItemData>();
        ParseDocument("Items.xml");
    }

    private enum ItemType
    {
        Item,
        WeaponItem,
        RangedWeaponItem,
        UsableItem,
        WearableItem
    }

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            if(!Enum.TryParse(node.Name, out ItemType itemType)) continue;
            string id = "";
            string name = "";
            char symbol = ' ';
            int minHitDamage = 0;
            int maxHitDamage = 0;
            int hitEnergy = 0;
            int ammoCapacity = 0;
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
                }
            }

            ItemData itemData = itemType switch
            {
                ItemType.UsableItem => new UsableItemData(symbol, name, id),
                ItemType.WearableItem => new WearableItemData(symbol, name, id),
                ItemType.WeaponItem => new WeaponItemData(symbol, name, id, minHitDamage, maxHitDamage, hitEnergy,
                    ammoCapacity, ammoList),
                ItemType.RangedWeaponItem => new RangedWeaponItemData(symbol, name, id, minHitDamage, maxHitDamage,
                    hitEnergy, ammoCapacity, ammoList),
                _ => new ItemData(symbol, name, id)
            };
            ItemData.Add(itemData.Id, itemData);
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

public record ItemData(char Symbol, string Name, string Id);

public record WeaponItemData(char Symbol, string Name, string Id, int MinHitDamage, int MaxHitDamage, int HitEnergy,
        int AmmoCapacity, AmmoDictionary AmmoTypes)
    : ItemData(Symbol, Name, Id);

public record RangedWeaponItemData(char Symbol, string Name, string Id, int MinHitDamage, int MaxHitDamage,
        int HitEnergy, int AmmoCapacity, 
        AmmoDictionary AmmoTypes) //Hit chance not yet included
    : WeaponItemData(Symbol, Name, Id, MinHitDamage, MaxHitDamage, HitEnergy, AmmoCapacity, AmmoTypes);

public record WearableItemData(char Symbol, string Name, string Id) : ItemData(Symbol, Name, Id); //WIP

public record UsableItemData(char Symbol, string Name, string Id) : ItemData(Symbol, Name, Id); //WIP