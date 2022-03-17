using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;
using TableItems = List<(string lootId, int chance, double gain, int limit)>;

public class LootTableDataExtractor : XmlExtractor
{
    public readonly Dictionary<string, ILoot> LootTables;


    public LootTableDataExtractor()
    {
        LootTables = new Dictionary<string, ILoot>();
        ParseDocument("LootTables.xml");
    }

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            switch (node.Name)
            {
                case "ItemLoot":
                    ParseItemLoot(node);
                    break;
                case "CollectionTable":
                    ParseLootTable(node, true);
                    break;
                case "DistributionTable":
                    ParseLootTable(node, false);
                    break;
            }
        }
    }

    private void ParseItemLoot(XmlNode itemLootNode)
    {
        string id = "";
        string itemId = "";
        int min = 1;
        int max = 1;
        double gain = 0;
        int limit = -1;
        if (itemLootNode.Attributes == null) return;
        foreach (XmlNode node in itemLootNode)
        {
            switch (node.Name)
            {
                case "Id":
                    id = node.InnerText;
                    break;
                case "ItemId":
                    itemId = node.InnerText;
                    break;
                case "Min":
                    min = Convert.ToInt32(node.InnerText);
                    break;
                case "Max":
                    max = Convert.ToInt32(node.InnerText);
                    break;
                case "Gain":
                    gain = Convert.ToDouble(node.InnerText);
                    break;
                case "Limit":
                    limit = Convert.ToInt32(node.InnerText);
                    break;
            }
        }
        LootTables.Add(id, new ItemLoot(id, itemId, min, max, gain, limit));
    }

    private void ParseLootTable(XmlNode tableNode, bool collection)
    {
        string id = "";
        TableItems items = null;
        foreach (XmlNode locNode in tableNode)
        {
            switch (locNode.Name)
            {
                case "Id":
                    id = locNode.InnerText;
                    break;
                case "Items":
                    items = ParseTableItems(locNode);
                    break;
            }
        }

        ILoot table = new LootTable(id, collection, items);
        LootTables.Add(id, table);
    }

    private TableItems ParseTableItems(XmlNode itemsNode)
    {
        TableItems items = new();
        foreach (XmlNode node in itemsNode)
        {
            if (node.Attributes is null) continue;
            string id = "";
            int chance = 100;
            double gain = 0;
            int limit = 100;
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "id":
                        id = attribute.InnerText;
                        break;
                    case "chance":
                        chance = Convert.ToInt32(attribute.InnerText);
                        break;
                    case "limit":
                        limit = Convert.ToInt32(attribute.InnerText);
                        break;
                    case "gain":
                        gain = Convert.ToDouble(attribute.InnerText);
                        break;
                }
            }
            items.Add((id, chance, gain, limit));
        }

        return items;
    }
    
}
public readonly record struct LootTable(string Id, bool IsCollection, TableItems Items) : ILoot;

public readonly record struct ItemLoot(string Id, string ItemId, int Min, int Max, double Gain, int Limit)
    : ILoot;

public interface ILoot { }