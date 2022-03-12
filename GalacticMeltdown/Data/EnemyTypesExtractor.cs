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

                }
            }

            //log an error if id is null or TileTypes contains id
            EnemyTypeData enemiesTypeData = new EnemyTypeData(id, name, symbol, color, bgColor, maxHp, maxEnergy, 
                defence, dexterity ,  viewRange);
            EnemiesTypes.Add(enemiesTypeData.Id, enemiesTypeData);
        }

    }
}
public readonly record struct EnemyTypeData(string Id, string Name, char Symbol, ConsoleColor Color, ConsoleColor BgColor,  
    int MaxHp, int MaxEnergy,  int Def, int Dex, int ViewRange);