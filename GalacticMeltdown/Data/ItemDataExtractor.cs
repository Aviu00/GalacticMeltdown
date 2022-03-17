using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;

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
            int minShootDamage = 0;
            int maxShootDamage = 0;
            string ammoId = "";
            int ammoCapacity = 0;
            int reloadEnergy = 0;
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
                    case "MinShootDamage":
                        minShootDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "MaxShootDamage":
                        maxShootDamage = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "AmmoId":
                        ammoId = locNode.InnerText;
                        break;
                    case "AmmoCapacity":
                        ammoCapacity = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "ReloadEnergy":
                        reloadEnergy = Convert.ToInt32(locNode.InnerText);
                        break;
                }
            }

            ItemData itemData = itemType switch
            {
                ItemType.UsableItem => new UsableItemData(symbol, name, id),
                ItemType.WearableItem => new WearableItemData(symbol, name, id),
                ItemType.WeaponItem => new WeaponItemData(symbol, name, id, minHitDamage, maxHitDamage, hitEnergy),
                ItemType.RangedWeaponItem => new RangedWeaponItemData(symbol, name, id, minHitDamage, maxHitDamage,
                    hitEnergy, minShootDamage, maxShootDamage, ammoId, ammoCapacity, reloadEnergy),
                _ => new ItemData(symbol, name, id)
            };
            ItemData.Add(itemData.Id, itemData);
        }
    }
}

public record ItemData(char Symbol, string Name, string Id);

public record WeaponItemData(char Symbol, string Name, string Id, int MinHitDamage, int MaxHitDamage, int HitEnergy)
    : ItemData(Symbol, Name, Id);

public record RangedWeaponItemData(char Symbol, string Name, string Id, int MinHitDamage, int MaxHitDamage,
        int HitEnergy, int MinShootDamage, int MaxShootDamage, string AmmoId, int AmmoCapacity, 
        int ReloadEnergy) //Hit chance not yet included
    : WeaponItemData(Symbol, Name, Id, MinHitDamage, MaxHitDamage, HitEnergy);

public record WearableItemData(char Symbol, string Name, string Id) : ItemData(Symbol, Name, Id); //WIP

public record UsableItemData(char Symbol, string Name, string Id) : ItemData(Symbol, Name, Id); //WIP