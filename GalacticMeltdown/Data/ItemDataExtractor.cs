using System;
using System.Collections.Generic;
using System.Linq;
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

    private void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string id = "";
            string name = "";
            char symbol = ' ';
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
                }
            }

            ItemData itemData = new ItemData(symbol, name, id);
            ItemData.Add(itemData.Id, itemData);
        }
    }
}

public readonly record struct ItemData(char Symbol, string Name, string Id);