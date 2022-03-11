using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.Data;

public class EnemiesTypesExtractor:XmlExtractor
{
    public Dictionary<string, EnemiesTypeData> EnemiesTypes { get; }
    
    public EnemiesTypesExtractor()
    {
        EnemiesTypes = new Dictionary<string, EnemiesTypeData>();
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
            int dev = 0;
            int dex = 0;
            int viewRange = 1;
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
                    case "Dev":
                        dev = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Dex":
                        dex = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "ViewRange":
                        viewRange = Convert.ToInt32(locNode.InnerText);
                        break;

                }
            }

            //log an error if id is null or TileTypes contains id
            EnemiesTypeData enemiesTypeData = new EnemiesTypeData(id, name, symbol, color, bgColor, maxHp, maxEnergy, 
                dev, dex,  viewRange);
            EnemiesTypes.Add(enemiesTypeData.Id, enemiesTypeData);
            for (int i = 0; i < 15; i++)
            {
                EnemiesTypeData connectionData = new EnemiesTypeData(id, name, symbol, color, bgColor, maxHp, maxEnergy,
                    dev, dex, viewRange);
                EnemiesTypes.Add(connectionData.Id, connectionData);
            }
        }

    }
}
public readonly record struct EnemiesTypeData(string Id, string Name, char Symbol, ConsoleColor Color, ConsoleColor BgColor,  
    int MaxHp, int MaxEnergy,  int Dev, int Dex, int ViewRange);