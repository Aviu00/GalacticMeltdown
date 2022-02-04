using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GalacticMeltdown.data;

public class RoomData : XmlExtractor
{
    public List<(int rarity, int exitCount, Room room)> Rooms { get; private set; }

    private readonly Dictionary<char, string> _defaultTiles = new()
    {
        {'.', "floor"},
        {'#', "wall"},
        {'N', "wall_if_north"},
        {'n', "floor_if_north"},
        {'E', "wall_if_east"},
        {'e', "floor_if_east"},
        {'S', "wall_if_south"},
        {'s', "floor_if_south"},
        {'W', "wall_if_west"},
        {'w', "floor_if_west"}
    };

    private Dictionary<string, TileTypeData> _tileTypes;

    public RoomData(Dictionary<string, TileTypeData> tileTypes)
    {
        _tileTypes = tileTypes;
        Rooms = new List<(int rarity, int exitCount, Room room)>();
        XmlDocument doc = GetXmlDocument("Rooms.xml");
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            TileTypeData[,] pattern = new TileTypeData[24, 24];
            foreach (XmlNode locNode in node)
            {
                switch (locNode.Name)
                {
                    case "Pattern":
                        pattern = ConvertPattern(locNode.InnerText);
                        break;
                }
            }
            
            Rooms.Add((0, 4, new Room(pattern)));
        }
    }

    private TileTypeData[,] ConvertPattern(string stringPattern)
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
            TileTypeData data = _tileTypes[_defaultTiles[c]];
            terrainObjects[i, 23 - j] = data;
            i++;
        }

        return terrainObjects;
    }
        
    public record struct Room(TileTypeData[,] Pattern);
}