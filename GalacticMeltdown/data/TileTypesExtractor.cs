using System;
using System.Collections.Generic;
using System.Xml;

namespace GalacticMeltdown.data;

public class TileTypesExtractor : XmlExtractor
{
    public Dictionary<string, TileTypeData> TileTypes { get; }

    public TileTypesExtractor()
    {
        TileTypes = new Dictionary<string, TileTypeData>();
        ParseDocument("Tile.xml");
    }

    protected sealed override void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string id = "";
            string name = "";
            bool isWalkable = false;
            bool isTransparent = false;
            char symbol = ' ';
            ConsoleColor color = ConsoleColor.White;
            bool isConnection = false;
            bool isConnectable = false;
            bool isDependingOnRoomConnection = false;
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
                    case "IsWalkable":
                        isWalkable = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "IsTransparent":
                        isTransparent = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "Color":
                        color = Utility.Colors[locNode.InnerText];
                        break;
                    case "IsConnection":
                        isConnection = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "IsConnectable":
                        isConnectable = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "IsDependingOnRoomConnection":
                        isDependingOnRoomConnection = Convert.ToBoolean(locNode.InnerText);
                        break;
                }
            }
            
            TileTypeData tileTypeData = 
                new TileTypeData(symbol, color, isWalkable, isTransparent, name, id, isConnection, isConnectable, 
                    isDependingOnRoomConnection);
            TileTypes.Add(id, tileTypeData);
        }
    }
}

public record struct TileTypeData(char Symbol, ConsoleColor Color, bool IsWalkable, bool IsTransparent, string Name, string Id,
    bool IsConnection, bool IsConnectable, bool IsDependingOnRoomConnection);