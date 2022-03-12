using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GalacticMeltdown.Data;

public class TileTypesExtractor : XmlExtractor
{
    public readonly Dictionary<string, TileTypeData> TileTypes;
    
    private static readonly string[] Directions =
    {
        "nesw", "nes", "new", "nsw", "esw", "ns", "ew", "ne", "es", "sw", "nw", "n", "e", "s", "w"
    };

    public TileTypesExtractor()
    {
        TileTypes = new Dictionary<string, TileTypeData>();
        ParseDocument("Tiles.xml");
        ParseDocument("TilesExtra.xml");
    }

    private void ParseDocument(string docName)
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
            char[] symbols = null;
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
                        color = DataHolder.ColorName[locNode.InnerText];
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
                    case "ConnectionSymbols":
                        symbols = locNode.InnerText.Split().Select(char.Parse).ToArray();
                        break;
                }
            }

            //log an error if id is null or TileTypes contains id
            TileTypeData tileTypeData = new TileTypeData(symbol, color, isWalkable, isTransparent, name, id,
                isConnection, isConnectable, isDependingOnRoomConnection);
            TileTypes.Add(tileTypeData.Id, tileTypeData);
            if (symbols is null || !isConnectable) continue;
            //generate connections
            //log an error if symbols length is not 15
            for (int i = 0; i < 15; i++)
            {
                TileTypeData connectionData = new TileTypeData(symbols[i], color, isWalkable, isTransparent, name,
                    id + "_" + Directions[i], isConnection, false, false);
                TileTypes.Add(connectionData.Id, connectionData);
            }
        }
    }
}

public readonly record struct TileTypeData(char Symbol, ConsoleColor Color, bool IsWalkable, bool IsTransparent,
    string Name, string Id, bool IsConnection, bool IsConnectable, bool IsDependingOnRoomConnection);