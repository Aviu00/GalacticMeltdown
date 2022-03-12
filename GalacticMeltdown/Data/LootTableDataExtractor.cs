using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;
using TableItems = List<(string id, int chance, int min, int max)>;

public class LootTableDataExtractor : XmlExtractor
{
    public readonly Dictionary<string, LootTable> LootTables;
    
    
    public LootTableDataExtractor()
    {
        LootTables = new Dictionary<string, LootTable>();
        ParseDocument("LootTables.xml");
    }

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string id = "";
            TableItems items = null;
            foreach (XmlNode locNode in node)
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
            
            LootTable table = new LootTable(id, items);
            LootTables.Add(id, table);
        }
    }

    public TableItems ParseTableItems(XmlNode itemsNode)
    {
        TableItems items = new();
        foreach (XmlNode node in itemsNode)
        {
            if (node.Attributes is null) continue;
            string id = "";
            int chance = 0;
            int min = 0;
            int max = 0;
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
public readonly record struct LootTable(string Id, TableItems Items);