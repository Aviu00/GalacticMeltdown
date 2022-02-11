using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace GalacticMeltdown.data;

public class RoomDataExtractor : XmlExtractor
{
    public List<Room> Rooms { get; }

    private readonly Dictionary<string, TileTypeData> _tileTypes;

    public RoomDataExtractor(Dictionary<string, TileTypeData> tileTypes)
    {
        _tileTypes = tileTypes;
        Rooms = new List<Room>();
        ParseDocument("Rooms.xml");
    }

    protected sealed override void ParseDocument(string docName)
    {
        XmlDocument doc = GetXmlDocument(docName);
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            string stringPattern = "";
            int type = 0;
            int commonness = 0;
            bool rotationalSymmetry = false;
            bool horizontalSymmetry = false;
            Dictionary<char, string> roomTerrain = null;
            foreach (XmlNode locNode in node)
            {
                switch (locNode.Name)
                {
                    case "Pattern":
                        stringPattern = locNode.InnerText;
                        break;
                    case "Commonness":
                        commonness = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Type":
                        type = Convert.ToInt32(locNode.InnerText);
                        break;
                    case "Terrain":
                        roomTerrain = ParseTerrain(locNode);
                        break;
                    case "Rotational Symmetry":
                        rotationalSymmetry = Convert.ToBoolean(locNode.InnerText);
                        break;
                    case "Horizontal Symmetry":
                        horizontalSymmetry = Convert.ToBoolean(locNode.InnerText);
                        break;
                }
            }

            TileTypeData[,] pattern = ConvertPattern(stringPattern, roomTerrain);
            Rooms.Add(new Room(pattern, type, commonness, rotationalSymmetry, horizontalSymmetry));
        }
    }

    /// <summary>
    /// This will be used later to get information about loot table placement
    /// </summary>
    private Dictionary<char, string> ParseTerrain(XmlNode terrainNode)
    {
        Dictionary<char, string> roomTerrain = new();
        foreach (XmlNode node in terrainNode)
        {
            if (node.Attributes is null)
                continue;
            char symbol = ' ';
            string id = "";
            foreach (XmlAttribute attribute in node.Attributes)
            {
                switch (attribute.Name)
                {
                    case "char":
                        symbol = Convert.ToChar(attribute.InnerText);
                        break;
                    case "tileId":
                        id = attribute.InnerText;
                        break;
                }
            }
            roomTerrain.Add(symbol, id);
        }

        return roomTerrain;
    }

    private TileTypeData[,] ConvertPattern(string stringPattern, Dictionary<char, string> roomTerrain)
    {
        int i = 0;
        int j = -1;
        TileTypeData[,] terrainObjects = new TileTypeData[24, 24];
        foreach (char c in stringPattern)
        {
            switch (c)
            {
                case ' ' or '\r':
                    continue;
                case '\n':
                    i = 0;
                    j++;
                    continue;
            }
            
            TileTypeData data = roomTerrain is not null && roomTerrain.ContainsKey(c) ? _tileTypes[roomTerrain[c]] 
                : _tileTypes.Values.First(tileType => tileType.Symbol == c);
            terrainObjects[i, 23 - j] = data;
            i++;
        }

        return terrainObjects;
    }
}

public readonly record struct Room(TileTypeData[,] Pattern, int Type, int Commonness, bool RotationalSymmetry,
    bool HorizontalSymmetry);
