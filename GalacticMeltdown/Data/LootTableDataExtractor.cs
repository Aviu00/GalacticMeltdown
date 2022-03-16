using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;
using CollectionItems = List<(string id, int chance, int min, int max)>;
using DistributionItems = List<(string lootId , int chance)>;

public class LootTableDataExtractor : XmlExtractor
{
    public readonly Dictionary<string, ITable> LootTables;
    
    
    public LootTableDataExtractor()
    {
        LootTables = new Dictionary<string, ITable>();
        ParseDocument("LootTables.xml");
    }

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string id = "";
            ICollection items = null;
            bool collection = true;
            foreach (XmlNode locNode in node)
            {
                switch (node.Name)
                {
                    case "CollectionTable":
                        collection = true;
                        break;
                    case "DistributionTable":
                        collection = false;
                        break;
                    default:
                        continue;
                }

                switch (locNode.Name)
                {
                    case "Id":
                        id = locNode.InnerText;
                        break;
                    case "Items":
                        items = collection ? ParseCollectionItems(locNode) : ParseDistributionItems(locNode);
                        break;
                }
            }

            ITable table = collection
                ? new CollectionTable(id, (CollectionItems) items)
                : new DistributionTable(id, (DistributionItems) items);
            LootTables.Add(id, table);
        }
    }

    private DistributionItems ParseDistributionItems(XmlNode itemsNode)
    {
        DistributionItems items = new();
        foreach (XmlElement node in itemsNode)
        {
            string id = "";
            int chance = 0;
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
                }
            }

            items.Add((id, chance));
        }

        return items;
    }

    private CollectionItems ParseCollectionItems(XmlNode itemsNode)
    {
        CollectionItems items = new();
        foreach (XmlNode node in itemsNode)
        {
            if (node.Attributes is null) continue;
            string id = "";
            int chance = 100;
            int min = 1;
            int max = 1;
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
                    case "min":
                        min = Convert.ToInt32(attribute.InnerText);
                        break;
                    case "max":
                        max = Convert.ToInt32(attribute.InnerText);
                        break;
                }
            }
            items.Add((id, chance, min, max));
        }

        return items;
    }
    
}
public readonly record struct CollectionTable(string Id, CollectionItems Items) : ITable;

public readonly record struct DistributionTable(string Id, DistributionItems Items) : ITable;

public interface ITable { }